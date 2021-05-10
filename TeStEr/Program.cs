using System;
using System.Collections.Generic;
using System.Linq;

namespace EEafp
{
    class TesterProgram
    {
        public delegate bool tester();

        static void TestSolvingEquation(Polynomial f, Polynomial g, Polynomial c)
        {
            Console.WriteLine("Решаем уравнение: f(x)*X + g(x)*Y = c(x)");
            f.Print('f');
            g.Print('g');
            c.Print('c');
            var fulsol = Polynomial.SolveEquation(f, g, c);
            Console.WriteLine(fulsol);
            Console.WriteLine();

        }

        static RingPolynomial PrepareInputBerlekamp(out List<RingPolynomial> dividers, int minPolycount = 3, int maxPolycount = 3, int minPower = 2, int maxPower = 2, bool withzero = true)
        {
            dividers = new List<RingPolynomial>();
            Random rnd = new Random();
            int polycount = rnd.Next(minPolycount, maxPolycount);
            RingPolynomial for_factorization = new RingPolynomial { 1 };

            for (int i = 0; i < polycount; i++)
            {
                RingPolynomial anotherOne;
                do
                {
                    anotherOne = new RingPolynomial();
                    for (int j = 0; j < rnd.Next(minPower, maxPower); j++)
                        anotherOne.Add(rnd.Next(true ? 0 : 1, RingBint.mod-1));
                } while (anotherOne.IsNull());

                for_factorization *= anotherOne;
                dividers.Add(anotherOne);
            }

            return for_factorization;
        }
        enum BerlekampMode
        {
            Easy,
            PostMedium,
            Medium,
            Hard,
            Ultra
        }
        static RingPolynomial PrepareInputBerlekamp(out List<RingPolynomial> dividers, BerlekampMode berlekampMode)
        {
            dividers = new List<RingPolynomial>();
            switch (berlekampMode)
            {
                case BerlekampMode.Easy:
                    return PrepareInputBerlekamp(out dividers, 3,3,2,2,false );
                case BerlekampMode.PostMedium:
                    return PrepareInputBerlekamp(out dividers, 3, 3, 3, 3, true);
                case BerlekampMode.Medium:
                    return PrepareInputBerlekamp(out dividers, 4, 5, 3, 5, true);
                case BerlekampMode.Hard:
                    return PrepareInputBerlekamp(out dividers, 10, 10, 2, 7, true);
                case BerlekampMode.Ultra:
                    return PrepareInputBerlekamp(out dividers, 10, 30, 2, 15, true);
            }
            return new RingPolynomial();
        }
        static bool TestBerlekampFactor(RingPolynomial for_factorization)
        {

            
            RingPolynomial factorization = new RingPolynomial { 1 };
            var res = for_factorization.BerlekampFactor();
            res.ForEach((val) => factorization *= val);
            Program.Log("Произведение делителей:");
            factorization.Print();

            Program.LogEnabled = false;
            bool allAreIrreducible = res.All((val) => val.BerlekampFactor().Count == 1);
            Program.LogEnabled = true;

            return (factorization - for_factorization).IsNull() && allAreIrreducible;
        }

        static void Main(string[] args)
        {
            //TestSolvingEquation(new Polynomial { -1, 0, 0, 2, 1 }, new Polynomial { 1, 0, 0, 1 }, new Polynomial { 1, 0, 1 });
            RingPolynomial.SetModContext(17);

            List<RingPolynomial> dividers;
            TestBerlekampFactor(new RingPolynomial { 1, 1, 1 });
            Program.LogEnabled = true;

            RingPolynomial test;
            do
            { 
                test = PrepareInputBerlekamp(out dividers, BerlekampMode.Ultra);

            } while (TestBerlekampFactor(test));

            Console.WriteLine("Test wrong!");
            TestBerlekampFactor(test);

            Console.Read();
        }


        static void RingPolyTest()
        {
            RingPolynomial.SetModContext(7);
            tester test = () =>
            {
                Random rnd = new Random();
                int flen = rnd.Next(5, 20);
                int glen = rnd.Next(5, 21);
                int clen = rnd.Next(5, 22);
                RingPolynomial f = new RingPolynomial();
                RingPolynomial g = new RingPolynomial();
                RingPolynomial c = new RingPolynomial();
                for (int i = 0; i < flen; i++)
                {
                    f.Add(rnd.Next(0, RingBint.mod));
                }
                for (int i = 0; i < glen; i++)
                {
                    g.Add(rnd.Next(0, RingBint.mod));
                }
                for (int i = 0; i < clen; i++)
                {
                    c.Add(rnd.Next(0, RingBint.mod));
                }
                f = f.Normalize();
                g = g.Normalize();
                c = c.Normalize();
                RingPolynomial.GCD(f, g, out RingPolynomial gcd);
                var sol = RingPolynomial.SolveEquation(f, g, c);
                if (!sol.isDefined)
                    Console.WriteLine("Решение не найдено!");

                f.Print('f');
                g.Print('g');
                c.Print('c');
                Console.WriteLine(sol);

                if (f.IsNull() && !g.IsNull())
                {
                    if (c.IsNull())
                    {
                        return sol.zeroSolution.Y.IsNull() && (sol.solutionStep.X - new RingPolynomial { 1 }).IsNull() && sol.solutionStep.Y.IsNull();
                    }
                    else
                    {
                        if ((c / g).Reminder.IsNull())
                        {
                            return sol.zeroSolution.Y == (c / g).Quotient.Normalize() && sol.solutionStep.Y.IsNull() && (sol.solutionStep.X - new RingPolynomial { 1 }).IsNull();
                        }
                        else
                        {
                            return sol.isDefined == false;
                        }
                    }
                }
                if (!f.IsNull() && g.IsNull())
                {
                    if (c.IsNull())
                    {
                        return sol.zeroSolution.X.IsNull() && (sol.solutionStep.Y - new RingPolynomial { 1 }).IsNull() && sol.solutionStep.X.IsNull();
                    }
                    else
                    {
                        if ((c / f).Reminder.IsNull())
                        {
                            return sol.zeroSolution.X == (c / f).Quotient.Normalize() && sol.solutionStep.X.IsNull() && (sol.solutionStep.Y - new RingPolynomial { 1 }).IsNull();
                        }
                        else
                        {
                            return sol.isDefined == false;
                        }
                    }
                }
                if (f.IsNull() && g.IsNull())
                {
                    if (!c.IsNull())
                    {
                        return sol.isDefined == false;
                    }
                    else
                    {
                        return (sol.solutionStep.X - new RingPolynomial { 1 }).IsNull() && (sol.solutionStep.Y - new RingPolynomial { 1 }).IsNull();
                    }
                }
                if (c.IsNull())
                {
                    return (((sol.solutionStep.X) * f + (sol.solutionStep.Y) * g) - c).IsNull() && (sol.zeroSolution.X / sol.solutionStep.X).Reminder.IsNull() && (sol.zeroSolution.Y / sol.solutionStep.Y).Reminder.IsNull();
                }

                if ((c / gcd).Reminder.IsNull())
                    return (((sol.zeroSolution.X + sol.solutionStep.X) * f + (sol.zeroSolution.Y + sol.solutionStep.Y) * g) - c).IsNull();
                else
                    return sol.isDefined == false;
            };
            int i = 0;
            bool res;
            while (res = test())
            {
                Console.WriteLine("i =" + i + (res ? " OK" : "ERR"));
                i++;
            }
        }
    }
}
