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
        public int polyCoefficient;
        List<RingPolynomial> AllDecomposeElems;

        public List<RingPolynomial> UniqDecomposeElems;
        List<int> nyamDecomposeElem;

        public BigInteger originalPolyCoefficient;

        public RingDecomposeList() 
        {
            UniqDecomposeElems = new List<RingPolynomial>();
            AllDecomposeElems = new List<RingPolynomial>();
            nyamDecomposeElem = new List<int>();
            this.polyCoefficient = 1;
            originalPolyCoefficient = 1;
        }

        public RingDecomposeList(RingDecomposeList list)
        {
            UniqDecomposeElems = new List<RingPolynomial>(list.UniqDecomposeElems);
            AllDecomposeElems = new List<RingPolynomial>(list.AllDecomposeElems);
            nyamDecomposeElem = new List<int>(list.nyamDecomposeElem);
            this.polyCoefficient = list.polyCoefficient;
            originalPolyCoefficient = list.originalPolyCoefficient;
        }
        public virtual void Add(RingPolynomial poly)
        {
            bool isDuplicate = false;
            AllDecomposeElems.Add(poly);
            for (int i = 0; i < UniqDecomposeElems.Count; i++)
            {
                if (poly == UniqDecomposeElems[i])
                {
                    nyamDecomposeElem[i]++;
                    isDuplicate = true;
                }
            }
            if (!isDuplicate)
            {
                UniqDecomposeElems.Add(poly);
                nyamDecomposeElem.Add(1);
            }
        }

        public virtual void AddRange(IEnumerable<RingPolynomial> RingPolyEnum)
        {
            foreach (RingPolynomial poly in RingPolyEnum)
            {
                RingPolynomial TmpPoly = poly;
                if (TmpPoly[TmpPoly.size - 1] != 1)
                {
                    polyCoefficient *= TmpPoly[TmpPoly.size - 1];
                    TmpPoly /= poly[poly.size - 1];
                }
                AllDecomposeElems.Add(TmpPoly);
                bool isDuplicate = false;
                for (int i = 0; i < UniqDecomposeElems.Count; i++)
                {
                    if (TmpPoly == UniqDecomposeElems[i])
                    {
                        nyamDecomposeElem[i]++;
                        isDuplicate = true;
                    }
                }
                if (!isDuplicate)
                {
                    UniqDecomposeElems.Add(TmpPoly);
                    nyamDecomposeElem.Add(1);
                }
            }
            AllDecomposeElems[0] *= polyCoefficient / AllDecomposeElems[0][AllDecomposeElems[0].size - 1];
            UniqDecomposeElems[0] *= polyCoefficient / UniqDecomposeElems[0][UniqDecomposeElems[0].size - 1];
        }

        public RingPolynomial this[int i]
        {
            get
            {
                return AllDecomposeElems[i];
            }
            set
            {
                if (nyamDecomposeElem[i] == 1 || UniqDecomposeElems[i]* value[value.size - 1] == value)
                {
                    UniqDecomposeElems[i] = value / value[value.size - 1];
                } else
                {
                    UniqDecomposeElems.Add(value);
                }
                if (value[value.size - 1] != 1 && i != 0)
                {
                    polyCoefficient *= value[value.size - 1];
                    AllDecomposeElems[i] = value / value[value.size - 1];
                    AllDecomposeElems[0] *= polyCoefficient / AllDecomposeElems[0][AllDecomposeElems[0].size - 1];
                    UniqDecomposeElems[0] *= polyCoefficient / UniqDecomposeElems[0][UniqDecomposeElems[0].size - 1];
                }
                else if (i == 0)
                {
                    polyCoefficient = value[value.size - 1];
                    AllDecomposeElems[0] = value;
                    UniqDecomposeElems[0] = value;
                } else
                {
                    AllDecomposeElems[i] = value;
                }
            }
        }

        public int Count
        {
            get
            {
                return AllDecomposeElems.Count;
            }
        }

        public int CountUniq
        {
            get
            {
                return UniqDecomposeElems.Count;
            }
        }

        public void Clear()
        {
            AllDecomposeElems.Clear();
            UniqDecomposeElems.Clear();
        }

        public void ForEach(Action<RingPolynomial> action)
        {
            for (int i=0; i < AllDecomposeElems.Count; i++)
            {
                action(AllDecomposeElems[i]);
            }
        }

        public IEnumerator<RingPolynomial> GetEnumerator()
        {
            return AllDecomposeElems.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return AllDecomposeElems.GetEnumerator();
        }
    }
}
