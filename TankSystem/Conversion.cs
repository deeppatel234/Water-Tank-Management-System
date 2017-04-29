using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankSystem
{
    class Conversion
    {
        public static int stringToint(String str)
        {
            int intValue = 0;
            string temp = "0";
            char[] charecter = str.ToCharArray();

            for (int i = 0; i < charecter.Length; i++)
            {
                temp += charecter[i].ToString();
            }

            intValue = int.Parse(temp);

            return intValue;
          //  int val = Convert.ToInt32(str);
           // return val;
        }
    }
}
