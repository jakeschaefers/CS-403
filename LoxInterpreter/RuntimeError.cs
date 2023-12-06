using System;
using System.Collections.Generic;

namespace LoxInterpreter
{
    class RuntimeError : Exception
    {
        public Token Token { get; }

        public RuntimeError(Token token, string message) : base(message)
        {
            Token = token;
        }
    }
}