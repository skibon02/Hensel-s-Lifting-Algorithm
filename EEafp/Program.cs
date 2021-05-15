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
            IntPolynomial f = new IntPolynomial { 3, 2, 59, 3, 23, 4, 1 };
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
            }

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
