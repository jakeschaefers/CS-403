using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace LoxInterpreter
{
    class Lox
    {
        private static bool hadError = false;

        static void Main(string[] args)
        {

            // Expr expression = new Expr.Binary(
            //     new Expr.Unary(
            //         new Token(TokenType.MINUS, "-", null, 1),
            //         new Expr.Literal(123)),
            //     new Token(TokenType.STAR, "*", null, 1),
            //     new Expr.Grouping(
            //         new Expr.Literal(45.67)));

            // AstPrinter printer = new AstPrinter();
            // Console.WriteLine(printer.Print(expression));

            try
            {
                if (args.Length > 1)
                {
                    Console.WriteLine("Usage: jlox [script]");
                    Environment.Exit(64);
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

            if (hadError) Environment.Exit(65);
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
            Expr expression = parser.Parse();

            // Stop if there was a syntax error.
            if (hadError) return;

            Console.WriteLine(new AstPrinter().Print(expression));
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
    }

}