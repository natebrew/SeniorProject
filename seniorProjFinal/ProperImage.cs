using System;


/********************************************************************
 * This is an integral image which is also known as a Summed Table Area.
 * It is used to sum the values of the image and generate a grayscale 
 * version of our image. The summed table area is the sum of all pixels
 * above the current pixel and to the left of the current pixel inclusive.
 * *****************************************************************/
public class ProperImage
{
    // member variables
    // RGB values that we will be using
    const float cR = .2989f;
    const float cG = .5870f;
    const float cB = .1140f;

    internal float[,] Matrix;
    public int Width, Height;


    // member functions
	public ProperImage()
	{
	}

    // private constructor 
    private ProperImage(int width, int height)
    {
        this.Width = width;
        this.Height = height;

        this.Matrix = new float[height, width];
    }

    // constructor to create a proper image from a bitmap
    public static ProperImage FromImage(Bitmap pImg)
    {
        // we are going to make a greyscale image
        ProperImage img = new ProperImage(pImg.width, pImg.height);

        float rowSum = 0;

        BitmapData dataIn = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
        unsafe
        {
            byte* pIn = (byte*)(dataIn.Scan0.ToPointer());
            for (int y = 0; y < dataIn.Height; y++)
            {
                rowsum = 0;
                for (int x = 0; x < dataIn.Width; x++)
                {
                    int cb = (byte)(pIn[0]);
                    int cg = (byte)(pIn[1]);
                    int cr = (byte)(pIn[2]);

                    //
                    rowsum += (cR * cr + cG * cg + cB * cb) / 255f;
                    // integral image is rowsum + value above     
                    if (y == 0)
                        img[0, x] = rowsum;
                    else
                        img[y, x] = rowsum + img[y - 1, x];

                    pIn += 3;
                }
                pIn += dataIn.Stride - dataIn.Width * 3;
            }
        }
        image.UnlockBits(dataIn);

        // return our new ProperImage
        return img;
    }

    // this is used for the Haar algorithm
    public float BoxInt(int row, int col, int rows, int cols)
    {
 
    }

    // getter and setter for our matrix height and width
    public float this[int y, int x]
    {
        get { return Matrix[y, x]; }
        set { Matrix[y, x] = value; }
    }
}
