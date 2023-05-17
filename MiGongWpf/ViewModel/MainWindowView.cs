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

        #region 相机参数
        private Point3D cameraPosition;
        /// <summary>
        /// 相机位置
        /// </summary>
        public Point3D CameraPosition
        {
            get { return cameraPosition; }
            set { cameraPosition = value; ReisePropertyChange("CameraPosition"); }
        }
        private Double cameraX;

        public Double CameraX
        {
            get { return cameraX; }
            set { 
                cameraX = value; 
                CameraPosition = new Point3D(cameraX, cameraY, cameraZ);
                ReisePropertyChange("CameraX");
            }
        }
        private Double cameraY;

        public Double CameraY
        {
            get { return cameraY; }
            set { 
                cameraY = value;
                CameraPosition = new Point3D(cameraX, cameraY, cameraZ);
                ReisePropertyChange("CameraY"); }
        }
        private Double cameraZ;

        public Double CameraZ
        {
            get { return cameraZ; }
            set { 
                cameraZ = value;
                CameraPosition = new Point3D(cameraX, cameraY, cameraZ);
                ReisePropertyChange("CameraZ"); 
            }
        }
        //"0,-10,-40"
        private Vector3D cameraLookDirection;

        public Vector3D CameraLookDirection
        {
            get { return cameraLookDirection; }
            set { cameraLookDirection = value; ReisePropertyChange("CameraLookDirection"); }
        }



        #endregion
        #endregion

        private MIGONGMethod mIGONGMethod = null;

        public MainWindowView()
        {
            basePoint = new Point(0, 0);
            rowCont = 15;
            columnCount = 15;
            colWidth = 15;
            this.RefreshMG = new DelegateCommand(new Action<object>(MainDrawing));
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

            //迷宫线段合并
            myLines = mIGONGMethod.MargeLine(myLines.ToList());
        }
        ///// <summary>
        ///// 迷宫线段合并
        ///// </summary>
        ///// <param name="paramater"></param>
        //public void GetMargeResult(object paramater)
        //{
        //    myMargeLines = mIGONGMethod.MargeLine(myLines.ToList());
        //    otherLineCount = myMargeLines == null ? 0 : myMargeLines.Count();
        //}
        /// <summary>
        /// 寻路
        /// </summary>
        /// <param name="paramater"></param>
        public void FindPath(object paramater)
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void ReisePropertyChange(string propertyName)
        {
            if(this.PropertyChanged != null)
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
