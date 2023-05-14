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

namespace MiGongWpf.ViewModel
{
    public class MainWindowView : INotifyPropertyChanged
    {
        public DelegateCommand RefreshMG;
        public DelegateCommand MargeMG;
        public Point basePoint;
        
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
                ReisePropertyChange("colWidth");
            }
            get { return _colWidth; }
        }


        private MIGONGMethod mIGONGMethod = null;

        public MainWindowView()
        {
            basePoint = new Point(0, 0);
            mIGONGMethod = new MIGONGMethod(15, 15, 15);
            myLines = mIGONGMethod.MainDrawing(new Point(0, 0));

            this.RefreshMG = new DelegateCommand();
            this.RefreshMG.ExecuteAction = new Action<object>(MainDrawing);

            this.MargeMG = new DelegateCommand();
            this.MargeMG.ExecuteAction = new Action<object>(GetMargeResult);
        }


        public void MainDrawing(object baseParameter)
        {
            //List<string> newew1 = new List<string>() { "1231", "123", "3333", "4444"};
            //var newew2 = newew1.Where(o=>o.IndexOf('3') > 0);
            //newew1.RemoveAt(0);
            mIGONGMethod = new MIGONGMethod(15, 15, 15);
            myLines = mIGONGMethod.MainDrawing(new Point(0, 0));
        }

        public void GetMargeResult(object paramater)
        {
            myMargeLines = mIGONGMethod.MargeLine(myLines.ToList());
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void ReisePropertyChange(string propertyName)
        {
            if(this.PropertyChanged != null)
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
