using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZwSoft.ZwCAD.ApplicationServices;
using ZwSoft.ZwCAD.DatabaseServices;
using ZwSoft.ZwCAD.EditorInput;
using ZwSoft.ZwCAD.Geometry;

namespace ClassLibrary1
{
    internal class DrawTool : IDisposable
    {
        private bool disposedValue;

        private Document doc = null;
        private Database db = null;
        private DocumentLock docLock = null;
        private Transaction transaction = null;
        public DrawTool()
        {
            doc = Application.DocumentManager.MdiActiveDocument;
            db = doc.Database;
            docLock = doc.LockDocument();
            transaction = db.TransactionManager.StartTransaction();
            HostApplicationServices.WorkingDatabase = ZwSoft.ZwCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Database;
        }
        internal void DebugLog(string msg)
        {
            doc.Editor.WriteMessage(msg);
        }
        internal Point3d? GetPoint(Point3d? basePoint)
        {
            PromptPointOptions prPointOptions = new PromptPointOptions("选择插入点");
            if (basePoint.HasValue)
            {
                prPointOptions.UseBasePoint = true;
                prPointOptions.BasePoint = basePoint.Value;
            }
            PromptPointResult prPointRes = doc.Editor.GetPoint(prPointOptions);
            if (prPointRes.Status == PromptStatus.OK)
            {
                return prPointRes.Value;
            }
            return null;
        }
        
        internal Entity GetEntity(IEnumerable<string> types)
        {
            PromptEntityOptions prOptions = new PromptEntityOptions("选择对象");
            PromptEntityResult prRes = doc.Editor.GetEntity(prOptions);
            if (prRes.Status == PromptStatus.OK)
            {
                return prRes.ObjectId.GetObject(OpenMode.ForRead) as Entity;
            }
            return null;
        }

        internal DBObject GetDBObject(ObjectId objectId, OpenMode openMode)
        {
            return transaction.GetObject(objectId, openMode);
        }

        internal void InsertDBObject(string layerName, params Entity[] objs)
        {
            CreateLayer(layerName);
            BlockTable bt = transaction.GetObject(db.BlockTableId, OpenMode.ForWrite) as BlockTable;
            BlockTableRecord btr = transaction.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
            foreach (Entity ent in objs)
            {
                ent.Layer = layerName;
                ObjectId oi = btr.AppendEntity(ent);
                transaction.AddNewlyCreatedDBObject(ent, true);
            }
        }

        internal void DeleteEntityList(List<Entity> dbList)
        {
            foreach (var oi in dbList)
            {
                DBObject ent = transaction.GetObject(oi.ObjectId, OpenMode.ForWrite);
                ent.UpgradeOpen();
                ent.Erase();
                ent.DowngradeOpen();
            }
        }

        internal Point3d? GetPointUndo(string mess, Point3d basePoint, ref bool isUndo)
        {
            PromptPointOptions prPointOptions = new PromptPointOptions("");
            prPointOptions.UseDashedLine = true;
            prPointOptions.UseBasePoint = true;
            prPointOptions.BasePoint = basePoint;
            prPointOptions.SetMessageAndKeywords("\n" + mess + "[Undo]: ", "Undo");
            PromptPointResult prPointRes = doc.Editor.GetPoint(prPointOptions);
            if (prPointRes.Status == PromptStatus.Keyword && prPointRes.StringResult == "Undo")
            {
                isUndo = true;
                return null;
            }
            if (prPointRes.Status == PromptStatus.OK)
            {
                return prPointRes.Value;
            }
            return null;
        }

        internal void CreateLayer(string layerName)
        {
            LayerTable lt = transaction.GetObject(db.LayerTableId, OpenMode.ForWrite) as LayerTable;
            if (!string.IsNullOrEmpty(layerName) && !lt.Has(layerName))
            {
                LayerTableRecord ltr = new LayerTableRecord();
                ltr.Name = layerName;
                lt.Add(ltr);
                transaction.AddNewlyCreatedDBObject(ltr, true);
            }
        }
        /// <summary>
        /// 绘制文本
        /// </summary>
        /// <param name="textString">文本内容</param>
        /// <param name="point"></param>
        /// <param name="TextHeight">字高</param>
        /// <param name="layerName">图层</param>
        /// <param name="rotation">角度</param>
        /// <returns></returns>
        public Entity DrawMText(string textString, Point3d point, double TextHeight, string layerName, double? rotation)
        {
            MText txt = new MText();
            txt.TextHeight = TextHeight;
            txt.Location = point;
            InsertDBObject(layerName, txt);
            if (!txt.IsWriteEnabled) txt.UpgradeOpen();

            txt.Color = ZwSoft.ZwCAD.Colors.Color.FromColorIndex(ZwSoft.ZwCAD.Colors.ColorMethod.ByLayer, 256);
            txt.Attachment = AttachmentPoint.MiddleCenter;
            txt.Rotation = rotation.Value;
            txt.Contents = textString;
            txt.Visible = true;
            return txt;
        }

        /// <summary>
        /// 得到周边关联的对象
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="dict"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public List<Entity> GetEntitysByType(Entity entity, double dict, IEnumerable<string> types)
        {
            bool isLine = entity is Curve && !(entity is Circle);
            TypedValue[] acTypValAr = new TypedValue[types.Count() + 2];
            acTypValAr.SetValue(new TypedValue((int)DxfCode.Operator, "<or"), 0);
            for (int i=0;i< types.Count();i++ )
            {
                acTypValAr.SetValue(new TypedValue((int)DxfCode.Start, types.ElementAt(i)), i+1);
            }
            acTypValAr.SetValue(new TypedValue((int)DxfCode.Operator, "or>"), types.Count() + 1);
            SelectionFilter filter = new SelectionFilter(acTypValAr);
            List<Entity> entitieList = new List<Entity>();
            if (!isLine)
            {
                Extents3d extents3D = entity.GeometricExtents;
                Vector3d vector3D = (extents3D.MaxPoint - extents3D.MinPoint);
                vector3D = vector3D / vector3D.Length;
                extents3D.AddPoint(extents3D.MinPoint.Add(-vector3D * dict));
                extents3D.AddPoint(extents3D.MaxPoint.Add(vector3D * dict));
                GetEntityByExtents3d(entity, extents3D, dict, filter, ref entitieList);
            }
            else
            {
                Curve curve = entity as Curve;
                Vector3d vector3D = new Vector3d(1,1,0);
                vector3D = vector3D / vector3D.Length;
                Extents3d extents3D = new Extents3d(curve.StartPoint.Add(-vector3D * dict), curve.StartPoint.Add(vector3D * dict));
                GetEntityByExtents3d(entity, extents3D, dict, filter, ref entitieList);

                extents3D = new Extents3d(curve.EndPoint.Add(-vector3D * dict), curve.EndPoint.Add(vector3D * dict));
                GetEntityByExtents3d(entity, extents3D, dict, filter, ref entitieList);
            }
            return entitieList;
        }

        private void GetEntityByExtents3d(Entity entity, Extents3d extents3D, double dict, SelectionFilter filter, ref List<Entity> entitieList)
        {
            PromptSelectionResult ents = doc.Editor.SelectCrossingWindow(extents3D.MinPoint, extents3D.MaxPoint, filter);
            if (ents.Status == PromptStatus.OK)
            {
                SelectionSet ss = ents.Value;
                foreach (ObjectId id in ss.GetObjectIds())
                {
                    if (id.Handle.ToString() == entity.Handle.ToString()) continue;
                    entitieList.Add(id.GetObject(OpenMode.ForRead) as Entity);
                }
            }
        }
        /// <summary>
        /// 绘制圆（用polyline表示实心效果
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public Polyline GetFullCircle(Point3d center, double radius)
        {
            Polyline pline = new Polyline();
            pline.AddVertexAt(0, center.Add(Vector3d.YAxis * radius / 2).toPoint2d(), 1, radius, radius);
            pline.AddVertexAt(1, center.Add(Vector3d.YAxis * -radius / 2).toPoint2d(), 1, radius, radius);
            pline.AddVertexAt(2, center.Add(Vector3d.YAxis * radius / 2).toPoint2d(), 0, 0, 0);
            return pline;
        }

        public static Point2d toPoint2d(Point3d pt)
        {
            return new Point2d(pt.X, pt.Y);
        }
        #region 几何计算
        public static bool isPointInRegion(Extents3d ext, Point3d point)
        {//利用向量叉乘判断，逆时针
            return isPointInRegion(ext, toPoint2d(point));
        }
        public static bool isPointInRegion(Extents3d ext, Point2d point)
        {
            int num = 0;
            num += GetClockWise(toPoint2d(ext.MaxPoint), point, new Point2d(ext.MaxPoint.X, ext.MinPoint.Y));
            num += GetClockWise(new Point2d(ext.MinPoint.X, ext.MaxPoint.Y), point, toPoint2d(ext.MaxPoint));
            num += GetClockWise(toPoint2d(ext.MinPoint), point, new Point2d(ext.MinPoint.X, ext.MaxPoint.Y));
            num += GetClockWise(new Point2d(ext.MaxPoint.X, ext.MinPoint.Y), point, toPoint2d(ext.MinPoint));
            return num == 4 ? true : false;
        }
        /// <summary>
        /// 判断矩形B是否与图框A相交
        /// 如果两个矩形相交，那么矩形A B的中心点和矩形的边长是有一定关系的。两个中心点间的距离肯定小于AB边长和的一半
        /// </summary>
        /// <param name="extentsA"></param>
        /// <param name="extentsB"></param>
        /// <returns>1:相交  0:包含  -1:无交集</returns>
        public static int RectangleIsInsert(Extents3d extentsA, Extents3d extentsB)
        {
            double zx = Math.Abs(extentsA.MinPoint.X + extentsA.MaxPoint.X - extentsB.MinPoint.X - extentsB.MaxPoint.X);
            double x = Math.Abs(extentsA.MinPoint.X - extentsA.MaxPoint.X) + Math.Abs(extentsB.MinPoint.X - extentsB.MaxPoint.X);
            double zy = Math.Abs(extentsA.MinPoint.Y + extentsA.MaxPoint.Y - extentsB.MinPoint.Y - extentsB.MaxPoint.Y);
            double y = Math.Abs(extentsA.MinPoint.Y - extentsA.MaxPoint.Y) + Math.Abs(extentsB.MinPoint.Y - extentsB.MaxPoint.Y);
            if (zx <= x && zy <= y)
            {
                if (!(isPointInRegion(extentsA, extentsB.MinPoint) && isPointInRegion(extentsA, extentsB.MaxPoint))
                    && !(isPointInRegion(extentsB, extentsA.MinPoint) && isPointInRegion(extentsB, extentsA.MaxPoint)))
                {
                    return 1;
                }
                return 0;
            }
            else
                return -1;

        }
        /// <summary>判断点在线的内部还是外部    内部或线上返回1，外部返回-1
        /// </summary>
        /// <param name="p1">基准点</param>
        /// <param name="p2">要判断的点</param>
        /// <param name="p3">原点</param>
        /// <returns></returns>
        public static int GetClockWise(Point2d p1, Point2d p2, Point2d p3)
        {
            if ((p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y) >= 0)
            {//逆时针
                return 1;
            }
            return -1;
        }
        #endregion


        public void Commit(bool isCreatNew = true)
        {
            transaction.Commit();
            if (isCreatNew)
            {
                transaction = db.TransactionManager.StartTransaction();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                }
                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                transaction.Dispose();
                if(docLock != null)
                {
                    docLock.Dispose();
                }
                disposedValue = true;
            }
        }

        // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        ~DrawTool()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
