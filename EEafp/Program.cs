using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EEafp
{
    public class Program
    {
        public static bool LogEnabled = true;
        static void Main(string[] args)
        {
            // TODO:
            // 1) Разобраться с проблемой кратных множителей
            // 2) Закончить Подъем Гензеля


            Console.ForegroundColor = ConsoleColor.White;
            RingPolynomial.SetModContext(5);
            //RingPolynomial checkCoeffsPoly = new RingPolynomial { 1, 2, 3 } * new RingPolynomial { 1, 1 } * new RingPolynomial {0, 1 } * new RingPolynomial { 1, 0, 0, 0, 1 };
            //RingPolynomial checkCoeffsPoly = new RingPolynomial { 1, 2, 3, 4, 2, 3, 5, 2, 1, 4, 5, 3, 6, 5 } * new RingPolynomial { 1, 2, 3 } * new RingPolynomial { 1, 2, 3 };
            ZPolynomial f = new ZPolynomial { 4,2 } * new ZPolynomial { 1,2 } * new ZPolynomial { 6, 1 } * new ZPolynomial { 4, 1};
            RingPolynomial f_inring = new RingPolynomial(f);
            f_inring.Print();
            var factorization = f_inring.BerlekampFactor();
            var GCDfactor = RingPolynomial.GetNODCoefficientForHensel(factorization);
            RingPolynomial allGCDResult = new RingPolynomial { 0 };

            List<RingPolynomial> factorsOfCoeff = new List<RingPolynomial>();
            for (int i = 0; i < factorization.CountUniq; i++)
            {
                factorsOfCoeff.Add(new RingPolynomial { 1 });
            }
            for (int i = 0; i < factorization.CountUniq; i++)
            {
                for (int j = 0; j < factorization.CountUniq; j++)
                {
                    if (i != j)
                    {
                        factorsOfCoeff[i] *= factorization.UniqDecomposeElems[j];
                        Console.Write("(" + factorization.UniqDecomposeElems[j] + ")*");
                    }
                }
                allGCDResult += factorsOfCoeff[i]*GCDfactor[i];
                Console.Write("(" + GCDfactor[i] + ")\n");
            }
            allGCDResult.Print();

            // после поднятия mod уже увеличился
            var liftedDecomposition = RingPolynomial.HenselLifting(f, factorization, GCDfactor, 2);
            RingPolynomial checkLiftedDecomposition = new RingPolynomial { liftedDecomposition.polyCoefficient};


            for (int i=0; i < liftedDecomposition.CountUniq; i++)
            {
                liftedDecomposition[i].Print();
                checkLiftedDecomposition *= liftedDecomposition[i];
            }
            Program.Log("Проверяем декомпозицию поднятую:");
            checkLiftedDecomposition.Print('r');
            f.Print();

            Console.Read(); 
        }

        public static int recDepth = 0;
        public static string GetTab {
            get {
                string res = "";
                for (int i = 0; i < (recDepth-1)*3; i++)
                {
                    res += "* ";
                }

                return res;
            }
        }
        public static void Log(string inp = "")
        {
            if (LogEnabled)
                Console.WriteLine(GetTab + inp);
        }
    }
}
