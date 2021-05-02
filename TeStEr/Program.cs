using System;

namespace EEafp
{
    class Program
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
        static void Main(string[] args)
        {
            TestSolvingEquation(new Polynomial { -1, 0, 0, 2, 1 }, new Polynomial { 1, 0, 0, 1 }, new Polynomial { 1, 0, 1 });


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
