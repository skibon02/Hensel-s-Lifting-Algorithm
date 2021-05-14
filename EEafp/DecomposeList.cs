using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EEafp
{
    public class RingDecomposeList: IEnumerable<RingPolynomial>
    {
        public List<RingPolynomial> UniqDecomposeElems;
        List<RingPolynomial> AllDecomposeElems;

        List<int> nyamDecomposeElem;

        public RingDecomposeList() 
        {
            UniqDecomposeElems = new List<RingPolynomial>();
            AllDecomposeElems = new List<RingPolynomial>();
            nyamDecomposeElem = new List<int>();
        }

        public RingDecomposeList(IEnumerable<RingPolynomial> RingPolyEnum)
        {
            UniqDecomposeElems = new List<RingPolynomial>();
            AllDecomposeElems = new List<RingPolynomial>();
            nyamDecomposeElem = new List<int>();

            foreach (RingPolynomial poly in RingPolyEnum)
            {
                AllDecomposeElems.Add(poly);
                bool isDuplicate = false;
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
                AllDecomposeElems.Add(poly);
                bool isDuplicate = false;
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
        }

        public RingPolynomial this[int i]
        {
            get
            {
                return AllDecomposeElems[i];
            }
            set
            {
                AllDecomposeElems[i] = value;
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
