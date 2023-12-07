using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace LoxInterpreter
{
    public class Token
    {
        public TokenType Type { get; }
        public string lexeme { get; }
        public object Literal { get; }
        public int Line { get; }

        public Token(TokenType type, string lexeme, object literal, int line)
        {
            Type = type;
            this.lexeme = lexeme;
            Literal = literal;
            Line = line;
        }

        public override string ToString()
        {
            return Type + " " + lexeme + " " + Literal;
        }
    }

}