﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace TeamProjectMPI
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


        public static List<Line> CreateLinesFromHoughSpace(int[][] H, double[]theta, int[]rho, int treshold)
        {
            List<Line> result = new List<Line>();
            Parallel.For(0, rho.Length, i =>
            {
                for (int j = 0; j < theta.Length; j++)
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

    }
}