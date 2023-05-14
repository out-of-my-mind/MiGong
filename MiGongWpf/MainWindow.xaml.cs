using MiGongWpf.CustomShape;
using MiGongWpf.MiGong;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
//using WindowMessageHook;
using System.Windows.Media.Media3D;
using MiGongWpf.ViewModel;

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
        /// <summary>
        /// 刷新迷宫
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            var method = FindResource("MainDrawing");
            var objectDataProvider = method as System.Windows.Data.ObjectDataProvider;
            objectDataProvider.Refresh();
        }

        /// <summary>
        /// 寻路迷宫
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FindPath_Click(object sender, RoutedEventArgs e)
        {
            var method = FindResource("MainDrawing");
            var objectDataProvider = method as System.Windows.Data.ObjectDataProvider;
            var source = (MIGONGMethod)objectDataProvider.ObjectInstance;
          
        }
        /// <summary>
        /// 合并线段
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Marge_Click(object sender, RoutedEventArgs e)
        {
            var method = FindResource("MainDrawing");
            var objectDataProvider = method as System.Windows.Data.ObjectDataProvider;
            var source = (MIGONGMethod)objectDataProvider.ObjectInstance;
            var currentData = ((IEnumerable<MyLine>)objectDataProvider.Data).ToList();
            lineCount.Content = $"对象个数：{currentData.Count}";

            var otherResult = source.GetMargeResult(currentData);
            migongcontrolother.ItemsSource = otherResult;
            otherLineCount.Content = $"对象个数：{otherResult.Count()}";
        }
        /// <summary>
        /// 转换3D
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Convert3D_Click(object sender, RoutedEventArgs e)
        {
            var sources = (IEnumerable<MyLine>)migongcontrolother.ItemsSource;

            Model3DGroup model3DGroup = new Model3DGroup();

            //< GeometryModel3D >
            //    < GeometryModel3D.Geometry >
            //        < MeshGeometry3D Positions = "
            //                    0,0,0  4,0,0  0,4,0  4,4,0
            //                    0,0,0  0,0,4  0,4,0  0,4,4
            //                    0,0,0  4,0,0  0,0,4  4,0,4
            //                    4,0,0  4,4,4  4,0,4  4,4,0
            //                    0,0,4  4,0,4  0,4,4  4,4,4
            //                    0,4,0  0,4,4  4,4,0  4,4,4
            //                    "
            //                    TriangleIndices = "
            //                    0,2,1  1,2,3
            //                    4,5,6  6,5,7
            //                    8,9,10  9,10,11
            //                    12,13,14  12,15,13
            //                    16,17,18  19,18,17
            //                    20,21,22  22,21,23
            //                    "
            //                    TextureCoordinates = "
            //                    0,0  0,1  1,0  1,1
            //                    1,1  0,1  1,0  0,0
            //                    0,0  1,0  0,1  1,1
            //                    0,0  1,0  0,1  1,1
            //                    1,1  0,1  1,0  0,0
            //                    1,1  0,1  1,0  0,0
            //                    "
            //                    />
            //    </ GeometryModel3D.Geometry >
            //    < GeometryModel3D.Material >
            //        < MaterialGroup >
            //            < DiffuseMaterial Brush = "Yellow" />
            //            < SpecularMaterial SpecularPower = "24" Brush = "LightYellow" />
            //        </ MaterialGroup >
            //    </ GeometryModel3D.Material >
            //</ GeometryModel3D >


            //model3DGroup.Children.Add();
        }
        private void MainWindow_Closed(object sender, System.EventArgs e)
        {
            //windowMessageHook.UnKeyboardHook();
        }
    }
}
