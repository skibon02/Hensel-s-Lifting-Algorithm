using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EEafp
{
    public class Program
    {
        public static bool LogEnabled = true;
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            RingPolynomial.SetModContext(5);


            Console.Read();
        }

        public static int recDepth = 0;
        public static string GetTab {
            get {
                string res = "";
                for (int i = 0; i < (recDepth-1)*3; i++)
                {
                    res += "* ";
                }

                return res;
            }
        }
        public static void Log(string inp = "")
        {
            if (LogEnabled)
                Console.WriteLine(GetTab + inp);
        }
    }
}
