using System;
using System.Collections.Generic;
using System.Text;

namespace EEafp
{
    public class RingMatrix
    {
        public static void PrintMatrix(RingBint[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                Console.Write(Program.GetTab);
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    Console.Write(matrix[i, j].val + " ");
                }

                Console.WriteLine();
            }
            Console.Write(Program.GetTab);
            Console.WriteLine();
            Console.Write(Program.GetTab);
            Console.WriteLine();
        }

        public static void PrintAnswer(RingBint[][] answer)
        {
            for (int i = 0; i < answer.Length; i++)
            {
                Console.Write(Program.GetTab);
                for (int j = 0; j < answer[i].Length; j++)
                {
                    Console.Write(answer[i][j] + " ");
                }
                Console.WriteLine();
            }
            Console.Write(Program.GetTab);
            Console.WriteLine();
            Console.Write(Program.GetTab);
            Console.WriteLine();
        }

        public static RingBint[][] SolveHSLE(RingBint[,] inp)
        {
            Program.Log("Начало упрощения Гаусса:");
            if (Program.LogEnabled)
                PrintMatrix(inp);
            int ioffset = 0; // сдвиг вверх от стандартного диагонального элемента
            for (int i = 0; i < Math.Min(inp.GetLength(0), inp.GetLength(1)); i++) // проход по диагонали
            {
                for (int j = i + 1 - ioffset; j < inp.GetLength(0); j++) // проход по столбцу от i
                {
                    // находим ненулевой элемент в столбце
                    for (int iNot0 = i - ioffset; iNot0 < inp.GetLength(0); iNot0++)
                    {
                        if (inp[iNot0, i] != 0 && iNot0 != i)
                        {
                            // меняем строки местами для получения диагонального вида
                            for (int k = 0; k < inp.GetLength(1); k++)
                            {
                                RingBint tmp;
                                tmp = inp[i- ioffset, k];
                                inp[i- ioffset, k] = inp[iNot0, k];
                                inp[iNot0, k] = tmp;
                            }
                            break;
                        }
                    }
                    if (inp[i- ioffset, i] != 0)
                    {
                        RingBint coef = inp[j, i] / inp[i- ioffset, i];
                        for (int k = 0; k < inp.GetLength(1); k++)
                        {
                            inp[j, k] -= inp[i- ioffset, k] * coef;
                        }
                    }
                }

                if(inp[i - ioffset, i] == 0)
                {
                    ioffset++;
                }
            }


            Program.Log("Конец упрощения Гаусса:");
            if (Program.LogEnabled)
                PrintMatrix(inp);

            //подсчёт пустых строк матрицы
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

            //подготовка массива ответов
            int answerSize = inp.GetLength(0);
            RingBint[][] answer = new RingBint[spareArguments - 1][]; // особый ответ, который встречается при любом решении 1, 0, 0, ... его вычитаем
            for (int i=0; i < spareArguments - 1; i++)
            {
                answer[i] = new RingBint[answerSize];
                for (int j = 0; j < answerSize; j++)
                    answer[i][j] = 0;
            }

            //Определение базиса системы
            bool[] in_basis = new bool[inp.GetLength(0)];
            in_basis[0] = false;
            int _j = 0;
            for (int i = 0; i < inp.GetLength(1); i++)
            {
                if(inp[_j, i] == 0)
                    in_basis[i+1] = true;
                else
                {
                    in_basis[i+1] = false;
                    _j++;
                }
            }

            // поиск остальных ответов
            int basisIndex = 0;
            for (int answerNum = 0; answerNum < spareArguments - 1; answerNum++)
            {
                for (int i = inp.GetLength(1); i > 0; i--)
                {
                    if (!in_basis[i])
                    {
                        int matrixRow = i - 1;
                        while (matrixRow >= 0 && inp[matrixRow, i - 1] == 0)
                            matrixRow--;

                        basisIndex = 0;
                        for (int j = inp.GetLength(1); j > i; j--)
                        {
                            if (in_basis[j])
                            {
                                if (basisIndex == answerNum)
                                {
                                    answer[answerNum][i] -= inp[matrixRow, j - 1];
                                    answer[answerNum][j] = 1;
                                }
                                basisIndex++;
                            }
                            else
                                answer[answerNum][i] -= answer[answerNum][j] * inp[matrixRow, j - 1];
                        }
                        answer[answerNum][i] /= inp[matrixRow, i - 1];
                    }
                }

                //те же действия для первого элемента решения (сам остается 0, нужный вектор базиса устанавливается в 1)
                basisIndex = 0;
                for (int j = inp.GetLength(1); j > 0; j--)
                {
                    if (in_basis[j] && basisIndex == answerNum)
                    {
                        answer[answerNum][j] = 1;
                        break;
                    }
                    if (in_basis[j])
                        basisIndex++;
                }
            }
            Program.Log("Независимый базис системы:");
            if (Program.LogEnabled)
                PrintAnswer(answer);
            return answer; 
        }
    }
}
