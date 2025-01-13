using System;
using Xunit;
using Lab2;

namespace Lab2.xUnitTests
{
    public class UnitTest1
    {
        // Тест: Проверка корректных входных данных для размера поля (формат "3 3")
        [Fact]
        public void Test_ValidateDimensions_CorrectData()
        {
            string input = "3 3";
            bool result = Program.ValidateDimensions(input, out int rowsTotal, out int colsTotal);

            Assert.True(result);
            Assert.Equal(3, rowsTotal);
            Assert.Equal(3, colsTotal);

            Console.WriteLine($"Test 1 passed: {result}, Field size: {rowsTotal} x {colsTotal}");
        }

        // Тест: Проверка некорректных входных данных (вместо числа - буква)
        [Fact]
        public void Test_ValidateDimensions_IncorrectData()
        {
            string input = "3 a";
            bool result = Program.ValidateDimensions(input, out int rowsTotal, out int colsTotal);

            Assert.False(result);
            Console.WriteLine($"Test 2 passed: {result}, Incorrect input data.");
        }

        // Тест: Проверка корректного заполнения поля с правильными данными
        [Fact]
        public void Test_ValidateAndFillField_CorrectData()
        {
            string[] inputData = new string[]
            {
                "3 3",  
                "2 1 2",
                "1 1 1",
                "1 1 0"
            };
            bool result = Program.ValidateAndFillField(inputData, 3, 3);

            Assert.True(result);
            Console.WriteLine($"Test 3 passed: {result}, Game field successfully filled.");
        }

        // Тест: Проверка некорректного числа строк в данных
        [Fact]
        public void Test_ValidateAndFillField_IncorrectRowCount()
        {
            string[] inputData = new string[]
            {
                "3 3",  
                "2 1 2",
                "1 1 1"
            };
            bool result = Program.ValidateAndFillField(inputData, 3, 3);

            Assert.False(result);
            Console.WriteLine($"Test 4 passed: {result}, Incorrect number of rows.");
        }

        // Тест: Проверка некорректного числа столбцов в данных
        [Fact]
        public void Test_ValidateAndFillField_IncorrectColumnCount()
        {
            string[] inputData = new string[]
            {
                "3 3",  
                "2 1 2",
                "1 1 1",
                "1 1"  // Некорректная строка (не хватает столбцов)
            };
            bool result = Program.ValidateAndFillField(inputData, 3, 3);

            Assert.False(result);
            Console.WriteLine($"Test 5 passed: {result}, Incorrect number of columns.");
        }

        // Тест: Проверка корректности движения вправо (возможное движение)
        [Fact]
        public void Test_MayGoToRight_ValidMove()
        {
            bool result = Program.MayGoToRight(2, 5, 2);  
            Assert.True(result);  
            Console.WriteLine($"Test 6 passed: {result}, Move to the right is possible.");
        }

        // Тест: Проверка некорректности движения вправо (невозможное движение)
        [Fact]
        public void Test_MayGoToRight_InvalidMove()
        {
            bool result = Program.MayGoToRight(3, 5, 3); 
            Assert.False(result);
            Console.WriteLine($"Test 7 passed: {result}, Move to the right is not possible.");
        }

        // Тест: Проверка корректности движения вниз (возможное движение)
        [Fact]
        public void Test_MayGoToDown_ValidMove()
        {
            bool result = Program.MayGoToDown(1, 4, 2);
            Assert.True(result);
            Console.WriteLine($"Test 8 passed: {result}, Move down is possible.");
        }

        // Тест: Проверка некорректности движения вниз (невозможное движение)
        [Fact]
        public void Test_MayGoToDown_InvalidMove()
        {
            bool result = Program.MayGoToDown(3, 3, 2);
            Assert.False(result);
            Console.WriteLine($"Test 9 passed: {result}, Move down is not possible.");
        }

        // Тест: Проверка количества возможных путей, когда существует несколько вариантов (сложный случай)
        [Fact]
        public void Test_GetVariantsCount_MultiplePaths()
        {
            Program.field = new int[,]
            {
                { 2, 1, 1, 2 },
                { 3, 2, 1, 44 },
                { 3, 1, 1, 0 }
            };

            int rows = Program.field.GetLength(0);
            int cols = Program.field.GetLength(1);

            long result = Program.GetVariantsCount(rows, cols);

            Assert.Equal(3, result);
            Console.WriteLine($"Test 10 passed: Multiple paths available. Number of possible paths = {result}");
        }
    }
}
