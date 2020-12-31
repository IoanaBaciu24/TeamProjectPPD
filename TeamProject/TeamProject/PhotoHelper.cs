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


    }
}
