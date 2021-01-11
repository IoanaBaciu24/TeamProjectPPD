using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;

namespace TeamProjectMPI
{
    class PhotoHelper
    {
        //public static int[][] H;

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


        public static List<Line> CreateLinesFromHoughSpace(int[,] H, double[]theta, int[]rho, int treshold)
        {
            List<Line> result = new List<Line>();
            Parallel.For(0, rho.Length, i =>
            {
                for (int j = 0; j < theta.Length; j++)
                {
                    if (H[i,j] >= treshold)
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
                   
                        result.Add(new Line(x1, y1, x2, y2));
                    }
                }
            });


            return result;

        }

        public static void AddLinesToPhoto(List<Line> lines, int width, int height, byte[] buffer)
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; ++j)
                {
                    if (Line.CheckIfPointShouldBeColoured(lines, i, j))
                    {
                        //Console.WriteLine("AICI");
                        var colorPixel = Color.FromArgb(255, 0, 0);
                        SetPx(width, height, buffer, i, j, colorPixel);
                    }
                }
            }
        }

        public static bool CheckIfPxIsInEdge(int width, int height, byte[] buffer, int i, int j)
        {
            var pixel = GetPx(width, height, buffer, i, j);
            return pixel.R == 0 && pixel.G == 0 && pixel.B == 0;
        }


        private static int FindDamnIndexInDamnArray(int[] arr, int elem)
        {
            for (int i = 0; i < arr.Length; i++)
                if (arr[i] == elem)
                    return i;
            return -1;
        }



        public static int[,] HSIncrementation(int width, int height, byte[] buffer, int index, int nrProcesses, double[]theta, int[]rho)
        {


            //Quadruple quadruple = (Quadruple)param[3];
            int[,] H = new int[rho.Length, theta.Length];
            Console.WriteLine("got here");
            var count = 0;

            int startRow, endRow;

            if (index == nrProcesses - 1)
            {
                startRow = index * (height / nrProcesses);
                endRow = height;
       

            }
            else
            {
                startRow = index * (height / nrProcesses);
                endRow = (index + 1) * (height / nrProcesses) - 1;
              
            }

            for (int i = startRow; i < endRow; i++)
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

                                if(H!=null)
                                {
                                    Interlocked.Increment(ref H[rh, iTheta]);

                                }
                            }
                        }
                    }
                }
            }

            return H;
        }


        public static void ConvertImageToGreyScaleAndTresholding(int width, int height, int treshold, byte[] buffer)
        {
            Parallel.For(0, height, i =>
            {
                Parallel.For(0, width, j =>
                {
                    var colorPixel = GetPx(width, height, buffer, i, j);
                    byte gray = (byte)((colorPixel.R + colorPixel.G + colorPixel.B) / 3);
                    byte luma = (byte)(0.299 * colorPixel.R + 0.587 * colorPixel.G + 0.114 * colorPixel.B);
                    gray = (byte)(luma > treshold ? 255 : 0);
                    colorPixel = Color.FromArgb(gray, gray, gray);
                    SetPx(width, height, buffer, i, j, colorPixel);
                });
            });
        }


    }
}
