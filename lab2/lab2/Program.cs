using System;
using System.IO;
using System.Runtime.CompilerServices;

// Видимость для тестов
[assembly: InternalsVisibleTo("Lab2.xUnitTests")]

namespace Lab2
{
    public class Program
    {
        // Максимальные размеры поля
        const int ROWS_TOTAL_MAX = 70;
        const int COLS_TOTAL_MAX = 70;

        // Игровое поле и таблица для подсчета вариантов
        internal static int[,] field = new int[ROWS_TOTAL_MAX, COLS_TOTAL_MAX]; // internal свойство для тестов
        static long[,] variantsCounter = new long[ROWS_TOTAL_MAX, COLS_TOTAL_MAX];

        public static void Main(string[] args)
        {
            // Определение путей для входного и выходного файлов
            string rootDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\.."));
            string inputPath = Path.Combine(rootDirectory, "INPUT.TXT");
            string outputPath = Path.Combine(rootDirectory, "OUTPUT.TXT");

            RunLab(inputPath, outputPath);
        }

        public static void RunLab(string inputPath, string outputPath)
        {
            try
            {
                // Чтение данных из входного файла
                string[] inputData = File.ReadAllLines(inputPath);

                // Проверка и чтение размеров поля
                if (!ValidateDimensions(inputData[0], out int rowsTotal, out int colsTotal))
                {
                    throw new Exception("Invalid field dimensions in the first line.");
                }

                // Проверка и заполнение игрового поля
                if (!ValidateAndFillField(inputData, rowsTotal, colsTotal))
                {
                    throw new Exception("Invalid data in the game field.");
                }

                // Получение количества вариантов путей
                long result = GetVariantsCount(rowsTotal, colsTotal);

                // Запись результата в выходной файл
                File.WriteAllText(outputPath, result.ToString());

                Console.WriteLine($"The number of possible paths: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        // Метод проверки и чтения размеров поля
        public static bool ValidateDimensions(string firstLine, out int rowsTotal, out int colsTotal)
        {
            rowsTotal = 0;
            colsTotal = 0;
            string[] dimensions = firstLine.Split(' ');

            // Проверка, что в первой строке два числа
            if (dimensions.Length != 2 ||
                !int.TryParse(dimensions[0], out rowsTotal) ||
                !int.TryParse(dimensions[1], out colsTotal))
            {
                Console.WriteLine("Error: The first line must contain two integers.");
                return false;
            }

            // Проверка, что размеры находятся в пределах 1 ≤ N, M ≤ 70
            if (rowsTotal < 1 || rowsTotal > 70 || colsTotal < 1 || colsTotal > 70)
            {
                Console.WriteLine("Error: The dimensions must be between 1 and 70.");
                return false;
            }

            return true;
        }

        // Метод проверки данных игрового поля и заполнения массива field
        public static bool ValidateAndFillField(string[] inputData, int rowsTotal, int colsTotal)
        {
            // Проверка количества строк
            if (inputData.Length - 1 != rowsTotal)
            {
                Console.WriteLine("Error: The number of rows in the input data does not match the specified dimensions.");
                return false;
            }

            // Заполнение поля
            for (int i = 0; i < rowsTotal; i++)
            {
                string[] rowData = inputData[i + 1].Split(' ');

                // Проверка количества элементов в строке
                if (rowData.Length != colsTotal)
                {
                    Console.WriteLine($"Error: Row {i + 1} does not contain the correct number of columns.");
                    return false;
                }

                for (int j = 0; j < colsTotal; j++)
                {
                    // Проверка, что каждый элемент — неотрицательное целое число от 0 до 100
                    if (!int.TryParse(rowData[j], out int cellValue) || cellValue < 0 || cellValue > 100)
                    {
                        Console.WriteLine($"Error: Invalid value '{rowData[j]}' at position ({i + 1}, {j + 1}). It must be between 0 and 100.");
                        return false;
                    }

                    field[i, j] = cellValue;
                }
            }

            return true;
        }

        // Метод проверки возможности шага вправо
        public static bool MayGoToRight(int stepsCount, int colsTotal, int col)
        {
            return col + stepsCount < colsTotal;
        }

        // Метод проверки возможности шага вниз
        public static bool MayGoToDown(int stepsCount, int rowsTotal, int row)
        {
            return row + stepsCount < rowsTotal;
        }

        // Метод для подсчета количества вариантов путей
        public static long GetVariantsCount(int rowsTotal, int colsTotal)
        {
            // Устанавливаем начальное количество путей из стартовой точки
            variantsCounter[0, 0] = 1;

            // Перебираем все клетки игрового поля
            for (int i = 0; i < rowsTotal; i++)
            {
                for (int j = 0; j < colsTotal; j++)
                {
                    // Если нет путей в текущую клетку или шаг равен 0, пропускаем её
                    if (variantsCounter[i, j] == 0 || field[i, j] == 0)
                    {
                        continue;
                    }

                    int stepsCount = field[i, j];

                    // Проверяем возможность шага вправо
                    if (MayGoToRight(stepsCount, colsTotal, j))
                    {
                        variantsCounter[i, j + stepsCount] += variantsCounter[i, j];
                    }

                    // Проверяем возможность шага вниз
                    if (MayGoToDown(stepsCount, rowsTotal, i))
                    {
                        variantsCounter[i + stepsCount, j] += variantsCounter[i, j];
                    }
                }
            }

            // Возвращаем количество вариантов путей до правого нижнего угла
            return variantsCounter[rowsTotal - 1, colsTotal - 1];
        }
    }
}
