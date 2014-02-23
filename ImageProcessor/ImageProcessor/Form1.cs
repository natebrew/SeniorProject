using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageProcessor
{
    
    public partial class Form1 : Form
    {

        private PictureModificator myPicAnalyzer = new PictureModificator();
        
        private Bitmap bitmap;
        private Bitmap imageNormal;


        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            OpenFileDialog open = new OpenFileDialog();
            if (open.ShowDialog() == DialogResult.OK)
            {
                imageNormal = new Bitmap(open.FileName);
                bitmap = imageNormal; //making them the same.
                pictureBox1.Image = bitmap;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Bitmap newBit = new Bitmap(bitmap.Width, bitmap.Height);

            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    Color curCol = bitmap.GetPixel(i, j);

                    int grayScale = (int)((curCol.R * .3) + (curCol.G * .59) + (curCol.B * .11));

                    Color newCol = Color.FromArgb(grayScale, grayScale, grayScale);

                    newBit.SetPixel(i, j, newCol);
                }
            }

            bitmap = newBit;

            pictureBox1.Image = bitmap;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            analyzePicture();
        }

        private System.Drawing.Point[] ToPointsArray(List<IntPoint> points)
        {
            System.Drawing.Point[] array = new System.Drawing.Point[points.Count];

            for (int i = 0, n = points.Count; i < n; i++)
            {
                array[i] = new System.Drawing.Point(points[i].X, points[i].Y);
            }

            return array;
        }

        private void analyzePicture()
        {
            try
            {
                myPicAnalyzer.setCurrentImage(bitmap);
                myPicAnalyzer.applyGrayscale();

                // use only one filter

                myPicAnalyzer.applySobelEdgeFilter();
                //myPicAnalyzer.Laplacian3x3Filter();
                //myPicAnalyzer.Laplacian5x5Filter();
                //myPicAnalyzer.Laplacian3x3OfGaussian5x5Filter2();
                //myPicAnalyzer.KirschFilter();
                //myPicAnalyzer.Laplacian5x5OfGaussian3x3Filter();
                //myPicAnalyzer.Laplacian5x5OfGaussian5x5Filter2();

                // this filter also applies a grey-scale to the image so we can skipp that step. 
                //myPicAnalyzer.Sobel3x3Filter();

                myPicAnalyzer.markKnownForms();
                pictureBox1.Image = myPicAnalyzer.getCurrentImage();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.StackTrace);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                //set the image
                myPicAnalyzer.setCurrentImage(bitmap);
                //filter the image
                myPicAnalyzer.Sobel3x3Filter();
                //find blobs
                myPicAnalyzer.findBlobs();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.StackTrace);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Bitmap newBit = new Bitmap(bitmap.Width, bitmap.Height);

            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    Color curCol = bitmap.GetPixel(i, j);
                    int red = (int)(curCol.R / (curCol.R + curCol.G + curCol.B));
                    int green = (int)(curCol.G / (curCol.R + curCol.G + curCol.B));
                    int blue = (int)(curCol.B / (curCol.R + curCol.G + curCol.B));
                    Color newCol = Color.FromArgb(red, green, blue);
                    newBit.SetPixel(i, j, newCol);
                }
            }

            bitmap = newBit;

            pictureBox1.Image = bitmap;
        }

        private void button6_Click(object sender, EventArgs e)
        {


            //float minBright = 0;
            //float maxBright = 1;

            Bitmap newBit = bitmap;

            uint pixels = (uint)newBit.Height * (uint)newBit.Width;
            decimal Const = 255 / (decimal)pixels;

            int x, y, R, G, B;

            ImageStatistics stats = new ImageStatistics(newBit);

            int[] cdfR = stats.Red.Values.ToArray();
            int[] cdfG = stats.Green.Values.ToArray();
            int[] cdfB = stats.Blue.Values.ToArray();

            for (int r = 1; r <= 255; r++)
            {
                cdfR[r] = cdfR[r] + cdfR[r - 1];
                cdfG[r] = cdfG[r] + cdfG[r - 1];
                cdfB[r] = cdfB[r] + cdfB[r - 1];
            }

            for (y = 0; y < newBit.Height; y++)
            {
                for (x = 0; x < newBit.Width; x++)
                {
                    Color pixCol = newBit.GetPixel(x, y);

                    R = (int)((decimal)cdfR[pixCol.R] * Const);
                    G = (int)((decimal)cdfG[pixCol.G] * Const);
                    B = (int)((decimal)cdfB[pixCol.B] * Const);

                    Color newCol = Color.FromArgb(R, G, B);
                    newBit.SetPixel(x, y, newCol);
                }
            }

            //for (int i = 0; i < bitmap.Width; i++)
            //{
            //    for (int j = 0; j < bitmap.Height; j++)
            //    {
            //        float pixBright = bitmap.GetPixel(i, j).GetBrightness();
            //        minBright = Math.Min(minBright, pixBright);
            //        maxBright = Math.Max(maxBright, pixBright);
            //    }
            //}

            //for (int x = 0; x < bitmap.Width; x++)
            //{
            //    for (int y = 0; y < bitmap.Height; y++)
            //    {
            //        Color pixCol = bitmap.GetPixel(x, y);
            //        float normPixBright = (pixCol.GetBrightness() - minBright) / (maxBright - minBright);
            //        Color normPixCol = ColorConverter.ColorFromAhsb(pixCol.A, pixCol.GetHue(), pixCol.GetSaturation(), normPixBright);
            //        newBit.SetPixel(x, y, normPixCol);
            //    }
            //}

            bitmap = newBit;

            pictureBox1.Image = bitmap;
        }
    }

    class ColorConverter
    {
        public static Color ColorFromAhsb(int a, float h, float s, float b)
        {
            if (0 > a || 255 < a)
            {
                throw new ArgumentOutOfRangeException("Invalid alpha value.");
            }
            if (0f > h || 360f < h)
            {
                throw new ArgumentOutOfRangeException("Invalid hue value.");
            }
            if (0f > s || 1f < s)
            {
                throw new ArgumentOutOfRangeException("Invalid saturation value.");
            }
            if (0f > b || 1f < b)
            {
                throw new ArgumentOutOfRangeException("Invalid brightness value.");
            }

            if (0 == s)
            {
                return Color.FromArgb(a, Convert.ToInt32(b * 255),
                Convert.ToInt32(b * 255), Convert.ToInt32(b * 255));
            }

            float fMax, fMid, fMin;
            int iSextant, iMax, iMid, iMin;

            if (0.5 < b)
            {
                fMax = b - (b * s) + s;
                fMin = b + (b * s) - s;
            }
            else
            {
                fMax = b + (b * s);
                fMin = b - (b * s);
            }

            iSextant = (int)Math.Floor(h / 60f);
            if (300f <= h)
            {
                h -= 360f;
            }
            h /= 60f;
            h -= 2f * (float)Math.Floor(((iSextant + 1f) % 6f) / 2f);
            if (0 == iSextant % 2)
            {
                fMid = h * (fMax - fMin) + fMin;
            }
            else
            {
                fMid = fMin - h * (fMax - fMin);
            }

            iMax = Convert.ToInt32(fMax * 255);
            iMid = Convert.ToInt32(fMid * 255);
            iMin = Convert.ToInt32(fMin * 255);

            switch (iSextant)
            {
                case 1:
                    return Color.FromArgb(a, iMid, iMax, iMin);
                case 2:
                    return Color.FromArgb(a, iMin, iMax, iMid);
                case 3:
                    return Color.FromArgb(a, iMin, iMid, iMax);
                case 4:
                    return Color.FromArgb(a, iMid, iMin, iMax);
                case 5:
                    return Color.FromArgb(a, iMax, iMin, iMid);
                default:
                    return Color.FromArgb(a, iMax, iMid, iMin);
            }
        }
    }
}
