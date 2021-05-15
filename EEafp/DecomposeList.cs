using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;

namespace EEafp
{
    public class RingDecomposeList: IEnumerable<RingPolynomial>
    {
        public struct Divisor
        {
            public RingPolynomial poly;
            public int count;
        }
        public RingBint polyCoef;
        public List<Divisor> divisors;

        public RingDecomposeList() 
        {
            polyCoef = 1;
            divisors = new List<Divisor>();
        }
        public void ForEach(Action<RingPolynomial> action)
        {
            divisors.ForEach(x => action(x.poly));
        }

        public RingDecomposeList(RingDecomposeList list)
        {
            polyCoef = list.polyCoef;
            divisors = new List<Divisor>(list.divisors);
        }


        public virtual void Add(RingPolynomial poly)
        {
            if (!poly.IsNull())
            {
                polyCoef *= poly[poly.degree];
                poly /= poly[poly.degree];
            }

            for (int i = 0; i < divisors.Count; i++)
            {
                if(divisors[i].poly == poly)
                {
                    divisors[i] = new Divisor { poly = divisors[i].poly, count = divisors[i].count + 1 };
                    return;
                }
            }

            divisors.Add(new Divisor { poly = poly, count = 1 });
        }

        public virtual void AddRange(IEnumerable<RingPolynomial> polys)
        {
            foreach (RingPolynomial poly in polys)
            {
                Add(poly);
            }
        }
        public virtual void AddRange(RingDecomposeList polys)
        {
            foreach (var divisor in polys.divisors)
            {
                for(int i = 0; i < divisor.count; i++)
                    Add(divisor.poly);
            }
            polyCoef *= polys.polyCoef;
        }
        public void Clear()
        {
            divisors.Clear();
            polyCoef = 1;
        }

        public RingPolynomial this[int i]
        {
            get
            {
                return divisors[i].poly;
            }
            set
            {
                divisors[i] = new Divisor { poly = value, count = 1 };
            }
        }

        public int Count => divisors.Sum(x=>x.count);

        public int CountUniq => divisors.Count;

        public IEnumerator<RingPolynomial> GetEnumerator()
        {
            foreach (var divisor in divisors)
            {
                yield return divisor.poly;
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return divisors.GetEnumerator();
        }

    }
}
