using System;
using System.Collections.Generic;
using System.Text;

namespace EEafp
{
    class RingMatrix
    {
        public static void PrintMatrix(RingBint[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {

                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    Console.Write(matrix[i, j].val + " ");
                }

                Console.Write("\n");
            }
            Console.WriteLine("\n");
        }

        public static RingBint[] GaussSumplify(RingBint[,] inp)
        {

            for (int i = 0; i < inp.GetLength(0); i++)
            {
                PrintMatrix(inp);
                for (int j = i + 1; j < inp.GetLength(0); j++)
                {
                    if (inp[i, i] == 0)
                        throw new Exception("fix this dude");
                    RingBint coef = inp[j, i] / inp[i, i];
                    for (int k = 0; k < inp.GetLength(1); k++)
                    {
                        inp[j, k] -= inp[i, k] * coef;
                    }
                    PrintMatrix(inp);
                }
            }

            int actualRowCount = inp.GetLength(0);
            for(int i = inp.GetLength(0) - 1; i >= 0; i--)
            {
                bool isOK = true;
                for(int j = 0; j < inp.GetLength(1); j++)
                {
                    if(inp[i,j] != 0)
                    {
                        isOK = false;
                        break;
                    }
                }
                if(isOK)
                    actualRowCount--;
                else
                    break;
            }
            RingBint[] answer = new RingBint[actualRowCount];

            for (int i = 0; i < answer.Length - 1; i++)
                answer[i] = 0;
            answer[answer.Length - 1] = 1;
            for (int i = inp.GetLength(0) - 1; i >= 0; i--)
            {
                for (int j = i + 1; j < actualRowCount; j++)
                {
                    answer[i] -= answer[j] * inp[i, j];
                }
                answer[i] /= inp[i, i];
            }
            return answer;

        }
    }
}
