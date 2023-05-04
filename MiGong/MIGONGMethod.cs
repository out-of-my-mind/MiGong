using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZwSoft.ZwCAD.DatabaseServices;
using ZwSoft.ZwCAD.Geometry;

namespace ClassLibrary1.MiGong
{
    public class MIGONGMethod
    {
        private readonly int rowCont;
        private readonly int columnCount;
        private readonly int colWidth;
        private Tuple<int, int> startCol = null;
        private Tuple<int, int> endCol = null;
        public Dictionary<Tuple<int, int>, MIGONGModel> AllCols { private set; get; }
        public MIGONGMethod(int rowCont, int columnCount, int colWidth)
        {
            this.rowCont = rowCont;
            this.columnCount = columnCount;
            this.colWidth = colWidth;
            AllCols = new Dictionary<Tuple<int, int>, MIGONGModel>();
        }

        private void Add(MIGONGModel mIGONGModel)
        {
            AllCols.addIsCover(new Tuple<int, int>(mIGONGModel.CurrentCol.Item1, mIGONGModel.CurrentCol.Item2), mIGONGModel, false);
        }
        /// <summary>
        /// 绘制迷宫格子
        /// </summary>
        /// <param name="basePoint"></param>
        /// <returns></returns>
        public List<Entity> DrawMiGongLines(Point3d basePoint)
        {
            List<Entity> entities = new List<Entity>();
            Line topline = new Line(basePoint, basePoint.Add(Vector3d.XAxis * rowCont * colWidth));
            Line bottomline = new Line(basePoint.Add(Vector3d.YAxis * columnCount * -colWidth), basePoint.Add(Vector3d.XAxis * rowCont * colWidth).Add(Vector3d.YAxis * columnCount * -colWidth));
            Line leftline = new Line(basePoint, basePoint.Add(Vector3d.YAxis * columnCount * -colWidth));
            Line rightline = new Line(basePoint.Add(Vector3d.XAxis * rowCont * colWidth), basePoint.Add(Vector3d.YAxis * columnCount * -colWidth).Add(Vector3d.XAxis * rowCont * colWidth));
            entities.Add(topline);
            entities.Add(bottomline);
            entities.Add(leftline);
            entities.Add(rightline);
            for (int r = 1; r <= rowCont; r++)
            {
                for (int c = 1; c <= columnCount; c++)
                {
                    MIGONGModel mIGONG = new MIGONGModel(r - 1, c - 1);
                    if (r != columnCount)
                    {
                        //横
                        Point3d point = basePoint.Add(Vector3d.YAxis * r * -colWidth).Add(Vector3d.XAxis * (c - 1) * colWidth);
                        Line line = new Line(point, point.Add(Vector3d.XAxis * colWidth));
                        entities.Add(line);
                        mIGONG.BorderLine.addIsCover("bottom", line, false);
                    }
                    if (c != rowCont)
                    {
                        //竖
                        Point3d point1 = basePoint.Add(Vector3d.XAxis * c * colWidth).Add(Vector3d.YAxis * (r - 1) * -colWidth);
                        Line line1 = new Line(point1, point1.Add(Vector3d.YAxis * -colWidth));
                        entities.Add(line1);
                        mIGONG.BorderLine.addIsCover("right", line1, false);
                    }
                    Add(mIGONG);
                }
            }
            for (int i = 0; i < AllCols.Count; i++)
            {
                var current = AllCols.ElementAt(i);
                if (!current.Value.BorderLine.ContainsKey("top"))
                {
                    var key = new Tuple<int, int>(current.Key.Item1 - 1, current.Key.Item2);
                    if (AllCols.ContainsKey(key))
                    {
                        current.Value.BorderLine.addIsCover("top", AllCols[key].BorderLine["bottom"], false);
                    }
                }
                if (!current.Value.BorderLine.ContainsKey("left"))
                {
                    var key = new Tuple<int, int>(current.Key.Item1, current.Key.Item2 - 1);
                    if (AllCols.ContainsKey(key))
                    {
                        current.Value.BorderLine.addIsCover("left", AllCols[key].BorderLine["right"], false);
                    }
                }
            }
            return entities;
        }

        public void SetStartAndEnd(Tuple<int, int> startCol, Tuple<int, int> endCol)
        {
            if(startCol == endCol || !AllCols.ContainsKey(startCol) || !AllCols.ContainsKey(endCol))
            {
                throw new Exception("不要瞎传值");
            }
            this.startCol = startCol;
            this.endCol = endCol;
            AllCols[startCol].IsStart = true;
            AllCols[endCol].IsEnd = true;
        }
       
        public List<Entity> GetRemoveLines()
        {
            List<Tuple<Tuple<int, int>, string>> step = new List<Tuple<Tuple<int, int>, string>>();
            var start = startCol;
            while (true)
            {
                if (AllCols.Where(o => !o.Value.IsPass).Count() == 0)
                {
                    break;
                }
                AllCols[start].IsPass = true;
                Dictionary<string, MIGONGModel> mIGONGModels = NextCol(AllCols[start]);
                Thread.Sleep(1);
                if (mIGONGModels.Count == 0)
                {//当前格子周边都经过了

                    //在当前已经经过的格子里找到 存在可以砸墙的格子 随机一个
                    var temp = AllCols.Where(o => o.Value.IsPass && NextCol(o.Value).Count > 0); ;
                    if (temp.Count() == 0)
                    {
                        break;
                    }
                    //得到下一个周边存在没有经过的格子
                    int nextT = new Random((int)DateTime.Now.Ticks).Next(0, temp.Count());
                    var nextdataT = temp.ElementAt(nextT);
                    start = nextdataT.Key;
                    continue;
                }
                int next = new Random((int)DateTime.Now.Ticks).Next(0, mIGONGModels.Count);
                var nextdata = mIGONGModels.ElementAt(next);
                step.Add(new Tuple<Tuple<int, int>, string>(start, nextdata.Key));
                start = nextdata.Value.CurrentCol;
            }
            List<Entity> entitiesRemove = new List<Entity>();
            for (int i = 0; i < step.Count; i++)
            {
                entitiesRemove.Add(AllCols[step[i].Item1].BorderLine[step[i].Item2]);
            }
            return entitiesRemove;
        }

        private Dictionary<string, MIGONGModel> NextCol(MIGONGModel currentCol)
        {
            int RowIndex = currentCol.CurrentCol.Item1;
            int ColumnIndex = currentCol.CurrentCol.Item2;
            Dictionary<string, MIGONGModel> temp = new Dictionary<string, MIGONGModel>();
            if (AllCols.ContainsKey(new Tuple<int, int>(RowIndex + 1, ColumnIndex)) && !AllCols[new Tuple<int, int>(RowIndex + 1, ColumnIndex)].IsPass)
            {//下
                temp.Add("bottom", AllCols[new Tuple<int, int>(RowIndex + 1, ColumnIndex)]);
            }
            if (AllCols.ContainsKey(new Tuple<int, int>(RowIndex - 1, ColumnIndex)) && !AllCols[new Tuple<int, int>(RowIndex - 1, ColumnIndex)].IsPass)
            {//上
                temp.Add("top", AllCols[new Tuple<int, int>(RowIndex - 1, ColumnIndex)]);
            }
            if (AllCols.ContainsKey(new Tuple<int, int>(RowIndex, ColumnIndex + 1)) && !AllCols[new Tuple<int, int>(RowIndex, ColumnIndex + 1)].IsPass)
            {//右
                temp.Add("right", AllCols[new Tuple<int, int>(RowIndex, ColumnIndex + 1)]);
            }
            if (AllCols.ContainsKey(new Tuple<int, int>(RowIndex, ColumnIndex - 1)) && !AllCols[new Tuple<int, int>(RowIndex, ColumnIndex - 1)].IsPass)
            {//左
                temp.Add("left", AllCols[new Tuple<int, int>(RowIndex, ColumnIndex - 1)]);
            }
            return temp;
        }
    }
}
