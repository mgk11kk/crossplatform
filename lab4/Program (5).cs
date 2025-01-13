using McMaster.Extensions.CommandLineUtils;
using System;
using System.IO;
using Lab1;
using Lab2;
using Lab3;

namespace Lab4
{
    [Command(Name = "Lab4", Description = "A tool for running labs.")] // Атрибут команды, задающий имя и описание.
    [Subcommand(typeof(VersionCommand), typeof(RunCommand), typeof(SetPathCommand))] // Указание подкоманд, доступных для выполнения.
    class Program
    {
        static int Main(string[] args)
        {
            var app = new CommandLineApplication<Program>();
            app.Conventions.UseDefaultConventions();

            // Действие, если команда не указана.
            app.OnExecute(() => 
            {
                // Отображение справочной информации.
                app.ShowHelp(); 
                return 1;
            });

            // Указание, как обрабатывать нераспознанные аргументы: прекратить разбор и собрать их.
            app.UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.StopParsingAndCollect;

            return app.Execute(args);
        }
    }

    // Команда для отображения версии программы и автора.
    [Command(Name = "version", Description = "Displays the program version and author.")]
    class VersionCommand
    {
        // Выполняется при вызове команды version.
        private int OnExecute() 
        {
            Console.WriteLine("Author: Zlatous Illia");
            Console.WriteLine("Version: 1.0.0");
            return 0;
        }
    }

    // Команда для запуска указанной лабораторной работы.
    [Command(Name = "run", Description = "Runs the specified lab.")] // Атрибут команды.
    [Subcommand(typeof(Lab1Command), typeof(Lab2Command), typeof(Lab3Command))] // Указание доступных подкоманд.
    class RunCommand
    {
        // Метод вызывается, если подкоманда не указана.
        private int OnExecute(CommandLineApplication app) 
        {
            Console.WriteLine("Please specify a lab to run (lab1, lab2, lab3).");
            app.ShowHelp();
            return 1;
        }
    }

    // Команда для установки пути к входным и выходным файлам.
    [Command(Name = "set-path", Description = "Sets the path for input and output files.")]
    class SetPathCommand
    {
        // Опция для задания пути через аргумент.
        [Option("-p|--path <PATH>", Description = "Path to the folder with input and output files.")]
        public string? Path { get; set; } 

        // Метод выполняется при вызове команды set-path.
        private int OnExecute() 
        {
            if (string.IsNullOrEmpty(Path))
            {
                Console.WriteLine("Error: Path is required. Use -p or --path to specify the folder.");
                return 1;
            }

            // Установка переменной окружения LAB_PATH.
            Environment.SetEnvironmentVariable("LAB_PATH", Path, EnvironmentVariableTarget.User);
            Console.WriteLine($"LAB_PATH has been set to: {Path}");
            return 0;
        }
    }

    // Абстрактный базовый класс для команд, запускающих лабораторные работы.
    abstract class LabCommandBase
    {
        // Опция для входного файла.
        [Option("-i|--input <INPUT_FILE>", Description = "Input file path.")]
        public string? InputFile { get; set; }

        // Опция для выходного файла.
        [Option("-o|--output <OUTPUT_FILE>", Description = "Output file path.")]
        public string? OutputFile { get; set; } 

        // Метод для выполнения лабораторной работы, передаваемый как делегат.
        protected int ExecuteLab(Action<string, string> runLabMethod)
        {
            string inputPath = ResolveFilePath(InputFile, "INPUT.TXT");
            string outputPath = ResolveFilePath(OutputFile, "OUTPUT.TXT");

            // Проверка существования входного файла.
            if (!File.Exists(inputPath))
            {
                Console.WriteLine($"Error: Input file '{inputPath}' does not exist.");
                return 1;
            }

            // Убедиться, что директория выходного файла существует или создать её.
            string? outputDirectory = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(outputDirectory) && !Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            Console.WriteLine("Running Lab\n");
            runLabMethod(inputPath, outputPath);

            return 0;
        }

        // Метод для определения пути к файлу с учетом пользовательских параметров и переменных окружения.
        private string ResolveFilePath(string? userProvidedPath, string defaultFileName)
        {   
            if (!string.IsNullOrEmpty(userProvidedPath))
            {
                return userProvidedPath;
            }

            string? labPath = Environment.GetEnvironmentVariable("LAB_PATH", EnvironmentVariableTarget.User);
            
            // Если переменная окружения задана и папка существует.
            if (!string.IsNullOrEmpty(labPath) && Directory.Exists(labPath))
            {
                string labPathFile = Path.Combine(labPath, defaultFileName);
                if (File.Exists(labPathFile)) return labPathFile;
            }

            // Последняя проверка в профиле пользователя.
            string userProfileFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), defaultFileName);
            return userProfileFile;
        }
    }

    // Команда для запуска Lab1.
    [Command(Name = "lab1", Description = "Run Lab 1")]
    class Lab1Command : LabCommandBase
    {
        private int OnExecute()
        {
            // Вызов метода RunLab из Lab1.
            return ExecuteLab((inputPath, outputPath) =>
            {
                Lab1.Program.RunLab(inputPath, outputPath); 
            });
        }
    }

    // Команда для запуска Lab2.
    [Command(Name = "lab2", Description = "Run Lab 2")]
    class Lab2Command : LabCommandBase
    {
        private int OnExecute()
        {
            // Вызов метода RunLab из Lab2.
            return ExecuteLab((inputPath, outputPath) =>
            {
                Lab2.Program.RunLab(inputPath, outputPath); 
            });
        }
    }

    // Команда для запуска Lab3.
    [Command(Name = "lab3", Description = "Run Lab 3")]
    class Lab3Command : LabCommandBase
    {
        private int OnExecute()
        {
            // Вызов метода RunLab из Lab3.
            return ExecuteLab((inputPath, outputPath) =>
            {
                Lab3.Program.RunLab(inputPath, outputPath); 
            });
        }
    }
}