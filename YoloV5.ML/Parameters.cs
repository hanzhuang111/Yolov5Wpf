using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace YoloV5.ML
{
    public class Parameters
    {
        public string ModelFile { get; set; }
        public double PenWidth { get; set; }
        public double ConfidenceThreshold { get; set; }
        public double IouThreshold { get; set; }
        public int BoxCountLimit { get; set; }
        public string[] Names { get; set; }

        public static Task<Parameters> FromJsonAsync(string path)
        {
            var t1 = Task.Factory.StartNew(() =>
            {
                var jsonStr = File.ReadAllText(path, Encoding.UTF8);
                var parameter = JsonSerializer.Deserialize<Parameters>(jsonStr);
                return parameter;
            }, TaskCreationOptions.LongRunning);
            return t1;
        }

        public static Parameters FromJson(string path)
        {
            var jsonStr = File.ReadAllText(path, Encoding.UTF8);
            var parameter = JsonSerializer.Deserialize<Parameters>(jsonStr);
            return parameter;
        }
    }
}
