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
        public static BigInteger mod;

        private BigInteger _val;
        public BigInteger val
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
        public RingBint(BigInteger v)
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
            for (BigInteger i = 0; i < mod; i++)
            {
                if (i * sec.val % mod == fir.val)
                    return i;
            }

            throw new DivideByZeroException("Why are you gay?");
        }
        public static bool operator ==(RingBint fir, object sec)
        {
            return fir.Equals(sec);
        }
        public static bool operator !=(RingBint fir, object sec)
        {
            return !fir.Equals(sec);
        }
        public static implicit operator BigInteger(RingBint el)
        {
            return el.val;
        }
        public static implicit operator RingBint(int el)
        {
            return new RingBint(el);
        }
        public static implicit operator RingBint(BigInteger el)
        {
            return new RingBint(el);
        }
        public override string ToString()
        {
            return val.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj is RingBint)
                return this.val == (obj as RingBint).val;
            else
                return this.val == obj as int?;
        }
    }

    public class RingPolynomial : AbstractPolynomial<RingBint, RingPolynomial>
    {
        public override string SetLetter => "Z";

        public static void SetModContext(BigInteger mod)
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

        public RingPolynomial(IntPolynomial p1)
        {
            for (int i = 0; i < p1.size; i++)
            {
                this.Add(p1[i]);
            }
        }



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
            if (gcd[gcd.degree] < (BigInteger)0)
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

            if (poly.degree < 2) // если многочлен меньше второй степени, то и раскладывать смысла нет
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
            GCD(poly, poly.Derivative(), out gcdDerivative);
            if (poly == gcdDerivative)
            {
                // 1.1) особый случай, когда производная равна 0. Тогда f(x) = g(x)^p, но тогда f(x) = g(x^p) - воспользуемся этим
                Program.Log("Обнаружена общая степень кратная модулю поля (" + RingBint.mod + ")");
                RingPolynomial g = new RingPolynomial((gcdDerivative.size - 1) / (int)RingBint.mod + 1);
                for (int i = 0; i < g.size; i++)
                {
                    g[i] = gcdDerivative[i * (int)RingBint.mod];
                }
                RingDecomposeList gFactorRes = g.BerlekampFactor();
                for (int i = 0; i < RingBint.mod; i++)
                {
                    factorRes.AddRange(gFactorRes);
                }
                //end
                Program.Log("Общее разложение на данном этапе: ");
                PrintDecompositionResult(factorRes);

                Program.recDepth--;
                return factorRes;
            }
            else
            if (gcdDerivative.degree > 0)
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
                Program.Log("Общее разложение на данном этапе: ");
                PrintDecompositionResult(factorRes);
                return factorRes;
            }
            // 2.1) Построение матрицы и нахождение ФСР ОСЛУ
            RingBint[,] matrix = new RingBint[poly.degree, poly.degree-1]; // транспонированное заполнение матрицы
            RingBint[][] decomposingPoly;
            for (int i = 1; i < poly.degree; i++)
            {
                RingPolynomial edinica = new RingPolynomial()
                {
                    [i * (int)RingBint.mod] = 1
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
                _BerlekampDecomposingPoly = decomposingPoly;

                RingBint leadingCoeff = poly[poly.degree];
                poly *= 1 / (RingBint)leadingCoeff; //normalize senior coeff
                var decomposeResult = BerlekampDecompose(poly);

                factorRes.AddRange(decomposeResult);
                factorRes.polyCoef *= leadingCoeff; 
            }
            else
            {
                //end
                Program.Log("Разложение для " + poly + " не требуется. Многочлен неприводимый.");
                Program.recDepth--;
                factorRes.Add(poly);

                Program.Log("Общее разложение на данном этапе: ");
                PrintDecompositionResult(factorRes);
                return factorRes;
            }

            //end
            Program.Log("Общее разложение на данном этапе: ");
            PrintDecompositionResult(factorRes);

            Program.recDepth--;
            return factorRes;
        }
        static void PrintDecompositionResult(RingDecomposeList factorRes)
        {
            if (Program.LogEnabled)
                for (int i = 0; i < factorRes.CountUniq; i++)
                {
                    factorRes[i].Print('g');
                    if (factorRes.divisors[i].count > 1)
                        Program.Log("\tКратность множителя: " + factorRes.divisors[i].count);
                }
        }

        static RingBint[][] _BerlekampDecomposingPoly;
        static RingDecomposeList BerlekampDecompose(RingPolynomial poly)
        { 
            RingDecomposeList res = new RingDecomposeList { poly };
            
            Program.Log("Поиск " + (_BerlekampDecomposingPoly.Length+1) + " множителей с помощью разлагающих многочленов для:");
            poly.Print('c');
            Program.Log("Найденые множители:");
            for (int m = 0; m < res.CountUniq; m++)
            {
                for (int i = 0; i < _BerlekampDecomposingPoly.Length; i++)
                {
                    RingPolynomial oneDecomposePoly = new RingPolynomial(_BerlekampDecomposingPoly[i]);

                    for (int j = 0; j < RingBint.mod; j++)
                    {
                        RingPolynomial gcd_result;
                        GCD(res[m], oneDecomposePoly, out gcd_result);
                        gcd_result *= (new RingBint(1) / gcd_result[gcd_result.degree]);
                        if (gcd_result.degree > 0)
                        {
                            if (gcd_result == res[m])
                                break;
                            else
                            {
                                res.Add(gcd_result);
                                res[m] = (res[m] / gcd_result).Quotient;
                                if (res.CountUniq == _BerlekampDecomposingPoly.Length+1)
                                {
                                    for (; m < res.CountUniq; m++)
                                    {
                                        res[m].Print('g');
                                    }
                                    Program.Log("На данный момент найдены все множители (" + res.CountUniq + "). Выход.");
                                    return res;
                                }
                            }
                        }

                        oneDecomposePoly[0] += 1;
                    }
                }
                res[m].Print('g');
            }

            return res;
        }

        public int FindNumOfMultipliers()
        {
            RingPolynomial poly = this;
            RingBint[,] matrix = new RingBint[poly.degree, poly.degree - 1]; // транспонированное заполнение матрицы
            RingBint[][] decomposingPoly;
            for (int i = 1; i < poly.degree; i++)
            {
                RingPolynomial edinica = new RingPolynomial()
                {
                    [i * (int)RingBint.mod] = 1
                };
                var reminder = (edinica / poly).Reminder;
                for (int j = 0; j < poly.degree; j++)
                {
                    matrix[j, i - 1] = reminder[j];
                }
                matrix[i, i - 1] -= 1;
            }
            decomposingPoly = RingMatrix.SolveHSLE(matrix);
            return decomposingPoly.Length;
        }

        public static List<RingPolynomial> GetGCDCoefficientForHensel(RingDecomposeList fDecompose)
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
                        factorsOfCoeff[i] *= fDecompose[j];
                    }
                }
                if (i != 0)
                {
                    // считаем, что первый множитель разложения содержит степень исходного многочлена
                    factorsOfCoeff[i] *= fDecompose.polyCoef;
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
                RingPolynomial currFact = new RingPolynomial(fDecompose[i]);
                if (i == 0)
                {
                    currFact *= fDecompose.polyCoef;
                }
                resultCoeff[i] = (resultCoeff[i] / currFact).Reminder;
                for (int j=0; j < resultCoeff[i].size; j++) resultCoeff[i][j] /= currentGCD[currentGCD.degree]; // нормализуем
            }

            return resultCoeff;
        }
        public static RingDecomposeList HenselLifting(IntPolynomial f, RingDecomposeList fFactorization, List<RingPolynomial> GCDCoeffs, int liftingDegree)
        {
            IntPolynomial hasSquares;
            IntPolynomial.GCD(f, f.Derivative(), out hasSquares);
            IntPolynomial SquareFreef = (f / hasSquares).Quotient;
            BigInteger p = RingBint.mod;

            BigInteger OriginalCoeff = f[f.size - 1];
            List<RingPolynomial> LiftingRes = new List<RingPolynomial>();
            for (int i = 0; i < fFactorization.CountUniq; i++)
            {
                LiftingRes.Add(fFactorization[i]);
            }
            LiftingRes[0] *= OriginalCoeff;

            for (int t = 1; t < liftingDegree; t++)
            {
                IntPolynomial multiplyFactor = new IntPolynomial { 1 };
                LiftingRes[0] *= OriginalCoeff;
                for (int i = 0; i < LiftingRes.Count; i++)
                {
                    multiplyFactor *= new IntPolynomial(LiftingRes[i]);
                }

                var temp = SquareFreef - multiplyFactor;
                RingPolynomial d = new RingPolynomial(temp / BigInteger.Pow(p, t));


                for (int i = 0; i < fFactorization.CountUniq; i++)
                {
                    RingPolynomial CurrUniqPoly = new RingPolynomial(fFactorization[i]);
                    if (i == 0) CurrUniqPoly *= OriginalCoeff;
                    RingPolynomial Gc = (d * GCDCoeffs[i] / CurrUniqPoly).Reminder;
                    SetModContext(BigInteger.Pow(p, t + 1));
                    LiftingRes[i] = LiftingRes[i] + Gc * BigInteger.Pow(p, t);
                    if (i == 0) LiftingRes[i] /= OriginalCoeff;
                    SetModContext(p);
                }

            }
            SetModContext(BigInteger.Pow(p, liftingDegree));
            RingDecomposeList res = new RingDecomposeList();
            res.AddRange(LiftingRes);
            res.polyCoef = OriginalCoeff;
            return res;
        }

        public static RingDecomposeList HenselLiftingUntilTheEnd(IntPolynomial f, RingDecomposeList fFactorization, List<RingPolynomial> GCDCoeffs)
        {
            IntPolynomial hasSquares;
            IntPolynomial.GCD(f, f.Derivative(), out hasSquares);
            IntPolynomial SquareFreef = (f / hasSquares).Quotient;
            BigInteger squareCoeff = hasSquares[hasSquares.size - 1];

            BigInteger p = RingBint.mod;

            BigInteger OriginalCoeff = SquareFreef[SquareFreef.size - 1];
            List<IntPolynomial> LiftingList = new List<IntPolynomial>();
            for (int i = 0; i < fFactorization.CountUniq; i++)
            {
                LiftingList.Add(new IntPolynomial(fFactorization[i]));
            }

            bool Decomposed = false;

            int t = 1;
            BigInteger currMod = p;
            while (!Decomposed)
            {
                currMod = BigInteger.Pow(p, t + 1);
                IntPolynomial multiplyFactor = new IntPolynomial { 1 };
                LiftingList[0] *= OriginalCoeff;
                for (int i = 0; i < LiftingList.Count; i++)
                {
                    multiplyFactor *= new IntPolynomial(LiftingList[i]);
                }

                var temp = SquareFreef - multiplyFactor;
                RingPolynomial d = new RingPolynomial(temp / BigInteger.Pow(p, t));


                for (int i = 0; i < fFactorization.CountUniq; i++)
                {
                    RingPolynomial CurrUniqPoly = new RingPolynomial(fFactorization[i]);
                    if (i == 0) CurrUniqPoly *= fFactorization.polyCoef;
                    RingPolynomial Gc = ((d * GCDCoeffs[i]) / CurrUniqPoly).Reminder;
                    SetModContext(currMod);
                    LiftingList[i] = LiftingList[i] + new IntPolynomial(Gc) * BigInteger.Pow(p, t);
                    if (i == 0)
                    {
                        LiftingList[i] = new IntPolynomial(new RingPolynomial(LiftingList[i]) / OriginalCoeff);
                    }
                    SetModContext(p);
                }
                t++;
                if (currMod > f[f.degree])
                {
                    SetModContext(currMod);
                    RingPolynomial res = new RingPolynomial { 1 };
                    for (int u=0; u < fFactorization.CountUniq; u++)
                    {
                        for (int j = 0; j < fFactorization.divisors[u].count; j++)
                        {
                            res *= new RingPolynomial(LiftingList[u]);
                        }
                    }
                    res *= OriginalCoeff* squareCoeff;
                    IntPolynomial resInt = new IntPolynomial(res);
                    if (f == resInt)
                    {
                        Decomposed = true;
                    }
                }
            }

            SetModContext(currMod);
            RingDecomposeList liftigRes = new RingDecomposeList();
            for (int i = 0; i < LiftingList.Count; i++) liftigRes.Add(new RingPolynomial(LiftingList[i]));
            liftigRes.polyCoef = OriginalCoeff * squareCoeff;
            return liftigRes;
        }
    }
}
