// using System;
// using System.Collections.Generic;
// using System.Diagnostics;
// using System.IO;
// using System.Text;

// namespace LoxInterpreter
// {
//     public class AstPrinter : Expr.Visitor<string>
//     {
//         public string VisitBinaryExpr(Expr.Binary expr)
//         {
//             return Parenthesize(expr.Operator.Lexeme, expr.Left, expr.Right);
//         }

//         public string VisitGroupingExpr(Expr.Grouping expr)
//         {
//             return Parenthesize("group", expr.Expression);
//         }

//         public string VisitLiteralExpr(Expr.Literal expr)
//         {
//             if (expr.Value == null) return "nil";
//             return expr.Value.ToString();
//         }

//         public string VisitUnaryExpr(Expr.Unary expr)
//         {
//             return Parenthesize(expr.Operator.Lexeme, expr.Right);
//         }

//         private string Parenthesize(string name, params Expr[] exprs)
//         {
//             StringBuilder builder = new StringBuilder();

//             builder.Append("(").Append(name);
//             foreach (var expr in exprs)
//             {
//                 builder.Append(" ");
//                 builder.Append(expr.Accept(this));
//             }
//             builder.Append(")");

//             return builder.ToString();
//         }

//         // Use this method to print the AST for an expression
//         public string Print(Expr expr)
//         {
//             return expr.Accept(this);
//         }
//     }
// }