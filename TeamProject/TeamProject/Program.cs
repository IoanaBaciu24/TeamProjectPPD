using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamProject
{
    class Program
    {
        static void Main(string[] args)
        {

            //PhotoHelper.ImRead("C:\\Users\\papuci\\Documents\\PPD\\TeamProj\\TeamProjectPPD\\p2.PNG", out var width, out var height, out var buffer);
            //PhotoHelper.ConvertImageToGreyScaleAndTresholding(width, height, 75, buffer);
            //PhotoHelper.ImWrite("C:\\Users\\papuci\\Documents\\PPD\\TeamProj\\TeamProjectPPD\\grey2.jpg", width, height, buffer);

            //PhotoHelper.ImRead("C:\\Users\\papuci\\Documents\\PPD\\TeamProj\\TeamProjectPPD\\grey_chisi2.jpg", out var width, out var height, out var buffer);
            PhotoHelper.ImRead("E:\\algoritzm\\pentagon.png", out var width, out var height, out var buffer);
            Console.WriteLine("READ THE IMAGE");
            var H = PhotoHelper.HoughTransform(width, height, buffer, out var theta, out var rho);
            Console.WriteLine("Done Hough Transform");
            var lines = PhotoHelper.CreateLinesFromHoughSpace(H, theta, rho, 20);
            Console.WriteLine(lines.Count);
            Console.WriteLine("Created Lines from HS");
            PhotoHelper.AddLinesToPhoto(lines, width, height, buffer);
            Console.WriteLine("Added lines");
            PhotoHelper.ImWrite("C:\\Users\\papuci\\Documents\\PPD\\TeamProj\\TeamProjectPPD\\lined_chisi.jpg", width, height, buffer);
            Console.WriteLine("Done!!");





        }
    }
}
