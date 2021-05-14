using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

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

        public RingPolynomial(ZPolynomial p1)
        {
            for (int i = 0; i < p1.size; i++)
            {
                int currElem = (int)(p1[i] % RingBint.mod);
                this.Add(currElem);
            }
        }

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

        protected override RingPolynomial PolyDiv(RingBint nyam)
        {
            RingPolynomial p1 = this;
            RingPolynomial res = new RingPolynomial(p1);
            for (int i = 0; i < p1.size; i++)
            {
                res[i] /= nyam;
            }
            return res;
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


        public RingDecomposeList BerlekampFactor()
        {
            Program.recDepth++;
            RingPolynomial poly = this.Normalize();

            if (this.degree < 2) // если многочлен меньше второй степени, то и раскладывать смысла нет
            {
                Program.Log("Разложение для " + poly + " не требуется.");
                //end
                Program.recDepth--;
                return new RingDecomposeList { poly };
            }

            Program.Log("Поиск разложения для многочлена:");
            if (Program.LogEnabled) poly.Print('a');
            RingDecomposeList factorRes = new RingDecomposeList(); // результат - список делителей


            // 1) убираем кратные множители
            RingPolynomial gcdDerivative;
            GCD(this, this.Derivative(), out gcdDerivative);
            if (this == gcdDerivative)
            {
                // 1.1) особый случай, когда производная равна 0. Тогда f(x) = g(x)^p, но тогда f(x) = g(x^p) - воспользуемся этим
                Program.Log("Обнаружена общая степень кратная модулю поля (" + RingBint.mod + ")");
                RingPolynomial g = new RingPolynomial((gcdDerivative.size-1)/RingBint.mod + 1);
                for (int i=0; i < g.size; i++)
                {
                    g[i] = gcdDerivative[i*RingBint.mod];
                }
                RingDecomposeList gFactorRes = g.BerlekampFactor();
                for (int i=0; i< RingBint.mod; i++)
                {
                    factorRes.AddRange(gFactorRes);
                }
                //end
                Program.Log("Результат разложения:");
                if (Program.LogEnabled)
                    for (int i = 0; i < factorRes.Count; i++)
                    {
                        factorRes[i].Print('g');
                    }
                Program.recDepth--;
                return factorRes;
            } else if (gcdDerivative.degree > 0)
            {
                //1.2) Обнаружены кратные множители. Разделение исходного многочлена a(x) на b(x) * c(x)
                //     - c(x) гарантированно не содержит кратных корней
                //     - b(x) - это оставшаяся часть обрабатывается рекурсивно
                gcdDerivative *= (new RingBint(1) / gcdDerivative[gcdDerivative.degree]);
                Program.Log("Обнаружены кратные множители. Рекурсивный вызов для b(x) = " + gcdDerivative);
                poly = (poly / gcdDerivative).Quotient; //текущий многочлен теперь c(x)
                RingDecomposeList factorResForGcdDerivative = gcdDerivative.BerlekampFactor(); //рекурсивный вызов для b(x)
                factorRes.AddRange(factorResForGcdDerivative);
            } 
            else
            {
                Program.Log("Кратные множители отсутствуют.");
            }

            // 2) Обработка c(x)
            Program.Log("Разложение многочлена без кратных корней c(x) = " + poly);
            if (poly.degree < 2) // если многочлен меньше второй степени, то и раскладывать смысла нет
            {
                Program.Log("Разложение для " + poly + " не требуется.");
                factorRes.Add(poly);
                //end
                Program.recDepth--;
                return factorRes;
            }
            // 2.1) Построение матрицы и нахождение ФСР ОСЛУ
            RingBint[,] matrix = new RingBint[poly.degree, poly.degree-1]; // транспонированное заполнение матрицы
            RingBint[][] decomposingPoly;
            for (int i = 1; i < poly.degree; i++)
            {
                RingPolynomial edinica = new RingPolynomial()
                {
                    [i * RingBint.mod] = 1
                };
                var reminder = (edinica / poly).Reminder;
                for(int j = 0; j < poly.degree; j++)
                {
                    matrix[j, i - 1] = reminder[j]; 
                }
                matrix[i, i-1 ] -= 1;
            }
            decomposingPoly = RingMatrix.SolveHSLE(matrix);
            
            // 2.2) рекурсивное разложение с помощью разлазающих многочленов (в статической переменной)
            if (decomposingPoly.Length > 0)
            {
                Program.recDepth--;
                _BerlekampDecomposingPoly = decomposingPoly;
                RingBint leadingCoeff = poly[poly.degree];
                poly *= new RingBint(1) / leadingCoeff;
                var decomposeResult = BerlekampDecompose(poly, 0);
                Program.recDepth++;

                factorRes.AddRange(decomposeResult);
                factorRes[0] *= leadingCoeff;
            }
            else
            {
                //end
                Program.Log("Разложение для " + poly + " не требуется. Многочлен неприводимый.");
                Program.recDepth--;
                factorRes.Add(poly);
                return factorRes;
            }

            //end
            Program.Log("Результат разложения:");
            if(Program.LogEnabled)
                for (int i = 0; i < factorRes.Count; i++)
                {
                    factorRes[i].Print('g');
                }

            Program.recDepth--;
            return factorRes;
        }

        static RingBint[][] _BerlekampDecomposingPoly;
        static RingDecomposeList BerlekampDecompose(RingPolynomial curPoly, int depth)
        {
            if (depth >= _BerlekampDecomposingPoly.Length)
                return new RingDecomposeList { curPoly };  

            Program.recDepth++;
            RingDecomposeList tempres = new RingDecomposeList();
            RingDecomposeList res = new RingDecomposeList();

            Program.Log("Поиск возможных множителей с помощью разлагающих многочленов");
            for (int i = depth; i < _BerlekampDecomposingPoly.Length; i++)
            {
                RingPolynomial oneDecomposePoly = new RingPolynomial(_BerlekampDecomposingPoly[i]);
                for (int j = 0; j < RingBint.mod; j++)
                {
                    RingPolynomial gcd_result;
                    GCD(curPoly, oneDecomposePoly, out gcd_result);
                    gcd_result *= (new RingBint(1) / gcd_result[gcd_result.degree]);
                    if (gcd_result.degree > 0)
                    {
                        tempres.Add(gcd_result);
                    }
                    oneDecomposePoly[0] += 1;
                }
                if(tempres.Count > 1)
                {
                    Program.Log("Найден множитель:");
                    for (int k = 0; k < tempres.Count; k++)
                    {
                        tempres[k].Print('g');
                        res.AddRange(BerlekampDecompose(tempres[k], depth));
                    }
                    Program.recDepth--;
                    return res;
                }
                tempres.Clear();
            }

            Program.Log("Многочлен неприводимый. выход.");

            Program.recDepth--;
            return new RingDecomposeList { curPoly };
        }

        public static List<RingPolynomial> GetNODCoefficientForHensel(RingDecomposeList fDecompose)
        {
            List<RingPolynomial> factorsOfCoeff = new List<RingPolynomial>();
            List<RingPolynomial> resultCoeff = new List<RingPolynomial>();
            // изначально задаем все значения коэффициентов единичными
            // и множителей разложения тоже
            for (int i = 0; i < fDecompose.CountUniq; i++)
            {
                resultCoeff.Add(new RingPolynomial { 1 });
                factorsOfCoeff.Add(new RingPolynomial { 1 });
            }
            // запоминаем все сомножители искомого разложения
            for (int i = 0; i < fDecompose.CountUniq; i++)
            {
                for (int j = 0; j < fDecompose.CountUniq; j++)
                {
                    if (i != j)
                    {
                        factorsOfCoeff[i] *= fDecompose.UniqDecomposeElems[j];
                    }
                }
            }

            RingPolynomial currentGCD = new RingPolynomial();
            for (int i = 0; i < fDecompose.CountUniq - 1; i++)
            {
                GCDResult currCoefficient = GCD(factorsOfCoeff[i], factorsOfCoeff[i+1], out currentGCD);
                // раскрываем последовательно GCD, ища GCD соседних множителей [gcd(a, b, c) = gcd(gcd(a, b), c) = gcd(currentGCD, c)]
                factorsOfCoeff[i + 1] = currentGCD;
                for (int j=0; j < i+1; j++)
                {
                    resultCoeff[j] *= currCoefficient.Coef1;
                }
                resultCoeff[i+1] *= currCoefficient.Coef2;
            }

            // Заменяем коэффициенты их остатками от деления на i элемент факторизации (deg(resultCoeff[i]) < deg(fDecompose[i]))
            for (int i = 0; i < fDecompose.CountUniq; i++)
            {
                resultCoeff[i] = (resultCoeff[i] / fDecompose.UniqDecomposeElems[i]).Reminder;
                for (int j=0; j < resultCoeff[i].size; j++) resultCoeff[i][j] /= currentGCD[0]; // нормализуем
            }

            return resultCoeff;
        }

        public static RingDecomposeList HenselLifting(ZPolynomial f, RingDecomposeList fFactorization, List<RingPolynomial> GCDCoeffs, int liftingDegree)
        {
            //ZPolynomial.GCD(f, f.Derivative(), out hasSquares);
            //ZPolynomial SquareFreef = (f / hasSquares).Quotient;
            ZPolynomial SquareFreef = f;
            int p = RingBint.mod;

            fFactorization.originalPolyCoefficient = f[f.size - 1];

            RingDecomposeList LiftingRes = new RingDecomposeList(fFactorization);

            for (int t=1; t < liftingDegree; t++)
            {
                ZPolynomial multiplyFactor = new ZPolynomial { 1 };
                for (int i = 0; i < LiftingRes.CountUniq; i++)
                {
                    multiplyFactor *= new ZPolynomial(LiftingRes.UniqDecomposeElems[i]);
                }
                multiplyFactor *= fFactorization.originalPolyCoefficient / multiplyFactor[multiplyFactor.size - 1];

                var temp = SquareFreef - multiplyFactor;
                RingPolynomial d = new RingPolynomial(temp / (BigInteger)Math.Pow(p, t));


                for (int i=0; i < fFactorization.CountUniq; i++)
                {
                    RingPolynomial currentUniqPoly = fFactorization.UniqDecomposeElems[i];
                    RingPolynomial Gc =  (d * GCDCoeffs[i] / currentUniqPoly).Reminder;
                    SetModContext((int)Math.Pow(p, t+1));
                    LiftingRes.UniqDecomposeElems[i] = LiftingRes.UniqDecomposeElems[i] + Gc * (RingBint)Math.Pow(p, t);
                    SetModContext(p);
                }

            }
            SetModContext((int)Math.Pow(p, liftingDegree));
            // Здесь должен быть bigint!!!!  -----|
            LiftingRes.UniqDecomposeElems[0] *= (int)fFactorization.originalPolyCoefficient / LiftingRes.UniqDecomposeElems[0][LiftingRes.UniqDecomposeElems[0].size - 1];
            return LiftingRes;
        }

    }
}
