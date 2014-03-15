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

        for (int i = 0; i < pImg.Width; i++)
        {
            Color c = pImg.GetPixel(i, 0);
            rowSum += (cR * c.R + cG * c.G + cB * c.B) / 255f;
            img[0, i] = rowSum;
        }

        for (int i = 1; i < pImg.Height; i++)
        {
            rowSum = 0;
            for (int j = 0; j < pImg.Width; j++)
            {
                Color c = pImg.GetPixel(i, j);
                rowSum += (cR * c.R + cG * c.G + cB * c.B) / 255f;

                // sum up the row and the value
                img[i, j] = rowSum + img[i - 1, j];
            }
        }

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
