using System;
using System.Collections.Generic;

namespace CSharp
{
    public interface LoxCallable
    {
        public int Arity();
        public object Call(Interpreter interpreter, List<object> arguments);
    }
}