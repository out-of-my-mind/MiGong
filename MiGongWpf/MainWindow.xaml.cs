using MiGongWpf.CustomShape;
using MiGongWpf.MiGong;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
//using WindowMessageHook;
using System.Windows.Media.Media3D;
using MiGongWpf.ViewModel;
using System.Windows.Media;
using System.Text;
using System;

namespace MiGongWpf
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        ShowInfo showInfo = null;
        //public KeyboardHook windowMessageHook = new KeyboardHook();
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            Closed += MainWindow_Closed;
            this.DataContext = new MainWindowView();

           
            //WavefrontObjLoader wavefrontObjLoader = new WavefrontObjLoader();
            //ModelVisual3DWithName modelVisual3DWith = wavefrontObjLoader.LoadObjFile(@"H:\Learn\demo\MiGongWpf\untitled.obj");
            //view3d.Children.Add(modelVisual3DWith);

            //windowMessageHook.SetKeyboardHook(Base.HookType.KeyboardLL);
            //windowMessageHook.KeyDownEvent += WindowMessageHook_KeyDownEvent;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            showInfo = new ShowInfo();
            showInfo.Show();
        }

        //private void WindowMessageHook_KeyDownEvent(object sender, System.Windows.Forms.KeyEventArgs e)
        //{

        //}

        private void MainWindow_Closed(object sender, System.EventArgs e)
        {
            showInfo.Close();
            //windowMessageHook.UnKeyboardHook();
        }

        private void Conver3DMG_Click(object sender, RoutedEventArgs e)
        {
            Convert3D();
        }

        /// <summary>
        /// 在0,0,0处创建正方体，之后的创建都是根据这个位置的图形进行移动和缩放
        /// </summary>
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

            double cubeLength = 4;
            MeshGeometry3D meshGeometry3D = new MyCube().GetCube(cubeLength);

            StringBuilder msg = new StringBuilder();
            foreach (var item in mainWindowView.myLines)
            {
                GeometryModel3D geometryModel3D = new GeometryModel3D();
                geometryModel3D.Geometry = meshGeometry3D;
                geometryModel3D.Material = materialGroup;
                geometryModel3D.BackMaterial = materialGroup;
                /*
                 得到的线段点位数据 是在Canvas上绘制，二维坐标系Y轴 向下是正方向。3D坐标系里Y轴向上是正方向。
                转换成3D的时候需要处理Y坐标
                 */
                Transform3DGroup transform3DGroup = new Transform3DGroup();
                TranslateTransform3D translateTransform3D = new TranslateTransform3D(
                    item.GetOffsetX * cubeLength, 
                    -item.GetOffsetY * cubeLength + mainWindowView.rowLength * cubeLength, //向上移动总高度（将原点变成左下角
                    0);//移动
                ScaleTransform3D scaleTransform3D = new ScaleTransform3D(item.GetScaleX, -item.GetScaleY, 1, 0, 0, 0);//缩放
                transform3DGroup.Children.Add(scaleTransform3D);
                transform3DGroup.Children.Add(translateTransform3D);
                geometryModel3D.Transform = transform3DGroup;

                msg.AppendLine(string.Format("startPoint:{0},{1}。endPoint:{2},{3}。XLength:{4},YLength:{5}。X:{6},Y:{7}", 
                    item.startPoint.X, item.startPoint.Y, item.endPoint.X, item.endPoint.Y, item.GetScaleX, item.GetScaleY, item.GetOffsetX, item.GetOffsetY));
                model3DGroup.Children.Add(geometryModel3D);

            }

            mainWindowView.CameraX = mainWindowView.columnLength * cubeLength / 2;
            mainWindowView.CameraY = 0;
            mainWindowView.CameraLookDirection = new Point3D(mainWindowView.CameraX, mainWindowView.columnLength * cubeLength / 2, mainWindowView.CameraY) - mainWindowView.CameraPosition;



            showInfo.showPanel.Text = msg.ToString();

            MiGongMap.Content = model3DGroup;
        }

    }
}
