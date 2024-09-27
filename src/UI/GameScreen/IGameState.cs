using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoNeat.src.UI.GameScreen
{
    public interface IGameState
    {
        void Update();
        void Draw();
    }
}