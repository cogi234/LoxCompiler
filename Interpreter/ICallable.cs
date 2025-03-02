using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter
{
    internal interface ICallable
    {
        int Arity();
        object? Call(Interpreter interpreter, List<object?> arguments);
    }
}
