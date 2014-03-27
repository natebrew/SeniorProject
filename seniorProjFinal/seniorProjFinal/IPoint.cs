using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace seniorProjFinal
{
    public class IPoint
    {
        // Default ctor
        public IPoint()
        {
            orientation = 0;
        }

        public IPoint(string line)
        {
            int i = 0;
            char[] delimiterChars = { ',' };
            string text = line;
            // split the string on the delimiter
            string[] words = text.Split(delimiterChars);
            
            // the format of the string is 
            //IPointNumber,x,y,scale,response,orientation,laplacian,descriptorLength,descriptor1,..descriptor64

            //Console.WriteLine(words[i]);                            // 0 
            i++;
            this.x = Convert.ToSingle(words[i]);                    // 1
            i++;
            this.y = Convert.ToSingle(words[i]);                    // 2
            i++;
            this.scale = Convert.ToSingle(words[i]);                // 3
            i++;
            this.response = Convert.ToSingle(words[i]);              // 4
            i++;
            this.orientation = Convert.ToSingle(words[i]);          // 5
            i++;
            this.laplacian = Convert.ToInt32(words[i]);             // 6
            i++;
            this.descriptorLength = Convert.ToInt32(words[i]);      // 7
            i++;

            SetDescriptorLength(this.descriptorLength);

            for (int j = 0; j < this.descriptorLength; j++, i++)
            {
                descriptor[j] = Convert.ToSingle(words[i]);         // 8 - 72
            }
        }

        // Coordinates of the detected interest point
        public float x, y;

        // Detected scale
        public float scale;

        // Response of the detected feature (strength)
        public float response;

        // Orientation measured anti-clockwise from +ve x-axis
        public float orientation;

        // Sign of laplacian for fast matching purposes
        public int laplacian;

        // Descriptor array length
        public int descriptorLength;

        // Descriptor array (of floats)
        public float [] descriptor = null;

        // Set the Descriptor Length
        public void SetDescriptorLength(int Size)
        {
            descriptorLength = Size;
            descriptor = new float[Size];
        }
    }
}
