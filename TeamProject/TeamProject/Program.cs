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

            //PhotoHelper.ImRead("C:\\Users\\papuci\\Documents\\PPD\\TeamProj\\TeamProjectPPD\\chis3.jpg", out var width, out var height, out var buffer);
            //PhotoHelper.ConvertImageToGreyScaleAndTresholding(width, height, 200, buffer);
            //PhotoHelper.ImWrite("C:\\Users\\papuci\\Documents\\PPD\\TeamProj\\TeamProjectPPD\\chisi_test.jpg", width, height, buffer);

            //PhotoHelper.ImRead("C:\\Users\\papuci\\Documents\\PPD\\TeamProj\\TeamProjectPPD\\grey_chisi.jpg", out var width, out var height, out var buffer);
            PhotoHelper.ImRead("E:\\algoritzm\\pentagon.png", out var width, out var height, out var buffer);
            Console.WriteLine("READ THE IMAGE");
            var H = PhotoHelper.HoughTransform(width, height, buffer, out var theta, out var rho);
            //var HS = PhotoHelper.HoughTransform2(width, height, buffer);
            Console.WriteLine("Done Hough Transform");
            //var lines = PhotoHelper.GetLinesFromHS(HS, width, height, buffer, 20);

            var lines = PhotoHelper.CreateLinesFromHoughSpace(H, theta, rho, 20);
            Console.WriteLine(lines.Count);
            Console.WriteLine("Created Lines from HS");
            PhotoHelper.AddLinesToPhoto(lines, width, height, buffer);
            Console.WriteLine("Added lines");
            PhotoHelper.ImWrite("C:\\Users\\papuci\\Documents\\PPD\\TeamProj\\TeamProjectPPD\\result4.png", width, height, buffer);
            Console.WriteLine("Done!!");





        }
    }
}
