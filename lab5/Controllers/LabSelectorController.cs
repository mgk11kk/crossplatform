using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Lab5.Models;
using LabLibrary;

namespace Lab5.Controllers
{
    public class LabSelectorController : Controller
    {
        public LabSelectorController()
        {
        }

        public IActionResult Lab1()
        {
            return View();
        }

        public IActionResult Lab2()
        {
            return View();
        }

        public IActionResult Lab3()
        {
            return View();
        }

        // Метод для обработки выполнения лабораторной работы
        [HttpPost]
        public IActionResult RunLab(string labName, IFormFile inputFile, IFormFile outputFile)
        {
            if (inputFile == null || outputFile == null)
            {
                ViewBag.ErrorMessage = "Please select both input and output files.";
                return View(labName);
            }

            // Определение директории на сервере для хранения загруженных файлов
            var serverDirectory = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");

            if (!Directory.Exists(serverDirectory))
            {
                Directory.CreateDirectory(serverDirectory);
            }

            // Генерация полных путей для сохранения файлов на сервере
            var inputFilePath = Path.Combine(serverDirectory, inputFile.FileName);
            var outputFilePath = Path.Combine(serverDirectory, outputFile.FileName);

            try
            {
                // Сохранение входного файла на сервере
                using (var inputStream = new FileStream(inputFilePath, FileMode.Create))
                {
                    inputFile.CopyTo(inputStream);
                }

                // Сохранение выходного файла на сервере
                using (var outputStream = new FileStream(outputFilePath, FileMode.Create))
                {
                    outputFile.CopyTo(outputStream);
                }

                // Выполнение выбранной лабораторной работы с использованием сохранённых путей к файлам
                LabRunner.RunChosenLab(labName, inputFilePath, outputFilePath);

                string inputFileContent = System.IO.File.ReadAllText(inputFilePath);
                string outputFileContent = System.IO.File.ReadAllText(outputFilePath);

                ViewBag.SuccessMessage = $"{labName} executed successfully!";
                ViewBag.InputFileContent = inputFileContent;
                ViewBag.OutputFileContent = outputFileContent;
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Error while running {labName}: {ex.Message}";
            }

            return View(labName);
        }
    }
}