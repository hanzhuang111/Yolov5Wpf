using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms.Image;
using Microsoft.ML.Transforms.Onnx;
using static YoloV5.ML.Settings;

namespace YoloV5.ML
{
    public class YoloV5Onnx
    {
        private readonly MLContext mLContext;
        private PredictionEngine<ModelInput, ModelOutput> engine;
        public YoloV5Onnx(MLContext mLContext)
        {
            this.mLContext = mLContext;
            LoadModel();
        }

        private void LoadModel()
        {
            var data = mLContext.Data.LoadFromEnumerable(new List<ModelInput>());
            var transformer = mLContext.Transforms.ExtractPixels(ONNX_MODEL_INPUT_COLUMN, nameof(ModelInput.Image), ImagePixelExtractingEstimator.ColorBits.Rgb, ImagePixelExtractingEstimator.ColorsOrder.ARGB, false, 0, 1.0f / 255.0f)
                .Append(mLContext.Transforms.ApplyOnnxModel(outputColumnNames: new[] { ONNX_MODEL_OUTPUT20_COLUMN, ONNX_MODEL_OUTPUT40_COLUMN, ONNX_MODEL_OUTPUT80_COLUMN }, inputColumnNames: new[] { ONNX_MODEL_INPUT_COLUMN }, modelFile: Params.ModelFile));
            var model = transformer.Fit(data);
            engine = mLContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(model, false);
        }

        public ModelOutput Predict(ModelInput input)
        {
            return engine.Predict(input);
        }
    }
}
