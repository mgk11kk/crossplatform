using System;
using System.IO;
using System.Runtime.CompilerServices;

// Дозволяємо видимість для тестових проектів
[assembly: InternalsVisibleTo("Lab2.xUnitTests")]

namespace Lab2
{
    public class Program
    {
        // Максимальні розміри поля
        const int ROWS_TOTAL_MAX = 70;
        const int COLS_TOTAL_MAX = 70;

        // Ігрове поле та таблиця для підрахунку варіантів
        internal static int[,] field = new int[ROWS_TOTAL_MAX, COLS_TOTAL_MAX]; // Внутрішнє поле для тестів
        static long[,] variantsCounter = new long[ROWS_TOTAL_MAX, COLS_TOTAL_MAX];

        public static void Main(string[] args)
        {
            // Визначення шляхів для вхідного та вихідного файлів
            string rootDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\.."));
            string inputPath = Path.Combine(rootDirectory, "INPUT.TXT");
            string outputPath = Path.Combine(rootDirectory, "OUTPUT.TXT");

            RunLab(inputPath, outputPath);
        }

        public static void RunLab(string inputPath, string outputPath)
        {
            try
            {
                // Читання даних з вхідного файлу
                string[] inputData = File.ReadAllLines(inputPath);

                // Перевірка та читання розмірів поля
                if (!ValidateDimensions(inputData[0], out int rowsTotal, out int colsTotal))
                {
                    throw new Exception("Invalid field dimensions in the first line.");
                }

                // Перевірка та заповнення ігрового поля
                if (!ValidateAndFillField(inputData, rowsTotal, colsTotal))
                {
                    throw new Exception("Invalid data in the game field.");
                }

                // Отримання кількості варіантів шляхів
                long result = GetVariantsCount(rowsTotal, colsTotal);

                // Запис результату у вихідний файл
                File.WriteAllText(outputPath, result.ToString());

                Console.WriteLine($"The number of possible paths: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        // Метод для перевірки та читання розмірів поля
        public static bool ValidateDimensions(string firstLine, out int rowsTotal, out int colsTotal)
        {
            rowsTotal = 0;
            colsTotal = 0;
            string[] dimensions = firstLine.Split(' ');

            // Перевірка, що в першому рядку два числа
            if (dimensions.Length != 2 ||
                !int.TryParse(dimensions[0], out rowsTotal) ||
                !int.TryParse(dimensions[1], out colsTotal))
            {
                Console.WriteLine("Error: The first line must contain two integers.");
                return false;
            }

            // Перевірка, що розміри знаходяться в межах 1 ≤ N, M ≤ 70
            if (rowsTotal < 1 || rowsTotal > 70 || colsTotal < 1 || colsTotal > 70)
            {
                Console.WriteLine("Error: The dimensions must be between 1 and 70.");
                return false;
            }

            return true;
        }

        // Метод для перевірки даних ігрового поля та заповнення масиву field
        public static bool ValidateAndFillField(string[] inputData, int rowsTotal, int colsTotal)
        {
            // Перевірка кількості рядків
            if (inputData.Length - 1 != rowsTotal)
            {
                Console.WriteLine("Error: The number of rows in the input data does not match the specified dimensions.");
                return false;
            }

            // Заповнення поля
            for (int i = 0; i < rowsTotal; i++)
            {
                string[] rowData = inputData[i + 1].Split(' ');

                // Перевірка кількості елементів у рядку
                if (rowData.Length != colsTotal)
                {
                    Console.WriteLine($"Error: Row {i + 1} does not contain the correct number of columns.");
                    return false;
                }

                for (int j = 0; j < colsTotal; j++)
                {
                    // Перевірка, що кожен елемент — невід'ємне ціле число від 0 до 100
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

        // Метод перевірки можливості кроку вправо
        public static bool MayGoToRight(int stepsCount, int colsTotal, int col)
        {
            return col + stepsCount < colsTotal;
        }

        // Метод перевірки можливості кроку вниз
        public static bool MayGoToDown(int stepsCount, int rowsTotal, int row)
        {
            return row + stepsCount < rowsTotal;
        }

        // Метод для підрахунку кількості варіантів шляхів
        public static long GetVariantsCount(int rowsTotal, int colsTotal)
        {
            // Встановлюємо початкову кількість шляхів зі стартової точки
            variantsCounter[0, 0] = 1;

            // Перебираємо всі клітинки ігрового поля
            for (int i = 0; i < rowsTotal; i++)
            {
                for (int j = 0; j < colsTotal; j++)
                {
                    // Якщо немає шляхів у поточну клітинку або крок дорівнює 0, пропускаємо її
                    if (variantsCounter[i, j] == 0 || field[i, j] == 0)
                    {
                        continue;
                    }

                    int stepsCount = field[i, j];

                    // Перевірка можливості кроку вправо
                    if (MayGoToRight(stepsCount, colsTotal, j))
                    {
                        variantsCounter[i, j + stepsCount] += variantsCounter[i, j];
                    }

                    // Перевірка можливості кроку вниз
                    if (MayGoToDown(stepsCount, rowsTotal, i))
                    {
                        variantsCounter[i + stepsCount, j] += variantsCounter[i, j];
                    }
                }
            }

            // Повертаємо кількість варіантів шляхів до правого нижнього кута
            return variantsCounter[rowsTotal - 1, colsTotal - 1];
        }
    }
}
