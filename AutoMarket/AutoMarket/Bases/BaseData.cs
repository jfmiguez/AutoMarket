using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoMarket.Bases
{
    public class BaseData
    {
        public String BaseName { set; get; }
        public DateTime DateBaseStarts { set; get; }
        public DateTime DateBaseEnds { set; get; }
        public Decimal HeightBaseStarts { set; get; }
        public Decimal HeightBaseEnds { set; get; }
        public Decimal HeightMidpointBase { set; get; }
        public Decimal DepthBasePercent { set; get; }
        public Decimal PivotPoint { set; get; }
        public DateTime DateHandleStarts { set; get; }
        public Decimal DepthHandlePercent { set; get; }

        public Boolean isHandleRising { set; get; }
    }
}
