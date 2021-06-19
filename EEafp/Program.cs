using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace EEafp
{
    public class Program
    {
        public static bool LogEnabled = true;
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine("Выберите пример (1 - НОД, 2 - Берлекамп, 3 - Гензель)");

            int chosen_example = Console.Read() - '0';

            if (chosen_example == 1)
            {
                int mod = 13;
                RingPolynomial.SetModContext(mod);

                Console.WriteLine("Выбран НОД.");
                Console.WriteLine("1) НОД полиномов над R[x]: (Расширенный алгоритм Евклида)");
                Polynomial f_r = new Polynomial { 1, 1, 1 } * new Polynomial {(decimal)0.5, 1, 1} * new Polynomial { 2, 1};
                f_r.Print('f');
                Polynomial g_r = new Polynomial { 1, 1 } * new Polynomial { 7, 7, 7 } * new Polynomial { 3, 1};
                g_r.Print('g');
                Console.WriteLine("Их НОД и коэффициенты:");
                Polynomial gcd_r;

                Polynomial.GCDResult coeffs = Polynomial.GCD(f_r, g_r, out gcd_r);

                Console.WriteLine(gcd_r + " = (" + coeffs.Coef1 + ")*(" + f_r + ") + (" + coeffs.Coef2 + ")*(" + g_r + ")");

                Console.WriteLine("---------------------------------------------------------");

                Console.WriteLine("2) НОД полиномов над Zp[x]: (Расширенный алгоритм Евклида), модуль p = " +  mod);
                RingPolynomial f_p = new RingPolynomial { 2, 1, 1 } * new RingPolynomial {5, 1 } * new RingPolynomial { 2, 1 };
                f_p.Print('f');
                RingPolynomial g_p = new RingPolynomial { 1, 1 } * new RingPolynomial { 14, 7, 7 } * new RingPolynomial { 3, 1 };
                g_p.Print('g');
                Console.WriteLine("Их НОД и коэффициенты:");
                RingPolynomial gcd_p;

                RingPolynomial.GCDResult coeffs_p = RingPolynomial.GCD(f_p, g_p, out gcd_p);

                Console.WriteLine(gcd_p + " = (" + coeffs_p.Coef1 + ")*(" + f_p + ") + (" + coeffs_p.Coef2 + ")*(" + g_p + ")");

                Console.WriteLine("---------------------------------------------------------");

                Console.WriteLine("3) НОД полиномов над Z[x]: (алгоритм Евклида с псевдоделением)");
                IntPolynomial f_z = new IntPolynomial { 1, 1 } * new IntPolynomial { 5, 1 } * new IntPolynomial { 2, 1 };
                f_z.Print('f');
                IntPolynomial g_z = new IntPolynomial { 1, 1 } * new IntPolynomial { 2, 7, 7 } * new IntPolynomial { 3, 1 } * new IntPolynomial { 2, 1 };
                g_z.Print('g');
                Console.WriteLine("Их НОД:");
                IntPolynomial gcd_z;
                IntPolynomial.GCD(f_z, g_z, out gcd_z);

                Console.WriteLine(gcd_z);

            } else if (chosen_example == 2)
            {
                int mod = 7;
                RingPolynomial.SetModContext(mod);
                Console.WriteLine("Выбран алгоритм Берклемпа факторизации многочлена. Модуль кольца равен: " + mod);
                RingPolynomial f = new RingPolynomial { 2, 1 } * new RingPolynomial { 5, 1 } * new RingPolynomial { 25, 10 } * new RingPolynomial { 3, 1 } * new RingPolynomial { 1, 1, 1} ;

                var LiftedFactorisation = f.BerlekampFactor();

                RingPolynomial res = new RingPolynomial { 1 };
                Program.Log("Получена факторизация:");
                for (int i = 0; i < LiftedFactorisation.CountUniq; i++)
                {
                    for (int j = 0; j < LiftedFactorisation.divisors[i].count; j++)
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

                if (f == res)
                {
                    Program.Log("Верно!");
                }
                else
                {
                    Program.Log("Неверно...");
                }
            } else if (chosen_example == 3)
            {
                IntPolynomial f = new IntPolynomial { 1, 2 } * new IntPolynomial { 1, 4 } * new IntPolynomial { 1, 4 } * new IntPolynomial { 1, 5 } * new IntPolynomial { 1, 3, 4, 2, 3, 4, 6 };

                IntPolynomial hasSquares;
                IntPolynomial.GCD(f, f.Derivative(), out hasSquares);
                IntPolynomial SquareFreef = (f / hasSquares).Quotient;

                Console.WriteLine("Выбран алгоритм Гензеля подъема многочленов.");

                Console.WriteLine("Поиск подходящего модуля кольца: ");
                BigInteger mod = IntPolynomial.SelectAppropriateMod(f, SquareFreef);
                Console.WriteLine("Найденный модуль: " + mod);

                RingPolynomial.SetModContext(mod);
 
                Console.WriteLine("Изначальный многочлен над Z[x]");
                f.Print();
                RingPolynomial f_inring = new RingPolynomial(f);
                Console.WriteLine("Разложение его над Z" + mod + "[x]:");
                var factorization = f_inring.BerlekampFactor();
                var GCDfactor = RingPolynomial.GetGCDCoefficientForHensel(factorization);
                RingPolynomial allGCDResult = new RingPolynomial { 0 };

                List<RingPolynomial> factorsOfCoeff = new List<RingPolynomial>();
                Console.WriteLine("Подбор коэффициентов для представления единицы в алгоритме Гензеля:");
                Console.WriteLine("Отдельные слагаемые с коэффициентами:");
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
                Console.WriteLine("Проверка их суммы на равенство 1:");
                allGCDResult.Print();

                // после поднятия mod уже увеличился
                var liftedDecomposition = RingPolynomial.HenselLifting(f, factorization, GCDfactor, 4);
                RingPolynomial checkLiftedDecomposition = new RingPolynomial { 1 };

                Console.WriteLine("Поднятая факторизация:");
                for (int i = 0; i < liftedDecomposition.CountUniq; i++)
                {
                    for (int j = 0; j < factorization.divisors[i].count; j++)
                    {
                        if (i == 0 && j == 0)
                        {
                            (liftedDecomposition[i] * f[f.size - 1]).Print();
                            checkLiftedDecomposition *= (liftedDecomposition[i] * f[f.size - 1]);
                        }
                        else
                        {
                            liftedDecomposition[i].Print();
                            checkLiftedDecomposition *= liftedDecomposition[i];
                        }
                    }
                }
                Program.Log("Проверяем декомпозицию поднятую:");
                checkLiftedDecomposition.Print('r');
                f.Print();
            }

            Console.Read(); 
        }

        public static int recDepth = 0;
        public static string GetTab {
            get {
                string res = "";
                for (int i = 0; i < (recDepth)*3; i++)
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
