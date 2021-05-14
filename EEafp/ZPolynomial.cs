using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Linq;

namespace EEafp
{
    public class ZPolynomial : AbstractPolynomial<BigInteger, ZPolynomial>
    {
        public override string SetLetter => "Z";

        public ZPolynomial() : base() { }
        public ZPolynomial(BigInteger[] Bints) : base(Bints) { }
        public ZPolynomial(ZPolynomial p1) : base(p1) { }
        public ZPolynomial(RingPolynomial p1)
        {
            for (int i = 0; i < p1.size; i++)
            {
                BigInteger curElem = new BigInteger(p1[i]);
                this.Add(curElem);
            }
        }
        public ZPolynomial(int size) : base(size) { }


        public override ZPolynomial Normalize()
        {
            int i;
            for (i = degree; i >= 0 && this[i] == 0; i--) ;
            if (i == -1)
            {
                return new ZPolynomial();
            }
            else
            {
                coef.RemoveRange(i + 1, coef.Count - i - 1);
                coef.Capacity = i + 1;
                return this;
            }
        }
        public override ZPolynomial Pow(int times)
        {
            ZPolynomial res = new ZPolynomial(this);
            for (int i = 0; i < times; i++)
            {
                res *= res;
            }
            return res;
        }
        public ZPolynomial Derivative()
        {
            ZPolynomial res = new ZPolynomial(size - 1);

            for (int i = 0; i < res.size; i++)
            {
                res[i] = this[i + 1] * (i + 1);
            }
            return res;
        }
        public override ZPolynomial Shift(int nyam)
        {
            ZPolynomial res = new ZPolynomial(this);

            res.coef.InsertRange(0, Enumerable.Repeat(new BigInteger(), nyam).ToArray());

            return res;
        }

        #region operators

        protected override ZPolynomial PolySum(ZPolynomial p2)
        {
            ZPolynomial p1 = this;
            ZPolynomial maxp;
            ZPolynomial minp;
            if (p2.degree > p1.degree)
            {
                maxp = new ZPolynomial(p2);
                minp = p1;
            }
            else
            {
                maxp = new ZPolynomial(p1);
                minp = p2;
            }

            for (int i = 0; i < minp.size; i++)
            {
                maxp[i] = maxp[i] + minp[i];
            }

            return maxp.Normalize();
        }

        protected override ZPolynomial PolyMin()
        {
            ZPolynomial p1 = this;
            ZPolynomial res = new ZPolynomial(p1);

            for (int i = 0; i < p1.size; i++)
            {
                res[i] = 0 - res[i];
            }
            return res;
        }
        protected override ZPolynomial PolyMin(ZPolynomial p2)
        {
            ZPolynomial p1 = this;
            return (p1 + (-p2)).Normalize();
        }

        protected override ZPolynomial PolyMul(ZPolynomial p2)
        {
            ZPolynomial p1 = this;
            ZPolynomial res = new ZPolynomial(p1.degree + p2.degree + 1);

            for (int i = 0; i < p1.size; i++)
            {
                for (int j = 0; j < p2.size; j++)
                {
                    res[i + j] = res[i + j] + p1[i] * p2[j];
                }
            }
            return res;
        }

        protected override ZPolynomial PolyMul(BigInteger nyam)
        {
            ZPolynomial p1 = this;
            if (nyam == 0)
            {
                return new ZPolynomial();
            }

            ZPolynomial res = new ZPolynomial(p1);

            for (int i = 0; i < res.size; i++)
            {
                res[i] = nyam * res[i];
            }

            return res;
        }

        protected override DividionResult PolyDiv(ZPolynomial p2)
        {
            ZPolynomial p1 = this;
            ZPolynomial r = new ZPolynomial(p1);
            ZPolynomial q = new ZPolynomial();
            if (p2.IsNull())
                throw new DivideByZeroException("Why are you gay?");
            for (int i = p1.degree; i >= p2.degree; i--)
            {
                BigInteger curq;
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

        protected override ZPolynomial PolyDiv(BigInteger nyam)
        {
            ZPolynomial p1 = this;
            ZPolynomial res = new ZPolynomial(p1);
            for (int i = 0; i < p1.size; i++)
            {
                res[i] *= nyam;
            }
            return res;
        }

        protected override GCDResult PolyGCD(ZPolynomial g, out ZPolynomial gcd)
        {
            ZPolynomial f = this;
            GCDResult A = new GCDResult(new ZPolynomial { 1 }, new ZPolynomial { 0 });
            GCDResult B = new GCDResult(new ZPolynomial { 0 }, new ZPolynomial { 1 });
            GCDResult Temp;
            ZPolynomial a, b;
            if (f.degree > g.degree)
            {
                a = new ZPolynomial(f);
                b = new ZPolynomial(g);
            }
            else
            {
                a = new ZPolynomial(g);
                b = new ZPolynomial(f);
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
    }
}
