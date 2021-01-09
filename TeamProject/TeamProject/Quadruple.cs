using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamProject
{
    class Quadruple
    {
        public int startRow;
        public int endRow;
        public int startCol;
        public int endCol;

        public Quadruple(int sr, int er, int sc, int ec)
        {
            startRow = sr;
            endRow = er;
            startCol = sc;
            endCol = ec;
        }

        public override string ToString()
        {
            var res = "Start Row: " + startRow + ", End Row: " + endRow + "\nStart Column: " + startCol + ", End Column: " + endCol;
            return res;
  
        }

    }
}
