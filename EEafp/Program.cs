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
            RingPolynomial.SetModContext(5);

            RingPolynomial one = new RingPolynomial { 1, 2, 1, 3 };
            RingPolynomial two = new RingPolynomial { 1, 1, 1 };
            RingPolynomial for_factorization = one* one* one* one* one* two* two* two* two* two;

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

            RingPolynomial gcd_res;
            Console.WriteLine("Проверим линейную независимость:");
            for(int i = 0; i < res.Count - 1; i++)
            {
                RingPolynomial.GCD(res[i], res[i+1], out gcd_res);
                gcd_res.Print();
            }

            Console.Read();
        }
    }
}
