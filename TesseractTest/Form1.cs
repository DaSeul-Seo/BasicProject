using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tesseract;

namespace TesseractTest
{
    public partial class Form1 : Form
    {
        private static string imageFilter = "Image Files (*.bmp;*.jpg;*.jpeg;*.png)|*.BMP;*.JPG;*.JPEG;*.PNG";
        private static string imgFile = string.Empty;
        private Point _startPoint = new Point(0, 0);
        private Point _endPoint = new Point(0, 0);
        private int _minX, _minY, _maxX, _maxY;
        private bool isDrag = false;
        private Bitmap cropBmpImage;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // 언어 종류 리스트
            string[] langList = { "eng", "kor", "eng+kor" };
            comboBox1.Items.AddRange(langList);
            comboBox1.SelectedIndex = 0;
            //pictureBox2 = null;
        }

        /// <summary>
        /// 이미지 파일 불러오기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            // 파일 탐색기 열기
            OpenFileDialog dialog = new OpenFileDialog();
            // 이미지 확장자 지정
            dialog.Filter = imageFilter;

            // 취소 누를 때
            if (dialog.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            // 이미지 파일 나타내기
            imgFile = dialog.FileName;
            pictureBox1.Image = Image.FromFile(imgFile);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            label7.Text = $"{pictureBox1.Image.Width} x {pictureBox1.Image.Height}";

            // 파일경로 작성
            textBox1 .Text = imgFile;
        }

        /// <summary>
        /// Full OCR 실행하기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";
            // 이미지가 없을 경우는 실행하지 않는다.
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("이미지를 선택해주세요.");
                return;
            }

            using (Bitmap bmp = (Bitmap)pictureBox1.Image.Clone())
            {
                string ocrText = OcrProcess(bmp);
                string rstText = ocrText.Replace("\n", "\r\n") + "\r\n";

                textBox2.AppendText(rstText);
            }
        }

        private string OcrProcess(Bitmap bmp)
        {
            try
            {
                string path = Application.StartupPath + @"\tessdata";
                string langType = comboBox1.SelectedItem.ToString();
                using (var engine = new TesseractEngine(path, langType, EngineMode.TesseractOnly))
                {
                    using (var page = engine.Process(bmp))
                    {
                        return page.GetText();
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        /// <summary>
        /// Crop OCR 실행하기
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";
            // 이미지가 없을 경우는 실행하지 않는다.
            if (pictureBox2.Image == null)
            {
                MessageBox.Show("이미지를 크롭하여 주세요.");
                return;
            }

            using (Bitmap bmp = (Bitmap)pictureBox2.Image.Clone())
            {
                string ocrText = OcrProcess(bmp);
                string rstText = ocrText.Replace("\n", "\r\n") + "\r\n";

                textBox2.AppendText(rstText);
            }
        }

        /// <summary>
        /// Full OCR - Grayscale
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            // 이미지가 없을 경우는 실행하지 않는다.
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("이미지를 선택해주세요.");
                return;
            }

            using (Bitmap bmp = (Bitmap)pictureBox1.Image.Clone())
            {
                ImageOpenCvSharp openCv = new ImageOpenCvSharp();
                Bitmap grayBmp = openCv.Grayscale(bmp);
                pictureBox1.Image = grayBmp;
            }
        }

        /// <summary>
        /// Full OCR - Binary
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            // 이미지가 없을 경우는 실행하지 않는다.
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("이미지를 선택해주세요.");
                return;
            }

            using (Bitmap bmp = (Bitmap)pictureBox1.Image.Clone())
            {
                ImageOpenCvSharp openCv = new ImageOpenCvSharp();
                Bitmap grayBmp = openCv.Binary(bmp);
                pictureBox1.Image = grayBmp;
            }
        }

        /// <summary>
        /// Full OCR - Resize
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            // 이미지가 없을 경우는 실행하지 않는다.
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("이미지를 선택해주세요.");
                return;
            }

            using (Bitmap bmp = (Bitmap)pictureBox1.Image.Clone())
            {
                ImageOpenCvSharp openCv = new ImageOpenCvSharp();
                double resizeVal;

                if (string.IsNullOrEmpty(textBox5.Text))
                {
                    resizeVal = 2;
                }
                else if (!double.TryParse(textBox5.Text, out resizeVal))
                {
                    MessageBox.Show("숫자를 입력해주세요.");
                    return;
                }

                Bitmap resizeBmp = openCv.Resize(bmp, resizeVal);
                pictureBox1.Image = resizeBmp;
            }
        }

        /// <summary>
        /// Full OCR - 초기화
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button10_Click(object sender, EventArgs e)
        {
            // 이미지가 없을 경우는 실행하지 않는다.
            if (pictureBox1.Image == null)
            {
                MessageBox.Show("이미지를 선택해주세요.");
                return;
            }

            pictureBox1.Image = Image.FromFile(imgFile);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
        }

        /// <summary>
        /// Crop OCR - Grayscale
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button11_Click(object sender, EventArgs e)
        {
            // 이미지가 없을 경우는 실행하지 않는다.
            if (pictureBox2.Image == null)
            {
                MessageBox.Show("이미지를 선택해주세요.");
                return;
            }

            using (Bitmap bmp = (Bitmap)pictureBox2.Image.Clone())
            {
                ImageOpenCvSharp openCv = new ImageOpenCvSharp();
                Bitmap grayBmp = openCv.Grayscale(bmp);
                pictureBox2.Image = grayBmp;
            }
        }

        /// <summary>
        /// Crop OCR - Binary
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button12_Click(object sender, EventArgs e)
        {
            // 이미지가 없을 경우는 실행하지 않는다.
            if (pictureBox2.Image == null)
            {
                MessageBox.Show("이미지를 선택해주세요.");
                return;
            }

            using (Bitmap bmp = (Bitmap)pictureBox2.Image.Clone())
            {
                ImageOpenCvSharp openCv = new ImageOpenCvSharp();
                Bitmap grayBmp = openCv.Binary(bmp);
                pictureBox2.Image = grayBmp;
            }
        }

        /// <summary>
        /// Crop OCR - Resize
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button13_Click(object sender, EventArgs e)
        {
            // 이미지가 없을 경우는 실행하지 않는다.
            if (pictureBox2.Image == null)
            {
                MessageBox.Show("이미지를 선택해주세요.");
                return;
            }

            using (Bitmap bmp = (Bitmap)pictureBox2.Image.Clone())
            {
                ImageOpenCvSharp openCv = new ImageOpenCvSharp();
                double resizeVal;

                if (string.IsNullOrEmpty(textBox6.Text))
                {
                    resizeVal = 2;
                }
                else if(!double.TryParse(textBox6.Text, out resizeVal))
                {
                    MessageBox.Show("숫자를 입력해주세요.");
                    return;
                }

                Bitmap resizeBmp = openCv.Resize(bmp, resizeVal);
                pictureBox2.Image = resizeBmp;
            }
        }

        /// <summary>
        /// Crop OCR - 초기화
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button16_Click(object sender, EventArgs e)
        {
            // 이미지가 없을 경우는 실행하지 않는다.
            if (pictureBox2.Image == null)
            {
                MessageBox.Show("이미지를 선택해주세요.");
                return;
            }

            pictureBox2.Image = cropBmpImage;
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            pictureBox2.Image = null;
            if (e.Button != MouseButtons.Left) return;

            isDrag = true;
            _startPoint = e.Location;
            _endPoint = e.Location;

            pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (!isDrag) return;
            if (e.Button != MouseButtons.Left) return;
            isDrag = false;

            Point imgStart = ToImagePoint(_startPoint);
            Point imgEnd = ToImagePoint(_endPoint);

            int x, y, width, height;

            x = Math.Min(imgStart.X, imgEnd.X);
            y = Math.Min(imgStart.Y, imgEnd.Y);
            width = Math.Abs(imgStart.X - imgEnd.X);
            height = Math.Abs(imgStart.Y - imgEnd.Y);

            // 이미지 crop
            Rectangle rect = new Rectangle(x, y, width, height);
            // 원본이미지
            Bitmap bmp = (Bitmap)pictureBox1.Image;
            rect.Intersect(new Rectangle(0, 0, bmp.Width, bmp.Height));
            // crop
            Bitmap cropBmp = bmp.Clone(rect, bmp.PixelFormat);
            // crop이미지 나타내기
            
            cropBmpImage = cropBmp;
            pictureBox2.Image = cropBmp;
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            Point imgPoint = GetImagePoint(e.Location);

            if (imgPoint != Point.Empty)
            {
                label2.Text = $"X: {imgPoint.X}, Y: {imgPoint.Y}";
                
                Bitmap bmp = (Bitmap)pictureBox1.Image;

                if (imgPoint.X < bmp.Width && imgPoint.Y < bmp.Height)
                {
                    Color color = bmp.GetPixel(imgPoint.X, imgPoint.Y);
                    //label5.Text = $"RGB: {color.R},{color.G},{color.B}";
                }
            }

            if (!isDrag) return;
            if (e.Button != MouseButtons.Left) return;

            _endPoint = e.Location;

            int w = Math.Abs(_endPoint.X - _startPoint.X);
            int h = Math.Abs(_endPoint.Y - _startPoint.Y);

            label6.Text = $"selection: {w} x {h}";

            pictureBox1.Invalidate();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (!isDrag) return;
            Graphics g = e.Graphics;

            int x, y, width, height;

            x = Math.Min(_startPoint.X, _endPoint.X);
            y = Math.Min(_startPoint.Y, _endPoint.Y);
            width = Math.Abs(_startPoint.X - _endPoint.X);
            height = Math.Abs(_startPoint.Y - _endPoint.Y);

            using (Pen pen = new Pen(Color.Red, 2))
            {
                g.DrawRectangle(pen, x, y, width, height);
            }
        }

        private Point ToImagePoint(Point p)
        {
            Rectangle imgRect = GetImageRect();

            if (!imgRect.Contains(p)) return Point.Empty;

            float scaleX = (float)pictureBox1.Image.Width / imgRect.Width;
            float scaleY = (float)pictureBox1.Image.Height / imgRect.Height;

            int x = (int)((p.X - imgRect.X) * scaleX);
            int y= (int)((p.Y - imgRect.Y) * scaleY);

            return new Point(x, y);
        }

        private Rectangle GetImageRect()
        {
            if (pictureBox1.Image == null) return Rectangle.Empty;

            float imageAspect = (float)pictureBox1.Image.Width / pictureBox1.Image.Height;
            float boxAspect = (float)pictureBox1.Width / pictureBox1.Height;

            int x, y, width, height;

            if (imageAspect > boxAspect)
            {
                width = pictureBox1 .Width;
                height = (int)(pictureBox1.Width / imageAspect);
                x = 0;
                y = (pictureBox1.Height - height) / 2;
            }
            else
            {
                height = pictureBox1.Height;
                width = (int)(pictureBox1.Height * imageAspect);
                x = (pictureBox1.Width - width) / 2;
                y = 0;
            }

            return new Rectangle(x, y, width, height);
        }

        private Point GetImagePoint(Point mousePoint)
        {
            if (pictureBox1.Image == null) return Point.Empty;

            Image img = pictureBox1.Image;

            float imageAspect = (float)img.Width / img.Height;
            float boxAspect = (float)pictureBox1 .Width / pictureBox1.Height;

            int drawWidth, drawHeight, offsetX, offsetY;

            if (imageAspect > boxAspect)
            {
                drawWidth = pictureBox1.Width;
                drawHeight = (int)(pictureBox1.Width / imageAspect);
                offsetX = 0;
                offsetY = (pictureBox1.Height - drawHeight) / 2;
            }
            else
            {
                drawWidth = (int)(pictureBox1.Height * imageAspect);
                drawHeight = pictureBox1.Height;
                offsetX = (pictureBox1.Width - drawWidth) / 2;
                offsetY = 0;
            }

            if (mousePoint.X < offsetX || mousePoint.X > offsetX + drawWidth ||
                mousePoint.Y < offsetY || mousePoint.Y > offsetY + drawHeight)
            {
                return Point.Empty;
            }

            float scaleX = (float)img.Width / drawWidth;
            float scaleY = (float)img.Height / drawHeight;

            int x = (int)((mousePoint.X - offsetX) * scaleX);
            int y = (int)((mousePoint.Y - offsetY) * scaleY);

            return new Point(x, y);
        }
    }
}
