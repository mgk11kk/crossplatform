using System;
using Xunit;
using Lab3;

namespace Lab3.xUnitTests
{
    public class ChessPuzzleTests
    {
        // Тест 1: Проверка корректности входных данных (валидный файл)
        [Fact]
        public void Test_1_InputValidation_CorrectInput()
        {
            // Arrange
            var input = new string[]
            {
                "WBB ..W",
                "W.. ..W",
                "... .BB"
            };

            // Act
            bool result = Program.ValidateInputFormat(input, out string errorMessage);

            // Assert
            Assert.True(result);
            Assert.Null(errorMessage);

            Console.WriteLine("Test 1 passed: Input is valid.");
        }

        // Тест 2: Проверка некорректного количества строк в данных
        [Fact]
        public void Test_2_InputValidation_IncorrectRowCount()
        {
            // Arrange
            var input = new string[]
            {
                "WBB ..W",
                "W.. ..W"
            };

            // Act
            bool result = Program.ValidateInputFormat(input, out string errorMessage);

            // Assert
            Assert.False(result);
            Assert.Equal("Input file must contain exactly 3 lines.", errorMessage);

            Console.WriteLine("Test 2 passed: Incorrect number of rows.");
        }

        // Тест 3: Проверка наличия недопустимых символов в данных
        [Fact]
        public void Test_3_InputValidation_InvalidCharacter()
        {
            // Arrange
            var input = new string[]
            {
                "WBB ..W",
                "W.. XX.",
                "... .BB"
            };

            // Act
            bool result = Program.ValidateInputFormat(input, out string errorMessage);

            // Assert
            Assert.False(result);
            Assert.Equal("Line 2 contains invalid characters. Allowed: 'W', 'B', '.'.", errorMessage);

            Console.WriteLine("Test 3 passed: Invalid character found in input.");
        }

        // Тест 4: Проверка неправильного формата строки (неправильная длина или отсутствие пробела)
        [Fact]
        public void Test_4_InputValidation_InvalidFormat()
        {
            // Arrange
            var input = new string[]
            {
                "WBB. ..W",
                "W.. ..W",
                "... .BB"
            };

            // Act
            bool result = Program.ValidateInputFormat(input, out string errorMessage);

            // Assert
            Assert.False(result);
            Assert.Equal("Line 1 must have exactly 7 characters with a space at position 4.", errorMessage);

            Console.WriteLine("Test 4 passed: Invalid format detected in input.");
        }

        // Тест 5: Проверка правильности выполнения алгоритма для решаемой задачи
        [Fact]
        public void Test_5_SolvePuzzle_CorrectCase()
        {
            // Arrange
            var input = new string[]
            {
                "WBB ..W",
                "W.. ..W",
                "... .BB"
            };

            var (startBoard, endBoard) = Program.ParseInput(input);
            var startPositions = Program.ParsePositions(startBoard);
            var endPositions = Program.ParsePositions(endBoard);

            // Act
            int result = Program.SolvePuzzle(startPositions, endPositions);

            // Assert
            Assert.Equal(4, result);

            Console.WriteLine("Test 5 passed: Puzzle solved in 4 moves.");
        }

        // Тест 6: Проверка невозможности решения задачи
        [Fact]
        public void Test_6_SolvePuzzle_ImpossibleCase()
        {
            // Arrange
            var input = new string[]
            {
                "..B ..B",
                "W.B ..B",
                "W.. WW."
            };

            var (startBoard, endBoard) = Program.ParseInput(input);
            var startPositions = Program.ParsePositions(startBoard);
            var endPositions = Program.ParsePositions(endBoard);

            // Act
            int result = Program.SolvePuzzle(startPositions, endPositions);

            // Assert
            Assert.Equal(-1, result);

            Console.WriteLine("Test 6 passed: Puzzle is impossible to solve.");
        }
    }
}