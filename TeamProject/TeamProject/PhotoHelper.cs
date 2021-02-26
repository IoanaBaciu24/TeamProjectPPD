using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//using static System.Net.Mime.MediaTypeNames;

namespace TeamProject
{
    class PhotoHelper
    {
        public static double[] theta;
        public static int[] rho;
        public static int[][] H;
        public static void ImRead(string path, out int width, out int height, out byte[] buffer)
        {
            var image = (Bitmap)Image.FromFile(path);
            var lockedImage = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb);
            width = image.Width;
            height = image.Height;
            var length = lockedImage.Stride * lockedImage.Height;
            buffer = new byte[length];
            Marshal.Copy(lockedImage.Scan0, buffer, 0, length);
            image.UnlockBits(lockedImage);
        }


        public static void ImWrite(string path, int width, int height, byte[] buffer)
        {
            var image = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            var lockedImage = image.LockBits(new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format24bppRgb);
            Marshal.Copy(buffer, 0, lockedImage.Scan0, buffer.Length);
            image.UnlockBits(lockedImage);
            image.Save(path, ImageFormat.Jpeg);
        }

        public static Color GetPx(int width, int height, byte[] buffer, int i, int j)
        {
            if (i < 0 || i >= height)
                throw new ArgumentOutOfRangeException(nameof(i), "Invalid row identifier!");
            if (j < 0 || j >= width)
                throw new ArgumentOutOfRangeException(nameof(j), "Invalid column identifier!");
            var b = buffer[3 * (i * width + j) + 0];
            var g = buffer[3 * (i * width + j) + 1];
            var r = buffer[3 * (i * width + j) + 2];
            return Color.FromArgb(r, g, b);
        }

        public static void SetPx(int width, int height, byte[] buffer, int i, int j, Color c)
        {
            if (i < 0 || i >= height)
                throw new ArgumentOutOfRangeException(nameof(i), "Invalid row identifier!");
            if (j < 0 || j >= width)
                throw new ArgumentOutOfRangeException(nameof(j), "Invalid column identifier!");
            buffer[3 * (i * width + j) + 0] = c.B;
            buffer[3 * (i * width + j) + 1] = c.G;
            buffer[3 * (i * width + j) + 2] = c.R;
        }



        public static void ConvertImageToGreyScaleAndTresholding(int width, int height, int treshold, byte[] buffer)
        {
            Parallel.For(0, height, i =>
            {
            Parallel.For(0, width, j =>
              {
                  // numai un parallel for!!
                  var colorPixel = GetPx(width, height, buffer, i, j);
                  byte gray = (byte)((colorPixel.R + colorPixel.G + colorPixel.B) / 3);
                  byte luma = (byte)(0.299 * colorPixel.R + 0.587 * colorPixel.G + 0.114 * colorPixel.B);
                  gray = (byte)(luma > treshold ? 255 : 0);
                  colorPixel = Color.FromArgb(gray, gray, gray);
                  SetPx(width, height, buffer, i, j, colorPixel);
              });
            });
        }


        public static bool CheckIfPxIsInEdge(int width, int height, byte[] buffer, int i, int j)
        {
            var pixel = GetPx(width, height, buffer, i, j);
            return pixel.R == 0 && pixel.G == 0 && pixel.B == 0;
        }

        public static void FindClosestRhoToDistance(int[] rho, double dist, out double minDist, out int indexInArray)
        {
            minDist = double.MaxValue;
            indexInArray = -1;
            for(int i=0;i<rho.Length;i++)
            {
                if(Math.Abs(rho[i] - dist) < minDist)
                {
                    minDist = Math.Abs(rho[i] - dist);
                    indexInArray = i;
                }
            }
        }

        public static int[,] HoughTransform(int width, int height, byte[] buffer, out double[] theta, out int[] rho)
        {
            int distMax = (int)Math.Round(Math.Sqrt(height * height + width * width));
            theta = new double[180];
            rho = new int[distMax*2+1];
            var H = new int[rho.Length, theta.Length];
            int idx = 0;
            for (int i = 0; i < 180; i++)
            { theta[idx] = (Math.PI) / 180 * i; idx++; }

            idx = 0;
            for(int i=-distMax;i<distMax;i++)
            {
                rho[idx] = i;
                idx++;
            }

            Console.WriteLine("Heihgt and width: {0}, {1}", height, width);
            for(int i=0;i<height;i++)
            {
                for(int j=0;j<width;j++)
                {
                    if(CheckIfPxIsInEdge(width,height,buffer,i,j))
                    {
                        for (var iTheta= 0; iTheta < theta.Length; iTheta++)
                        {
                            var r = i * Math.Cos(theta[iTheta]) + j * Math.Sin(theta[iTheta]);

                            var rh = FindDamnIndexInDamnArray(rho, (int)r);
                            H[rh, iTheta]++;
                  

                        }
                    }
                }

                Console.WriteLine("i is {0}", i);
            }

            return H;

        }


        public static void HSIncrementation(Object threadObj)
        {
            List<Object> param = (List<Object>)threadObj;
            byte[] buffer = (byte[])param[0];
            int width = (int)param[1];
            int height = (int)param[2];
            int index = (int)param[3];
            int nthr = (int)param[4];

            //Quadruple quadruple = (Quadruple)param[3];
            //int[,] HS = (int[,])param[4];
            Console.WriteLine("got here");
            var count = 0;

            int startRow, endRow, startCol, endCol;

            if(index == nthr-1)
            {
                startRow = index * (height / nthr);
                endRow = height;
                startCol = index * (width / nthr);
                endCol = width;

            }
            else
            {
                startRow = index * (height / nthr);
                endRow = (index+1) * (height / nthr)-1;
                startCol = index * (width / nthr);
                endCol = (index + 1) * (width / nthr) - 1; ;
            }

            Quadruple q = new Quadruple(startRow, endRow, startCol, endCol);
            Console.WriteLine(q);
            for(int i=startRow; i<endRow; i++)
            {
                {
                    for (int j = 0; j < width; j++)
                    {
                        count++;

                        if (CheckIfPxIsInEdge(width, height, buffer, i, j))
                        {
                            for (var iTheta = 0; iTheta < theta.Length; iTheta++)
                            {
                                var r = i * Math.Cos(theta[iTheta]) + j * Math.Sin(theta[iTheta]);


                                var rh = FindDamnIndexInDamnArray(rho, (int)r);

                                Interlocked.Increment(ref H[rh][iTheta]);
                            }
                        }
                    }
                }
            }
            Console.WriteLine("Count: {0}", count);
        }





        public static void HoughTransformThreads(int width, int height, byte[] buffer,  int numberOfThreads)
        {
            int distMax = (int)Math.Round(Math.Sqrt(height * height + width * width));
            theta = new double[180];
            rho = new int[distMax * 2 + 1];
            H = new int[rho.Length][];
            for (int i = 0; i < rho.Length; i++)
            {
                H[i] = new int[theta.Length];
            }
            int idx = 0;
            for (int i = 0; i < 180; i++)
            { theta[idx] = (Math.PI) / 180 * i; idx++; }
            idx = 0;
            for (int i = -distMax; i < distMax; i++)
            {
                rho[idx] = i;
                idx++;
            }
            int chunkRows = height / numberOfThreads;
            int chunkColumns = width / numberOfThreads;
            List<ManualResetEvent> events = new List<ManualResetEvent>();
            Console.WriteLine("chunk rows: {0}, chunk cols: {1}", chunkRows, chunkColumns);
            List<Quadruple> indices = new List<Quadruple>();
            for (int i = 0; i < numberOfThreads - 1; i++)
            {

                indices.Add(new Quadruple(i * chunkRows, (i + 1) * chunkRows - 1, i * chunkColumns, (i + 1) * chunkColumns - 1));
            }

            indices.Add(new Quadruple((numberOfThreads - 1) * chunkRows, height - 1, (numberOfThreads - 1) * chunkColumns, width - 1));
            Console.WriteLine("indices size: {0}", indices.Count);
            ThreadPool.SetMaxThreads(numberOfThreads, numberOfThreads);
            ThreadPool.SetMinThreads(numberOfThreads, numberOfThreads);
            for (int i = 0; i < numberOfThreads; i++)
            {
                ManualResetEvent resetEvent = new ManualResetEvent(false);
                var index = new int();
                index = i;
                ThreadPool.QueueUserWorkItem(arg =>
                {

                    List<Object> thrParameter = new List<Object>();
                    thrParameter.Add(buffer);
                    thrParameter.Add(width);
                    thrParameter.Add(height);
                    thrParameter.Add(index);
                    thrParameter.Add(numberOfThreads);

                    HSIncrementation(thrParameter);

                    resetEvent.Set();
                });
                events.Add(resetEvent);
            }
            WaitHandle.WaitAll(events.ToArray());
            //List<Thread> threads = new List<Thread>();

            //for (int i = 0; i < numberOfThreads; i++)
            //{
            //    Thread thr = new Thread(new ParameterizedThreadStart(HSIncrementation));
            //    threads.Add(thr);
            //    int index = new int();
            //    index = i; 
            //    thr.Start(new List<Object>() { buffer, width, height, indices[index]});
            //}
            //for (int k = 0; k < threads.Count; k++)
            //{
            //    threads[k].Join();
            //}
            //return H; 
        }



        private static int FindDamnIndexInDamnArray(int[] arr, int elem)
        {
            for (int i = 0; i < arr.Length; i++)
                if (arr[i] == elem)
                    return i;
            return -1;
        }


        public static HoughSpace HoughTransform2(int width, int height, byte[] buffer)
        {
            HoughSpace result = new HoughSpace();

            int distMax = (int)Math.Round(Math.Sqrt(height * height + width * width));
            var theta = new double[179];
            var rho = new int[distMax*2+1];
            //var H = new int[rho.Length, theta.Length];
            int idx = 0;
            for (int i = 0; i < 90; i++)
            { theta[idx] = (Math.PI) / 180 * i; idx++; }
            for (int i = 91; i < 180; i++)
            { theta[idx] = (Math.PI) / 180 * i; idx++; }
            //{ theta[idx] = i;idx++; }

            idx = 0;
            for (int i = -distMax; i <= distMax; i++)
            {
                rho[idx] = i;
                idx++;
            }

            Console.WriteLine("Heihgt and width: {0}, {1}", height, width);
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (CheckIfPxIsInEdge(width, height, buffer, i, j))
                    {
                        for (var iTheta = 0; iTheta < theta.Length; iTheta++)
                        {
                            //var angleInRadians = theta[iTheta] * Math.PI / 180;
                            //var distanceFromOrigin = j * Math.Cos(angleInRadians) + i * Math.Sin(angleInRadians);
                            var r = i * Math.Cos(theta[iTheta]) + j * Math.Sin(theta[iTheta]);
                            //FindClosestRhoToDistance(rho, r, out var minDist, out var indexInArray);
                            var rhoIndex = FindDamnIndexInDamnArray(rho, (int)r);
                            //H[(int)r + distMax, iTheta]++;
                            var kv = new KeyValuePair<double, int>(theta[iTheta],rhoIndex);
                            if(result.HS.ContainsKey(kv))
                            {
                                result.HS[kv].Add(new KeyValuePair<int, int>(i, j)); 

                            }
                            else
                            {
                                var l = new List<KeyValuePair<int, int>>();
                                l.Add(new KeyValuePair<int, int>(i, j));
                                result.HS.Add(kv, l);
                            }
                            

                            //if (minDist < 1)
                            //{
                            //    H[indexInArray, iTheta]++;
                            //}


                        }
                    }
                }

                Console.WriteLine("i is {0}", i);
            }

            return result;

        }





        public static List<Line> CreateLinesFromHoughSpace(int[][] H, int treshold)
        {
            List<Line> result = new List<Line>();
            Parallel.For(0, rho.Length, i =>
            {
                for (int j = 0; j< theta.Length; j++)
                {
                    if (H[i][j] >= treshold)
                    {
                        var a = Math.Cos(theta[j]);
                        var b = Math.Sin(theta[j]);
                        var x0 = a * rho[i];
                        var y0 = b * rho[i];

                        var x1 = (int)(x0 + 50 * b);
                        var y1 = (int)(y0 - 50 * a);
                        var x2 = (int)(x0 - 50 * b);
                        var y2 = (int)(y0 + 50 * a);
                        var l = new Line(x1, y1, x2, y2);
                        l.theta = theta[j];
                        l.rho = rho[i];
                        result.Add(new Line(x1, y1, x2, y2));
                    }
                }
            });
            

            return result;

        }


        public static List<Line> GetLinesFromHS(HoughSpace hs, int width, int height, byte[] buffer, int threshold)
        {
            List<Line> result = new List<Line>();

            foreach(var key in hs.HS.Keys)
            {
                if(hs.HS[key].Count > threshold)
                {
                    result.Add(new Line(hs.HS[key][0].Key, hs.HS[key][0].Value, hs.HS[key][3].Key, hs.HS[key][3].Value));
                }
            }

            return result;
        }


        public static void AddLinesToPhoto(List<Line> lines, int width, int height, byte[] buffer)
        {
            for(int i=0; i<height; i++)
            {
                for (int j = 0; j < width; ++j)
                {
                    if(Line.CheckIfPointShouldBeColoured(lines, i,j, false))
                    {
                        //Console.WriteLine("AICI");
                        var colorPixel = Color.FromArgb(255, 0, 0);
                        SetPx(width, height, buffer, i, j, colorPixel);
                    }
                }
            }
        }



        public static void DrawLines(List<Line> lines, int width, int height, byte[] buffer)
        {
            foreach(var line in lines)
            {
                DrawALine(line, width, height, buffer);
            }
        }

        public static void DrawALine(Line line, int width, int height, byte[] buffer)
        {
            for(int i=0; i<500;i++)
            {
                if(line.slope !=double.NaN)
                {
                    var j = Math.Abs((int)(line.slope * i + line.yIntersect));
                    if(i<height && j<width)
                    {
                        SetPx(width, height, buffer, i, j, Color.FromArgb(255, 0, 0));
                    }
                }
            }
        }

    }
}
