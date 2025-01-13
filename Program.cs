using System;
using System.IO;

namespace Lab1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Визначення відносних шляхів для вхідного та вихідного файлів
            string rootDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\"));
            string inputPath = Path.Combine(rootDirectory, "INPUT.TXT");
            string outputPath = Path.Combine(rootDirectory, "OUTPUT.TXT");

            RunLab(inputPath, outputPath);
        }

        public static void RunLab(string inputPath, string outputPath)
        {
            try
            {
                // Зчитування даних з вхідного файлу
                string[] inputData = GetDataFromFile(inputPath);

                // Перевірка вхідних даних
                string validationError = ValidateInput(inputData);
                if (validationError != null)
                {
                    Console.WriteLine(validationError);
                    return;
                }

                // Спроба перетворення даних у число
                int x = int.Parse(inputData[0].Trim());

                // Отримання кількості рішень
                int result = GetWaysToRepresentAsSum(x);

                // Запис результату у вихідний файл
                File.WriteAllText(outputPath, result.ToString());

                Console.WriteLine($"Кількість способів представити {x} як суму чотирьох чисел: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Сталася помилка: {ex.Message}");
            }
        }

        // Метод для обчислення кількості способів представити x як суму чотирьох чисел
        public static int GetWaysToRepresentAsSum(int x)
        {
            int count = 0;

            // Перебір усіх можливих значень a, b, c
            for (int a = 1; a <= x - 3; a++)  // a ≤ x - 3, оскільки d мінімум 1
            {
                for (int b = a; b <= x - 2; b++)  // b ≥ a, b ≤ x - 2, оскільки c і d мінімум 1
                {
                    for (int c = b; c <= x - 1; c++)  // c ≥ b, c ≤ x - 1, оскільки d мінімум 1
                    {
                        int d = x - a - b - c;  // знаходимо d, решта від x
                        if (d >= c)  // перевіряємо умову d ≥ c
                        {
                            count++;  // якщо умова виконана, збільшуємо лічильник
                        }
                    }
                }
            }

            return count;
        }

        // Метод для зчитування даних з файлу
        public static string[] GetDataFromFile(string path)
        {
            // Перевірка існування файлу
            if (!File.Exists(path))
            {
                Console.WriteLine($"Помилка: Вхідний файл '{path}' не існує.");
                return Array.Empty<string>();
            }

            return File.ReadAllLines(path);
        }

        // Метод для перевірки вхідних даних
        public static string ValidateInput(string[] inputData)
        {
            // Перевірка наявності даних у файлі
            if (inputData.Length == 0)
            {
                return "Помилка: Вхідний файл порожній.";
            }

            // Спроба перетворення даних у число
            if (!int.TryParse(inputData[0].Trim(), out int x))
            {
                return "Помилка: Вхідні дані не є коректним цілим числом.";
            }

            // Перевірка діапазону числа x
            if (x < 1 || x > 1500)
            {
                return "Помилка: Число x повинно бути в діапазоні 1 ≤ x ≤ 1500.";
            }

            return null; // Якщо всі перевірки успішно пройдено
        }
    }
}
