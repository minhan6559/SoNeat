using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoNeat.src.UI
{
    // Interface for sub screen states
    public interface ISubScreenState
    {
        void Update();
        void Draw();
    }
}