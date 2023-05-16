using MiGongWpf.Commands;
using MiGongWpf.CustomShape;
using MiGongWpf.MiGong;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;

namespace MiGongWpf.ViewModel
{
    public class MainWindowView : INotifyPropertyChanged
    {
        #region 命令
        public ICommand RefreshMG { set; get; }
        public ICommand MargeMG { set; get; }
        public ICommand FindPathMG { set; get; }
        //public ICommand Conver3DMG { set; get; }
        #endregion
        public Point basePoint;

        #region 页面数据
        private IEnumerable<MyLine> _myLines;

        public IEnumerable<MyLine> myLines
        {
            get { return _myLines; }
            set { _myLines = value; ReisePropertyChange("myLines"); }
        }
        private IEnumerable<MyLine> _myMargeLines;
        public IEnumerable<MyLine> myMargeLines
        {
            get { return _myMargeLines; }
            set { _myMargeLines = value; ReisePropertyChange("myMargeLines"); }
        }

        private int _rowCont { set; get; }
        private int _columnCount;
        private int _colWidth;
        /// <summary>
        /// 行数
        /// </summary>
        public int rowCont
        {
            set
            {
                _rowCont = value;
                rowLength = _rowCont * colWidth;
                ReisePropertyChange("rowCont");
            }
            get { return _rowCont; }
        }
        /// <summary>
        /// 列数
        /// </summary>
        public int columnCount
        {
            set
            {
                _columnCount = value;
                columnLength = _columnCount * colWidth;
                ReisePropertyChange("columnCount");
            }
            get { return _columnCount; }
        }
        /// <summary>
        /// 格子宽度
        /// </summary>
        public int colWidth
        {
            set
            {
                _colWidth = value;
                rowLength = _rowCont * _colWidth;
                columnLength = _columnCount * _colWidth;
                ReisePropertyChange("colWidth");
            }
            get { return _colWidth; }
        }

        private int _rowLength;

        public int rowLength
        {
            get { return _rowLength; }
            set { _rowLength = value; ReisePropertyChange("rowLength"); }
        }
        private int _columnLength;
        public int columnLength
        {
            get { return _columnLength; }
            set { _columnLength = value; ReisePropertyChange("columnLength"); }
        }

        /// <summary>
        /// 线个数
        /// </summary>
        private int _lineCount;
        public int lineCount
        {
            get { return _lineCount; }
            set { _lineCount = value; ReisePropertyChange("lineCount"); }
        }
        /// <summary>
        /// 线个数
        /// </summary>
        private int _otherLineCount;
        public int otherLineCount
        {
            get { return _otherLineCount; }
            set { _otherLineCount = value; ReisePropertyChange("otherLineCount"); }
        }
        #endregion

        private MIGONGMethod mIGONGMethod = null;

        public MainWindowView()
        {
            basePoint = new Point(0, 0);
            rowCont = 15;
            columnCount = 15;
            colWidth = 15;
            this.RefreshMG = new DelegateCommand(new Action<object>(MainDrawing));
            this.MargeMG = new DelegateCommand(new Action<object>(GetMargeResult));
            this.FindPathMG = new DelegateCommand(new Action<object>(FindPath));
            //this.Conver3DMG = new DelegateCommand(new Action<object>(Convert3D));
        }

        /// <summary>
        /// 迷宫刷新
        /// </summary>
        /// <param name="baseParameter"></param>
        public void MainDrawing(object baseParameter)
        {
            var parameter = (Tuple<int,int,int>)baseParameter;
            rowCont = parameter.Item1;
            columnCount = parameter.Item2;
            colWidth = parameter.Item3;
            mIGONGMethod = new MIGONGMethod(parameter.Item1, parameter.Item2, parameter.Item3);
            myLines = mIGONGMethod.MainDrawing(new Point(0, 0));
            lineCount = myLines == null ? 0 : myLines.Count();
        }
        /// <summary>
        /// 迷宫线段合并
        /// </summary>
        /// <param name="paramater"></param>
        public void GetMargeResult(object paramater)
        {
            myMargeLines = mIGONGMethod.MargeLine(myLines.ToList());
            otherLineCount = myMargeLines == null ? 0 : myMargeLines.Count();
        }
        /// <summary>
        /// 寻路
        /// </summary>
        /// <param name="paramater"></param>
        public void FindPath(object paramater)
        {

        }
        /// <summary>
        /// 转换成3D
        /// </summary>
        /// <param name="patamater"></param>
        public void Convert3D(object patamater)
        {
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

        public event PropertyChangedEventHandler PropertyChanged;
        public void ReisePropertyChange(string propertyName)
        {
            if(this.PropertyChanged != null)
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
