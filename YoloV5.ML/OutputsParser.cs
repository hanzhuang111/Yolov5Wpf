using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using static YoloV5.ML.Settings;

namespace YoloV5.ML
{
    public class OutputParser
    {
        private static Color[] colors;
        private readonly PreProcessing preProcessing;

        public readonly IEnumerable<ITreeViewItemInfo> boxesInfo;        // 绘图的结果。
        public readonly IEnumerable<IBoxInfo> yoloV5Boxes;      // 所有符合要求的结果。

        public OutputParser(ModelOutput modelOutput, PreProcessing preProcessing)
        {
            this.preProcessing = preProcessing;

            var t1 = FilterAsync(modelOutput.Output20, ONNX_MODEL_OUTPUT20_COLUMN);
            t1.Start();
            var t2 = FilterAsync(modelOutput.Output40, ONNX_MODEL_OUTPUT40_COLUMN);
            t2.Start();
            var t3 = FilterAsync(modelOutput.Output80, ONNX_MODEL_OUTPUT80_COLUMN);
            t3.Start();
            Task.WaitAll(t1, t2, t3);

            var allVectors = t1.Result.Concat(t2.Result)
                .Concat(t3.Result);
            yoloV5Boxes = NMS(allVectors);
            boxesInfo = Functions.Draw(preProcessing, yoloV5Boxes);
        }

        private static Task<IEnumerable<OutputVector>> FilterAsync(float[] output, string key)
        {
            return new Task<IEnumerable<OutputVector>>(() =>
            {
                var result = new List<OutputVector>();
                var (anchors, stride, grids) = AnchorsDict[key];
                for (int channel = 0; channel < anchors.Length; channel++)
                {
                    for (int row = 0; row < grids; row++)
                    {
                        for (int col = 0; col < grids; col++)
                        {
                            var start = (grids * grids * channel + grids * row + col) * VECTOR_LENGTH;
                            var objectConfidence = Functions.Sigmoid(output[start + 4]);
                            if (objectConfidence < Params.ConfidenceThreshold)
                                continue;

                            var span = new ReadOnlySpan<float>(output, start, VECTOR_LENGTH);
                            var scores = span.Slice(5, VECTOR_LENGTH - 5).ToArray()
                                .Select(c => Functions.Sigmoid(c));
                            var vector = new OutputVector
                            {
                                ObjectConfidence = objectConfidence,
                                X = (Functions.Sigmoid(span[0]) * 2 - 0.5f + col) * stride,
                                Y = (Functions.Sigmoid(span[1]) * 2 - 0.5f + row) * stride,
                                W = MathF.Pow(Functions.Sigmoid(span[2]) * 2, 2) * anchors[channel].width,
                                H = MathF.Pow(Functions.Sigmoid(span[3]) * 2, 2) * anchors[channel].height,
                                ClassScores = scores.ToArray()
                            };

                            if (vector.ClassMaxScore >= Params.ConfidenceThreshold)
                            {
                                result.Add(vector);
                            }
                        }
                    }
                }
                return result;
            });
        }

        /// <summary>
        /// NMS：优先取出得分最高的预测框，筛选掉IOU大于阈值的框。
        /// </summary>
        /// <param name="vectors">经过ObjectConfidence, ClassMaxScore两种预测值与ConfidenceThreshold的比较，筛选。</param>
        /// <returns>返回筛选过后的框。</returns>
        private IEnumerable<IBoxInfo> NMS(IEnumerable<OutputVector> vectors)
        {
            vectors = vectors.OrderByDescending(v => v.ClassMaxScore);
            var results = new List<IBoxInfo>();
            var isActiveBoxes = new BitArray(vectors.Count(), true);

            for (int i = 0; i < vectors.Count(); i++)
            {
                var v1 = vectors.ElementAt(i);
                if (isActiveBoxes[i])
                {
                    results.Add(new YoloBox
                    {
                        Position = MappingPosition(v1.Position),
                        Title = Params.Names[v1.ObjectId],
                        Color = GetColor(v1.ObjectId),
                        Score = v1.ClassMaxScore,
                    });
                    if (results.Count >= Params.BoxCountLimit)
                        break;
                }
                for (var j = i + 1; j < isActiveBoxes.Length; j++)
                {
                    if (isActiveBoxes[j])
                    {
                        var v2 = vectors.ElementAt(j);
                        if (Functions.IoU(v1, v2) >= Params.IouThreshold && v1.ObjectId == v2.ObjectId)
                        {
                            isActiveBoxes[j] = false;
                        }
                    }
                }
            }
            return results;
        }

        /// <summary>
        /// 将640*640图片中的预测框，映射到原图中去。
        /// </summary>
        /// <param name="rect">640*640图片中的预测框。</param>
        /// <returns>原图的框。</returns>
        private Rectangle MappingPosition(Rectangle rect)
        {
            var x1 = (rect.Left - preProcessing.leftPadding) / preProcessing.scaleRatio;
            var y1 = (rect.Top - preProcessing.topPadding) / preProcessing.scaleRatio;
            var width = rect.Width / preProcessing.scaleRatio;
            var height = rect.Height / preProcessing.scaleRatio;
            var newRect = new Rectangle((int)x1, (int)y1, (int)width, (int)height);
            return newRect;
        }

        /// <summary>
        /// 使用随机的方式，生成80种颜色。概率上讲，相同的颜色几乎不可能。
        /// </summary>
        /// <param name="index">序号，取值0~80.</param>
        /// <returns>返回一种颜色。</returns>
        private static Color GetColor(int index)
        {
            if (colors is null)
            {
                colors = new Color[VECTOR_LENGTH - 5];
                var random = new Random();
                for (int i = 0; i < colors.Length; i++)
                {
                    var r = random.Next(0, 256);
                    var g = random.Next(0, 256);
                    var b = random.Next(0, 256);
                    colors[i] = Color.FromArgb((byte)r, (byte)g, (byte)b);
                }
            }
            return colors[index];
        }
    }
}
