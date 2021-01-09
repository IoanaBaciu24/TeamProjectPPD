using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamProject
{
    class Program
    {
        List<KeyValuePair<String, int[]>> pictures = new List<KeyValuePair<string, int[]>>()
        {
            new KeyValuePair<string, int[]>("E:\\algoritzm\\pentagon.png", new int[2]{ 100, 20}),
            new KeyValuePair<string, int[]>("E:\\algoritzm\\pentagon.png", new int[2]{ 100, 20}),//todo put the right names
            new KeyValuePair<string, int[]>("E:\\algoritzm\\pentagon.png", new int[2]{ 100, 20}),
            new KeyValuePair<string, int[]>("C:\\Users\\papuci\\Documents\\PPD\\TeamProj\\TeamProjectPPD\\chisi_new.jpg", new int[2]{ 100, 175}),
        };
        int NR_THREADS = 6;

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

        public static void PrintMenu()
        {
            Console.WriteLine("1. Simple Pentagon");
            Console.WriteLine("2. Vertical Grid");
            Console.WriteLine("3. Diagonal Grid");
            Console.WriteLine("4. Cat");
            Console.WriteLine("0. Exit");
            Console.WriteLine(">");


        }

        public static void RunConsole()
        {
            while(true)
            {
                PrintMenu();
                var cmd = Console.ReadLine();
                switch(cmd)
                {
                    case "1":
                        Console.WriteLine();
                        break;
                    default:
                        break;
                }

            }
        }

        public static void ExecuteTransform(String path, int bwTreshold, int htTreshold, int picNr)
        {
            PhotoHelper.ImRead(path, out var width, out var height, out var buffer);
            PhotoHelper.ConvertImageToGreyScaleAndTresholding(width, height, bwTreshold, buffer);
            Console.WriteLine("CONVERTED TO BINARY IMAGE, WIDTH: {0}, HEIGHT: {1}", width, height);
            PhotoHelper.HoughTransformThreads(width, height, buffer, 8);
            Console.WriteLine("Done Hough Transform");
            var lines = PhotoHelper.CreateLinesFromHoughSpace(PhotoHelper.H, 175);
            Console.WriteLine(lines.Count);
            Console.WriteLine("Created Lines from HS");
            PhotoHelper.AddLinesToPhoto(lines, width, height, buffer);
            Console.WriteLine("Added lines");
            PhotoHelper.ImWrite("C:\\Users\\papuci\\Documents\\PPD\\TeamProj\\TeamProjectPPD\\result" +picNr +".png", width, height, buffer);
            Console.WriteLine("Done!!");
        }
    }
}
