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


        private void MainWindow_Closed(object sender, System.EventArgs e)
        {
            //windowMessageHook.UnKeyboardHook();
        }
    }
}
