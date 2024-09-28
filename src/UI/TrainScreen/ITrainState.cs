using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoNeat.src.UI.TrainScreen
{
    public interface ITrainState
    {
        void Update();
        void Draw();
    }
}