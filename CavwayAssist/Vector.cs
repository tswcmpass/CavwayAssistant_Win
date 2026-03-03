using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CavwayAssist
{
    class Vector
    {
        public double x = 0;  //!< X component
        public double y = 0;  //!< Y component
        public double z = 0;  //!< Z component

        public static Vector operator +(Vector lhs, Vector rhs)
        {
            Vector result = lhs; 
            result.x += rhs.x;
            result.y += rhs.y;
            result.z += rhs.z;
            return result;
         }

    }
}
