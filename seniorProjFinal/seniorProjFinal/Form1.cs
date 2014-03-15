using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace seniorProjFinal
{
    public partial class Form1 : Form
    {

        List<IPoint> ipts1 = new List<IPoint>();
        List<IPoint> ipts2 = new List<IPoint>();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            string pathToFile = openFileDialog.FileName;

            OpenFileDialog open = new OpenFileDialog();
            open.ShowDialog();
            string path = open.FileName;

            // Processing starts here
            Stopwatch watch = new Stopwatch();
            watch.Start();

            try
            {
                // Load an Image
                Bitmap img = new Bitmap(pathToFile);
                Bitmap img2 = new Bitmap(path);

                img.SetResolution(360, 360);
                img2.SetResolution(360, 360);

                //pictureBox1.Image = img;
                //pictureBox2.Image = img2;

                // trying edge detection here
                //img = ImgProcessor.Sobel3x3Filter(img, true);
                //img2 = ImgProcessor.Sobel3x3Filter(img2, true);

                img = ImgProcessor.PrewittFilter(img, false);
                img2 = ImgProcessor.PrewittFilter(img2, false);

                pictureBox1.Image = img;
                pictureBox2.Image = img2;

                // Create Integral Image
                IntegralImage iimg = IntegralImage.FromImageGrey(img);
                IntegralImage iimg2 = IntegralImage.FromImageGrey(img2);

                // Extract the interest points
                ipts1 = FastHessian.getIpoints(0.0002f, 5, 2, iimg);
                ipts2 = FastHessian.getIpoints(0.0002f, 5, 2, iimg2);

                // Describe the interest points
                SurfDescriptor.DecribeInterestPoints(ipts1, false, false, iimg);
                SurfDescriptor.DecribeInterestPoints(ipts2, false, false, iimg2);

                List<IPoint>[] matches = SurfMatch.getMatches(ipts1, ipts2);

                Console.WriteLine("Matches[0] size" + matches[0].Count);
                Console.WriteLine("Matches[1] size" + matches[1].Count);

                // Draw points on the image
                PaintSURF(img, ipts1);
                PaintSURF(img2, ipts2);

            }
            catch
            {

            }

            // Processing stops here
            watch.Stop();
            this.Text = "DemoSURF - Elapsed time: " + watch.Elapsed +
                        " for " + ipts1.Count + "points and " + ipts2.Count + "points in img2";
        }

        private void PaintSURF(Bitmap img, List<IPoint> ipts)
        {
            Graphics g = Graphics.FromImage(img);

            Pen redPen = new Pen(Color.Red);
            Pen bluePen = new Pen(Color.Blue);
            Pen myPen;

            foreach (IPoint ip in ipts)
            {
                int S = 2 * Convert.ToInt32(2.5f * ip.scale);
                int R = Convert.ToInt32(S / 2f);

                Point pt = new Point(Convert.ToInt32(ip.x), Convert.ToInt32(ip.y));
                Point ptR = new Point(Convert.ToInt32(R * Math.Cos(ip.orientation)), Convert.ToInt32(R * Math.Sin(ip.orientation)));

                myPen = (ip.laplacian > 0 ? bluePen : redPen);

                g.DrawEllipse(myPen, pt.X - R, pt.Y - R, S, S);
                g.DrawLine(new Pen(Color.FromArgb(0, 255, 0)), new Point(pt.X, pt.Y), new Point(pt.X + ptR.X, pt.Y + ptR.Y));
            }
        }
    }
}
