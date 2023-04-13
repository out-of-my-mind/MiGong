{
    [CommandMethod("MYMG")]
    public void MIGONG()
    {
        int rowCont = 20;
        int columnCount = 20;
        int colWidth = 4;
        Dictionary<Tuple<int, int>, MIGONGModel> DataCols = new Dictionary<Tuple<int, int>, MIGONGModel>();
        List<Tuple<Tuple<int, int>, string>> step = new List<Tuple<Tuple<int, int>, string>>();
        Point3d? basePoint = null;
        using (DrawingUtility util = new DrawingUtility())
        {
            #region 绘制迷宫底格
            basePoint = util.GetPointNew();
            if(!basePoint.HasValue)
            {
                return;
            }
            List<MIGONGModel> mIGONGs = new List<MIGONGModel>();
            List<Entity> entities = new List<Entity>();
            Line topline = new Line(basePoint.Value, basePoint.Value.Add(Vector3d.XAxis * rowCont * colWidth));
            Line bottomline = new Line(basePoint.Value.Add(Vector3d.YAxis * columnCount * -colWidth), basePoint.Value.Add(Vector3d.XAxis * rowCont * colWidth).Add(Vector3d.YAxis * columnCount * -colWidth));
            Line leftline = new Line(basePoint.Value, basePoint.Value.Add(Vector3d.YAxis * columnCount * -colWidth));
            Line rightline = new Line(basePoint.Value.Add(Vector3d.XAxis * rowCont * colWidth), basePoint.Value.Add(Vector3d.YAxis * columnCount * -colWidth).Add(Vector3d.XAxis * rowCont * colWidth));
            entities.Add(topline);
            entities.Add(bottomline);
            entities.Add(leftline);
            entities.Add(rightline);
            for (int r = 1; r <= rowCont; r++)
            {
                for (int c = 1; c <= columnCount; c++)
                {
                    MIGONGModel mIGONG = new MIGONGModel(r - 1, c - 1, DataCols);
                    if (r != columnCount)
                    {
                        //横
                        Point3d point = basePoint.Value.Add(Vector3d.YAxis * r * -colWidth).Add(Vector3d.XAxis * (c - 1) * colWidth);
                        Line line = new Line(point, point.Add(Vector3d.XAxis * colWidth));
                        entities.Add(line);
                        mIGONG.BorderLine.addIsCover("bottom", line, false);
                    }
                    if (c != rowCont)
                    {
                        //竖
                        Point3d point1 = basePoint.Value.Add(Vector3d.XAxis * c * colWidth).Add(Vector3d.YAxis * (r - 1) * -colWidth);
                        Line line1 = new Line(point1, point1.Add(Vector3d.YAxis * -colWidth));
                        entities.Add(line1);
                        mIGONG.BorderLine.addIsCover("right", line1, false);
                    }
                    DataCols.Add(mIGONG.CurrentCol, mIGONG);
                    mIGONGs.Add(mIGONG);
                }
            }
            for(int i = 0; i < DataCols.Count; i++)
            {
                var current = DataCols.ElementAt(i);
                if(!current.Value.BorderLine.ContainsKey("top"))
                {
                    var key = new Tuple<int, int>(current.Key.Item1 - 1, current.Key.Item2);
                    if(DataCols.ContainsKey(key))
                    {
                        current.Value.BorderLine.addIsCover("top", DataCols[key].BorderLine["bottom"], false);
                    }
                }
                if (!current.Value.BorderLine.ContainsKey("left"))
                {
                    var key = new Tuple<int, int>(current.Key.Item1, current.Key.Item2 - 1);
                    if (DataCols.ContainsKey(key))
                    {
                        current.Value.BorderLine.addIsCover("left", DataCols[key].BorderLine["right"], false);
                    }
                }
            }
            util.InsertDBObject("迷宫", entities.ToArray());
            DataCols[new Tuple<int, int>(0, 0)].IsStart = true;
            DataCols.Last().Value.IsEnd = true;
            #endregion
            Tuple<int, int> start = new Tuple<int, int>(0, 0);
            
            while (true)
            {
                if (DataCols.Where(o=>!o.Value.IsPass).Count() == 0)
                {
                    break;
                }
                DataCols[start].IsPass = true;
                Dictionary<string, MIGONGModel> mIGONGModels = DataCols[start].NextCol;
                if(mIGONGModels.Count == 0)
                {
                    var temp = DataCols.Where(o => o.Value.IsPass && o.Value.NextCol.Count > 0);
                    if(temp.Count() == 0)
                    {
                        break;
                    }
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
            for(int i = 0; i < step.Count; i++)
            {
                entitiesRemove.Add(DataCols[step[i].Item1].BorderLine[step[i].Item2]);
            }
            util.DeleteEntityList(entitiesRemove.Cast<DBObject>().ToList());
        }
        var startCol = DataCols.Where(o => o.Value.IsStart).FirstOrDefault();
        var endCol = DataCols.Where(o => o.Value.IsEnd).FirstOrDefault();
        using (DrawingUtility util = new DrawingUtility())
        {
            Point3d startP = basePoint.Value.Add(Vector3d.XAxis * (startCol.Key.Item2 * colWidth + colWidth / 2)).Add(Vector3d.YAxis * -(startCol.Key.Item1 * colWidth + colWidth / 2));
            Point3d endP = basePoint.Value.Add(Vector3d.XAxis * (endCol.Key.Item2 * colWidth + colWidth / 2)).Add(Vector3d.YAxis * -(endCol.Key.Item1 * colWidth + colWidth / 2));
            ZwSoft.ZwCAD.DatabaseServices.Polyline pline = util.GetFullCircle(startP, 1.5);
            util.InsertDBObject("迷宫", pline);
            //while (true)
            //{
            //    Point3d point3Base = startP;
            //    string str = DrawingUtility.Keywords("", new string[] { "A", "S","W","D", "C" }, new string[] { "(A)", "(S)","(W)","(D)","(C)" });
            //    if ("A".Equals(str))
            //    {
            //        point3Base = MovePoint("A", colWidth, point3Base);
            //    }                                   
            //    else if ("W".Equals(str))
            //    {  
            //        point3Base = MovePoint("W", colWidth, point3Base);
            //    }
            //    else if ("S".Equals(str))
            //    {
            //        point3Base = MovePoint("S", colWidth, point3Base);
            //    }
            //    else if ("D".Equals(str))
            //    {
            //        point3Base = MovePoint("D", colWidth, point3Base);
            //    }
            //    else if ("C".Equals(str))
            //    {
            //        break;
            //    }
            //    util.Move(pline, point3Base - startP);
            //    util.Flush();
            //    startP = point3Base;
            //}
        }
    }
    public Point3d MovePoint(string movetype, int widthCol,Point3d point3Base)
    {
        switch (movetype)
        {
            case "A": point3Base = point3Base.Add(Vector3d.XAxis * -widthCol); break;
            case "D": point3Base = point3Base.Add(Vector3d.XAxis * widthCol); break;
            case "S": point3Base = point3Base.Add(Vector3d.YAxis * -widthCol); break;
            case "W": point3Base = point3Base.Add(Vector3d.YAxis * widthCol); break;
        }
        return point3Base;
    }
}
public class MIGONGModel
{
    private readonly int RowIndex;
    private readonly int ColumnIndex;
    private Dictionary<Tuple<int,int>, MIGONGModel> AllCols { set; get; }
    public MIGONGModel(int rowindex, int columnindex, Dictionary<Tuple<int, int>, MIGONGModel> allcols)
    {
        this.RowIndex = rowindex;
        this.ColumnIndex = columnindex;
        this.IsPass = false;
        this.AllCols = allcols;
        this.IsEnd = false;
        this.BorderLine = new Dictionary<string, Entity>();
    }
    /// <summary>
    /// 当前的格子位置y, x
    /// </summary>
    public Tuple<int,int> CurrentCol { get {
            return new Tuple<int, int>(this.RowIndex, this.ColumnIndex);
        }
    }

    /// <summary>
    /// 相邻的格子线
    /// </summary>
    public Dictionary<string, Entity> BorderLine { set; get; }

    /// <summary>
    /// 下一步可以选择的格子(没有经过的
    /// </summary>
    public Dictionary<string, MIGONGModel> NextCol {
        get {
            Dictionary<string, MIGONGModel> temp = new Dictionary<string, MIGONGModel>();
            if(AllCols.ContainsKey(new Tuple<int,int>(RowIndex + 1, ColumnIndex)) && !AllCols[new Tuple<int, int>(RowIndex + 1, ColumnIndex)].IsPass)
            {//下
                temp.Add("bottom", AllCols[new Tuple<int, int>(RowIndex + 1, ColumnIndex)]);
            }
            if(AllCols.ContainsKey(new Tuple<int, int>(RowIndex - 1, ColumnIndex)) && !AllCols[new Tuple<int, int>(RowIndex - 1, ColumnIndex)].IsPass)
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
    /// <summary>
    /// 是否经过
    /// </summary>
    public bool IsPass { set; get; }
    /// <summary>
    /// 是否出口
    /// </summary>
    public bool IsEnd { set; get; }
    /// <summary>
    /// 是否起点
    /// </summary>
    public bool IsStart { set; get; }
}
