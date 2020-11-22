﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.ComponentModel;

namespace PhotoEditor
{
    abstract class Filters
    {
        public Random rand = new Random();

        protected virtual Color CalculatePixelColor(Bitmap sourceImage, int x, int y)
        {
            return Color.Black;
        }

        public int Clamp(int value, int min, int max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }

        public virtual Bitmap ProcessImage(Bitmap sourceImage, BackgroundWorker worker)
        {
            BeforeProcessImage(sourceImage);
            Bitmap resultImage = new Bitmap(sourceImage.Width, sourceImage.Height);
            for (int i = 0; i < sourceImage.Width; i++)
            {
                worker.ReportProgress((int)((float)i / resultImage.Width * 100));
                if (worker.CancellationPending)
                    return null;
                for (int j = 0; j < sourceImage.Height; j++)
                {
                    resultImage.SetPixel(i, j, CalculatePixelColor(sourceImage, i, j));
                }
            }
            return resultImage;
        }

        protected virtual void BeforeProcessImage(Bitmap sourceImage)
        {
        }
    }
}
