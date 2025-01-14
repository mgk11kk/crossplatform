using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace Lab3
{
    // Можливі ходи коня (всі можливі "Г"-подібні переміщення)
    private static readonly (int, int)[] Moves =
    {
        (-2, -1), (-1, -2), (1, -2), (2, -1),
        (2, 1), (1, 2), (-1, 2), (-2, 1)
    };

    public static void Main(string[] args)
    {
        // Визначення шляхів до вхідного та вихідного файлів
        string rootDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\.."));
        string inputPath = Path.Combine(rootDirectory, "INPUT.TXT");
        string outputPath = Path.Combine(rootDirectory, "OUTPUT.TXT");

        RunLab(inputPath, outputPath);
    }

    public static void RunLab(string inputPath, string outputPath)
    {
        // Читання даних із вхідного файлу
        var inputLines = File.ReadAllLines(inputPath);

        // Перевірка формату вхідних даних на коректність
        if (!ValidateInputFormat(inputLines, out string errorMessage))
        {
            Console.WriteLine($"Input validation error: {errorMessage}");
            return;
        }

        // Розподіл даних на початкову та кінцеву дошки
        var (startBoard, endBoard) = ParseInput(inputLines);

        // Отримання позицій фігур із дошок з їх кольорами
        var startPositions = ParsePositions(startBoard);
        var endPositions = ParsePositions(endBoard);

        // Перевірка, що початкова та кінцева дошки містять однакові фігури
        if (!ValidateBoards(startPositions, endPositions))
        {
            File.WriteAllText(outputPath, "-1");
            Console.WriteLine("Result: -1");
            return;
        }

        // Виведення початкової та кінцевої дошок
        Console.WriteLine("Start board:");
        PrintBoard(RecreateBoard(startPositions));

        Console.WriteLine("End board:");
        PrintBoard(RecreateBoard(endPositions));

        // Розв'язання задачі за допомогою багатовимірного BFS
        int result = SolvePuzzle(startPositions, endPositions);

        // Запис результату у вихідний файл і виведення у консоль
        File.WriteAllText(outputPath, result.ToString());
        Console.WriteLine($"Result: {result}");
    }

    // Перевірка формату вхідних даних на коректність
    public static bool ValidateInputFormat(string[] inputLines, out string errorMessage)
    {
        // Перевірка, що вхідний файл містить рівно 3 рядки
        if (inputLines.Length != 3)
        {
            errorMessage = "Input file must contain exactly 3 lines.";
            return false;
        }

        // Перевірка кожного рядка на коректність
        for (int i = 0; i < inputLines.Length; i++)
        {
            string line = inputLines[i];
            // Перевірка довжини рядка і наявності пробілу на потрібній позиції
            if (line.Length != 7 || line[3] != ' ')
            {
                errorMessage = $"Line {i + 1} must have exactly 7 characters with a space at position 4.";
                return false;
            }

            // Розподіл рядка на ліву та праву частини
            string leftPart = line.Substring(0, 3);
            string rightPart = line.Substring(4, 3);

            // Перевірка, що кожна частина містить тільки допустимі символи ('W', 'B', '.')
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

    // Розбір вхідних даних: створює масиви для початкової та кінцевої дошок
    public static (char[,], char[,]) ParseInput(string[] inputLines)
    {
        char[,] startBoard = new char[3, 3];
        char[,] endBoard = new char[3, 3];

        for (int i = 0; i < 3; i++)
        {
            var parts = inputLines[i].Split(' '); // Розподіл рядків на початкову та кінцеву частини
            for (int j = 0; j < 3; j++)
            {
                startBoard[i, j] = parts[0][j];
                endBoard[i, j] = parts[1][j];
            }
        }

        return (startBoard, endBoard);
    }

    // Витягує позиції всіх фігур із дошки
    public static List<((int, int), char)> ParsePositions(char[,] board)
    {
        var positions = new List<((int, int), char)>();
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (board[i, j] != '.') // Вибираємо тільки клітинки з фігурами
                    positions.Add(((i, j), board[i, j]));
            }
        }
        return positions;
    }

    // Перевірка, що набори фігур на початковій і кінцевій дошках однакові
    public static bool ValidateBoards(
        List<((int, int), char)> start,
        List<((int, int), char)> end)
    {
        // Підрахунок кількості кожної фігури на дошках
        var startCounts = start.GroupBy(x => x.Item2).ToDictionary(g => g.Key, g => g.Count());
        var endCounts = end.GroupBy(x => x.Item2).ToDictionary(g => g.Key, g => g.Count());

        // Порівняння наборів фігур на дошках
        return startCounts.OrderBy(kv => kv.Key).SequenceEqual(endCounts.OrderBy(kv => kv.Key));
    }

    // Відтворює дошку у вигляді масиву символів на основі позицій фігур
    private static char[,] RecreateBoard(List<((int, int), char)> positions)
    {
        var board = new char[3, 3];

        // Ініціалізація порожньої дошки
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                board[i, j] = '.';

        // Заповнення дошки фігурами
        foreach (var (position, color) in positions)
        {
            var (x, y) = position;
            board[x, y] = color;
        }

        return board;
    }

    // Виведення дошки в консоль
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

    // Розв'язання задачі: багатовимірний BFS для знаходження мінімальної кількості ходів
    public static int SolvePuzzle(
        List<((int, int), char)> startPositions,
        List<((int, int), char)> endPositions)
    {
        var queue = new Queue<(List<((int, int), char)> positions, int steps)>();
        var visited = new HashSet<string>();

        // Кодує поточний стан фігур на дошці у строку
        string EncodeState(List<((int, int), char)> positions) =>
            string.Join(",", positions.OrderBy(pos => pos.Item1).Select(pos => $"{pos.Item1.Item1}{pos.Item1.Item2}{pos.Item2}"));

        // Ініціалізація BFS
        queue.Enqueue((startPositions, 0));
        visited.Add(EncodeState(startPositions));

        while (queue.Count > 0)
        {
            var (currentPositions, steps) = queue.Dequeue();

            // Перевірка, чи досягнуто цільового стану
            if (currentPositions.OrderBy(x => x.Item1).SequenceEqual(endPositions.OrderBy(x => x.Item1)))
                return steps;

            // Генерація можливих ходів для всіх фігур
            for (int i = 0; i < currentPositions.Count; i++)
            {
                var currentKnight = currentPositions[i];
                foreach (var move in Moves)
                {
                    int nx = currentKnight.Item1.Item1 + move.Item1;
                    int ny = currentKnight.Item1.Item2 + move.Item2;

                    // Перевірка, чи допустима клітинка
                    if (!IsValid(nx, ny, currentPositions.Select(p => p.Item1).ToHashSet()))
                        continue;

                    // Формування нового стану після ходу
                    var newPositions = new List<((int, int), char)>(currentPositions)
                    {
                        [i] = ((nx, ny), currentKnight.Item2)
                    };

                    string newState = EncodeState(newPositions);

                    // Додавання нового стану в чергу, якщо воно ще не відвідано
                    if (visited.Add(newState))
                    {
                        queue.Enqueue((newPositions, steps + 1));
                    }
                }
            }
        }

        // Якщо рішення не знайдено
        return -1;
    }

    // Перевірка, чи допустима клітинка для переміщення
    private static bool IsValid(int x, int y, HashSet<(int, int)> occupied)
    {
        return x >= 0 && x < 3 && y >= 0 && y < 3 && !occupied.Contains((x, y));
    }
}
