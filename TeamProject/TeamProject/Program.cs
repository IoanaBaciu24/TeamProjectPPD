using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamProject
{
    class Program
    {
        static List<KeyValuePair<String, int[]>> pictures = new List<KeyValuePair<string, int[]>>()
        {
            new KeyValuePair<string, int[]>("E:\\algoritzm\\pentagon.png", new int[2]{ 100, 50}),
            new KeyValuePair<string, int[]>("C:\\Users\\papuci\\Documents\\PPD\\TeamProj\\TeamProjectPPD\\grid2_smol.jpg", new int[2]{ 100, 100}),//todo put the right names
            new KeyValuePair<string, int[]>("C:\\Users\\papuci\\Documents\\PPD\\TeamProj\\TeamProjectPPD\\grid_rotated_smol.jpg", new int[2]{ 100, 100}),
            new KeyValuePair<string, int[]>("C:\\Users\\papuci\\Documents\\PPD\\TeamProj\\TeamProjectPPD\\chisi_new.jpg", new int[2]{ 100, 300}),
        };
        static int NR_THREADS = 6;

        static void Main(string[] args)
        {


            RunConsole();



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
                        Console.WriteLine("creating pentagon");
                        ExecuteTransform(pictures[0].Key, pictures[0].Value[0], pictures[0].Value[1], 1);
                        break;
                    case "2":
                        Console.WriteLine("creating grid");
                        ExecuteTransform(pictures[1].Key, pictures[1].Value[0], pictures[1].Value[1], 2);
                        break;
                    case "3":
                        Console.WriteLine("creating rotated grid");
                        ExecuteTransform(pictures[2].Key, pictures[2].Value[0], pictures[2].Value[1], 3);
                        break;
                    case "4":
                        Console.WriteLine("creating cat");
                        ExecuteTransform(pictures[3].Key, pictures[3].Value[0], pictures[3].Value[1], 4);
                        break;
                    case "0":
                        return;
                    default:
                      
                        break;
                }

            }
        }

        public static void ExecuteTransform(String path, int bwTreshold, int htTreshold, int picNr)
        {
            PhotoHelper.ImRead(path, out var width, out var height, out var buffer);
            PhotoHelper.ConvertImageToGreyScaleAndTresholding(width, height, bwTreshold, buffer);
            //PhotoHelper.ImWrite("C:\\Users\\papuci\\Documents\\PPD\\TeamProj\\TeamProjectPPD\\grey_grid.png", width, height, buffer);

            Console.WriteLine("CONVERTED TO BINARY IMAGE, WIDTH: {0}, HEIGHT: {1}", width, height);
            PhotoHelper.HoughTransformThreads(width, height, buffer, NR_THREADS);
            Console.WriteLine("Done Hough Transform");
            var lines = PhotoHelper.CreateLinesFromHoughSpace(PhotoHelper.H, htTreshold);
            Console.WriteLine(lines.Count);
            Console.WriteLine("Created Lines from HS");
            PhotoHelper.AddLinesToPhoto(lines, width, height, buffer);
            Console.WriteLine("Added lines");
            PhotoHelper.ImWrite("C:\\Users\\papuci\\Documents\\PPD\\TeamProj\\TeamProjectPPD\\result" +picNr +".png", width, height, buffer);
            Console.WriteLine("Done!!");
        }
    }
}
