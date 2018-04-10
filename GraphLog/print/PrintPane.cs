using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Drawing;

namespace Print
{
    public abstract class PrintPane
    {
        public int graphWidth{get;set;}
        public int graphHeight { get; set; }
        public abstract void paintAllNotBackground(Graphics g);
    }
}
