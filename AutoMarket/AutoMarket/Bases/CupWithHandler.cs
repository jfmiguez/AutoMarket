using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoMarket.Bases
{
    class CupWithHandler : BaseFinder
    {

        public CupWithHandler()
        {
            String test = "test";
        }


        public String FindBase()
        {
            return "Cup with handler";
        }


        public virtual String Findsomething(String parameter1, int parameter2)
        {
            return parameter1 + parameter2.ToString();
        }

        public String FindIt()
        {
            return "test2";
        }


    }
}
