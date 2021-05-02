using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;


namespace EEafp
{
    using Bint = System.Decimal;

    public class Polynomial : AbstractPolynomial<Bint, Polynomial>
    {
        public override string SetLetter => "R";

        public Polynomial() : base() { }
        public Polynomial(Polynomial poly) : base(poly) { }
        public Polynomial(int size) : base(size) { }

        public override Polynomial Normalize()
        {
            int i;
            for (i = degree; i >= 0 && (long)(coef[i] * 1000000000) == 0; i--)
                if (degree == 0 && (long)(coef[i] * 1000000000) == 0)
                    coef[0] = 0;
            if (i == -1)
            {
                return new Polynomial();
            }
            else
            {
                coef.RemoveRange(i + 1, coef.Count - i - 1);
                coef.Capacity = i + 1;
                return this;
            }
        }
        public override Polynomial Pow(int times)
        {
            Polynomial res = new Polynomial(this);
            for (int i = 0; i < times; i++)
            {
                res *= res;
            }
            return res;
        }
        public override Polynomial Shift(int nyam)
        {
            Polynomial res = new Polynomial(this);

            res.coef.InsertRange(0, Enumerable.Repeat(new Bint(), nyam).ToArray());

            return res;
        }


        #region operators
        protected override Polynomial PolySum(Polynomial p2)
        {
            Polynomial p1 = this;
            Polynomial maxp;
            Polynomial minp;
            if (p1.degree > p2.degree)
            {
                maxp = new Polynomial(p1);
                minp = p2;
            }
            else
            {
                maxp = new Polynomial(p2);
                minp = p1;
            }

            for (int i = 0; i < minp.size; i++)
            {
                maxp[i] = maxp[i] + minp[i];
            }

            return maxp.Normalize();
        }
        protected override Polynomial PolyMin()
        {
            Polynomial p1 = this;
            Polynomial res = new Polynomial(p1);

            for (int i = 0; i < p1.size; i++)
            {
                res[i] = 0 - res[i];
            }
            return res;
        }
        protected override Polynomial PolyMin(Polynomial p2)
        {
            Polynomial p1 = this;
            return (p1 + (-p2)).Normalize();
        }


        protected override Polynomial PolyMul(Polynomial p2)
        {
            Polynomial p1 = this;
            Polynomial res = new Polynomial(p1.degree + p2.degree + 1);

            for (int i = 0; i < p1.size; i++)
            {
                for (int j = 0; j < p2.size; j++)
                {
                    res[i + j] += p1[i] * p2[j];
                }
            }
            return res;
        }
        protected override Polynomial PolyMul(Bint nyam)
        {
            Polynomial p1 = this;
            if (nyam == 0)
            {
                return new Polynomial();
            }

            Polynomial res = new Polynomial(p1);

            for (int i = 0; i < res.size; i++)
            {
                res[i] = nyam * res[i];
            }

            return res;
        }
        protected override DividionResult PolyDiv(Polynomial p2)
        {
            Polynomial p1 = this;
            Polynomial r = new Polynomial(p1);
            Polynomial q = new Polynomial();
            if (p2.IsNull())
                throw new DivideByZeroException("Why are you gay?");
            for (int i = p1.degree; i >= p2.degree; i--)
            {
                Bint curq;
                if (i > r.degree)
                {
                    q = q.Shift(1);
                    continue;
                }
                else
                    curq = r[i] / p2[p2.degree];

                r -= (p2 * curq).Shift(i - p2.degree);
                q = q.Shift(1);
                q[0] = curq;
            }
            r.Normalize();
            return new DividionResult(q, r);
        }
        protected override GCDResult PolyGCD(Polynomial g, out Polynomial gcd)
        {
            Polynomial f = this;
            GCDResult A = new GCDResult(new Polynomial { 1 }, new Polynomial { 0 });
            GCDResult B = new GCDResult(new Polynomial { 0 }, new Polynomial { 1 });
            GCDResult Temp;
            Polynomial a, b;
            if (f.degree > g.degree)
            {
                a = new Polynomial(f);
                b = new Polynomial(g);
            }
            else
            {
                a = new Polynomial(g);
                b = new Polynomial(f);
            }
            while (!b.IsNull())
            {
                DividionResult divres = a / b;
                a = b;
                b = divres.Item2;
                Temp = new GCDResult(A - new GCDResult(B.Item1 * divres.Quotient, B.Item2 * divres.Quotient));
                A = B;
                B = Temp;

            }
            gcd = a;
            if (f.degree <= g.degree)
            {
                A = new GCDResult(A.Coef2, A.Coef1);
            }
            if (gcd[gcd.degree] < 0)
            {
                gcd = -gcd;
                return new GCDResult(-A.Item1, -A.Item2);
            }
            return new GCDResult(A.Item1, A.Item2);
        }


        #endregion

        public static FullSolution SolveEquation(Polynomial f, Polynomial g, Polynomial c)
        {
            FullSolution solution = new FullSolution();

            //f=0; g=0; c=0 | c!=0
            if (f.IsNull() && g.IsNull())
            {
                if (c.IsNull())
                {
                    solution.zeroSolution = new Solution(new Polynomial(), new Polynomial());
                    solution.solutionStep = new Solution(null, null);
                    solution.areCoefsIndependent = true;
                }
                else
                {
                    solution.isDefined = false;
                }
                return solution;
            }

            if (c.IsNull() && f.IsNull() != g.IsNull())
            {
                solution.zeroSolution = new Solution(new Polynomial(), new Polynomial());
                if (f.IsNull() && !g.IsNull())
                {
                    solution.solutionStep = new Solution(null, new Polynomial());
                }
                else if (!f.IsNull() && g.IsNull())
                {
                    solution.solutionStep = new Solution(new Polynomial(), null);
                }
                return solution;
            }
            else if (f.IsNull() != g.IsNull())
            {

                if (f.IsNull())
                {
                    DividionResult divres = c / g;
                    if (!divres.Reminder.IsNull())
                    {
                        solution.isDefined = false;
                        return solution;
                    }

                    solution.solutionStep = new Solution(null, new Polynomial { 0 });
                    solution.zeroSolution = new Solution(new Polynomial { 0 }, divres.Quotient);
                }
                else
                {
                    DividionResult divres = c / f;
                    if (!divres.Reminder.IsNull())
                    {
                        solution.isDefined = false;
                        return solution;
                    }
                    solution.solutionStep = new Solution(new Polynomial { 0 }, null);
                    solution.zeroSolution = new Solution(divres.Quotient, new Polynomial { 0 });
                }

                return solution;
            }

            Polynomial gcd = new Polynomial();
            GCDResult expCoefs = GCD(f, g, out gcd);
            DividionResult tempX0 = (expCoefs.Coef1 * c / gcd);
            DividionResult tempY0 = (expCoefs.Coef2 * c / gcd);

            // К этому моменту f и g  не нулевые (ветвление для c)
            if (c.IsNull())
            {
                solution.zeroSolution = new Solution(new Polynomial(), new Polynomial());
                solution.solutionStep = new Solution((g / gcd).Quotient, -(f / gcd).Quotient);
            }
            else
            {
                if (tempX0.Reminder.IsNull() && tempY0.Reminder.IsNull())
                {
                    solution.zeroSolution = new Solution(tempX0.Quotient, tempY0.Quotient);
                    solution.solutionStep = new Solution((g / gcd).Quotient, -(f / gcd).Quotient);
                }
                else
                {
                    return new FullSolution() { isDefined = false };
                }
            }

            return solution;
        }
    }
}
