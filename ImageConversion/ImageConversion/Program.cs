using ImageConversion.Models;
using ImageConversion.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static void Main()
    {
        string inputString = Console.ReadLine();

        string inputPath = inputString.Split(",")[0].Trim();
        string targetExtension = inputString.Split(",")[1].Trim();


        if (inputPath.Length == 0) return;

        var request = new ConvertRequest
        {
            InputPath = inputPath,
            TargetExtension = targetExtension
        };

        var originExtension = Path.GetExtension(request.InputPath);

        var service = new ImageConvertService();

        try
        {
            string resultPath = service.Convert(request, ShowProgress);
            Console.WriteLine(resultPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine("오류 : ");
            Console.WriteLine(ex.Message);
        }

    }

    public static void ShowProgress(int current, int total)
    {
        int width = 30;
        int filled = (int)((double)current / total * width);

        string bar = new string('■', filled) + new string('□', width - filled);
        int percent = (int)((double)current / total * 100);

        Console.Write($"\r[{bar}] {percent}% ({current} / {total})");

        if (current == total) Console.WriteLine();

    }
}