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

            //PhotoHelper.ImRead("C:\\Users\\papuci\\Documents\\PPD\\TeamProj\\TeamProjectPPD\\chisi_new.jpg", out var width, out var height, out var buffer);
            //PhotoHelper.ConvertImageToGreyScaleAndTresholding(width, height, 200, buffer);
            //PhotoHelper.ImWrite("C:\\Users\\papuci\\Documents\\PPD\\TeamProj\\TeamProjectPPD\\chisi_new_ts.jpg", width, height, buffer);

            PhotoHelper.ImRead("C:\\Users\\papuci\\Documents\\PPD\\TeamProj\\TeamProjectPPD\\chisi_new_ts.jpg", out var width, out var height, out var buffer);
            //PhotoHelper.ImRead("E:\\algoritzm\\pentagon.png", out var width, out var height, out var buffer);

            Console.WriteLine("HEIGHT: {0}, WIDTH: {1}", height, width);
            Console.WriteLine("READ THE IMAGE");
            PhotoHelper.HoughTransformThreads(width, height, buffer, 8);
            //var HS = PhotoHelper.HoughTransform2(width, height, buffer);
            Console.WriteLine("Done Hough Transform");
            //var lines = PhotoHelper.GetLinesFromHS(HS, width, height, buffer, 20);

            var lines = PhotoHelper.CreateLinesFromHoughSpace(PhotoHelper.H, 175);
            Console.WriteLine(lines.Count);
            Console.WriteLine("Created Lines from HS");
            PhotoHelper.AddLinesToPhoto(lines, width, height, buffer);
            Console.WriteLine("Added lines");
            PhotoHelper.ImWrite("C:\\Users\\papuci\\Documents\\PPD\\TeamProj\\TeamProjectPPD\\result6.png", width, height, buffer);
            Console.WriteLine("Done!!");





        }
    }
}
