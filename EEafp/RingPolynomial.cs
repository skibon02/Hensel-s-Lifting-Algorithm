using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EEafp
{
    public class RingBint
    {
        public static int mod;

        private int _val;
        public int val
        {
            get
            {
                return _val;
            }
            set
            {
                _val = ((value % mod) + mod) % mod;
            }
        }
        public RingBint(int v)
        {
            val = v;
        }
        public RingBint()
        {
            val = 0;
        }


        public static RingBint operator +(RingBint fir, RingBint sec)
        {
            return (fir.val + sec.val) % mod;
        }
        public static RingBint operator -(RingBint fir, RingBint sec)
        {
            return (fir.val - sec.val + mod) % mod;
        }
        public static RingBint operator *(RingBint fir, RingBint sec)
        {
            return (fir.val * sec.val) % mod;
        }
        public static RingBint operator /(RingBint fir, RingBint sec)
        {
            for (int i = 0; i < mod; i++)
            {
                if (i * sec.val % mod == fir.val)
                    return i;
            }

            throw new DivideByZeroException("Why are you gay?");
        }
        public static implicit operator int(RingBint el)
        {
            return el.val;
        }
        public static implicit operator RingBint(int el)
        {
            return new RingBint(el);
        }
        public override string ToString()
        {
            return val.ToString();
        }

        public override bool Equals(object obj)
        {
            return this.val == (obj as RingBint).val;
        }
    }

    public class RingPolynomial : AbstractPolynomial<RingBint, RingPolynomial>
    {
        public override string SetLetter => "Z";

        public static void SetModContext(int mod)
        {
            if (mod > 0)
                RingBint.mod = mod;
            else
                throw new ArgumentException("Wrong mod value!");
        }

        public RingPolynomial() : base() { }
        public RingPolynomial(RingBint[] ringBints) : base(ringBints) { }
        public RingPolynomial(RingPolynomial p1) : base(p1) { }
        public RingPolynomial(int size) : base(size) { }


        public override RingPolynomial Normalize()
        {
            int i;
            for (i = degree; i >= 0 && this[i] == 0; i--) ;
            if (i == -1)
            {
                return new RingPolynomial();
            }
            else
            {
                coef.RemoveRange(i + 1, coef.Count - i-1);
                coef.Capacity = i + 1;
                return this;
            }
        }
        public override RingPolynomial Pow(int times)
        {
            RingPolynomial res = new RingPolynomial(this);
            for (int i = 0; i < times; i++)
            {
                res *= res;
            }
            return res;
        }
        public RingPolynomial Derivative()
        {
            RingPolynomial res = new RingPolynomial(size-1);

            for(int i = 0; i < res.size; i++)
            {
                res[i] = this[i + 1] * (i + 1);
            }
            return res;
        }
        public override RingPolynomial Shift(int nyam)
        {
            RingPolynomial res = new RingPolynomial(this);

            res.coef.InsertRange(0, Enumerable.Repeat(new RingBint(), nyam).ToArray());

            return res;
        }

        #region operators

        protected override RingPolynomial PolySum(RingPolynomial p2)
        {
            RingPolynomial p1 = this;
            RingPolynomial maxp;
            RingPolynomial minp;
            if (p2.degree > p1.degree)
            {
                maxp = new RingPolynomial(p2);
                minp = p1;
            }
            else
            {
                maxp = new RingPolynomial(p1);
                minp = p2;
            }

            for (int i = 0; i < minp.size; i++)
            {
                maxp[i] = maxp[i] + minp[i];
            }

            return maxp.Normalize();
        }

        protected override RingPolynomial PolyMin()
        {
            RingPolynomial p1 = this;
            RingPolynomial res = new RingPolynomial(p1);

            for (int i = 0; i < p1.size; i++)
            {
                res[i] = 0 - res[i];
            }
            return res;
        }
        protected override RingPolynomial PolyMin(RingPolynomial p2)
        {
            RingPolynomial p1 = this;
            return (p1 + (-p2)).Normalize();
        }

        protected override RingPolynomial PolyMul(RingPolynomial p2)
        {
            RingPolynomial p1 = this;
            RingPolynomial res = new RingPolynomial(p1.degree + p2.degree + 1);

            for (int i = 0; i < p1.size; i++)
            {
                for (int j = 0; j < p2.size; j++)
                {
                    res[i + j] = res[i + j] + p1[i] * p2[j];
                }
            }
            return res;
        }

        protected override RingPolynomial PolyMul(RingBint nyam)
        {
            RingPolynomial p1 = this;
            if (nyam == 0)
            {
                return new RingPolynomial();
            }

            RingPolynomial res = new RingPolynomial(p1);

            for (int i = 0; i < res.size; i++)
            {
                res[i] = nyam * res[i];
            }

            return res;
        }

        protected override DividionResult PolyDiv(RingPolynomial p2)
        {
            RingPolynomial p1 = this;
            RingPolynomial r = new RingPolynomial(p1);
            RingPolynomial q = new RingPolynomial();
            if (p2.IsNull())
                throw new DivideByZeroException("Why are you gay?");
            for (int i = p1.degree; i >= p2.degree; i--)
            {
                RingBint curq;
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

        protected override GCDResult PolyGCD(RingPolynomial g, out RingPolynomial gcd)
        {
            RingPolynomial f = this;
            GCDResult A = new GCDResult(new RingPolynomial { 1 }, new RingPolynomial { 0 });
            GCDResult B = new GCDResult(new RingPolynomial { 0 }, new RingPolynomial { 1 });
            GCDResult Temp;
            RingPolynomial a, b;
            if (f.degree > g.degree)
            {
                a = new RingPolynomial(f);
                b = new RingPolynomial(g);
            }
            else
            {
                a = new RingPolynomial(g);
                b = new RingPolynomial(f);
            }
            while (!b.IsNull())
            {
                DividionResult divres;
                try
                {
                    divres = a / b;
                }
                catch (DivideByZeroException)
                {
                    gcd = new RingPolynomial(a);
                    if(f.IsNull())
                    {
                        return new GCDResult(new RingPolynomial { 0 }, new RingPolynomial { 1 });
                    }
                    else
                    {
                        return new GCDResult(new RingPolynomial { 1 }, new RingPolynomial { 0 });
                    }
                }


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

        public static FullSolution SolveEquation(RingPolynomial f, RingPolynomial g, RingPolynomial c)
        {
            FullSolution solution = new FullSolution();

            //f=0; g=0; c=0 | c!=0
            if (f.IsNull() && g.IsNull())
            {
                if(c.IsNull())
                {
                    solution.zeroSolution = new Solution(new RingPolynomial(), new RingPolynomial());
                    solution.solutionStep = new Solution(new RingPolynomial { 1 }, new RingPolynomial { 1 });
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
                solution.zeroSolution = new Solution(new RingPolynomial(), new RingPolynomial());
                if (f.IsNull() && !g.IsNull())
                {
                    solution.solutionStep = new Solution(new RingPolynomial { 1 }, new RingPolynomial());
                }
                else if (!f.IsNull() && g.IsNull())
                {
                    solution.solutionStep = new Solution(new RingPolynomial(), new RingPolynomial { 1 });
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

                    solution.solutionStep = new Solution( new RingPolynomial { 1 }, new RingPolynomial { 0 } );
                    solution.zeroSolution = new Solution( new RingPolynomial { 0 }, divres.Quotient);
                }
                else
                {
                    DividionResult divres = c / f;
                    if (!divres.Reminder.IsNull())
                    {
                        solution.isDefined = false;
                        return solution;
                    }
                    solution.solutionStep = new Solution(new RingPolynomial { 0 }, new RingPolynomial { 1 });
                    solution.zeroSolution = new Solution( divres.Quotient, new RingPolynomial { 0 });
                }

                return solution;
            }

            RingPolynomial gcd = new RingPolynomial();
            GCDResult expCoefs = GCD(f, g, out gcd);
            DividionResult tempX0 = (expCoefs.Coef1* c / gcd);
            DividionResult tempY0 = (expCoefs.Coef2 * c / gcd);

            // К этому моменту f и g  не нулевые (ветвление для c)
            if(c.IsNull())
            {
                solution.zeroSolution = new Solution(new RingPolynomial(), new RingPolynomial());
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

        public List<RingPolynomial> BerlekampFactor()
        {
            // если многочлен меньше второй степени, то и раскладывать смысла нет
            if (this.degree < 2) return new List<RingPolynomial> { this };
            // запоминаем изначальный коэффициент многочлена
            RingBint originalPolyCoefficient = this.coef[this.size - 1];
            List<RingPolynomial> factorRes = new List<RingPolynomial>();
            RingPolynomial currPoly = this;

            // убираем кратные множители
            RingPolynomial gcdDerivative;
            GCD(this, this.Derivative(), out gcdDerivative);
            if (this == gcdDerivative)
            {
                // тогда f(x) = g(x)^p, но тогда f(x) = g(x^p) - воспользуемся этим
                RingPolynomial g = new RingPolynomial((gcdDerivative.size-1)/RingBint.mod + 1);
                for (int i=0; i < g.size; i++)
                {
                    g[i] = gcdDerivative[i*RingBint.mod];
                }
                List<RingPolynomial> gFactorRes = g.BerlekampFactor();
                for (int i=0; i< RingBint.mod; i++)
                {
                    for (int j = 0; j < gFactorRes.Count; j++)
                    {
                        factorRes.Add(gFactorRes[j]);
                    }
                }
                return factorRes;
            } else if (gcdDerivative.degree > 0)
            {
                for (int i = 0; i < gcdDerivative.size; i++) gcdDerivative.coef[i] /= gcdDerivative.coef[gcdDerivative.size - 1];
                currPoly = (this / gcdDerivative).Quotient;
                List<RingPolynomial> factorResForGcdDerivative = gcdDerivative.BerlekampFactor();
                for (int i = 0; i < factorResForGcdDerivative.Count; i++)
                {
                    factorRes.Add(factorResForGcdDerivative[i]);
                }
            } 
            RingPolynomial poly = currPoly;
            RingBint[,] matrix = new RingBint[poly.degree-1, poly.degree];
            RingBint[][] decomposingPoly;
            for (int i = 1; i < poly.degree; i++)
            {
                RingPolynomial edinica = new RingPolynomial(i * RingBint.mod+1);
                edinica[i * RingBint.mod] = 1;
                var reminder = (edinica / poly).Reminder;
                for(int j = 0; j < poly.degree; j++)
                {
                    matrix[i-1, j] = reminder[j]; 
                }
                matrix[i-1, i ] -= 1;
            }
            RingMatrix.PrintMatrix(matrix);
            RingBint[,] newmatrix = new RingBint[poly.degree, poly.degree - 1];
            for(int i = 0; i < poly.degree; i++)
            {
                for(int j = 0; j < poly.degree-1; j++)
                {
                    newmatrix[i, j] = matrix[j, i];
                }
            }
            RingMatrix.PrintMatrix(newmatrix);
            decomposingPoly = RingMatrix.GaussSumplifyForBerclecampFactor(newmatrix);
            RingMatrix.PrintMatrix(newmatrix);
            if (decomposingPoly.Length > 0)
            {
                RingMatrix.PrintAnswer(decomposingPoly);

                // найдем разложение c использованием первого вектора
                RingPolynomial oneDecomposePoly = new RingPolynomial(decomposingPoly[0]);
                RingPolynomial gcd_result = new RingPolynomial();
                for (int j = 0; j < RingBint.mod; j++)
                {
                    oneDecomposePoly[0] += j;
                    GCD(this, oneDecomposePoly, out gcd_result);
                    for (int k = 0; k < gcd_result.size; k++)  // gcd с точностью до постоянного множителя избавляемся от него
                        gcd_result.coef[k] /= gcd_result.coef[gcd_result.size - 1];
                    if (gcd_result.degree > 0)
                    {
                        factorRes.Add(gcd_result);
                    }
                    oneDecomposePoly[0] -= j;
                }
                // Если нашли не все векторы разложения продолжаем поиск
                if (factorRes.Count < decomposingPoly.Length + 1)
                {
                    bool all_find = false;
                    for (int p=0; p < factorRes.Count && !all_find; p++)
                    {
                        RingPolynomial polyForFactor = new RingPolynomial(factorRes[p]);
                        for (int l = 1; l < decomposingPoly.Length && !all_find; l++)
                        {
                            oneDecomposePoly = new RingPolynomial(decomposingPoly[l]);
                            for (int i = 1; i < decomposingPoly.Length && !all_find; i++)
                            {
                                oneDecomposePoly = new RingPolynomial(decomposingPoly[i]);
                                for (int j = 0; j < RingBint.mod && !all_find; j++)
                                {
                                    oneDecomposePoly[0] += j;
                                    GCD(polyForFactor, oneDecomposePoly, out gcd_result);
                                    for (int k = 0; k < gcd_result.size; k++)  // gcd с точностью до постоянного множителя избавляемся от него
                                        gcd_result.coef[k] /= gcd_result.coef[gcd_result.size - 1];
                                    if (gcd_result.degree > 0 && factorRes.All(x => x != gcd_result))
                                    {
                                        if (factorRes[p] == polyForFactor)
                                        {
                                            factorRes.RemoveAt(p);
                                        }
                                        factorRes.Add(gcd_result);
                                        if (factorRes.Count == decomposingPoly.Length + 1)
                                        {
                                            all_find = true;
                                            break;
                                        }
                                    }
                                    oneDecomposePoly[0] -= j;
                                }
                            }
                        }
                    }
                }
                factorRes[0] *= originalPolyCoefficient;
            } else
            {
                factorRes.Add(poly);
            }

            return factorRes;
        }

    }
}
