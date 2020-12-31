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

            PhotoHelper.ImRead("C:\\Users\\papuci\\Documents\\PPD\\TeamProj\\TeamProjectPPD\\chisi.jpg", out var width, out var height, out var buffer);
            PhotoHelper.ConvertImageToGreyScaleAndTresholding(width, height, 75, buffer);
            PhotoHelper.ImWrite("C:\\Users\\papuci\\Documents\\PPD\\TeamProj\\TeamProjectPPD\\grey_chisi.jpg", width, height, buffer);
        }
    }
}
