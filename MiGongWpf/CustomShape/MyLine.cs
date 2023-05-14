﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MiGongWpf.CustomShape
{
    public class MyLine : INotifyPropertyChanged
    {
        private string guid;
        public MyLine()
        {
        }
        public MyLine(Point startPoint, Point endPoint)
        {
            this._startPoint = startPoint;
            this._endPoint = endPoint;
            this.guid = Guid.NewGuid().ToString("N");
        }
        private Point _startPoint;
        private Point _endPoint;
        public Point startPoint { set { _startPoint = value; OnPropertyChanged("startPoint"); } get { return _startPoint; } }
        public Point endPoint { set { _endPoint = value; OnPropertyChanged("endPoint"); } get { return _endPoint; } }



        /// <summary>
        /// 是否水平线
        /// </summary>
        public bool IsHorizontal { get { return startPoint.Y == endPoint.Y; } }

        public double GetScaleX { get { return Math.Abs(startPoint.X - endPoint.X) / 4 + 1; } }
        public double GetScaleY { get { return Math.Abs(startPoint.Y - endPoint.Y) / 4 + 1; } }
        public double GetOffsetX { get { return IsHorizontal ? 0 : Math.Abs(startPoint.X - endPoint.X); } }
        public double GetOffsetY { get { return IsHorizontal ? Math.Abs(startPoint.Y - endPoint.Y) : 0; } }


        public bool MargeLine(MyLine otherLine)
        {
            if(otherLine.IsHorizontal != this.IsHorizontal)
            {
                return false;
            }
            List<Point> points = new List<Point>() { startPoint, endPoint };
            List<Point> otherPoints = new List<Point>() { otherLine.startPoint, otherLine.endPoint };
            IEnumerable<Point> samePoints = points.Intersect(otherPoints);
            if (samePoints.Count() == 1)
            {//满足合并条件
                IEnumerable<Point> newPoints = points.Where(o => !samePoints.Contains(o)).Union(otherPoints.Where(o => !samePoints.Contains(o)));
                startPoint = newPoints.ElementAt(0);
                endPoint = newPoints.ElementAt(1);
                return true;
            }
            if (samePoints.Count() == 2)
            {
                return true;
            }
            return false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
        public override int GetHashCode()
        {
            return guid.GetHashCode();
        }
    }
}
