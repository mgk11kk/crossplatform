using Lab1;
using Lab2;
using Lab3;

namespace LabLibrary
{
    public static class LabRunner
    {
        public static void RunChosenLab(string labName, string inputFilePath, string outputFilePath)
        {
            switch (labName)
            {
                case "Lab1":
                    Lab1.Program.RunLab(inputFilePath, outputFilePath);
                    break;
                case "Lab2":
                    Lab2.Program.RunLab(inputFilePath, outputFilePath);
                    break;
                case "Lab3":
                    Lab3.Program.RunLab(inputFilePath, outputFilePath);
                    break;
                default:
                    throw new ArgumentException("Invalid lab name.");
            }
        }
    }
}