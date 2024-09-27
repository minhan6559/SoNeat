using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoNeat.src.UI
{
    public interface IScreenState
    {
        void EnterState();
        void Update();
        void Draw();
        void ExitState();
    }
}