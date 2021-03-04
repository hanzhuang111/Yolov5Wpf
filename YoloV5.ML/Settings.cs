using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YoloV5.ML
{
    public class Settings
    {
        public const int INPUT_IMAGE_WIDTH = 640;
        public const int INPUT_IMAGE_HEIGHT = 640;
        public const int VECTOR_LENGTH = 85;

        public const string ONNX_MODEL_INPUT_COLUMN = "images";
        public const string ONNX_MODEL_OUTPUT80_COLUMN = "output80";
        public const string ONNX_MODEL_OUTPUT40_COLUMN = "output40";
        public const string ONNX_MODEL_OUTPUT20_COLUMN = "output20";
        public const string ONNX_MODEL_OUTPUT = "output";
        public const string PARAMS_FILE_PATH = "Assets\\Parameters.json";


        public static Dictionary<string, ((int width, int height)[] anchors, int stride, int grids)> AnchorsDict = new Dictionary<string, ((int width, int height)[] anchor, int stride, int grids)>
        {
            [ONNX_MODEL_OUTPUT80_COLUMN] = (new[] { (10, 13), (16, 30), (33, 23) }, 8, 80),
            [ONNX_MODEL_OUTPUT40_COLUMN] = (new[] { (30, 61), (62, 45), (59, 119) }, 16, 40),
            [ONNX_MODEL_OUTPUT20_COLUMN] = (new[] { (116, 90), (156, 198), (373, 326) }, 32, 20)
        };
        public static Parameters Params { get; set; }
    }
}
