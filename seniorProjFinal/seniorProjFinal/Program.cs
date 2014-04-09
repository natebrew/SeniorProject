/******************************************************************
 * Program: Piggy Bank application
 * Written By: Nathan Brewster and Jeremy McCurdy
 * 
 * This program takes a picture of a single piece of currency and 
 * creates the interest points from an integral image. It will then
 * look for a directory of interest point text files to compare against.
 * This will identify what type of currency you are taking a picture of.
 * It will work quite reliably with bills but can use a lot of help 
 * with its identification of coins. It uses the Haar algorithm to 
 * determine what an interest point is and what the orientation of the
 * point is. It uses a sobel 3x3 filter for the edge detection. This code
 * is our original code with inspiration from the following open source
 * projects:
 * 
 * Chris Evens http://www.chrisevansdev.com/computer-vision-opensurf.html
 * Dewald Esterhuizen http://softwarebydefault.com
 * vabc3 https://github.com/vabc3/MineStudio/blob/master/OpenSURFcs/Match.cs
 * 
 * This code is free to use, redistribute, and refactor. We have no
 * liability towards the unauthorized use or misuse of this software.
 * It is not guarunteed to work for any particular purpose and as such
 * should be modified to fit your particular needs. Thank you for viewing
 * and or using our software, we hope you enjoy.
 * ***************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace seniorProjFinal
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
