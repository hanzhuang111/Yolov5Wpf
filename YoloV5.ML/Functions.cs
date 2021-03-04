using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoloV5.ML
{
    public class Functions
    {
        public static float Sigmoid(float x)
        {
            return MathF.Exp(x) / (1 + MathF.Exp(x));
        }

        public static float[] Softmax(float[] values)
        {
            var maxVal = values.Max();
            var exp = values.Select(v => MathF.Exp(v - maxVal));
            var sumExp = exp.Sum();

            return exp.Select(e => e / sumExp).ToArray();
        }

        public static float IoU(OutputVector v1, OutputVector v2)
        {
            float x1 = Math.Max(v1.X, v2.X);
            float y1 = Math.Max(v1.Y, v2.Y);
            float x2 = Math.Min(v1.X + v1.W, v2.X + v2.W);
            float y2 = Math.Min(v1.Y + v1.H, v2.Y + v2.H);
            float area = Math.Max(0, (x2 - x1) * (y2 - y1));
            float iou = area / (v1.W * v1.H + v2.W * v2.H - area);

            return iou;
        }

        public static IEnumerable<IBoxInfo> Draw(PreProcessing preProcessing, IEnumerable<IBoxInfo> yoloBoxes, string title = null)
        {
            preProcessing.outputBitmap = (Bitmap)preProcessing.originalBitmap.Clone();
            using var g = Graphics.FromImage(preProcessing.outputBitmap);

            var result = new List<IBoxInfo>();
            foreach(var box in yoloBoxes)
            {
                if(!string.IsNullOrEmpty(title))
                {
                    if(box.Title != title)
                    {
                        continue;
                    }
                }
                if (result.Find(b => b.Title == box.Title) is YoloBox yoloBox)
                {
                    yoloBox.Count++;
                }
                else
                {
                    result.Add(box);
                }
                var pen = new Pen(box.Color, (float)Settings.Params.PenWidth);
                g.DrawRectangle(pen, box.Position);
            }

            return result;
        }
    }
}
