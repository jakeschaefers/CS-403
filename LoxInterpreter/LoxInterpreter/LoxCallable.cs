using System;
using System.Collections.Generic;

namespace LoxInterpreter
{
    public interface LoxCallable
    {
        public int Arity();
        public object Call(Interpreter interpreter, List<object> arguments);
    }
}