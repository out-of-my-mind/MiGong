using MiGongWpf.CustomShape;
using MiGongWpf.MiGong;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
//using WindowMessageHook;
using System.Windows.Media.Media3D;
using MiGongWpf.ViewModel;
using System.Windows.Media;

namespace MiGongWpf
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //public KeyboardHook windowMessageHook = new KeyboardHook();
        public MainWindow()
        {
            InitializeComponent();
            Closed += MainWindow_Closed;
            this.DataContext = new MainWindowView();

            //migongcontrol.loa
            
            //WavefrontObjLoader wavefrontObjLoader = new WavefrontObjLoader();
            //ModelVisual3DWithName modelVisual3DWith = wavefrontObjLoader.LoadObjFile(@"H:\Learn\demo\MiGongWpf\untitled.obj");
            //view3d.Children.Add(modelVisual3DWith);

            //windowMessageHook.SetKeyboardHook(Base.HookType.KeyboardLL);
            //windowMessageHook.KeyDownEvent += WindowMessageHook_KeyDownEvent;
        }

        //private void WindowMessageHook_KeyDownEvent(object sender, System.Windows.Forms.KeyEventArgs e)
        //{

        //}


        private void MainWindow_Closed(object sender, System.EventArgs e)
        {
            //windowMessageHook.UnKeyboardHook();
        }

        private void Conver3DMG_Click(object sender, RoutedEventArgs e)
        {
            Convert3D();
        }

        public void Convert3D()
        {
            MainWindowView mainWindowView = this.DataContext as MainWindowView;
            Model3DGroup model3DGroup = new Model3DGroup();
            MaterialGroup materialGroup = new MaterialGroup();//材质
            //设置颜色
            Brush greenBrush = new SolidColorBrush(Colors.Green);
            Brush greenLightBrush = new SolidColorBrush(Colors.LightGreen);
            SpecularMaterial specularMaterial = new SpecularMaterial(greenLightBrush, 24);
            DiffuseMaterial diffuseMaterial = new DiffuseMaterial(greenBrush);
            materialGroup.Children.Add(diffuseMaterial);
            materialGroup.Children.Add(specularMaterial);

            MeshGeometry3D meshGeometry3D = new MeshGeometry3D();
            #region 正方体的点
            Point3DCollection point3Ds = new Point3DCollection();
            point3Ds.Add(new Point3D(0, 0, 0));
            point3Ds.Add(new Point3D(4, 0, 0));
            point3Ds.Add(new Point3D(0, 4, 0));
            point3Ds.Add(new Point3D(4, 4, 0));

            point3Ds.Add(new Point3D(0, 0, 0));
            point3Ds.Add(new Point3D(0, 0, 4));
            point3Ds.Add(new Point3D(0, 4, 0));
            point3Ds.Add(new Point3D(0, 4, 4));

            point3Ds.Add(new Point3D(0, 0, 0));
            point3Ds.Add(new Point3D(4, 0, 0));
            point3Ds.Add(new Point3D(0, 0, 4));
            point3Ds.Add(new Point3D(4, 0, 4));

            point3Ds.Add(new Point3D(4, 0, 0));
            point3Ds.Add(new Point3D(4, 4, 4));
            point3Ds.Add(new Point3D(4, 0, 4));
            point3Ds.Add(new Point3D(4, 4, 0));

            point3Ds.Add(new Point3D(0, 0, 4));
            point3Ds.Add(new Point3D(4, 0, 4));
            point3Ds.Add(new Point3D(0, 4, 4));
            point3Ds.Add(new Point3D(4, 4, 4));

            point3Ds.Add(new Point3D(0, 4, 0));
            point3Ds.Add(new Point3D(0, 4, 4));
            point3Ds.Add(new Point3D(4, 4, 0));
            point3Ds.Add(new Point3D(4, 4, 4));
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
          


            foreach (var item in mainWindowView.myMargeLines)
            {
                GeometryModel3D geometryModel3D = new GeometryModel3D();
                geometryModel3D.Geometry = meshGeometry3D;
                geometryModel3D.Material = materialGroup;


                Transform3DGroup transform3DGroup = new Transform3DGroup();
                TranslateTransform3D translateTransform3D = new TranslateTransform3D(item.GetOffsetX, item.GetOffsetY, 0);//移动
                ScaleTransform3D scaleTransform3D = new ScaleTransform3D(item.GetScaleX, item.GetScaleY, 1);//缩放
                transform3DGroup.Children.Add(translateTransform3D);
                transform3DGroup.Children.Add(scaleTransform3D);

                geometryModel3D.Transform = transform3DGroup;
                model3DGroup.Children.Add(geometryModel3D);
            }
            MiGongMap.Content = model3DGroup;
        }

    }
}
