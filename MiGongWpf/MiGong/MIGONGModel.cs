using MiGongWpf.CustomShape;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace MiGongWpf.MiGong
{
    public class MIGONGModel
    {
        private readonly int RowIndex;
        private readonly int ColumnIndex;
        public MIGONGModel(int rowindex, int columnindex)
        {
            this.RowIndex = rowindex;
            this.ColumnIndex = columnindex;
            this.IsPass = false;
            this.IsStart = false;
            this.IsEnd = false;
            this.BorderLine = new Dictionary<string, MyLine>();
        }
        /// <summary>
        /// 当前的格子位置y, x
        /// </summary>
        public Tuple<int, int> CurrentCol
        {
            get
            {
                return new Tuple<int, int>(this.RowIndex, this.ColumnIndex);
            }
        }

        /// <summary>
        /// 相邻的格子线
        /// </summary>
        public Dictionary<string, MyLine> BorderLine { set; get; }

        /// <summary>
        /// 是否经过
        /// </summary>
        public bool IsPass { set; get; }
        /// <summary>
        /// 是否出口
        /// </summary>
        public bool IsEnd { set; get; }
        /// <summary>
        /// 是否起点
        /// </summary>
        public bool IsStart { set; get; }
    }
}
