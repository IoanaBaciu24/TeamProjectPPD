using MPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamProjectMPI
{
    class Program
    {

        static int[,] AccumulateAccumulators(List<int[,]> HSResults, int len1, int len2)
        {
            var result = new int[len1, len2];

            for(int i=0;i<len1;i++)
            {
                for(int j=0;j<len2;j++)
                {
                    foreach (var hs in HSResults)
                        result[i,j] += hs[i,j];
                }
            }

            return result;
        }


        static List<KeyValuePair<String, int[]>> pictures = new List<KeyValuePair<string, int[]>>()
        {
            new KeyValuePair<string, int[]>("E:\\algoritzm\\pentagon.png", new int[2]{ 100, 50}),
            new KeyValuePair<string, int[]>("C:\\Users\\papuci\\Documents\\PPD\\TeamProj\\TeamProjectPPD\\grid2_smol.jpg", new int[2]{ 100, 100}),//todo put the right names
            new KeyValuePair<string, int[]>("C:\\Users\\papuci\\Documents\\PPD\\TeamProj\\TeamProjectPPD\\grid_rotated_smol.jpg", new int[2]{ 100, 100}),
            new KeyValuePair<string, int[]>("C:\\Users\\papuci\\Documents\\PPD\\TeamProj\\TeamProjectPPD\\chisi_new.jpg", new int[2]{ 100, 300}),
        };

        public static void Master()
        {
            PrintMenu();
            var cmd = Console.ReadLine();
            int pic = Int32.Parse(cmd);

            PhotoHelper.ImRead(pictures[pic - 1].Key, out int width, out int height, out byte[] buffer);


            PhotoHelper.ConvertImageToGreyScaleAndTresholding(width, height, pictures[pic-1].Value[0], buffer);


            Console.WriteLine("starting HS");
            int distMax = (int)Math.Round(Math.Sqrt(height * height + width * width));
            double[] theta = new double[180];
            int[] rho = new int[distMax * 2 + 1];
            int[,]H = new int[rho.Length, theta.Length];
           
            int idx = 0;
            for (int i = 0; i < 180; i++)
            { theta[idx] = (Math.PI) / 180 * i; idx++; }
            idx = 0;
            for (int i = -distMax; i < distMax; i++)
            {
                rho[idx] = i;
                idx++;
            }
            Console.WriteLine("got here 1");
            for (int i = 1; i < Communicator.world.Size; i++)
            {
                Communicator.world.Send<double[]>(theta, i, 0);
                Communicator.world.Send<int[]>(rho, i, 0);
                Communicator.world.Send<byte[]>(buffer, i, 0);
                Communicator.world.Send<int>(width, i, 0);
                Communicator.world.Send<int>(height, i, 0);
            }

            List<int[,]> hsMatrices = new List<int[,]>();
            Console.WriteLine("got here 2");


            hsMatrices.Add(PhotoHelper.HSIncrementation(width, height, buffer, 0, Communicator.world.Size, theta, rho));
            for(int i=1;i<Communicator.world.Size;i++)
            {
                hsMatrices.Add(Communicator.world.Receive<int[,]>(i, 0));
            }
            Console.WriteLine("accumulating matrices....");
            Console.WriteLine(hsMatrices.Count);
            H = AccumulateAccumulators(hsMatrices, rho.Length, theta.Length);
            Console.WriteLine("Done Hough Transform");
            var lines = PhotoHelper.CreateLinesFromHoughSpace(H,theta, rho,  pictures[pic-1].Value[1]);
            Console.WriteLine(lines.Count);
            Console.WriteLine("Created Lines from HS");
            PhotoHelper.AddLinesToPhoto(lines, width, height, buffer);
            Console.WriteLine("Added lines");
            PhotoHelper.ImWrite("C:\\Users\\papuci\\Documents\\PPD\\TeamProj\\TeamProjectPPD\\result" + pic + ".png", width, height, buffer);
            Console.WriteLine("Done!!");
        }

        public static void Slave()
        {
            var theta = Communicator.world.Receive<double[]>(0, 0);
            var rho = Communicator.world.Receive<int[]>(0, 0);
            var buffer = Communicator.world.Receive<byte[]>(0, 0);
            var width = Communicator.world.Receive<int>(0, 0);
            var height = Communicator.world.Receive<int>(0, 0);

            var res = PhotoHelper.HSIncrementation(width, height, buffer, Communicator.world.Rank, Communicator.world.Size, theta, rho);
            Console.WriteLine("process {0} came back from hs incr", Communicator.world.Rank);
            Communicator.world.Send(res, 0, 0);


        }

        static void Main(string[] args)
        {
            MPI.Environment.Run(ref args, communicator => {
                if (Communicator.world.Rank == 0)
                {
                    Master();
                }
                else
                { //child process
                    Slave();
                }
            });
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
    }
}
