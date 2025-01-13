using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace Lab3
{
    public class Program
    {
        // Возможные ходы коня (все возможные "Г"-образные перемещения)
        private static readonly (int, int)[] Moves =
        {
            (-2, -1), (-1, -2), (1, -2), (2, -1),
            (2, 1), (1, 2), (-1, 2), (-2, 1)
        };

        public static void Main(string[] args)
        {
            // Определение путей к входному и выходному файлам
            string rootDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\.."));
            string inputPath = Path.Combine(rootDirectory, "INPUT.TXT");
            string outputPath = Path.Combine(rootDirectory, "OUTPUT.TXT");

            RunLab(inputPath, outputPath);
        }

        public static void RunLab(string inputPath, string outputPath)
        {
            // Чтение данных из входного файла
            var inputLines = File.ReadAllLines(inputPath);

            // Проверка формата входных данных на корректность
            if (!ValidateInputFormat(inputLines, out string errorMessage))
            {
                Console.WriteLine($"Input validation error: {errorMessage}");
                return;
            }

            // Разделение данных на начальную и конечную доски
            var (startBoard, endBoard) = ParseInput(inputLines);

            // Извлечение позиций фигур с их цветами (белый/чёрный конь) для каждой доски
            var startPositions = ParsePositions(startBoard);
            var endPositions = ParsePositions(endBoard);

            // Проверка, что начальная и конечная доски содержат одинаковые фигуры
            if (!ValidateBoards(startPositions, endPositions))
            {
                File.WriteAllText(outputPath, "-1");
                Console.WriteLine("Result: -1");
                return;
            }

            // Вывод начальной и конечной досок
            Console.WriteLine("Start board:");
            PrintBoard(RecreateBoard(startPositions));

            Console.WriteLine("End board:");
            PrintBoard(RecreateBoard(endPositions));

            // Решение задачи с использованием многомерного BFS
            int result = SolvePuzzle(startPositions, endPositions);

            // Запись результата в выходной файл и вывод в консоль
            File.WriteAllText(outputPath, result.ToString());
            Console.WriteLine($"Result: {result}");
        }

        // Проверка формата входных данных на корректность
        public static bool ValidateInputFormat(string[] inputLines, out string errorMessage)
        {
            // Проверка, что входной файл содержит ровно 3 строки
            if (inputLines.Length != 3)
            {
                errorMessage = "Input file must contain exactly 3 lines.";
                return false;
            }

            // Проверка каждой строки на корректность
            for (int i = 0; i < inputLines.Length; i++)
            {
                string line = inputLines[i];
                // Проверка длины строки и наличия пробела в нужной позиции
                if (line.Length != 7 || line[3] != ' ')
                {
                    errorMessage = $"Line {i + 1} must have exactly 7 characters with a space at position 4.";
                    return false;
                }

                // Разделение строки на левую и правую части
                string leftPart = line.Substring(0, 3);
                string rightPart = line.Substring(4, 3);

                // Проверка, что каждая часть содержит только допустимые символы ('W', 'B', '.')
                if (!leftPart.All(c => c == 'W' || c == 'B' || c == '.') ||
                    !rightPart.All(c => c == 'W' || c == 'B' || c == '.'))
                {
                    errorMessage = $"Line {i + 1} contains invalid characters. Allowed: 'W', 'B', '.'.";
                    return false;
                }
            }

            errorMessage = null;
            return true;
        }

        // Разбор входных данных: создаёт массивы для начальной и конечной досок
        public static (char[,], char[,]) ParseInput(string[] inputLines)
        {
            char[,] startBoard = new char[3, 3];
            char[,] endBoard = new char[3, 3];

            for (int i = 0; i < 3; i++)
            {
                var parts = inputLines[i].Split(' '); // Разделяем строки на начальную и конечную части
                for (int j = 0; j < 3; j++)
                {
                    startBoard[i, j] = parts[0][j];
                    endBoard[i, j] = parts[1][j];
                }
            }

            return (startBoard, endBoard);
        }

        // Извлекает позиции всех фигур с их цветами (например, 'W' или 'B') с доски
        public static List<((int, int), char)> ParsePositions(char[,] board)
        {
            var positions = new List<((int, int), char)>();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board[i, j] != '.') // Фильтруем только клетки с фигурами
                        positions.Add(((i, j), board[i, j]));
                }
            }
            return positions;
        }

        // Проверяет, что набор фигур одинаков на начальной и конечной досках
        public static bool ValidateBoards(
            List<((int, int), char)> start,
            List<((int, int), char)> end)
        {
            // Подсчитывает количество каждой фигуры (например, 'W', 'B') на каждой доске
            var startCounts = start.GroupBy(x => x.Item2).ToDictionary(g => g.Key, g => g.Count());
            var endCounts = end.GroupBy(x => x.Item2).ToDictionary(g => g.Key, g => g.Count());

            // Сравнивает наборы фигур на начальной и конечной досках
            return startCounts.OrderBy(kv => kv.Key).SequenceEqual(endCounts.OrderBy(kv => kv.Key));
        }

        // Воссоздаёт доску в виде массива символов на основе позиций фигур
        private static char[,] RecreateBoard(List<((int, int), char)> positions)
        {
            var board = new char[3, 3];

            // Инициализируем пустую доску
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    board[i, j] = '.';

            // Заполняем доску фигурами
            foreach (var (position, color) in positions)
            {
                var (x, y) = position;
                board[x, y] = color;
            }

            return board;
        }

        // Выводит доску в консоль
        private static void PrintBoard(char[,] board)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Console.Write(board[i, j]);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        // Решение задачи: многомерный BFS для нахождения минимального количества ходов
        public static int SolvePuzzle(
            List<((int, int), char)> startPositions,
            List<((int, int), char)> endPositions)
        {
            var queue = new Queue<(List<((int, int), char)> positions, int steps)>();
            var visited = new HashSet<string>();

            // Кодирует текущее состояние фигур на доске в строку
            string EncodeState(List<((int, int), char)> positions) =>
                string.Join(",", positions.OrderBy(pos => pos.Item1).Select(pos => $"{pos.Item1.Item1}{pos.Item1.Item2}{pos.Item2}"));

            // Инициализация BFS
            queue.Enqueue((startPositions, 0));
            visited.Add(EncodeState(startPositions));

            while (queue.Count > 0)
            {
                var (currentPositions, steps) = queue.Dequeue();

                // Проверяем, достигли ли мы целевого состояния
                if (currentPositions.OrderBy(x => x.Item1).SequenceEqual(endPositions.OrderBy(x => x.Item1)))
                    return steps;

                // Генерируем возможные ходы для всех фигур
                for (int i = 0; i < currentPositions.Count; i++)
                {
                    var currentKnight = currentPositions[i];
                    foreach (var move in Moves)
                    {
                        int nx = currentKnight.Item1.Item1 + move.Item1;
                        int ny = currentKnight.Item1.Item2 + move.Item2;

                        // Проверяем, допустима ли клетка
                        if (!IsValid(nx, ny, currentPositions.Select(p => p.Item1).ToHashSet()))
                            continue;

                        // Формируем новое состояние после хода
                        var newPositions = new List<((int, int), char)>(currentPositions)
                        {
                            [i] = ((nx, ny), currentKnight.Item2)
                        };

                        string newState = EncodeState(newPositions);

                        // Добавляем новое состояние в очередь, если оно ещё не посещено
                        if (visited.Add(newState))
                        {
                            queue.Enqueue((newPositions, steps + 1));
                        }
                    }
                }
            }

            // Если решение не найдено
            return -1;
        }

        // Проверяет, допустима ли клетка для перемещения
        private static bool IsValid(int x, int y, HashSet<(int, int)> occupied)
        {
            return x >= 0 && x < 3 && y >= 0 && y < 3 && !occupied.Contains((x, y));
        }
    }
}