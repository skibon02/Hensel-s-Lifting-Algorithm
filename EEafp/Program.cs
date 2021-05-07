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

            RingPolynomial for_factorization = new RingPolynomial { 1, 1, 1 } * new RingPolynomial { 1, 1, 1 } * new RingPolynomial { 1, 1, 1 } * new RingPolynomial { 1, 2, 1 };

            RingPolynomial factorization = new RingPolynomial(4);
            factorization[0] += 1;

            var res = (for_factorization).BerlekampFactor();

            for_factorization.Print();

            for (int i=0; i < res.Count; i++)
            {
                factorization *= res[i];
                res[i].Print('g');
            }

            factorization.Normalize();
            factorization.Print();

            Console.Read();
        }
    }
}
