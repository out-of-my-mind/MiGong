using MiGongWpf.CustomShape;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MiGongWpf.MiGong
{
    public class MIGONGMethod
    {
        /// <summary>
        /// 行数
        /// </summary>
        public int rowCont;
        /// <summary>
        /// 列数
        /// </summary>
        public int columnCount;
        /// <summary>
        /// 格子宽度
        /// </summary>
        public int colWidth;

        public int MiGongWidth { get { return rowCont * colWidth; } }
        public int MiGongHeight { get { return columnCount * colWidth; } }

        /// <summary>
        /// 起始单元格子
        /// </summary>
        public Tuple<int, int> startCol { private set; get; }
        /// <summary>
        /// 终点单元格子
        /// </summary>
        public Tuple<int, int> endCol { private set; get; }
        private Vector vectorX = new Vector(1, 0);
        private Vector vectorY = new Vector(0, 1);
        public Dictionary<Tuple<int, int>, MIGONGModel> AllCols { private set; get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public MIGONGMethod(int rowCont, int columnCount, int colWidth)
        {
            this.rowCont = rowCont;
            this.columnCount = columnCount;
            this.colWidth = colWidth;
            AllCols = new Dictionary<Tuple<int, int>, MIGONGModel>();
        }



        /// <summary>
        /// 得到迷宫最终线段
        /// </summary>
        /// <param name="basePoint"></param>
        /// <returns></returns>
        public IEnumerable<MyLine> MainDrawing(Point basePoint)
        {
            //List<string> newew1 = new List<string>() { "1231", "123", "3333", "4444"};
            //var newew2 = newew1.Where(o=>o.IndexOf('3') > 0);
            //newew1.RemoveAt(0);

            AllCols = new Dictionary<Tuple<int, int>, MIGONGModel>();
            List<MyLine> myLines = DrawMiGongLines(basePoint);
            SetStartAndEnd(new Tuple<int, int>(0, 0), new Tuple<int, int>(rowCont - 1, columnCount - 1));
            var removeLine = GetRemoveLines();
            return myLines.Except(removeLine);
        }

        public IEnumerable<MyLine> GetMargeResult(List<MyLine> lines)
        {
            return MargeLine(lines);
        }

        #region 绘制流程
        private void Add(MIGONGModel mIGONGModel)
        {
            var key = new Tuple<int, int>(mIGONGModel.CurrentCol.Item1, mIGONGModel.CurrentCol.Item2);
            if (!AllCols.ContainsKey(key)) AllCols.Add(key, mIGONGModel);
        }
        /// <summary>
        /// 绘制迷宫格子
        /// </summary>
        /// <param name="basePoint">左上角</param>
        /// <returns></returns>
        public List<MyLine> DrawMiGongLines(Point basePoint)
        {
            List<MyLine> entities = new List<MyLine>();
            MyLine topline = new MyLine(basePoint, basePoint.Add(vectorX * columnCount * colWidth));
            MyLine leftline = new MyLine(basePoint, basePoint.Add(vectorY * rowCont * colWidth));
            MyLine bottomline = new MyLine(basePoint.Add(vectorY * rowCont * colWidth), topline.endPoint.Add(vectorY * rowCont * colWidth));
            MyLine rightline = new MyLine(topline.endPoint, bottomline.endPoint);
            entities.Add(topline);
            entities.Add(bottomline);
            entities.Add(leftline);
            entities.Add(rightline);

            for (int r = 1; r <= rowCont; r++)
            {
                for (int c = 1; c <= columnCount; c++)
                {
                    MIGONGModel mIGONG = new MIGONGModel(r - 1, c - 1);
                    if (c != columnCount)
                    {
                        //竖
                        Point point1 = basePoint.Add(vectorX * c * colWidth).Add(vectorY * (r - 1) * colWidth);
                        MyLine line1 = new MyLine(point1, point1.Add(vectorY * colWidth));
                        entities.Add(line1);
                        mIGONG.BorderLine.Add("right", line1);
                    }
                    if (r != rowCont)
                    {
                        //横
                        Point point = basePoint.Add(vectorY * r * colWidth).Add(vectorX * (c - 1) * colWidth);
                        MyLine line = new MyLine(point, point.Add(vectorX * colWidth));
                        entities.Add(line);
                        mIGONG.BorderLine.Add("bottom", line);
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
                        if (AllCols[key].BorderLine.ContainsKey("bottom")) current.Value.BorderLine.Add("top", AllCols[key].BorderLine["bottom"]);
                    }
                }
                if (!current.Value.BorderLine.ContainsKey("left"))
                {
                    var key = new Tuple<int, int>(current.Key.Item1, current.Key.Item2 - 1);
                    if (AllCols.ContainsKey(key))
                    {
                        if (AllCols[key].BorderLine.ContainsKey("right")) current.Value.BorderLine.Add("left", AllCols[key].BorderLine["right"]);
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
        public List<MyLine> GetRemoveLines()
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
            List<MyLine> entitiesRemove = new List<MyLine>();
            for (int i = 0; i < step.Count; i++)
            {
                entitiesRemove.Add(AllCols[step[i].Item1].BorderLine[step[i].Item2]);
                AllCols[step[i].Item1].BorderLine.Remove(step[i].Item2);
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
        #endregion

        /// <summary>
        /// 对于相连的线 合并成一条线（仅处理垂直线与垂直线  水平线与水平线
        /// </summary>
        /// <param name="myLines"></param>
        /// <returns></returns>
        public List<MyLine> MargeLine(List<MyLine> myLines)
        {
            List<MyLine> result = new List<MyLine>();
            while (myLines.Count > 0)
            {
                MyLine myLine = new MyLine(myLines[0].startPoint, myLines[0].endPoint);
                myLines.RemoveAt(0);
                //相同角度的线
                bool isCancel = false;
                var sameAngleLines = myLines.Where(o => o.IsHorizontal == myLine.IsHorizontal);
                while (!isCancel)
                {
                    isCancel = true;
                    for (int i=0; i< sameAngleLines.Count();i++)
                    {
                        if (myLine.MargeLine(sameAngleLines.ElementAt(i)))
                        {//如果出现合并，继续遍历
                            //联动删除sameAngleLines
                            myLines.Remove(sameAngleLines.ElementAt(i));
                            isCancel = false;
                        }
                    }
                }
                result.Add(myLine);
            }
            return result;
        }

        #region 查找迷宫路径
        /// <summary>
        /// 查找周边下一个可去的单元格
        /// </summary>
        /// <param name="currentCol"></param>
        /// <returns></returns>
        public Dictionary<string, MIGONGModel> FindNextCol(MIGONGModel currentCol)
        {
            int RowIndex = currentCol.CurrentCol.Item1;
            int ColumnIndex = currentCol.CurrentCol.Item2;
            Dictionary<string, MIGONGModel> temp = new Dictionary<string, MIGONGModel>();
            if (!currentCol.BorderLine.ContainsKey("bottom") && RowIndex != rowCont - 1)
            {//不是最后一行
                temp.Add("bottom", AllCols[new Tuple<int, int>(RowIndex + 1, ColumnIndex)]);
            }
            if (!currentCol.BorderLine.ContainsKey("top") && RowIndex != 0)
            {//不是第一行
                temp.Add("top", AllCols[new Tuple<int, int>(RowIndex - 1, ColumnIndex)]);
            }
            if (!currentCol.BorderLine.ContainsKey("right") && RowIndex != columnCount - 1)
            {//不是右最后一行
                temp.Add("right", AllCols[new Tuple<int, int>(RowIndex, ColumnIndex + 1)]);
            }
            if (!currentCol.BorderLine.ContainsKey("left") && columnCount != 0)
            {//不是左第一行
                temp.Add("left", AllCols[new Tuple<int, int>(RowIndex, ColumnIndex - 1)]);
            }
            return temp;
        }

        #endregion
    }
}
