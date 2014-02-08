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
                pictureBox2.Image = imageNormal;
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
                myPicAnalyzer.applySobelEdgeFilter();
                myPicAnalyzer.markKnownForms();
                pictureBox1.Image = myPicAnalyzer.getCurrentImage();
            }
            catch (Exception exc)
            {

            }
        }


    }
}
