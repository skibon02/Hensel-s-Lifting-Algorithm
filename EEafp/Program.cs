using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EEafp
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            RingPolynomial.SetModContext(13);

            var res = (new RingPolynomial { 1, 2 }* new RingPolynomial { 1, 1, 1 }).BerlekampFactor();

            Console.Read();
        }
    }
}
