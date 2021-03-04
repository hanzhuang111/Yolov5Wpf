using System.Drawing;
using System.Drawing.Imaging;

namespace YoloV5.ML
{
    public class PreProcessing
    {
        private static readonly Color paddingColor = Color.Black;
        private readonly int width;
        private readonly int height;

        public int topPadding;
        public int leftPadding;
        public float scaleRatio;
        public Bitmap originalBitmap;
        public Bitmap outputBitmap;

        public ModelInput ModelInput { get; }

        public PreProcessing(string imagePath)
        {
            originalBitmap = new Bitmap(imagePath);
            (width, height) = (originalBitmap.Width, originalBitmap.Height);
            originalBitmap = originalBitmap.Clone(new Rectangle(0, 0, width, height), PixelFormat.Format24bppRgb);

            ModelInput = new ModelInput
            {
                Image = ScalePadding()
            };
        }

        private Bitmap ScalePadding()
        {
            Bitmap dstBitmap, scaledBitmap;
            if (width == height)
            {
                scaleRatio = 640f / width;
                topPadding = leftPadding = 0;
                dstBitmap = new Bitmap(originalBitmap, 640, 640);
            }
            else
            {
                if (width > height)
                {
                    scaleRatio = 640f / width;
                    var newHeight = (int)(scaleRatio * height);
                    scaledBitmap = new Bitmap(originalBitmap, 640, newHeight);
                    leftPadding = 0;
                    topPadding = (640 - newHeight) / 2;
                }
                else
                {
                    scaleRatio = 640f / height;
                    var newWidth = (int)(scaleRatio * width);
                    scaledBitmap = new Bitmap(originalBitmap, newWidth, 640);
                    topPadding = 0;
                    leftPadding = (640 - newWidth) / 2;
                }

                var brush = new SolidBrush(paddingColor);
                dstBitmap = new Bitmap(640, 640);
                using var g = Graphics.FromImage(dstBitmap);
                g.FillRectangle(brush, 0, 0, dstBitmap.Width, dstBitmap.Height);
                g.DrawImage(scaledBitmap, leftPadding, topPadding);
            }
            return dstBitmap;
        }
    }
}
