using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoNeat.src.UI
{
    public interface ISubScreenState
    {
        void Update();
        void Draw();
    }
}