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
    public class ModelInput
    {
        [ImageType(INPUT_IMAGE_WIDTH, INPUT_IMAGE_HEIGHT)]
        public Bitmap Image;
    }

    public class ModelOutput
    {
        [ColumnName(ONNX_MODEL_OUTPUT20_COLUMN)]
        public float[] Output20;

        [ColumnName(ONNX_MODEL_OUTPUT40_COLUMN)]
        public float[] Output40;

        [ColumnName(ONNX_MODEL_OUTPUT80_COLUMN)]
        public float[] Output80;
    }

    public class YoloBox : IBoxInfo
    {
        public string Title { get; init; }
        public Color Color { get; init; }
        public int Count { get; set; }
        public int ObjectID { get; init; }
        public Rectangle Position { get; init; }
        public float Score { get; init; }
        public string Description => $"{Title}  {Count}个";
    }
}
