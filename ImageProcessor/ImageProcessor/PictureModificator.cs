using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Reflection;
 
using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;
using System.Runtime.InteropServices;


namespace ImageProcessor
{
    class PictureModificator
    {
        private Bitmap currentImage;
 
        public PictureModificator(Bitmap currentImage)
        {
            this.currentImage = currentImage;
        }
 
        public PictureModificator()
        {
            this.currentImage = null;
        }


        //
        // added this for the filtering
        //
        public Bitmap ConvolutionFilter(double[,] filterMatrix,
                                                  double factor = 1,
                                                       int bias = 0,
                                             bool grayscale = false)
        {
            BitmapData sourceData = currentImage.LockBits(new Rectangle(0, 0,
                                     currentImage.Width, currentImage.Height),
                                                       ImageLockMode.ReadOnly,
                                                 PixelFormat.Format32bppArgb);

            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];
            byte[] resultBuffer = new byte[sourceData.Stride * sourceData.Height];

            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
            currentImage.UnlockBits(sourceData);

            if (grayscale == true)
            {
                float rgb = 0;

                for (int k = 0; k < pixelBuffer.Length; k += 4)
                {
                    rgb = pixelBuffer[k] * 0.11f;
                    rgb += pixelBuffer[k + 1] * 0.59f;
                    rgb += pixelBuffer[k + 2] * 0.3f;


                    pixelBuffer[k] = (byte)rgb;
                    pixelBuffer[k + 1] = pixelBuffer[k];
                    pixelBuffer[k + 2] = pixelBuffer[k];
                    pixelBuffer[k + 3] = 255;
                }
            }

            double blue = 0.0;
            double green = 0.0;
            double red = 0.0;

            int filterWidth = filterMatrix.GetLength(1);
            int filterHeight = filterMatrix.GetLength(0);

            int filterOffset = (filterWidth - 1) / 2;
            int calcOffset = 0;

            int byteOffset = 0;

            for (int offsetY = filterOffset; offsetY <
                currentImage.Height - filterOffset; offsetY++)
            {
                for (int offsetX = filterOffset; offsetX <
                    currentImage.Width - filterOffset; offsetX++)
                {
                    blue = 0;
                    green = 0;
                    red = 0;

                    byteOffset = offsetY *
                                 sourceData.Stride +
                                 offsetX * 4;

                    for (int filterY = -filterOffset;
                        filterY <= filterOffset; filterY++)
                    {
                        for (int filterX = -filterOffset;
                            filterX <= filterOffset; filterX++)
                        {

                            calcOffset = byteOffset +
                                         (filterX * 4) +
                                         (filterY * sourceData.Stride);

                            blue += (double)(pixelBuffer[calcOffset]) *
                                    filterMatrix[filterY + filterOffset,
                                                        filterX + filterOffset];

                            green += (double)(pixelBuffer[calcOffset + 1]) *
                                     filterMatrix[filterY + filterOffset,
                                                        filterX + filterOffset];

                            red += (double)(pixelBuffer[calcOffset + 2]) *
                                   filterMatrix[filterY + filterOffset,
                                                      filterX + filterOffset];
                        }
                    }

                    blue = factor * blue + bias;
                    green = factor * green + bias;
                    red = factor * red + bias;

                    if (blue > 255)
                    { blue = 255; }
                    else if (blue < 0)
                    { blue = 0; }

                    if (green > 255)
                    { green = 255; }
                    else if (green < 0)
                    { green = 0; }

                    if (red > 255)
                    { red = 255; }
                    else if (red < 0)
                    { red = 0; }

                    resultBuffer[byteOffset] = (byte)(blue);
                    resultBuffer[byteOffset + 1] = (byte)(green);
                    resultBuffer[byteOffset + 2] = (byte)(red);
                    resultBuffer[byteOffset + 3] = 255;
                }
            }

            Bitmap resultBitmap = new Bitmap(currentImage.Width, currentImage.Height);

            BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0,
                                     resultBitmap.Width, resultBitmap.Height),
                                                      ImageLockMode.WriteOnly,
                                                 PixelFormat.Format32bppArgb);

            Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length);
            resultBitmap.UnlockBits(resultData);

            return resultBitmap;
        }

        public Bitmap ConvolutionFilter(double[,] xFilterMatrix,
                                                double[,] yFilterMatrix,
                                                      double factor = 1,
                                                           int bias = 0,
                                                 bool grayscale = false)
        {
            BitmapData sourceData = currentImage.LockBits(new Rectangle(0, 0,
                                     currentImage.Width, currentImage.Height),
                                                       ImageLockMode.ReadOnly,
                                                  PixelFormat.Format32bppArgb);

            byte[] pixelBuffer = new byte[sourceData.Stride * sourceData.Height];
            byte[] resultBuffer = new byte[sourceData.Stride * sourceData.Height];

            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);
            currentImage.UnlockBits(sourceData);

            if (grayscale == true)
            {
                float rgb = 0;

                for (int k = 0; k < pixelBuffer.Length; k += 4)
                {
                    rgb = pixelBuffer[k] * 0.11f;
                    rgb += pixelBuffer[k + 1] * 0.59f;
                    rgb += pixelBuffer[k + 2] * 0.3f;

                    pixelBuffer[k] = (byte)rgb;
                    pixelBuffer[k + 1] = pixelBuffer[k];
                    pixelBuffer[k + 2] = pixelBuffer[k];
                    pixelBuffer[k + 3] = 255;
                }
            }

            double blueX = 0.0;
            double greenX = 0.0;
            double redX = 0.0;

            double blueY = 0.0;
            double greenY = 0.0;
            double redY = 0.0;

            double blueTotal = 0.0;
            double greenTotal = 0.0;
            double redTotal = 0.0;

            int filterOffset = 1;
            int calcOffset = 0;

            int byteOffset = 0;

            for (int offsetY = filterOffset; offsetY <
                currentImage.Height - filterOffset; offsetY++)
            {
                for (int offsetX = filterOffset; offsetX <
                    currentImage.Width - filterOffset; offsetX++)
                {
                    blueX = greenX = redX = 0;
                    blueY = greenY = redY = 0;

                    blueTotal = greenTotal = redTotal = 0.0;

                    byteOffset = offsetY *
                                 sourceData.Stride +
                                 offsetX * 4;

                    for (int filterY = -filterOffset;
                        filterY <= filterOffset; filterY++)
                    {
                        for (int filterX = -filterOffset;
                            filterX <= filterOffset; filterX++)
                        {
                            calcOffset = byteOffset +
                                         (filterX * 4) +
                                         (filterY * sourceData.Stride);

                            blueX += (double)(pixelBuffer[calcOffset]) *
                                      xFilterMatrix[filterY + filterOffset,
                                              filterX + filterOffset];

                            greenX += (double)(pixelBuffer[calcOffset + 1]) *
                                      xFilterMatrix[filterY + filterOffset,
                                              filterX + filterOffset];

                            redX += (double)(pixelBuffer[calcOffset + 2]) *
                                      xFilterMatrix[filterY + filterOffset,
                                              filterX + filterOffset];

                            blueY += (double)(pixelBuffer[calcOffset]) *
                                      yFilterMatrix[filterY + filterOffset,
                                              filterX + filterOffset];

                            greenY += (double)(pixelBuffer[calcOffset + 1]) *
                                      yFilterMatrix[filterY + filterOffset,
                                              filterX + filterOffset];

                            redY += (double)(pixelBuffer[calcOffset + 2]) *
                                      yFilterMatrix[filterY + filterOffset,
                                              filterX + filterOffset];
                        }
                    }

                    blueTotal = Math.Sqrt((blueX * blueX) + (blueY * blueY));
                    greenTotal = Math.Sqrt((greenX * greenX) + (greenY * greenY));
                    redTotal = Math.Sqrt((redX * redX) + (redY * redY));

                    if (blueTotal > 255)
                    { blueTotal = 255; }
                    else if (blueTotal < 0)
                    { blueTotal = 0; }

                    if (greenTotal > 255)
                    { greenTotal = 255; }
                    else if (greenTotal < 0)
                    { greenTotal = 0; }

                    if (redTotal > 255)
                    { redTotal = 255; }
                    else if (redTotal < 0)
                    { redTotal = 0; }

                    resultBuffer[byteOffset] = (byte)(blueTotal);
                    resultBuffer[byteOffset + 1] = (byte)(greenTotal);
                    resultBuffer[byteOffset + 2] = (byte)(redTotal);
                    resultBuffer[byteOffset + 3] = 255;
                }
            }

            Bitmap resultBitmap = new Bitmap(currentImage.Width, currentImage.Height);

            BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0,
                                     resultBitmap.Width, resultBitmap.Height),
                                                      ImageLockMode.WriteOnly,
                                                  PixelFormat.Format32bppArgb);

            Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length);
            resultBitmap.UnlockBits(resultData);

            return resultBitmap;
        }

        public void Laplacian3x3Filter(bool grayscale = true)
        {
            currentImage = ConvolutionFilter(Matrix.Laplacian3x3, 1.0, 0, grayscale);
        }

        public void Laplacian5x5Filter(bool grayscale = true)
        {
            currentImage = ConvolutionFilter(Matrix.Laplacian5x5, 1.0, 0, grayscale);
        }

        public void LaplacianOfGaussianFilter()
        {
            currentImage = ConvolutionFilter(Matrix.LaplacianOfGaussian, 1.0, 0, true);
        }

        public void Laplacian3x3OfGaussian3x3Filter()
        {
            currentImage = ConvolutionFilter(Matrix.Gaussian3x3, 1.0 / 16.0, 0, true);

            currentImage = ConvolutionFilter(Matrix.Laplacian3x3, 1.0, 0, false);
        }

        public void Laplacian3x3OfGaussian5x5Filter1()
        {
            currentImage = ConvolutionFilter(Matrix.Gaussian5x5Type1, 1.0 / 159.0, 0, true);

            currentImage = ConvolutionFilter(Matrix.Laplacian3x3, 1.0, 0, false);
        }

        public void Laplacian3x3OfGaussian5x5Filter2()
        {
            currentImage = ConvolutionFilter(Matrix.Gaussian5x5Type2, 1.0 / 256.0, 0, true);

            currentImage = ConvolutionFilter( Matrix.Laplacian3x3, 1.0, 0, false);
        }

        public void Laplacian5x5OfGaussian3x3Filter()
        {
            currentImage = ConvolutionFilter(Matrix.Gaussian3x3, 1.0 / 16.0, 0, true);

            currentImage = ConvolutionFilter(Matrix.Laplacian5x5, 1.0, 0, false);
        }

        public void Laplacian5x5OfGaussian5x5Filter1()
        {
            currentImage = ConvolutionFilter(Matrix.Gaussian5x5Type1, 1.0 / 159.0, 0, true);

            currentImage = ConvolutionFilter(Matrix.Laplacian5x5, 1.0, 0, false);
        }

        public void Laplacian5x5OfGaussian5x5Filter2()
        {
            currentImage = ConvolutionFilter(Matrix.Gaussian5x5Type2, 1.0 / 256.0, 0, true);

            currentImage = ConvolutionFilter(Matrix.Laplacian5x5, 1.0, 0, false);
        }

        public void Sobel3x3Filter(bool grayscale = true)
        {
            //currentImage = ConvolutionFilter(Matrix.Gaussian3x3, 1.0 / 16.0, 0, true);
            currentImage = ConvolutionFilter(Matrix.Sobel3x3Horizontal, Matrix.Sobel3x3Vertical, 1.0, 0, grayscale);

        }

        public void PrewittFilter(bool grayscale = true)
        {
            currentImage = ConvolutionFilter(Matrix.Prewitt3x3Horizontal, Matrix.Prewitt3x3Vertical, 1.0, 0, grayscale);
        }

        public void KirschFilter(bool grayscale = true)
        {
            //currentImage = ConvolutionFilter(Matrix.Gaussian3x3, 1.0 / 16.0, 0, true);

            currentImage = ConvolutionFilter(Matrix.Kirsch3x3Horizontal, Matrix.Kirsch3x3Vertical, 1.0, 0, grayscale);  
        }



        
        //
        // this is just too easy...
        //
        public bool applySobelEdgeFilter()
        {
            if (currentImage != null)
            {
                try
                {
                    // create filter
                    SobelEdgeDetector filter = new SobelEdgeDetector();
                    // apply the filter
                    filter.ApplyInPlace(currentImage);
                    return true;
                }
                catch (Exception e)
                {
 
                }
            }
            return false;
        }
 
        public bool applyGrayscale()
        {
            if (currentImage != null)
            {
                try
                {
                    // create grayscale filter (BT709)
                    Grayscale filter = new Grayscale(0.2125, 0.7154, 0.0721);
                    // apply the filter
                    currentImage = filter.Apply(currentImage);
                    return true;
                }
                catch (Exception e)
                { }
            }
            return false;
        }
 
        public bool markKnownForms()
        {
            if (currentImage != null)
            {
                try
                {
                    Bitmap image = new Bitmap(this.currentImage);
                    // lock image
                    BitmapData bmData = image.LockBits(
                        new Rectangle(0, 0, image.Width, image.Height),
                        ImageLockMode.ReadWrite, image.PixelFormat);
 
                    // turn background to black
                    ColorFiltering cFilter = new ColorFiltering();
                    cFilter.Red = new IntRange(0, 64);
                    cFilter.Green = new IntRange(0, 64);
                    cFilter.Blue = new IntRange(0, 64);
                    cFilter.FillOutsideRange = false;
                    cFilter.ApplyInPlace(bmData);
                    
 
                    // locate objects
                    BlobCounter bCounter = new BlobCounter();
 
                    bCounter.FilterBlobs = true;
                    bCounter.MinHeight = 20;
                    bCounter.MinWidth = 20;
 
                    bCounter.ProcessImage(bmData);
                    Blob[] baBlobs = bCounter.GetObjectsInformation();
                    image.UnlockBits(bmData);
 
                    // coloring objects
                    SimpleShapeChecker shapeChecker = new SimpleShapeChecker();
 
                    Graphics g = Graphics.FromImage(image);
                    Pen yellowPen = new Pen(Color.Yellow, 2); // circles
                    Pen redPen = new Pen(Color.Red, 2);       // quadrilateral
                    Pen brownPen = new Pen(Color.Brown, 2);   // quadrilateral with known sub-type
                    Pen greenPen = new Pen(Color.Green, 2);   // known triangle
                    Pen bluePen = new Pen(Color.Blue, 2);     // triangle
 
                    for (int i = 0, n = baBlobs.Length; i < n; i++)
                    {
                        List<IntPoint> edgePoints = bCounter.GetBlobsEdgePoints(baBlobs[i]);
 
                        AForge.Point center;
                        float radius;
 
                        // is circle ?
                        if (shapeChecker.IsCircle(edgePoints, out center, out radius))
                        {
                            g.DrawEllipse(yellowPen,
                                (float)(center.X - radius), (float)(center.Y - radius),
                                (float)(radius * 2), (float)(radius * 2));
                        }
                        else
                        {
                            List<IntPoint> corners;
 
                            // is triangle or quadrilateral
                            if (shapeChecker.IsConvexPolygon(edgePoints, out corners))
                            {
                                PolygonSubType subType = shapeChecker.CheckPolygonSubType(corners);
                                Pen pen;
                                if (subType == PolygonSubType.Unknown)
                                {
                                    pen = (corners.Count == 4) ? redPen : bluePen;
                                }
                                else
                                {
                                    pen = (corners.Count == 4) ? brownPen : greenPen;
                                }
 
                                g.DrawPolygon(pen, ToPointsArray(corners));
                            }
                        }
                    }
                    yellowPen.Dispose();
                    redPen.Dispose();
                    greenPen.Dispose();
                    bluePen.Dispose();
                    brownPen.Dispose();
                    g.Dispose();
                    this.currentImage = image;
                    return true;
                }
                catch (Exception e)
                {
                    
                }
            }
            return false;
        }

        public bool findBlobs()
        {
            if (currentImage != null)
            {
                try
                {
                    Bitmap image = new Bitmap(this.currentImage);
                    // lock image
                    BitmapData bmData = image.LockBits(
                        new Rectangle(0, 0, image.Width, image.Height),
                        ImageLockMode.ReadWrite, image.PixelFormat);

                    // turn background to black
                    ColorFiltering cFilter = new ColorFiltering();
                    cFilter.Red = new IntRange(0, 64);
                    cFilter.Green = new IntRange(0, 64);
                    cFilter.Blue = new IntRange(0, 64);
                    cFilter.FillOutsideRange = false;
                    cFilter.ApplyInPlace(bmData);


                    // locate objects
                    BlobCounter bCounter = new BlobCounter();

                    bCounter.FilterBlobs = true;
                    bCounter.MinHeight = 20;
                    bCounter.MinWidth = 20;

                    bCounter.ProcessImage(bmData);
                   
                    //unlock the image before doing anything with it.
                    image.UnlockBits(bmData);
                    Blob[] baBlobs = bCounter.GetObjects(image, true);

                    // blobs is an array of the object gathered


                    foreach (Blob b in baBlobs)
                    {
                       
                        // some test code to shwo the new images
                        using (Form form = new Form())
                        {
                            // this line is used to convert the blob to a new bitmap img
                            Bitmap img = b.Image.ToManagedImage();

                            form.StartPosition = FormStartPosition.CenterScreen;
                            form.Size = img.Size;

                            PictureBox pb = new PictureBox();
                            pb.Dock = DockStyle.Fill;
                            pb.Image = img;

                            form.Controls.Add(pb);
                            form.ShowDialog();
                        }

                    }

                    return true;
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.Write("There was an excption.");
                }
            }

            //        // locate objects
            //        BlobCounter bCounter = new BlobCounter();

            //        bCounter.FilterBlobs = true;
            //        bCounter.MinHeight = 30;
            //        bCounter.MinWidth = 30;

            //        bCounter.ProcessImage(bmData);
            //        Blob[] baBlobs = bCounter.GetObjectsInformation();
            //        image.UnlockBits(bmData);

            //        // coloring objects
            //        SimpleShapeChecker shapeChecker = new SimpleShapeChecker();

            //        Graphics g = Graphics.FromImage(image);
            //        Pen yellowPen = new Pen(Color.Yellow, 2); // circles
            //        Pen redPen = new Pen(Color.Red, 2);       // quadrilateral
            //        Pen brownPen = new Pen(Color.Brown, 2);   // quadrilateral with known sub-type
            //        Pen greenPen = new Pen(Color.Green, 2);   // known triangle
            //        Pen bluePen = new Pen(Color.Blue, 2);     // triangle

            //        for (int i = 0, n = baBlobs.Length; i < n; i++)
            //        {
            //            List<IntPoint> edgePoints = bCounter.GetBlobsEdgePoints(baBlobs[i]);

            //            AForge.Point center;
            //            float radius;

            //            // is circle ?
            //            if (shapeChecker.IsCircle(edgePoints, out center, out radius))
            //            {
            //                g.DrawEllipse(yellowPen,
            //                    (float)(center.X - radius), (float)(center.Y - radius),
            //                    (float)(radius * 2), (float)(radius * 2));
            //            }
            //            else
            //            {
            //                List<IntPoint> corners;

            //                // is triangle or quadrilateral
            //                if (shapeChecker.IsConvexPolygon(edgePoints, out corners))
            //                {
            //                    PolygonSubType subType = shapeChecker.CheckPolygonSubType(corners);
            //                    Pen pen;
            //                    if (subType == PolygonSubType.Unknown)
            //                    {
            //                        pen = (corners.Count == 4) ? redPen : bluePen;
            //                    }
            //                    else
            //                    {
            //                        pen = (corners.Count == 4) ? brownPen : greenPen;
            //                    }

            //                    g.DrawPolygon(pen, ToPointsArray(corners));
            //                }
            //            }
            //        }
            //        yellowPen.Dispose();
            //        redPen.Dispose();
            //        greenPen.Dispose();
            //        bluePen.Dispose();
            //        brownPen.Dispose();
            //        g.Dispose();
            //        this.currentImage = image;
            //        return true;
            //    }
            //    catch (Exception e)
            //    {

            //    }
            //}
            return false;
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
 
        public void setCurrentImage(Bitmap currentImage)
        {
            this.currentImage = currentImage;
        }
 
        public Bitmap getCurrentImage()
        {
            return currentImage;
        }

    }
}