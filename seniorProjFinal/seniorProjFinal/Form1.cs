using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace seniorProjFinal
{
    public partial class Form1 : Form
    {

        List<IPoint> ipts1 = new List<IPoint>();
        List<IPoint> iptsGet = new List<IPoint>();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            currency.Text = "None";

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            string pathToFile = openFileDialog.FileName;

            // Processing starts here
            Stopwatch watch = new Stopwatch();
            watch.Start();

            try
            {
                // Load an Image
                Bitmap img = new Bitmap(pathToFile);

                pictureBox1.Image = img;

                // trying edge detection here
                img = ImgProcessor.Sobel3x3Filter(img, true);

                // Create Integral Image
                IntegralImage iimg = IntegralImage.FromImageGrey(img);

                // 50% matching on bills and 10-15% on similar coins 0-10% on different coins
                ipts1 = FastHessian.getIpoints(0.001f, 5, 2, iimg);

                // Describe the interest points
                SurfDescriptor.DecribeInterestPoints(ipts1, false, false, iimg);

                // loop through our datasets, which is represented by each file
                float best = 0;
                float avg = 0;
                string currencyDir = "none";
                //string[] files = Directory.GetFiles(@"J:\Dropbox\My Projects\New folder\SeniorProject\currencyData", "*.jpg");
                string[] files = Directory.GetFiles(@"C:\Users\Jeremy\Dropbox\My Projects\New folder\SeniorProject\currencyData", "*.txt");
                
                for (int i = 0; i < files.Length; i++)
                {
                    List<IPoint> ipts2 = new List<IPoint>();

                    // attempting to get IPoints from a txt file instead of a picture

                    string[] lines = System.IO.File.ReadAllLines(files[i]);

                    foreach (string str in lines)
                    {
                        ipts2.Add(new IPoint(str));
                    }



                    //Console.WriteLine("creating a new image from file " + files[i]);
                    // create a new image
                    //Bitmap newImg = new Bitmap(files[i]);

                    //Console.WriteLine("doing the edge detection");
                    // edge detection
                    //newImg = ImgProcessor.Sobel3x3Filter(newImg, true);

                   // Console.WriteLine("making the integral image");
                    // new integral image
                    //IntegralImage iimg2 = IntegralImage.FromImageGrey(newImg);

                  //  Console.WriteLine("gathering interest points");
                    // our list of interest points from our data images
                    //ipts2 = FastHessian.getIpoints(0.001f, 5, 2, iimg2);
                    //SurfDescriptor.DecribeInterestPoints(ipts2, false, false, iimg2);

                  //  Console.WriteLine("getting the number of matches");
                    // get the matches from our current comparison
                    List<IPoint>[] matches = SurfMatch.getMatches(ipts1, ipts2);

                  //  Console.WriteLine("computing the average");
                    // compute the avg
                 //   Console.WriteLine(matches[0].Count() + " matches, list 1 count = " + ipts1.Count()
                 //                     + " list 2 count = " + ipts2.Count());
                    Console.WriteLine(files[i]);
                    avg = ((float)matches[1].Count() / (float)ipts1.Count() + (float)matches[0].Count() / (float)ipts2.Count()) / 2 * 100;
                    Console.WriteLine("MATCHES 1 = " + matches[1].Count());
                    Console.WriteLine("IPTS 1 = " + ipts1.Count());
                    Console.WriteLine("MATCHES 2 = " + matches[0].Count());
                    Console.WriteLine("IPTS 2 = " + ipts2.Count());
                    Console.WriteLine("AVERAGE = " + avg);                 
                    
                    // add results to a new list if matches % is better change currency to better match
                    if (avg > best && avg > 10.0)
                    {
                  //      Console.WriteLine("making the assignments because we found something better");
                        best = avg;
                        currencyDir = files[i];
                    }

                    // clear our list for the next iteration
                    //Array.Clear(matches, 0, matches.Length);

                    for (int j = 0; j < matches.Count(); j++)
                    {
                        matches[j].Clear();
                    }
                    ipts2.Clear();
                }

                // function used to identify the selected picture
                setText(currencyDir);

                //Console.WriteLine(currencyDir);
            }
            catch
            {
                // empty on purpose
            }

            // Processing stops here
            watch.Stop();
            this.Text = "DemoSURF - Elapsed time: " + watch.Elapsed;
        }

        private void setText(string currencyDir)
        {
            char[] delimiterChars = { '\\' };
            string text = currencyDir;
            // split the string on the delimiter
            string[] words = text.Split(delimiterChars);
            // only use the last string in the array
            currency.Text = words.Last();
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

        private void btn2_Click(object sender, EventArgs e)
        {
            // open the picture
            OpenFileDialog pic = new OpenFileDialog();
            pic.ShowDialog();
            string pathToFile = pic.FileName;

            // select the txt file
            OpenFileDialog text = new OpenFileDialog();
            text.ShowDialog();
            string textFile = text.FileName;

            if (textFile == "" || pathToFile == "")
            {
                // if no files finish here. 
                Console.WriteLine("Files Not Valid");
                return;
            }
            else
            {
                Console.WriteLine(textFile);

                string lineToFile = "";

                try
                {
                    Bitmap img = new Bitmap(pathToFile);

                    // edge detection here
                    img = ImgProcessor.Sobel3x3Filter(img, true);

                    // Create Integral Image
                    IntegralImage iimg = IntegralImage.FromImageGrey(img);

                    // 50% matching on bills and 10-15% on similar coins 0-10% on different coins
                    iptsGet = FastHessian.getIpoints(0.001f, 5, 2, iimg);

                    // Describe the interest points
                    SurfDescriptor.DecribeInterestPoints(iptsGet, false, false, iimg);

                    // trying to write the IPoints to a file

                    // the format is 
                    // IPointNumber,x,y,scale,response,orientation,laplacian,descriptorLength,descriptor1,..descriptor64

                    int num = 1;

                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(textFile))
                    {

                        foreach (IPoint ip in iptsGet)
                        {
                            lineToFile = num.ToString() + "," +
                                         ip.x.ToString() + "," + ip.y.ToString() + "," +
                                         ip.scale.ToString() + "," + ip.response.ToString() + "," +
                                         ip.orientation.ToString() + "," + ip.laplacian.ToString() + "," +
                                         ip.descriptorLength.ToString() + ",";
                            string descriptors = "";

                            for (int i = 0; i < ip.descriptorLength; i++)
                            {
                                if (i < 63)
                                    descriptors = descriptors + ip.descriptor[i].ToString() + ",";
                                else
                                    descriptors = descriptors + ip.descriptor[i].ToString();
                            }

                            lineToFile = lineToFile + descriptors;

                            file.WriteLine(lineToFile);
                            num++;
                        }

                        Console.WriteLine("Finished");
                    }
                }
                catch (Exception)
                {
                    // left blank on purpose
                }
            }
        }
    }
}
