using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Calculator.Models
{
    public class CalculatorModel
    {
        public double Evaluate(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
                return 0;

            var tokens = Tokenize(expression);
            var rpn = ToRpn(tokens);
            return EvaluateRpn(rpn);
        }

        private enum TokenType { Number, Operator }

        private class Token
        {
            public TokenType Type { get; }
            public string Text { get; }
            public Token(TokenType t, string text) { Type = t; Text = text; }
            public override string ToString() => Text;
        }

        private static List<Token> Tokenize(string s)
        {
            var res = new List<Token>();
            int i = 0;
            while (i < s.Length)
            {
                if (char.IsWhiteSpace(s[i])) { i++; continue; }

                char c = s[i];
                if (c == '+' || c == '*' || c == '/')
                {
                    res.Add(new Token(TokenType.Operator, c.ToString()));
                    i++;
                    continue;
                }

                if (c == '-')
                {
                    // унарный минус если в начале или после оператора
                    bool isUnary = res.Count == 0 || res[res.Count - 1].Type == TokenType.Operator;
                    if (isUnary)
                    {
                        // прочитать число со знаком
                        var sb = new StringBuilder("-");
                        i++;
                        // читать цифры и точку
                        while (i < s.Length && (char.IsDigit(s[i]) || s[i] == '.'))
                        {
                            sb.Append(s[i]); i++;
                        }
                        res.Add(new Token(TokenType.Number, sb.ToString()));
                        continue;
                    }
                    else
                    {
                        res.Add(new Token(TokenType.Operator, "-"));
                        i++;
                        continue;
                    }
                }

                // число (включая дробную часть)
                if (char.IsDigit(c) || c == '.')
                {
                    var sb = new StringBuilder();
                    while (i < s.Length && (char.IsDigit(s[i]) || s[i] == '.'))
                    {
                        sb.Append(s[i]); i++;
                    }
                    res.Add(new Token(TokenType.Number, sb.ToString()));
                    continue;
                }

                // недопустимый символ
                throw new FormatException($"Unexpected character: {c}");
            }

            return res;
        }

        private static int Precedence(string op)
        {
            if (op == "+" || op == "-") return 1;
            if (op == "*" || op == "/") return 2;
            return 0;
        }

        private static List<Token> ToRpn(List<Token> tokens)
        {
            var output = new List<Token>();
            var ops = new Stack<Token>();

            foreach (var tok in tokens)
            {
                if (tok.Type == TokenType.Number)
                {
                    output.Add(tok);
                }
                else // operator
                {
                    while (ops.Count > 0 && Precedence(ops.Peek().Text) >= Precedence(tok.Text))
                    {
                        output.Add(ops.Pop());
                    }
                    ops.Push(tok);
                }
            }

            while (ops.Count > 0) output.Add(ops.Pop());
            return output;
        }


        private static double EvaluateRpn(List<Token> rpn)
        {
            var st = new Stack<double>();
            foreach (var tok in rpn)
            {
                if (tok.Type == TokenType.Number)
                {
                    if (!double.TryParse(tok.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double val))
                        throw new FormatException($"Invalid number: {tok.Text}");
                    st.Push(val);
                }
                else
                {
                    if (st.Count < 2) throw new FormatException("Invalid expression");
                    var b = st.Pop();
                    var a = st.Pop();
                    double res;
                    switch (tok.Text)
                    {
                        case "+": res = a + b; break;
                        case "-": res = a - b; break;
                        case "*": res = a * b; break;
                        case "/":
                            if (b == 0) throw new DivideByZeroException();
                            res = a / b; break;
                        default:
                            throw new InvalidOperationException($"Unknown operator {tok.Text}");
                    }
                    st.Push(res);
                }
            }

            if (st.Count != 1) throw new FormatException("Invalid expression");
            return st.Pop();
        }
    }
}
