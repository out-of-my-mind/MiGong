using ClassLibrary1.MiGong;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZwSoft.ZwCAD.DatabaseServices;
using ZwSoft.ZwCAD.Geometry;
using ZwSoft.ZwCAD.Runtime;

[assembly: CommandClass(typeof(ClassLibrary1.DrawMiGong))]
namespace ClassLibrary1
{
    /// <summary>
    /// 灵感来源 https://www.bilibili.com/video/BV1tK4y1W777/
    /// </summary>
    public class DrawMiGong
    {
        [CommandMethod("MYMG")]
        public void MIGONG()
        {
            using (DrawTool drawTool = new DrawTool())
            {
                Point3d? basePoint = null;
                basePoint = drawTool.GetPoint(null);
                if (!basePoint.HasValue)
                {
                    return;
                }
                int colWidth = 4;
                MIGONGMethod migongMethod = new MIGONGMethod(20, 20, colWidth);
                //绘制迷宫底格
                List<Entity> entities = migongMethod.DrawMiGongLines(basePoint.Value);
                drawTool.InsertDBObject("迷宫", entities.ToArray());
                drawTool.Commit();
                //设置起点 终点
                Tuple<int, int> startCol = new Tuple<int, int>(0, 0);
                Tuple<int, int> endCol = new Tuple<int, int>(19, 19);
                //设置起点终点
                migongMethod.SetStartAndEnd(startCol, endCol);
                //删除迷宫路径非墙壁的线
                var entitiesRemove = migongMethod.GetRemoveLines();
                drawTool.DeleteEntityList(entitiesRemove);

                Point3d startP = basePoint.Value.Add(Vector3d.XAxis * (startCol.Item2 * colWidth + colWidth / 2)).Add(Vector3d.YAxis * -(startCol.Item1 * colWidth + colWidth / 2));
                Point3d endP = basePoint.Value.Add(Vector3d.XAxis * (endCol.Item2 * colWidth + colWidth / 2)).Add(Vector3d.YAxis * -(endCol.Item1 * colWidth + colWidth / 2));
                Polyline plineS = drawTool.GetFullCircle(startP, 1.5);
                Polyline plineE = drawTool.GetFullCircle(endP, 1.5);
                drawTool.InsertDBObject("迷宫", plineS, plineE);
                drawTool.Commit(false);
            }
        }
        //public Point3d MovePoint(string movetype, int widthCol, Point3d point3Base)
        //{
        //    switch (movetype)
        //    {
        //        case "A": point3Base = point3Base.Add(Vector3d.XAxis * -widthCol); break;
        //        case "D": point3Base = point3Base.Add(Vector3d.XAxis * widthCol); break;
        //        case "S": point3Base = point3Base.Add(Vector3d.YAxis * -widthCol); break;
        //        case "W": point3Base = point3Base.Add(Vector3d.YAxis * widthCol); break;
        //    }
        //    return point3Base;
        //}
    }
}
