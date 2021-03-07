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

        public static void Draw(PreProcessing preProcessing, IEnumerable<IBoxInfo> yoloBoxes, string title)
        {
            preProcessing.outputBitmap = (Bitmap)preProcessing.originalBitmap.Clone();
            using var g = Graphics.FromImage(preProcessing.outputBitmap);
            var boxes = yoloBoxes.Where(b => b.Title == title);
            foreach(var box in boxes)
            {
                var pen = new Pen(box.Color, (float)Settings.Params.PenWidth);
                g.DrawRectangle(pen, box.Position);
            }
        }

        public static void Draw(PreProcessing preProcessing, IEnumerable<IBoxInfo> yoloBoxes, IBoxInfo selected)
        {
            if(selected is not null)
            {
                preProcessing.outputBitmap = (Bitmap)preProcessing.originalBitmap.Clone();
                using var g = Graphics.FromImage(preProcessing.outputBitmap);
                var boxes = yoloBoxes.Where(b => b.Position == selected.Position && b.Title == selected.Title);
                foreach (var box in boxes)
                {
                    var pen = new Pen(box.Color, (float)Settings.Params.PenWidth);
                    g.DrawRectangle(pen, box.Position);
                }
            }
        }
        public static IEnumerable<ITreeViewItemInfo> Draw(PreProcessing preProcessing, IEnumerable<IBoxInfo> yoloBoxes)
        {
            preProcessing.outputBitmap = (Bitmap)preProcessing.originalBitmap.Clone();
            using var g = Graphics.FromImage(preProcessing.outputBitmap);

            var result = new List<ITreeViewItemInfo>();
            foreach(var box in yoloBoxes)
            {
                if (result.Find(b => b.Title == box.Title) is ITreeViewItemInfo treeViewItemInfo)
                {
                    treeViewItemInfo.Count++;
                    treeViewItemInfo.Boxes.Add(box);
                }
                else
                {
                    var info = new TreeViewInfo
                    {
                        Boxes = new List<IBoxInfo>(),
                        Count = 1,
                        Title = box.Title,
                        Color = box.Color
                    };
                    info.Boxes.Add(box);
                    result.Add(info);
                }
                var pen = new Pen(box.Color, (float)Settings.Params.PenWidth);
                g.DrawRectangle(pen, box.Position);
            }
            var comparer = new Comparison<IBoxInfo>((x, y) =>
                x.Position.X.CompareTo(y.Position.X));
            foreach(var r in result)
            {
                r.Boxes.Sort(comparer);
            }

            return result;
        }
    }
}
