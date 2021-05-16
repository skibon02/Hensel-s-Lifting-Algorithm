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
            IntPolynomial f = new IntPolynomial { 1, 2 } * new IntPolynomial { 1, 4 } * new IntPolynomial { 1,4} * new IntPolynomial { 1, 5 } * new IntPolynomial { 1,3, 4, 2,3,4 ,6 };

            IntPolynomial hasSquares;
            IntPolynomial.GCD(f, f.Derivative(), out hasSquares);
            IntPolynomial SquareFreef = (f / hasSquares).Quotient;

            RingPolynomial.SetModContext(IntPolynomial.SelectAppropriateMod(f, SquareFreef));

            RingPolynomial f_inring = new RingPolynomial(f);
            f_inring.Print();
            var factorization = f_inring.BerlekampFactor();
            var GCDfactor = RingPolynomial.GetGCDCoefficientForHensel(factorization);
            RingPolynomial allGCDResult = new RingPolynomial { 0 };

            List<RingPolynomial> factorsOfCoeff = new List<RingPolynomial>();
            for (int i = 0; i < factorization.CountUniq; i++)
            {
                factorsOfCoeff.Add(new RingPolynomial { 1 });
            }
            for (int i = 0; i < factorization.CountUniq; i++)
            {
                for (int j = 0; j < factorization.CountUniq; j++)
                {
                    if (i != j)
                    {
                        factorsOfCoeff[i] *= factorization[j];
                        Console.Write("(" + factorization[j] + ")*");
                    }
                }
                if (i != 0)
                {
                    factorsOfCoeff[i] *= factorization.polyCoef;
                }
                allGCDResult += factorsOfCoeff[i] * GCDfactor[i];
                Console.Write("(" + GCDfactor[i] + ")\n");
            }
            allGCDResult.Print();

            // после поднятия mod уже увеличился
            var liftedDecomposition = RingPolynomial.HenselLifting(f, factorization, GCDfactor, 4);
            RingPolynomial checkLiftedDecomposition = new RingPolynomial { 1 };


            for (int i = 0; i < liftedDecomposition.CountUniq; i++)
            {
                for (int j = 0; j < factorization.divisors[i].count; j++)
                {
                    if (i == 0 && j == 0)
                    {
                        (liftedDecomposition[i] * f[f.size - 1]).Print();
                        checkLiftedDecomposition *= (liftedDecomposition[i] * f[f.size - 1]);
                    } else
                    {
                        liftedDecomposition[i].Print();
                        checkLiftedDecomposition *= liftedDecomposition[i];
                    }
                }
            }
            Program.Log("Проверяем декомпозицию поднятую:");
            checkLiftedDecomposition.Print('r');
            f.Print();  

            /*IntPolynomial f = new IntPolynomial { 1, 2 } * new IntPolynomial { 1, 5 } * new IntPolynomial { 1, 5 };
            RingDecomposeList fFactorisation;
            var LiftedFactorisation = f.FactorIntPolynomialOverBigModule(out fFactorisation);

            RingPolynomial res = new RingPolynomial { 1 };
            Program.Log("Получена факторизация:");
            for (int i = 0; i < LiftedFactorisation.CountUniq; i++)
            {
                for (int j = 0; j < fFactorisation.divisors[i].count; j++)
                {
                    RingPolynomial currPoly;
                    if (i == 0 && j == 0)
                        currPoly = LiftedFactorisation[i] * LiftedFactorisation.polyCoef;
                    else
                        currPoly = LiftedFactorisation[i];

                    currPoly.Print('g');
                    res *= currPoly;
                }
            }

            Program.Log("Проверка на соответствие исходному многочлену произведения:");
            Program.recDepth++;
            Program.Log("Исходный многочлен");
            f.Print();
            Program.Log("Результат перемножения факторизации:");
            res.Print();
            Program.recDepth--;

            IntPolynomial resInt = new IntPolynomial(res);
            if (f == resInt)
            {
                Program.Log("Верно!");
            }
            else
            {
                Program.Log("Неверно...");
            } */

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
