using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace PhotoEditor
{
    public partial class Form1 : Form
    {

        Bitmap image;
        Stack<Bitmap> BitmapStack = new Stack<Bitmap>();
        Random rnd = new Random();

        public Form1()
        {
            InitializeComponent();
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "image files|*.jpg;*.jpeg;*.png";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                image = new Bitmap(dialog.FileName);
                pictureBox1.Image = image;
                pictureBox1.Refresh();
            }
        }
        private void отменаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Bitmap img = BitmapStack.Pop();
                pictureBox1.Image = img;
                image = img;
                pictureBox1.Refresh();
            }
            catch (Exception)
            {
                MessageBox.Show("Стек изображений пуст.");
            }
        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                BitmapStack.Push((Bitmap)pictureBox1.Image);
                Bitmap newImage = ((Filters)e.Argument).ProcessImage(image, backgroundWorker1);
                if (backgroundWorker1.CancellationPending != true)
                    image = newImage;
            }
            catch (Exception)
            {
                MessageBox.Show("Ошибка. Нет изображения.");
            }

        }

        private void backgroundWorker1_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                pictureBox1.Image = image;
                pictureBox1.Refresh();
            }
            progressBar1.Value = 0;
        }

        private void button_cancel_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
        }

        private void чернобелоеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new GrayScaleFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void бинаризацияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new Binarization();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void медианныйToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new GaussianFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void медианныйФильтрToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new MedianFilter(5);
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void фильтрСобеляToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new SobelFilter();
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void крест3x3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new OpeningFilter(MaskType.Cross3);
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void квадрат3x3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new OpeningFilter(MaskType.Square3);
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void квадрат3x3ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Filters filter = new OpeningFilter(MaskType.Square5);
            backgroundWorker1.RunWorkerAsync(filter);
        }

        private void motionBlurToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Filters filter = new MotionBlurFilter(9);
            backgroundWorker1.RunWorkerAsync(filter);
        }

    }
}
