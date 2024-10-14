using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoNeat.src.NEAT
{
    // Define an interface for cloneable objects
    public interface IPrototype<T>
    {
        T Clone();
    }
}