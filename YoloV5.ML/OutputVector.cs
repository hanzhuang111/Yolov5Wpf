using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoloV5.ML
{
    public class OutputVector
    {
        public float X { get; init; }
        public float Y { get; init; }
        public float W { get; init; }
        public float H { get; init; }
        public float ObjectConfidence { get; init; }
        public float[] ClassScores { get; init; }

        private float _classMaxScore = 0f;
        public float ClassMaxScore
        {
            get
            {
                if (_classMaxScore == 0f)
                {
                    (_, _classMaxScore) = GetTopResult();
                }
                return _classMaxScore;
            }
        }

        private int _objectId = 0;
        public int ObjectId
        {
            get
            {
                if (_objectId == 0)
                {
                    (_objectId, _) = GetTopResult();
                }
                return _objectId;
            }
        }

        public RectangleF PositionF =>
            new RectangleF(X - W / 2f, Y - H / 2f, W, H);

        public Rectangle Position =>
            new Rectangle((int)(X - W / 2f), (int)(Y - H / 2f), (int)W, (int)H);


        private (int classId, float classMaxScore) GetTopResult()
        {
            return ClassScores
                .Select((predictedClass, index) => (Index: index, Value: predictedClass * ObjectConfidence))
                .OrderByDescending(result => result.Value)
                .First();
        }
    }
}
