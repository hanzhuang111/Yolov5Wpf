using Microsoft.ML;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using YoloV5.ML;
using System.Linq;

namespace YoloV5.View.ViewModels
{
    using static YoloV5.ML.Settings;
    public class MainWindowViewModel : BindableBase
    {
        private readonly MLContext mLContext;
        private YoloV5Onnx yoloV5Onnx;
        private PreProcessing currentPreProcessing;
        private OutputParser currentParser;
        public MainWindowViewModel()
        {
            SetParamsAsync();
            mLContext = new MLContext();
            LoadModelCommand = new DelegateCommand(LoadModelAsync);
            ParametersSettingCommand = new DelegateCommand(ParametersSetting);
            SaveOutputCommand = new DelegateCommand(SaveOutput);
            LoadImageCommand = new DelegateCommand(LoadImage);
            ModelPredictCommand = new DelegateCommand(ModelPredict);
            DrawSelectionCommand = new DelegateCommand(DrawSelectiion);
        }

        private void DrawSelectiion()
        {
            if(currentParser.yoloV5Boxes.Count() > 0)
            {
                Functions.Draw(currentPreProcessing, currentParser.yoloV5Boxes, DrawItemTitle);
                OutputSource = currentPreProcessing.outputBitmap.ToBitmapSource();
            }
        }

        public void SelectedItemChanged(IBoxInfo item)
        {
            Functions.Draw(currentPreProcessing, currentParser.yoloV5Boxes, item);
            OutputSource = currentPreProcessing.outputBitmap.ToBitmapSource();
        }

        private void ModelPredict()
        {
            if (CheckImagePath())
            {
                var output = yoloV5Onnx?.Predict(currentPreProcessing.ModelInput);

                currentParser = new OutputParser(output, currentPreProcessing);
                OutputSource = currentPreProcessing.outputBitmap.ToBitmapSource();
                ViewItems.Clear();
                foreach (var info in currentParser.boxesInfo)
                {
                    ViewItems.Add(info);
                }
            }
        }

        private void LoadImage()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "images(*.png;*.jpg;*.bmp)|*.png;*.bmp;*jpg",
                Title = "打开图片"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                ImagePath = openFileDialog.FileName;
                currentPreProcessing = new PreProcessing(ImagePath);
                InputSource = currentPreProcessing.originalBitmap.ToBitmapSource();
            }
        }

        private void SaveOutput()
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "images(*.bmp)|*.bmp",
                Title = "保存图片"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                currentPreProcessing.outputBitmap?.Save(saveFileDialog.FileName);
            }
        }

        private void ParametersSetting()
        {
            var process = Process.Start("notepad", PARAMS_FILE_PATH);
            process.WaitForExit();
            Thread.Sleep(100);
            SetParamsAsync();
        }

        private async void LoadModelAsync()
        {
            var t1 = Task.Factory.StartNew(() =>
            {
                try
                {
                    LoadModelButtonAvailable = false;
                    LoadModelButtonState = "加载中...";
                    yoloV5Onnx = new YoloV5Onnx(mLContext);
                    LoadingModelResultColor = Brushes.Green;
                    LoadingModelResult = $"{Params.ModelFile}，加载成功!";
                    LoadModelButtonState = "加载完成";
                }
                catch (Exception exception)
                {
                    MessageBox.Show($"错误信息：{exception.Message}", "加载模型出错", MessageBoxButton.OK, MessageBoxImage.Error);
                    LoadModelButtonAvailable = false;
                    LoadingModelResultColor = Brushes.Red;
                    LoadingModelResult = "模型,加载失败!";
                    LoadModelButtonState = "加载失败";
                }
            }, TaskCreationOptions.LongRunning);
            await t1;
        }

        private bool CheckImagePath()
        {
            if (!File.Exists(ImagePath))
            {
                MessageBox.Show(string.IsNullOrEmpty(ImagePath) ? "路径为空！"
                    : $"图片：{ImagePath}不存在！", "错误");
                return false;
            }
            return true;
        }

        private static async void SetParamsAsync()
        {
            try
            {
                var message = "";
                var caption = "文件Parameters.json内容存在错误";
                var task = Parameters.FromJsonAsync(PARAMS_FILE_PATH);
                await task;

                var parameters = task.Result;

                if (!File.Exists(parameters.ModelFile))
                {
                    message = $"模型路径: {parameters.ModelFile}不存在！";
                }
                else if (parameters.ConfidenceThreshold > 1.0 || parameters.ConfidenceThreshold < 0)
                {
                    message = $"置信度 = {parameters.ConfidenceThreshold}，取值不在0.0~1.0之间.";
                }
                else if (parameters.IouThreshold > 1.0 || parameters.IouThreshold < 0.0)
                {
                    message = $"Iou阈值 = {parameters.IouThreshold}，取值不在0.0~1.0之间.";
                }
                else if (parameters.Names.Length != 80)
                {
                    message = "分类列表存在问题.";
                }
                else if (parameters.BoxCountLimit <= 0)
                {
                    message = $"最大Box数量限制：{parameters.BoxCountLimit} <= 0";
                }

                if (string.IsNullOrEmpty(message))
                {
                    Params = parameters;
                }
                else
                {
                    MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show($"出现异常: {exception.Message}", "异常错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public DelegateCommand LoadModelCommand { get; }
        public DelegateCommand ParametersSettingCommand { get; }
        public DelegateCommand SaveOutputCommand { get; }
        public DelegateCommand LoadImageCommand { get; }
        public DelegateCommand ModelPredictCommand { get; }
        public DelegateCommand DrawSelectionCommand { get; }    



        private string _loadModelButtonState = "加载模型";
        public string LoadModelButtonState
        {
            get => _loadModelButtonState;
            set => SetProperty(ref _loadModelButtonState, value);
        }

        private string _imagePath;
        public string ImagePath
        {
            get => _imagePath;
            set => SetProperty(ref _imagePath, value);
        }

        private BitmapSource _inputSource;
        public BitmapSource InputSource
        {
            get => _inputSource;
            set => SetProperty(ref _inputSource, value);
        }

        private BitmapSource _outputSource;
        public BitmapSource OutputSource
        {
            get => _outputSource;
            set => SetProperty(ref _outputSource, value);
        }

        private bool _loadModelButtonAvailable = true;
        public bool LoadModelButtonAvailable
        {
            get => _loadModelButtonAvailable;
            set => SetProperty(ref _loadModelButtonAvailable, value);
        }

        private string _loadingModelResult;
        public string LoadingModelResult
        {
            get => _loadingModelResult;
            set => SetProperty(ref _loadingModelResult, value);
        }

        private Brush _loadingModelResultColor = Brushes.Black;
        public Brush LoadingModelResultColor
        {
            get => _loadingModelResultColor;
            set => SetProperty(ref _loadingModelResultColor, value);
        }

        private string _drawItemTitle;
        public string DrawItemTitle
        {
            get => _drawItemTitle;
            set => SetProperty(ref _drawItemTitle, value);
        }
        public ObservableCollection<ITreeViewItemInfo> ViewItems { get; } = new ObservableCollection<ITreeViewItemInfo>();
    }
}
