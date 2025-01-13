using System;
using Xunit;
using Lab1;

namespace Lab1.xUnitTests
{
    public class ProgramTests
    {
        // Тест: Перевірка порожнього вхідного файлу (помилка: файл порожній)
        [Fact]
        public void Test_ValidateInput_EmptyFile()
        {
            string[] inputData = new string[] { };
            string result = Program.ValidateInput(inputData);

            Assert.Equal("Error: The input file is empty.", result);
            Console.WriteLine("Test 1 пройдено: Вхідний файл порожній.");
        }

        // Тест: Перевірка некоректного формату даних (введено не число)
        [Fact]
        public void Test_ValidateInput_InvalidInteger()
        {
            string[] inputData = new string[] { "abc" };
            string result = Program.ValidateInput(inputData);

            Assert.Equal("Error: The input data is not a valid integer.", result);
            Console.WriteLine("Test 2 пройдено: Вхідні дані не є коректним числом.");
        }

        // Тест: Перевірка числа, яке менше мінімально допустимого значення (менше 1)
        [Fact]
        public void Test_ValidateInput_NumberLessThan1()
        {
            string[] inputData = new string[] { "0" };
            string result = Program.ValidateInput(inputData);

            Assert.Equal("Error: The number x must be in the range 1 ≤ x ≤ 1500.", result);
            Console.WriteLine("Test 3 пройдено: Число менше 1.");
        }

        // Тест: Перевірка числа, яке більше максимально допустимого значення (більше 1500)
        [Fact]
        public void Test_ValidateInput_NumberGreaterThan1500()
        {
            string[] inputData = new string[] { "1501" };
            string result = Program.ValidateInput(inputData);

            Assert.Equal("Error: The number x must be in the range 1 ≤ x ≤ 1500.", result);
            Console.WriteLine("Test 4 пройдено: Число більше 1500.");
        }

        // Тест: Перевірка коректного числа в допустимому діапазоні
        [Fact]
        public void Test_ValidateInput_ValidNumber()
        {
            string[] inputData = new string[] { "50" };
            string result = Program.ValidateInput(inputData);

            // Очікується, що метод поверне null, якщо число в допустимому діапазоні
            Assert.Null(result);

            Console.WriteLine($"Test 5 пройдено: Число є коректним. Результат: {result}");
        }

        // Тест: Перевірка правильності результату для x = число в діапазоні 1 ≤ x ≤ 1500
        [Fact]
        public void Test_GetWaysToRepresentAsSum_CorrectResult_For_X_50()
        {
            int x = 50;
            int result = Program.GetWaysToRepresentAsSum(x);

            Assert.Equal(920, result);

            Console.WriteLine($"Test 6 пройдено: Кількість способів представити {x} як суму дорівнює {result}");
        }
    }
}
