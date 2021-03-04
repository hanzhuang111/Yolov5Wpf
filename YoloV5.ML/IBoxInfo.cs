using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace YoloV5.ML
{
    public interface IBoxInfo
    {
        int Count { get; set; }
        Color Color { get; init; }
        string Title { get; init; }
        int ObjectID { get; init; }
        Rectangle Position { get; init; }
        float Score { get; init; }
        string Description { get; }
    }
}
