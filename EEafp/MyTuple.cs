using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EEafp
{
    // T - тип элементов многочленов
    // U - тип элементов кортежа ( Polynomial / RingPolynomial)
    public class MyTuple2<T, U> where T : new() where U: new()
    {
        public U Item1, Item2;
        public MyTuple2()
        {
            Item1 = new U();
            Item2 = new U();
        }
        public MyTuple2(U item1, U item2)
        {
            Item1 = item1;
            Item2 = item2;
        }
        public MyTuple2(AbstractPolynomial<T, U>.GCDResult gcdres)
        {
            Item1 = gcdres.Item1;
            Item2 = gcdres.Item2;
        }
        public MyTuple2(AbstractPolynomial<T, U>.DividionResult gcdres)
        {
            Item1 = gcdres.Item1;
            Item2 = gcdres.Item2;
        }
        public static MyTuple2<T, U> operator +(MyTuple2<T, U> fir, MyTuple2<T, U> sec)
        {
            return new MyTuple2<T, U>(sec.Item1 as AbstractPolynomial<T,U> + fir.Item1, fir.Item2 as AbstractPolynomial<T, U> + sec.Item2);
        }
        public static MyTuple2<T, U> operator -(MyTuple2<T, U> fir, MyTuple2<T, U> sec)
        {
            return new MyTuple2<T, U>(fir.Item1 as AbstractPolynomial < T, U > - sec.Item1, fir.Item2 as AbstractPolynomial < T, U >  -sec.Item2);
        }
        public static MyTuple2<T, U> operator -(MyTuple2<T, U> fir)
        {
            return new MyTuple2<T, U>(-(fir.Item1 as AbstractPolynomial<T, U>), -(fir.Item2 as AbstractPolynomial<T, U>));
        }
        public static MyTuple2<T, U> operator *(MyTuple2<T, U> fir, T nyam)
        {
            return new MyTuple2<T, U>((fir.Item1 as AbstractPolynomial<T, U>) * nyam, (fir.Item2 as AbstractPolynomial<T, U>) * nyam);
        }
        public static MyTuple2<T, U> operator *(MyTuple2<T, U> fir, MyTuple2<T, U> sec)
        {
            return new MyTuple2<T, U>((fir.Item1 as AbstractPolynomial<T, U>) * sec.Item1, (fir.Item2 as AbstractPolynomial<T, U>) * sec.Item2);
        }
    }



}
