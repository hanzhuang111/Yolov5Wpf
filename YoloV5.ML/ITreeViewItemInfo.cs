using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoloV5.ML
{
    public interface ITreeViewItemInfo
    {
        Color Color { get; init; }
        string Title { get; init; }
        int Count { get; set; }
        List<IBoxInfo> Boxes { get; init; }
    }

    public interface IBoxInfo
    {
        Color Color { get; init; }
        string Title { get; init; }
        Rectangle Position { get; init; }
        float Score { get; init; }
    }
}
