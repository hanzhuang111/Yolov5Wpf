using Microsoft.ML.Data;
using Microsoft.ML.Transforms.Image;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using static YoloV5.ML.Settings;
using static YoloV5.ML.Functions;
using System.Threading.Tasks;

namespace YoloV5.ML
{
    /// <summary>
    /// YOLOV5 ONNX模型的输入。
    /// </summary>
    public class ModelInput
    {
        [ImageType(INPUT_IMAGE_WIDTH, INPUT_IMAGE_HEIGHT)]
        public Bitmap Image;
    }

    /// <summary>
    /// YOLOV5 ONNX模型的输出。
    /// </summary>
    public class ModelOutput
    {
        [ColumnName(ONNX_MODEL_OUTPUT20_COLUMN)]
        public float[] Output20;

        [ColumnName(ONNX_MODEL_OUTPUT40_COLUMN)]
        public float[] Output40;

        [ColumnName(ONNX_MODEL_OUTPUT80_COLUMN)]
        public float[] Output80;
    }

    /// <summary>
    /// 一个预测框所对应的处理后的信息。
    /// </summary>
    public class YoloBox : IBoxInfo
    {
        public string Title { get; init; }
        public Color Color { get; init; }
        public Rectangle Position { get; init; }
        public float Score { get; init; }

        public override string ToString()
        {
            return $"{Position.Left},{Position.Top},{Position.Right},{Position.Bottom} {Score:f2}";
        }
    }

    public class TreeViewInfo : ITreeViewItemInfo
    {
        public Color Color { get; init; }
        public string Title { get; init; }
        public int Count { get; set; }
        public List<IBoxInfo> Boxes { get; init; }

        public override string ToString()
        {
            return $"{Title}  {Count}个";
        }
    }

}
