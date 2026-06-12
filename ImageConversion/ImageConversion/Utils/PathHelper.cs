using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageConversion.Utils
{
    public class PathHelper
    {
        public static string GetOutputDirectory(string inputPath)
        {
            var baseDir = Path.GetDirectoryName(inputPath);
            var fileName = Path.GetFileNameWithoutExtension(inputPath);
            var outputDir = Path.Combine(baseDir, fileName);

            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            return outputDir;
        }

        public static string OutputPath(string inputPath, string targetExt)
        {
            var dir = GetOutputDirectory(inputPath);
            var fileName = Path.GetFileNameWithoutExtension(inputPath);
            return Path.Combine(dir, $"{fileName}.{targetExt}");
        }


        public static string OutputPathWithIndex(string inputPath, string targetExt, int index)
        {
            var dir = GetOutputDirectory(inputPath);
            var fileName = Path.GetFileNameWithoutExtension(inputPath);
            return Path.Combine(dir, $"{fileName}_{index}.{targetExt}");
        }
    }
}
