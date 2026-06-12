using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesseractTest
{
    public class ImageOpenCvSharp
    {
        public Bitmap Grayscale(Bitmap bmp)
        {
            Mat image = BitmapConverter.ToMat(bmp);

            // 회색조 처리
            Mat grayImage = new Mat();
            Cv2.CvtColor(image, grayImage, ColorConversionCodes.BGR2GRAY);

            return BitmapConverter.ToBitmap(grayImage);
        }

        public Bitmap Binary(Bitmap bmp, double value = 0.7)
        {
            Mat image = BitmapConverter.ToMat(bmp);
            double thresholdValue = value * 255;

            // 회색조 처리
            Mat grayImage = new Mat();

            // 이미 회색조라면 binary만 처리
            if (image.Channels() == 1)
            {
                grayImage = image.Clone();
            }
            else
            {
                Cv2.CvtColor(image, grayImage, ColorConversionCodes.BGR2GRAY);
            }
                
            // binary 처리
            Mat binaryImage = new Mat();
            Cv2.Threshold(grayImage, binaryImage, thresholdValue, 255, ThresholdTypes.Binary);

            return BitmapConverter.ToBitmap(binaryImage);
        }

        public Bitmap Resize(Bitmap bmp, double value)
        {
            Mat image = BitmapConverter.ToMat(bmp);

            Mat resizeImage = new Mat();

            Cv2.Resize(image, resizeImage, new OpenCvSharp.Size(), value, value, InterpolationFlags.Cubic);

            return BitmapConverter.ToBitmap(resizeImage);
        }
    }
}
