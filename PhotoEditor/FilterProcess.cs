using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace PhotoEditor
{
    class GrayScaleFilter : Filters
    {
        protected override Color CalculatePixelColor(Bitmap sourceImage, int x, int y)
        {
            int Intensity = (int)(0.3 * sourceImage.GetPixel(x, y).R +
                            0.59 * sourceImage.GetPixel(x, y).G +
                            0.11 * sourceImage.GetPixel(x, y).B);
            Intensity = Clamp(Intensity, 0, 255);
            Color c = Color.FromArgb(Intensity, Intensity, Intensity);
            return c;
        }
    }
    class Binarization : Filters
    {
        protected override Color CalculatePixelColor(Bitmap sourceImage, int x, int y)
        {
            int Intensity = (int)(0.3 * sourceImage.GetPixel(x, y).R +
                            0.59 * sourceImage.GetPixel(x, y).G +
                            0.11 * sourceImage.GetPixel(x, y).B);
            Intensity = Clamp(Intensity, 0, 255);
            if (Intensity > 200)
            {
                Intensity = 255;
            }
            else
            {
                Intensity = 0;
            }
            Color c = Color.FromArgb(Intensity, Intensity, Intensity);
            return c;
        }
    }
    class GaussianFilter : MatrixFilter
    {
        public GaussianFilter()
        {
            createGaussianKernel(5, 3);
        }
        public void createGaussianKernel(int radius, float sigma)
        {
            int size = 2 * radius + 1;
            kernel = new float[size, size];
            float norm = 0;
            for (int i = -radius; i <= radius; i++)
            {
                for (int j = -radius; j <= radius; j++)
                {
                    kernel[i + radius, j + radius] = (float)(Math.Exp(-(i * i + j * j) / (sigma * sigma)));
                    norm += kernel[i + radius, j + radius];
                }
            }
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    kernel[i, j] /= norm;
                }
            }
        }
    }
    class MedianFilter : MatrixFilter
    {
        public MedianFilter(int n)
        {
            kernel = new float[n, n];
        }

        protected override Color CalculatePixelColor(Bitmap sourceImage, int x, int y)
        {
            int radiusX = kernel.GetLength(0) / 2;
            int radiusY = kernel.GetLength(1) / 2;
            int n = kernel.GetLength(0) * kernel.GetLength(1);
            float resultR = 0;
            float resultG = 0;
            float resultB = 0;
            int[] cR = new int[n];
            int[] cB = new int[n];
            int[] cG = new int[n];
            int g = 0;

            for (int l = -radiusY; l <= radiusY; l++)
                for (int k = -radiusX; k <= radiusX; k++)
                {
                    int idX = Clamp(x + k, radiusX, sourceImage.Width - radiusX);
                    int idY = Clamp(y + l, radiusY, sourceImage.Height - radiusY);
                    Color c = sourceImage.GetPixel(idX, idY);
                    cR[g] = c.R;
                    cG[g] = c.G;
                    cB[g] = c.B;
                    g++;
                }
            quickSort(cR, 0, n - 1);
            quickSort(cG, 0, n - 1);
            quickSort(cB, 0, n - 1);
            int med = (int)(n / 2) + 1;
            resultR = cR[med];
            resultG = cG[med];
            resultB = cB[med];
            return Color.FromArgb(
                Clamp((int)resultR, 0, 255),
                Clamp((int)resultG, 0, 255),
                Clamp((int)resultB, 0, 255)
                );
        }
        static void quickSort(int[] a, int l, int r)
        {
            int temp;
            int x = a[l + (r - l) / 2];

            int i = l;
            int j = r;
            while (i <= j)
            {
                while (a[i] < x) i++;
                while (a[j] > x) j--;
                if (i <= j)
                {
                    temp = a[i];
                    a[i] = a[j];
                    a[j] = temp;
                    i++;
                    j--;
                }
            }
            if (i < r)
                quickSort(a, i, r);

            if (l < j)
                quickSort(a, l, j);
        }
    }
    class SobelFilter : MatrixFilter
    {
        public int[,] sobelX = new int[3, 3] { { -1, 0, 1 },
                                               { -2, 0, 2 },
                                               { -1, 0, 1 } };

        public int[,] sobelY = new int[3, 3] { { 1, 2, 1 },
                                               { 0, 0, 0 }, 
                                               { -1, -2, -1 } };

        private Color SumColor(Color a, Color b)
        {
            Color rescolor = Color.FromArgb(
                Clamp((int)Math.Sqrt(Math.Pow(a.R, 2) + Math.Pow(b.R, 2)), 0, 255),
                Clamp((int)Math.Sqrt(Math.Pow(a.G, 2) + Math.Pow(b.G, 2)), 0, 255),
                Clamp((int)Math.Sqrt(Math.Pow(a.B, 2) + Math.Pow(b.B, 2)), 0, 255)
                );
            return rescolor;
        }

        protected override Color CalculatePixelColor(Bitmap sourceImage, int x, int y)
        {
            int radiusX = 1;
            int radiusY = 1;
            float XresultR = 0;
            float XresultG = 0;
            float XresultB = 0;
            float YresultR = 0;
            float YresultG = 0;
            float YresultB = 0;
            for (int l = -radiusY; l <= radiusY; l++)
                for (int k = -radiusX; k <= radiusX; k++)
                {
                    int idX = Clamp(x + k, 0, sourceImage.Width - 1);
                    int idY = Clamp(y + 1, 0, sourceImage.Height - 1);
                    Color neighborColor = sourceImage.GetPixel(idX, idY);
                    XresultR += neighborColor.R * sobelX[k + radiusX, l + radiusY];
                    XresultG += neighborColor.G * sobelX[k + radiusX, l + radiusY];
                    XresultB += neighborColor.B * sobelX[k + radiusX, l + radiusY];
                    YresultR += neighborColor.R * sobelY[k + radiusX, l + radiusY];
                    YresultG += neighborColor.G * sobelY[k + radiusX, l + radiusY];
                    YresultB += neighborColor.B * sobelY[k + radiusX, l + radiusY];
                }
            Color cX = Color.FromArgb(
                Clamp((int)XresultR, 0, 255),
                Clamp((int)XresultG, 0, 255),
                Clamp((int)XresultB, 0, 255)
                );
            Color cY = Color.FromArgb(
                Clamp((int)YresultR, 0, 255),
                Clamp((int)YresultG, 0, 255),
                Clamp((int)YresultB, 0, 255)
                );
            Color res = SumColor(cX, cY);
            return res;
        }
    }
    class OpeningFilter : MatMorphology
    {
        public OpeningFilter(MaskType maskType)
            : base(maskType)
        {
        }

        public override Bitmap ProcessImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);
            resultImage = Opening(sourceImage, worker);
            return resultImage;
        }
    }
    class MotionBlurFilter : MatrixFilter
    {
        public MotionBlurFilter(int n)
        {
            int sizeX = n;
            int sizeY = n;
            kernel = new float[sizeX, sizeY];
            for (int i = 0; i < sizeX; i++)
            {
                for (int j = 0; j < sizeY; j++)
                {
                    if (i != j)
                    {
                        kernel[i, j] = 0;
                    }
                    else
                    {
                        kernel[i, j] = (float)(1.0 / n);
                    }
                }
            }
        }
    }
}
