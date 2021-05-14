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
            IntPolynomial polyInt = new IntPolynomial { 4, 4, 4 } * new IntPolynomial { 2, 2 } * new IntPolynomial { 2, 2 } * new IntPolynomial { 7, 6 } * new IntPolynomial { 7, 7 } * new IntPolynomial { 1, 1, 12 } * new IntPolynomial { 0, 1 };
            RingPolynomial poly = new RingPolynomial(polyInt);
            var decomposition = poly.BerlekampFactor();
            Console.WriteLine();
            for (int i =0; i < decomposition.CountUniq; i++)
            {
                if (i == 0)
                    (decomposition[i] * decomposition.polyCoef).Print();
                else
                    decomposition[i].Print();
            }
            List<RingPolynomial> GCDCoeff = RingPolynomial.GetGCDCoefficientForHensel(decomposition);
            List<RingPolynomial> factorsOfCoeffs = new List<RingPolynomial>(GCDCoeff);
            RingPolynomial gcdRes = new RingPolynomial { 0 };

            for(int i=0; i<GCDCoeff.Count; i++)
            {
                Console.Write("(" + GCDCoeff[i]+ ")");
                for (int j=0; j < decomposition.CountUniq; j++)
                {
                    if (j != i)
                    {
                        Console.Write("(" + decomposition[j] + ")");
                        factorsOfCoeffs[i] *= decomposition[j];
                    }
                }
                if (i != 0)
                {
                    factorsOfCoeffs[i] *= decomposition.polyCoef;
                }
                Console.Write("\n");
                gcdRes += factorsOfCoeffs[i];
            }
            gcdRes.Print();
            var liftedDecomposition = RingPolynomial.HenselLifting(polyInt, decomposition, GCDCoeff, 9);
            RingPolynomial res = new RingPolynomial { 1 };
            for (int i = 0; i < liftedDecomposition.CountUniq; i++)
            {
                for (int j=0; j < decomposition.divisors[i].count; j++)
                {
                    if (j != 0)
                    {
                        (liftedDecomposition[i] / liftedDecomposition[i][liftedDecomposition[i].degree]).Print();
                        res *= liftedDecomposition[i] / liftedDecomposition[i][liftedDecomposition[i].degree];
                    } else
                    {
                        liftedDecomposition[i].Print();
                        res *= liftedDecomposition[i];
                    }
                }
            }
            res.Print('r');
            polyInt.Print();

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
