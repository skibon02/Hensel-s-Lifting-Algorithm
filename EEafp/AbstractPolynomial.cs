using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EEafp
{
    // T: coefficients type
    // U: Polynomial or RingPolynomial
    public abstract class AbstractPolynomial<T, U> : IEnumerable<T> where T : new() where U : new()
    {
        protected List<T> coef;
        public T this[int i]
        {
            get {
                if (i == -1)
                {
                    throw new Exception();
                }
                if (i < coef.Count)
                    return coef[i];
                else
                    return new T();
            }
            set
            {
                if (i < coef.Count)
                    coef[i] = value;
                else
                {
                    coef.Capacity += i - coef.Count + 1;
                    coef.InsertRange(coef.Count, Enumerable.Repeat<T>(new T(), i - coef.Count).ToArray());
                    coef.Add(value);
                }
            }
        }
        public int size
        {
            get
            {
                return coef.Count;
            }
        }

        public int degree
        {
            get
            {
                return size - 1;
            }
        }
        public abstract string SetLetter
        {
            get;
        }

        public AbstractPolynomial()
        {
            coef = new List<T> {};
        }
        public AbstractPolynomial(T[] p1)
        {
            coef = new List<T>(p1);
        }
        public AbstractPolynomial(AbstractPolynomial<T, U> p1)
        {
            coef = new List<T>(p1.coef);
        }
        public AbstractPolynomial(int size)
        {
            if (size == -1)
                coef = new List<T>();
            else
                coef = new List<T>(Enumerable.Repeat<T>(new T(), size).ToArray());
        }
        public IEnumerator<T> GetEnumerator()
        {
            return coef.GetEnumerator();
        }
        public virtual void Add(T val)
        {
            coef.Add(val);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return coef.GetEnumerator();
        }


        public override string ToString()
        {
            string res = "";
            for (int i = degree; i > 0; i--)
            {
                if (coef[i] is decimal)
                    res += decimal.Parse(coef[i].ToString()).ToString("0.00");
                else
                    res += coef[i].ToString();
                res += 'x';
                if (i != 1)
                    res += "^" + i;
                res += " + ";
            }
            if (coef.Count > 0)
            {
                if (coef[0] is decimal)
                    res += decimal.Parse(coef[0].ToString()).ToString("0.00");
                else
                    res += coef[0].ToString();
            }
            else
                res += "0";

            return res;
        }
        public void Print(char letter = 'f', char variable = 'x')
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(letter + "(" + variable + ") = ");
            for (int i = degree; i > 0; i--)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;

                if (coef[i] is decimal)
                    Console.Write(decimal.Parse(coef[i].ToString()).ToString("0.00"));
                else
                    Console.Write(coef[i].ToString());
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(variable);
                if (i != 1)
                    Console.Write("^" + i);
                Console.Write(" + ");
            }
            Console.ForegroundColor = ConsoleColor.Magenta;
            if (coef.Count > 0)
            {
                if (coef[0] is decimal)
                    Console.WriteLine(decimal.Parse(coef[0].ToString()).ToString("0.00"));
                else
                    Console.WriteLine(coef[0].ToString());
            }
            else
                Console.WriteLine(0);
            Console.ForegroundColor = ConsoleColor.White;

        }


        public abstract U Pow(int times);
        public abstract U Shift(int nyam);
        public abstract U Normalize();
        public bool IsNull()
        {
            return (Normalize() as AbstractPolynomial<T,U>).degree == -1;
        }

        public override bool Equals(object obj)
        {
            AbstractPolynomial<T, U> fir = this;
            AbstractPolynomial<T, U> sec = obj as AbstractPolynomial<T, U>;

            int len = fir.degree > sec.degree ? fir.degree : sec.degree;

            for (int i = 0; i <= len; i++)
            {
                if (!fir.coef[i].Equals( sec.coef[i]))
                    return false;
            }

            return true;
        }

        public static U operator +(AbstractPolynomial<T, U> fir, U sec)
        {
            return fir.PolySum(sec);
        }
        protected abstract U PolySum(U p2);

        public static U operator -(AbstractPolynomial<T, U> fir, U sec)
        {
            return fir.PolyMin(sec);
        }
        protected abstract U PolyMin(U p2);

        public static U operator -(AbstractPolynomial<T, U> fir)
        {
            return fir.PolyMin();
        }
        protected abstract U PolyMin();

        public static U operator *(AbstractPolynomial<T, U> fir, U sec)
        {
            return fir.PolyMul(sec);
        }
        protected abstract U PolyMul(U p2);

        public static U operator *(AbstractPolynomial<T, U> fir, T nyam)
        {
            return fir.PolyMul(nyam);
        }
        protected abstract U PolyMul(T nyam);

        public static DividionResult operator /(AbstractPolynomial<T, U> fir, U sec)
        {
            return fir.PolyDiv(sec);
        }
        protected abstract DividionResult PolyDiv(U p2);

        public static bool operator ==(AbstractPolynomial<T, U> fir, U sec)
        {
            return fir.Equals(sec);
        }
        public static bool operator !=(AbstractPolynomial<T, U> fir, U sec)
        {
            return !fir.Equals(sec);
        }



        public static GCDResult GCD(AbstractPolynomial<T, U> f, U g, out U gcd)
        {
            return f.PolyGCD(g, out gcd);
        }
        protected abstract GCDResult PolyGCD(U g, out U gcd);


        public class DividionResult : MyTuple2<T, U>
        {
            public DividionResult(U q, U r) : base(q, r)
            {
            }
            public U Quotient
            {
                get
                {
                    return Item1;
                }
            }
            public U Reminder
            {
                get
                {
                    return Item2;
                }

            }
        }
        public class GCDResult : MyTuple2<T, U>
        {
            public GCDResult(U c1, U c2) : base(c1, c2)
            {
            }
            public GCDResult(MyTuple2<T, U> val) : base(val.Item1, val.Item2)
            {
            }
            public U Coef1
            {
                get
                {
                    return Item1;
                }
            }
            public U Coef2
            {
                get
                {
                    return Item2;
                }

            }
            public static implicit operator Solution(GCDResult val)
            {
                return new Solution(val);
            }
        }
        public class Solution : MyTuple2<T, U> // null item => any rational k
        {
            public Solution() : base()
            {

            }
            public Solution(U c1, U c2) : base(c1, c2)
            {
            }
            public Solution(MyTuple2<T, U> val) : base(val.Item1, val.Item2)
            {
            }
            public U X
            {
                get
                {
                    return Item1;
                }
            }
            public U Y
            {
                get
                {
                    return Item2;
                }

            }
        }
        public class FullSolution
        {
            public Solution zeroSolution;
            public Solution solutionStep;
            public bool isDefined = true;
            public bool areCoefsIndependent = false;
            public FullSolution()
            {
                zeroSolution = new Solution();
                solutionStep = new Solution();
            }
            public FullSolution(U p1, U p2, U p3, U p4)
            {
                zeroSolution = new Solution(p1, p2);
                solutionStep = new Solution(p3, p4);
            }
            public override string ToString()
            {
                if (!isDefined)
                {
                    return "No solution.";
                }

                List<KeyValuePair<string, string>> defs = new List<KeyValuePair<string, string>>();

                if (areCoefsIndependent)
                {
                    defs.Add(new KeyValuePair<string, string>("k1", (zeroSolution.X as AbstractPolynomial<T,U>).SetLetter));
                    defs.Add(new KeyValuePair<string, string>("k2", (zeroSolution.X as AbstractPolynomial<T, U>).SetLetter));
                }
                else
                {
                    defs.Add(new KeyValuePair<string, string>("k", (zeroSolution.X as AbstractPolynomial<T, U>).SetLetter));
                }

                string res = "X = ";
                if (!(zeroSolution.X as AbstractPolynomial<T, U>).IsNull())
                    res += zeroSolution.X + " + ";
                if (solutionStep.X != null)
                {
                    if ((solutionStep.X as AbstractPolynomial<T, U>).IsNull())
                        res += "0\n";
                    else
                        res += "(" + solutionStep.X + ")" + $"k" + (areCoefsIndependent ? "1" : "") + "\n";
                }
                else
                    res += $"k" + (areCoefsIndependent ? "1" : "") + "\n";
                res += "Y = ";
                if (!(zeroSolution.Y as AbstractPolynomial<T, U>).IsNull())
                    res += zeroSolution.Y + " + ";
                if (solutionStep.Y != null)
                {
                    if ((solutionStep.Y as AbstractPolynomial<T, U>).IsNull())
                        res += "0\n";
                    else
                        res += "(" + solutionStep.Y + ")" + $"k" + (areCoefsIndependent ? "2" : "") + "\n";
                }
                else
                    res += $"k" + (areCoefsIndependent ? "2" : "") + "\n";
                res += "where: ";
                defs.ForEach((KeyValuePair<string,string> pair) =>
                {
                    if(pair.Key == defs.ElementAt(0).Key)
                    {
                        res += pair.Key + " belongs to " + pair.Value;
                    }
                    else
                        res += ", " + pair.Key + " belongs to " + pair.Value;
                });
                res += ".";

                return res;
            }

            public U X() => solutionStep.X;
            public U Y() => solutionStep.Y;
            public U X0() => zeroSolution.X;
            public U Y0() => zeroSolution.Y;
        }
    }
}
