using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
//using static System.Net.Mime.MediaTypeNames;

namespace TeamProject
{
    class PhotoHelper
    {
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
                for(int j=0;j<width;++j)
                {
                    var colorPixel = GetPx(width, height, buffer, i, j); 
                    byte gray = (byte)((colorPixel.R + colorPixel.G + colorPixel.B) / 3);
                    gray = (byte)(gray > treshold ? 255 : 0);
                    colorPixel = Color.FromArgb( gray,gray, gray);
                    SetPx(width, height, buffer, i, j, colorPixel);
                }
            });
        }


        public static bool CheckIfPxIsInEdge(int width, int height, byte[] buffer, int i, int j)
        {
            var pixel = GetPx(width, height, buffer, i, j);
            return pixel.R == 0;
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
            rho = new int[distMax * 2 + 1];
            var H = new int[rho.Length, theta.Length];
            int idx = 0;
            for (int i = -90; i < 90; i++)
            { theta[idx] = (Math.PI)/180*i; idx++; }

            idx = 0;
            for(int i=-distMax;i<=distMax;i++)
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
                            //var angleInRadians = theta[iTheta] * Math.PI / 180;
                            //var distanceFromOrigin = j * Math.Cos(angleInRadians) + i * Math.Sin(angleInRadians);
                            //FindClosestRhoToDistance(rho, distanceFromOrigin, out var minDist, out var indexInArray);

                            //if(minDist<1)
                            //{
                            //    H[indexInArray, iTheta]++;
                            //}

                            var r = j * Math.Cos(theta[iTheta]) + i * Math.Sin(theta[iTheta]);
                            H[(int)r + distMax, iTheta]++;
                        }
                    }
                }

                Console.WriteLine("i is {0}", i);
            }

            return H;

        }



        public static List<Line> CreateLinesFromHoughSpace(int[,] H, double[] theta, int[] rho, int treshold)
        {
            List<Line> result = new List<Line>(); 
            for(int i =0;i<rho.Length;i++)
            {
                for(int j = 0;j<theta.Length;j++)
                {
                    if(H[i,j]>=treshold)
                    {
                        var a = Math.Cos(theta[j]);
                        var b = Math.Sin(theta[j]);
                        var x0 = a * rho[i];
                        var y0 = b * rho[i];

                        var x1 = (int)(x0 + 500 * (-b));
                        var y1 = (int)(y0 + 500 * (a));
                        var x2 = (int)(x0 - 500 * (-b));
                        var y2 = (int)(y0 - 500 * (a));

                        result.Add(new Line(x1, y1, x2, y2));
                    }
                }
            }

            return result;

        }


        public static void AddLinesToPhoto(List<Line> lines, int width, int height, byte[] buffer)
        {
            Parallel.For(0, height, i =>
            {
                for (int j = 0; j < width; ++j)
                {
                    if(Line.CheckIfPointShouldBeColoured(lines, i,j))
                    {
                        //Console.WriteLine("AICI");
                        var colorPixel = Color.FromArgb(255, 0, 0);
                        SetPx(width, height, buffer, i, j, colorPixel);
                    }
                }
            });
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
            for(int i=0; i<200;i++)
            {
                if(line.slope !=double.NaN)
                {
                    var j = (int)(line.slope * i + line.yIntersect);
                    if(i<height && j<width)
                    {
                        SetPx(width, height, buffer, i, j, Color.FromArgb(255, 0, 0));
                    }
                }
            }
        }

    }
}
