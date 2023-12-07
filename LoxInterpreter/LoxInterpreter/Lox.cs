using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace LoxInterpreter
{
    public class Lox
    {
        private static readonly Interpreter interpreter = new Interpreter();

        private static bool hadError = false;
        private static bool hadRuntimeError = false;

        static void Main(string[] args)
        {
            try
            {
                if (args.Length > 1)
                {
                    Console.WriteLine("Usage: jlox [script]");
                    System.Environment.Exit(64);
                }
                else if (args.Length == 1)
                {
                    RunFile(args[0]);
                }
                else
                {
                    RunPrompt();
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private static void RunFile(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);
            Run(Encoding.Default.GetString(bytes));

            if (hadError) System.Environment.Exit(65);
            if (hadRuntimeError) System.Environment.Exit(70);

        }

        private static void RunPrompt()
        {
            Console.WriteLine("Enter commands (CTRL+C to exit):");
            while (true)
            {
                Console.Write("> ");
                string line = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(line)) continue;

                Run(line);
                hadError = false;
            }
        }

        private static void Run(string source)
        {
            Scanner scanner = new Scanner(source);
            List<Token> tokens = scanner.ScanTokens();
            Parser parser = new Parser(tokens);
            List<Stmt> statements = parser.Parse();

            // Stop if there was a syntax error.
            if (hadError) return;

            Resolver resolver = new Resolver(interpreter);
            resolver.Resolve(statements);

            // Stop if there was a resolution error.
            if (hadError) return;

            interpreter.Interpret(statements);
        }

        public static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        private static void Report(int line, string where, string message)
        {
            Console.Error.WriteLine($"[line {line}] Error{where}: {message}");
            hadError = true;
        }

        public static void Error(Token token, string message)
        {
            if (token.Type == TokenType.EOF)
            {
                Report(token.Line, " at end", message);
            }
            else
            {
                Report(token.Line, " at '" + token.Lexeme + "'", message);
            }
        }

        public static void RuntimeError(RuntimeError error)
        {
            Console.Error.WriteLine($"{error.Message}\n[line {error.Token.Line}]");
            hadRuntimeError = true;
        }
    }
}