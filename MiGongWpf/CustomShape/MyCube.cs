using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows;

namespace MiGongWpf.CustomShape
{
    public class MyCube
    {
        MeshGeometry3D meshGeometry3D = null;
        public MyCube()
        {
        }
        /// <summary>
        /// 得到正方体形状数据
        /// </summary>
        /// <param name="length">边长</param>
        /// <returns></returns>
        public MeshGeometry3D GetCube(double length)
        {
            meshGeometry3D = new MeshGeometry3D();
            #region 正方体的点
            Point3DCollection point3Ds = new Point3DCollection();
            point3Ds.Add(new Point3D(0, 0, 0));
            point3Ds.Add(new Point3D(length, 0, 0));
            point3Ds.Add(new Point3D(0, length, 0));
            point3Ds.Add(new Point3D(length, length, 0));

            point3Ds.Add(new Point3D(0, 0, 0));
            point3Ds.Add(new Point3D(0, 0, length));
            point3Ds.Add(new Point3D(0, length, 0));
            point3Ds.Add(new Point3D(0, length, length));

            point3Ds.Add(new Point3D(0, 0, 0));
            point3Ds.Add(new Point3D(length, 0, 0));
            point3Ds.Add(new Point3D(0, 0, length));
            point3Ds.Add(new Point3D(length, 0, length));

            point3Ds.Add(new Point3D(length, 0, 0));
            point3Ds.Add(new Point3D(length, length, length));
            point3Ds.Add(new Point3D(length, 0, length));
            point3Ds.Add(new Point3D(length, length, 0));

            point3Ds.Add(new Point3D(0, 0, length));
            point3Ds.Add(new Point3D(length, 0, length));
            point3Ds.Add(new Point3D(0, length, length));
            point3Ds.Add(new Point3D(length, length, length));

            point3Ds.Add(new Point3D(0, length, 0));
            point3Ds.Add(new Point3D(0, length, length));
            point3Ds.Add(new Point3D(length, length, 0));
            point3Ds.Add(new Point3D(length, length, length));
            meshGeometry3D.Positions = point3Ds;
            #endregion
            #region 三角组成面
            Int32Collection ints = new Int32Collection() {
                0,2,1 , 1,2,3,
                4,5,6 , 6,5,7,
                8,9,10 , 9,10,11,
                12,13,14 , 12,15,13,
                16,17,18 , 19,18,17,
                20,21,22 , 22,21,23 };
            meshGeometry3D.TriangleIndices = ints;
            #endregion
            #region 每个点处的向量
            PointCollection points = new PointCollection();
            points.Add(new Point(0, 0));
            points.Add(new Point(0, 1));
            points.Add(new Point(1, 0));
            points.Add(new Point(1, 1));

            points.Add(new Point(1, 1));
            points.Add(new Point(0, 1));
            points.Add(new Point(1, 0));
            points.Add(new Point(0, 0));

            points.Add(new Point(0, 0));
            points.Add(new Point(1, 0));
            points.Add(new Point(0, 1));
            points.Add(new Point(1, 1));

            points.Add(new Point(0, 0));
            points.Add(new Point(1, 0));
            points.Add(new Point(0, 1));
            points.Add(new Point(1, 1));

            points.Add(new Point(1, 1));
            points.Add(new Point(0, 1));
            points.Add(new Point(1, 0));
            points.Add(new Point(0, 0));

            points.Add(new Point(1, 1));
            points.Add(new Point(0, 1));
            points.Add(new Point(1, 0));
            points.Add(new Point(0, 0));

            meshGeometry3D.TextureCoordinates = points;
            #endregion
            return meshGeometry3D;
        }
    }
}
