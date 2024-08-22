using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NEATRex.src.ScreenManager
{
    public abstract class Screen
    {
        public string Name { get; private set; }

        public Screen(string name)
        {
            Name = name;
        }

        public abstract void Update();
        public abstract void Draw();
    }
}