using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using OpenSURFcs;

namespace OpenSURFDemo
{
    public partial class DemoSURF : Form
    {
        public DemoSURF()
        {
            InitializeComponent();
        }

        List<IPoint> ipts = new List<IPoint>();
        List<IPoint> ipts2 = new List<IPoint>();

        private void btnRunSurf_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            string pathToFile = openFileDialog.FileName;

            OpenFileDialog open = new OpenFileDialog();
            open.ShowDialog();
            string path = open.FileName;


            Stopwatch watch = new Stopwatch();
            watch.Start();

            try
            {
                // Load an Image
                Bitmap img = new Bitmap(pathToFile);
                Bitmap img2 = new Bitmap(path);
                pbMainPicture.Image = img;
                pbMain2.Image = img2;
        
                // Create Integral Image
                IntegralImage iimg = IntegralImage.FromImage(img);
                IntegralImage iimg2 = IntegralImage.FromImage(img2);
        
                // Extract the interest points
                ipts = FastHessian.getIpoints(0.0002f, 5, 2, iimg);
                ipts2 = FastHessian.getIpoints(0.0002f, 5, 2, iimg2);

                // Describe the interest points
                SurfDescriptor.DecribeInterestPoints(ipts, false, false, iimg);
                SurfDescriptor.DecribeInterestPoints(ipts2, false, false, iimg2);

                //Do the comparison

                List<IPoint>[] matches = SurfMatch.getMatches(ipts, ipts2);

                Console.WriteLine("Matches[0] size" + matches[0].Count);
                Console.WriteLine("Matches[1] size" + matches[1].Count);

                // Draw points on the image
                PaintSURF(img, ipts);
                PaintSURF(img2, ipts2);

            }
            catch
            {

            }

            watch.Stop();
            this.Text = "DemoSURF - Elapsed time: " + watch.Elapsed + 
                        " for " + ipts.Count + "points and " + ipts2.Count + "points in img2" ;
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

        // compare SURF Descriptors
        private double compareSURFDescriptors(float[] d1, float[] d2, double best, int length)
        {
            double total_cost = 0;
            //assert( length % 4 == 0 );
            for (int i = 0; i < length; i += 4)
            {
                double t0 = d1[i] - d2[i];
                double t1 = d1[i + 1] - d2[i + 1];
                double t2 = d1[i + 2] - d2[i + 2];
                double t3 = d1[i + 3] - d2[i + 3];
                total_cost += t0 * t0 + t1 * t1 + t2 * t2 + t3 * t3;
                if (total_cost > best)
                    break;
            }

            //Console.WriteLine("compare des return = " + total_cost);

            return total_cost;
        }

        // naive Nearest Neighbor
        private int nNN(float[] vec, int length, int laplacian, List<IPoint> ipts)
        {
            int i, neighbor = -1;
            double d, dist1 = 1e6, dist2 = 1e6;

            foreach (IPoint ip in ipts)
            {
                for (i = 0; i < ip.descriptorLength; ++i)
                {
                    if (laplacian != ip.laplacian)
                        continue;
                    d = compareSURFDescriptors(vec, ip.descriptor, dist2, length);
                    if (d < dist1)
                    {
                        dist2 = dist1;
                        dist1 = d;
                        neighbor = i;
                    }
                    else if (d < dist2)
                    {
                        dist2 = d;
                    }
                }
            }

            //Console.WriteLine("dist1 = " + dist1);
            //Console.WriteLine("dist2 = " + dist2);
            //Console.WriteLine("Nei = " + neighbor);

            if (dist1 < 0.5*dist2)
            {
                return neighbor;
            }
            
            return -1;
        }

        // find key pairs of points
        void findPairs(List<IPoint> ipts, List<IPoint> ipts2, List<int> pairs)
        {
            int i;
            foreach (IPoint ip in ipts)
            {
                for (i = 0; i < ip.descriptorLength; ++i)
                {
                    // Nearest Neighbor
                    int nn = nNN(ip.descriptor, ip.descriptorLength, ip.laplacian, ipts2);
                    if (nn >= 0)
                    {
                        pairs.Add(i);
                        pairs.Add(nn);
                    }
                }
            }

        }


        public double subtract(IPoint i1, IPoint i2)
        {
            double sum = 0.0;
            for (int i = 0; i < i1.descriptorLength; ++i)
            {
                sum += (i1.descriptor[i] - i2.descriptor[i]) * (i1.descriptor[i] - i2.descriptor[i]);
            }

            return Math.Sqrt(sum);
        }

        void getMatches(List<IPoint> ipts, List<IPoint> ipts2, List<IPoint> matches)
        {
            double dist, d1, d2;
            IPoint match = null;
            matches.Clear();

            d1 = d2 = Double.MaxValue;

            for (int i = 0; i < ipts.Count(); i++)
            {
                d1 = d2 = Double.MaxValue;

                for (int j = 0; j < ipts2.Count(); j++)
                {
                    dist = subtract(ipts.ElementAt(i), ipts2.ElementAt(j));

                    if (dist < d1)
                    {
                        d2 = d1;
                        d1 = dist;
                        match = ipts2.ElementAt(j);
                    }
                    else if (dist < d2)
                    {
                        d2 = dist;
                    }
                }

                //Console.WriteLine("d1 = " + d1);
                //Console.WriteLine("d2 = " + d2);
                //Console.WriteLine("/ = " + d1 / d2);

                if (d1 / d2 < 0.72)
                {
                    //Console.WriteLine("Match Found");
                    matches.Add(match);
                }
            }
        }

    }  // DemoApp
} // OpenSURFDemo
