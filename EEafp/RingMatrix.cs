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

        public static void PrintAnswer(RingBint[][] answer)
        {
            for (int i = 0; i < answer.Length; i++)
            {
                for (int j = 0; j < answer[i].Length; j++)
                {
                    Console.Write(answer[i][j] + " ");
                }
                Console.Write('\n');
            }
            Console.Write('\n');
        }

public static RingBint[][] GaussSumplifyForBerclecampFactor(RingBint[,] inp)
        {

            for (int i = 0; i < inp.GetLength(0); i++)
            {
                for (int j = i + 1; j < inp.GetLength(0); j++)
                {
                    // находим ненулевой элемент в столбце
                    for (int iNot0 = i; iNot0 < inp.GetLength(0); iNot0++)
                    {
                        if (inp[iNot0, i] != 0)
                        {
                            // меняем строки местами для получения диагонального вида
                            for (int k = 0; k < inp.GetLength(1); k++)
                            {
                                RingBint tmp;
                                tmp = inp[i, k];
                                inp[i, k] = inp[iNot0, k];
                                inp[iNot0, k] = tmp;
                            }
                            break;
                        }
                    }
                    if (inp[i, i] != 0)
                    {
                        RingBint coef = inp[j, i] / inp[i, i];
                        for (int k = 0; k < inp.GetLength(1); k++)
                        {
                            inp[j, k] -= inp[i, k] * coef;
                        }
                    }
                }
            }

            int spareArguments = 0;
            for(int i = inp.GetLength(0) - 1; i >= 0; i--)
            {
                bool isBlankLine = true;
                for(int j = 0; j < inp.GetLength(1); j++)
                {
                    if(inp[i,j] != 0)
                    {
                        isBlankLine = false;
                        break;
                    }
                }
                if(isBlankLine)
                    spareArguments++;
                else
                    break;
            }

            int answerSize = inp.GetLength(0);
            RingBint[][] answer = new RingBint[spareArguments - 1][]; // особый ответ, который встречается при любом решении 1, 0, 0, ... его вычитаем
            for (int i=0; i < spareArguments - 1; i++)
            {
                answer[i] = new RingBint[answerSize];
            }
            // начальная инициализация ответа
            for (int i = 0; i < spareArguments - 1; i++)
            {
                for (int j = 0; j < answerSize; j++)
                {
                    if (j == answerSize - i - 1)
                    {
                        answer[i][j] = 1;
                    } else 
                    {
                        answer[i][j] = 0;
                    }
                }
            }

            // поиск остальных ответов
            for (int answerNum = 0; answerNum < spareArguments - 1; answerNum++)
            {
                for (int i = inp.GetLength(0) - spareArguments - 1; i >= 0; i--)
                {
                    for (int j = i + 1; j < inp.GetLength(1); j++)
                    {
                        answer[answerNum][i + 1] -= answer[answerNum][j + 1] * inp[i, j];
                    }
                    answer[answerNum][i + 1] /= inp[i, i];
                }
            }
            return answer; 
        }
    }
}
