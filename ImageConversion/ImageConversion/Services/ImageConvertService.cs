using ImageConversion.Models;
using ImageConversion.Utils;
using ImageMagick;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ImageConversion.Services
{
    public class ImageConvertService
    {
        public string Convert(ConvertRequest request, Action<int, int>? progress = null)
        {
            if (!File.Exists(request.InputPath))
            {
                throw new FileNotFoundException("파일 미존재.");
            }

            string inputExt = Path.GetExtension(request.InputPath).Trim('.').ToLower();
            string targerExt = request.TargetExtension.Trim('.').ToLower();

            // pdf -> 이미지
            if (inputExt == "pdf" && targerExt != "pdf")
            {
                var outputPath = ConvertPdfToImages(request.InputPath, targerExt, progress);
                return $"PDF -> 이미지 변환 완료 : {outputPath}";
            }

            // 이미지 -> pdf
            return ConvertImage(request.InputPath, targerExt);
        }

        public string ConvertImage(string inputPath, string targetExt)
        {
            string outputPath = string.Empty;
            string outputFilePath = PathHelper.OutputPath(inputPath, targetExt);

            using (var image = new MagickImage(inputPath))
            {
                image.Format = OutputMagickFormat(targetExt);

                image.Density = new Density(300);
                image.Quality = 100;
                image.BackgroundColor = MagickColors.White;
                image.Alpha(AlphaOption.Remove);
                //if (image.Format == MagickFormat.Pdf)
                //{
                //}

                image.Write(outputFilePath);
            }

            outputPath = Path.GetDirectoryName(outputFilePath);
            return outputPath;

        }

        public string ConvertPdfToImages(string inputPath, string targerExt, Action<int, int> progress)
        {
            var settings = new MagickReadSettings
            {
                Density = new Density(300)
            };

            string outputPath = string.Empty;
            string outputFilePath = string.Empty;

            using (var images = new MagickImageCollection(inputPath))
            {
                images.Read(inputPath, settings);

                int total = images.Count;
                int current = 0;

                foreach (var image in images)
                {
                    current++;

                    image.Format = OutputMagickFormat(targerExt);
                    image.BackgroundColor = MagickColors.White;
                    image.Alpha(AlphaOption.Remove);
                    image.Quality = 100;

                    outputFilePath = PathHelper.OutputPathWithIndex(inputPath, targerExt, current);
                    image.Write(outputFilePath);

                    // 진행상황 콜백
                    progress?.Invoke(current, total);
                }
            }

            outputPath = Path.GetDirectoryName(outputFilePath);
            return outputPath;
        }


        public MagickFormat OutputMagickFormat(string ext)
        {
            return ext switch
            {
                "jpg" or "jpeg" => MagickFormat.Jpeg,
                "png" => MagickFormat.Png,
                "pdf" => MagickFormat.Pdf,
                _ => throw new NotSupportedException($"지원하지 않는 확장자: {ext}")
            };
        }
    }
}
