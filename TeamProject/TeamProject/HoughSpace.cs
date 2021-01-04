using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamProject
{

    //class Key
    //{
    //    public double theta;
    //    public int rho;

    //    public Key(int t, int r)
    //    {
    //        theta = t;
    //        rho = r;
    //    }

    //    public override int GetHashCode()
    //    {
    //        return (int)(rho + theta);
    //    }
    //    public override bool Equals(object obj)
    //    {
    //        //return Equals(obj as Foo);
    //        var o = (Key)obj;
    //        return o.theta == theta && o.rho == rho;
    //    }
    //}

    class HoughSpace
    {
        public Dictionary<KeyValuePair<double, int>, List<KeyValuePair<int, int>>> HS; 

        public HoughSpace()
        {
            HS = new Dictionary<KeyValuePair<double, int>, List<KeyValuePair<int, int>>>();
        }
    }
}
