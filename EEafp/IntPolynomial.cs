using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Linq;

namespace EEafp
{
    public class IntPolynomial : AbstractPolynomial<BigInteger, IntPolynomial>
    {
        public override string SetLetter => "Z";

        public IntPolynomial() : base() { }
        public IntPolynomial(BigInteger[] Bints) : base(Bints) { }
        public IntPolynomial(IntPolynomial p1) : base(p1) { }
        public IntPolynomial(RingPolynomial p1)
        {
            for (int i = 0; i < p1.size; i++)
            {
                BigInteger curElem = p1[i];
                this.Add(curElem);
            }
        }
        public IntPolynomial(int size) : base(size) { }


        public override IntPolynomial Normalize()
        {
            int i;
            for (i = degree; i >= 0 && this[i] == 0; i--) ;
            if (i == -1)
            {
                return new IntPolynomial();
            }
            else
            {
                coef.RemoveRange(i + 1, coef.Count - i - 1);
                coef.Capacity = i + 1;
                return this;
            }
        }
        public override IntPolynomial Pow(int times)
        {
            IntPolynomial res = new IntPolynomial(this);
            for (int i = 0; i < times; i++)
            {
                res *= res;
            }
            return res;
        }
        public IntPolynomial Derivative()
        {
            IntPolynomial res = new IntPolynomial(size - 1);

            for (int i = 0; i < res.size; i++)
            {
                res[i] = this[i + 1] * (i + 1);
            }
            return res;
        }
        public override IntPolynomial Shift(int nyam)
        {
            IntPolynomial res = new IntPolynomial(this);

            res.coef.InsertRange(0, Enumerable.Repeat(new BigInteger(), nyam).ToArray());

            return res;
        }

        #region operators

        protected override IntPolynomial PolySum(IntPolynomial p2)
        {
            IntPolynomial p1 = this;
            IntPolynomial maxp;
            IntPolynomial minp;
            if (p2.degree > p1.degree)
            {
                maxp = new IntPolynomial(p2);
                minp = p1;
            }
            else
            {
                maxp = new IntPolynomial(p1);
                minp = p2;
            }

            for (int i = 0; i < minp.size; i++)
            {
                maxp[i] = maxp[i] + minp[i];
            }

            return maxp.Normalize();
        }

        protected override IntPolynomial PolyMin()
        {
            IntPolynomial p1 = this;
            IntPolynomial res = new IntPolynomial(p1);

            for (int i = 0; i < p1.size; i++)
            {
                res[i] = 0 - res[i];
            }
            return res;
        }
        protected override IntPolynomial PolyMin(IntPolynomial p2)
        {
            IntPolynomial p1 = this;
            return (p1 + (-p2)).Normalize();
        }

        protected override IntPolynomial PolyMul(IntPolynomial p2)
        {
            IntPolynomial p1 = this;
            IntPolynomial res = new IntPolynomial(p1.degree + p2.degree + 1);

            for (int i = 0; i < p1.size; i++)
            {
                for (int j = 0; j < p2.size; j++)
                {
                    res[i + j] = res[i + j] + p1[i] * p2[j];
                }
            }
            return res;
        }

        protected override IntPolynomial PolyMul(BigInteger nyam)
        {
            IntPolynomial p1 = this;
            if (nyam == 0)
            {
                return new IntPolynomial();
            }

            IntPolynomial res = new IntPolynomial(p1);

            for (int i = 0; i < res.size; i++)
            {
                res[i] = nyam * res[i];
            }

            return res;
        }

        protected override DividionResult PolyDiv(IntPolynomial p2)
        {
            IntPolynomial p1 = this;
            IntPolynomial r = new IntPolynomial(p1);
            IntPolynomial q = new IntPolynomial();
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
                {
                    if(r[i] % p2[p2.degree] != 0)
                    {
                        throw new Exception("U gay v2.0");
                    }
                    curq = r[i] / p2[p2.degree];
                }

                r -= (p2 * curq).Shift(i - p2.degree);
                q = q.Shift(1);
                q[0] = curq;
            }
            r.Normalize();
            return new DividionResult(q, r);
        }

        protected override IntPolynomial PolyDiv(BigInteger nyam)
        {
            IntPolynomial p1 = this;
            IntPolynomial res = new IntPolynomial(p1);
            for (int i = 0; i < p1.size; i++)
            {
                if(res[i] % nyam != 0)
                    throw new Exception("wrong way dude");
                res[i] /= nyam;
            }
            return res;
        }

        public static BigInteger NyamGCD(BigInteger a, BigInteger b)
        {
            BigInteger x = BigInteger.Abs(a);
            BigInteger y = BigInteger.Abs(b);
            BigInteger r;
            while (y != 0)
            {
                r = x % y;
                x = y;
                y = r;
            }
            return x;
        }

        public static bool isNyamPrime(BigInteger nyam)
        {
           if (nyam > 1)
            {
                BigInteger i = 2;
                while (i*i <= nyam)
                {
                    if (nyam % i == 0) return false;
                    i++;
                }
                return true;
            }
            return false;
        }

        public static BigInteger getNextPrime(BigInteger currPrime)
        {
            BigInteger nextPrime = currPrime + 1;
            while (!isNyamPrime(nextPrime)) nextPrime++;
            return nextPrime;
        }

        public BigInteger CoeffGCD()
        {
            BigInteger CurrGCD = this[0];
            for (int i=1; i < this.size; i++)
            {
                CurrGCD = NyamGCD(CurrGCD, this[i]);
            }
            return CurrGCD;
        }

        protected override GCDResult PolyGCD(IntPolynomial g, out IntPolynomial gcd)
        {
            IntPolynomial f = this;
            GCDResult A = new GCDResult(new IntPolynomial { 1 }, new IntPolynomial { 0 });
            GCDResult B = new GCDResult(new IntPolynomial { 0 }, new IntPolynomial { 1 });
            IntPolynomial a, b;
            if (f.degree > g.degree)
            {
                a = new IntPolynomial(f);
                b = new IntPolynomial(g);
            }
            else
            {
                a = new IntPolynomial(g);
                b = new IntPolynomial(f);
            }
            BigInteger CoeffReminder;
            while (!b.IsNull())
            {
                BigInteger CurrentMultiplyCoeff = BigInteger.Pow(b[b.size - 1], (a.size - 1) - (b.size - 1) + 1);
                DividionResult divres = (a * CurrentMultiplyCoeff) / b;
                a = b;
                CoeffReminder = divres.Item2.CoeffGCD();
                b = divres.Item2 / CoeffReminder;
            }
            CoeffReminder = a.CoeffGCD();
            gcd = a / CoeffReminder;
            if (gcd[gcd.size-1] < 0)
            {
                gcd *= -1;
            }
            return new GCDResult(A.Item1, A.Item2);
        }
        #endregion

        public static BigInteger SelectAppropriateMod(IntPolynomial f, IntPolynomial SquareFreef)
        {
            BigInteger mod = 2;
            while (true)
            {
                while (f[f.degree] % mod == 0)
                {
                    mod = getNextPrime(mod);
                }
                RingPolynomial.SetModContext(mod);
                RingPolynomial Ringf = new RingPolynomial(SquareFreef);
                RingPolynomial.GCD(Ringf, Ringf.Derivative(), out RingPolynomial gcdRes);
                if (gcdRes.degree < 1 && ((Ringf / gcdRes).Quotient).FindNumOfMultipliers() > 0)
                    break;

                mod = getNextPrime(mod);
            }
            return mod;
        }

        public RingDecomposeList FactorIntPolynomialOverBigModule(out RingDecomposeList fFactorization)
        {
            IntPolynomial f = this;
            IntPolynomial hasSquares;
            IntPolynomial.GCD(f, f.Derivative(), out hasSquares);
            IntPolynomial SquareFreef = (f / hasSquares).Quotient;

            RingDecomposeList LiftedFactorisation;

            if (SquareFreef.degree > 1)
            {
                BigInteger mod = SelectAppropriateMod(f, SquareFreef);
                RingPolynomial.SetModContext(mod);
                RingPolynomial fRing = new RingPolynomial(f);
                fFactorization = fRing.BerlekampFactor();
                List<RingPolynomial> GCDCoeffs = RingPolynomial.GetGCDCoefficientForHensel(fFactorization);

                LiftedFactorisation = RingPolynomial.HenselLiftingUntilTheEnd(f, fFactorization, GCDCoeffs);
                LiftedFactorisation.polyCoef = f[f.size - 1];
            } else
            {
                RingPolynomial.SetModContext(5);
                // f состоит из кратных множителей первой степени
                LiftedFactorisation = new RingDecomposeList();
                BigInteger coeff = SquareFreef.CoeffGCD();
                IntPolynomial Multiplier = SquareFreef / coeff;

                // находим наибольшее число в f, чтобы подобрать модуль
                BigInteger biggestCoeff = f[0];
                for (int i=0; i < f.size; i++)
                {
                    if (f[i] > biggestCoeff) 
                        biggestCoeff = f[i];
                }

                BigInteger mod = getNextPrime(biggestCoeff);
                RingPolynomial.SetModContext(mod);

                for (int i = 0; i < f.size - 1; i++) LiftedFactorisation.Add(new RingPolynomial(Multiplier));
                LiftedFactorisation.polyCoef *= (RingBint)coeff;
                fFactorization = LiftedFactorisation;
            }
            return LiftedFactorisation;
        }
    }
}
