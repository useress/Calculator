using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Calculator.Models
{
    // Информация об операторе
    public class OperatorInfo
    {
        public string Symbol { get; }
        public int Precedence { get; }
        public bool IsRightAssociative { get; }
        public Func<double, double, double> BinaryFunc { get; }  // для бинарных
        public Func<double, double> UnaryFunc { get; }            // для унарных (опционально)
        public int Arity { get; }  // 1 – унарный, 2 – бинарный

        // Конструктор для бинарного оператора
        public OperatorInfo(string symbol, int precedence, Func<double, double, double> binaryFunc, bool isRightAssociative = false)
        {
            Symbol = symbol;
            Precedence = precedence;
            BinaryFunc = binaryFunc;
            Arity = 2;
            IsRightAssociative = isRightAssociative;
        }

        // Конструктор для унарного оператора (например, унарный минус, факториал)
        public OperatorInfo(string symbol, Func<double, double> unaryFunc)
        {
            Symbol = symbol;
            UnaryFunc = unaryFunc;
            Arity = 1;
            // Для унарных приоритет не важен, они обрабатываются отдельно
        }
    }

    // Информация о функции (sin, cos и т.д.)
    public class FunctionInfo
    {
        public string Name { get; }
        public int Arity { get; }
        public Func<double[], double> Func { get; }

        public FunctionInfo(string name, int arity, Func<double[], double> func)
        {
            Name = name;
            Arity = arity;
            Func = func;
        }
    }

    public class CalculatorModel
    {
        // Словари операторов и функций (protected для доступа в наследниках)
        protected Dictionary<string, OperatorInfo> Operators { get; set; }
        protected Dictionary<string, FunctionInfo> Functions { get; set; }

        // Константы: можно хранить как функции без аргументов
        protected Dictionary<string, double> Constants { get; set; }

        public CalculatorModel()
        {
            Operators = new Dictionary<string, OperatorInfo>();
            Functions = new Dictionary<string, FunctionInfo>();
            Constants = new Dictionary<string, double>();

            // Регистрируем стандартные бинарные операторы
            Operators["+"] = new OperatorInfo("+", 1, (a, b) => a + b);
            Operators["-"] = new OperatorInfo("-", 1, (a, b) => a - b);
            Operators["*"] = new OperatorInfo("*", 2, (a, b) => a * b);
            Operators["/"] = new OperatorInfo("/", 2, (a, b) => a / b);

            // Унарный минус будет обрабатываться отдельно в токенизаторе
        }

        // Основной метод вычисления
        public double Evaluate(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
                return 0;

            var tokens = Tokenize(expression);
            var rpn = ToRpn(tokens);
            return EvaluateRpn(rpn);
        }

        // Токенизация
        protected virtual List<Token> Tokenize(string s)
        {
            var result = new List<Token>();
            int i = 0;
            int length = s.Length;

            while (i < length)
            {
                char c = s[i];

                // Пропускаем пробелы
                if (char.IsWhiteSpace(c))
                {
                    i++;
                    continue;
                }

                // Открывающая скобка
                if (c == '(')
                {
                    result.Add(new Token(TokenType.LeftParenthesis, "("));
                    i++;
                    continue;
                }

                // Закрывающая скобка
                if (c == ')')
                {
                    result.Add(new Token(TokenType.RightParenthesis, ")"));
                    i++;
                    continue;
                }

                // Обработка чисел и унарного минуса
                if (char.IsDigit(c) || c == '.' || c == '-')
                {
                    // Унарный минус: если минус и предыдущий токен отсутствует или это оператор/открывающая скобка
                    if (c == '-' && (result.Count == 0 ||
                                     result[result.Count - 1].Type == TokenType.Operator ||
                                     result[result.Count - 1].Type == TokenType.LeftParenthesis))
                    {
                        // Читаем число с минусом
                        var sb = new StringBuilder("-");
                        i++;
                        while (i < length && (char.IsDigit(s[i]) || s[i] == '.'))
                        {
                            sb.Append(s[i]);
                            i++;
                        }
                        result.Add(new Token(TokenType.Number, sb.ToString()));
                        continue;
                    }
                    else if (char.IsDigit(c) || c == '.')
                    {
                        // Обычное число
                        var sb = new StringBuilder();
                        while (i < length && (char.IsDigit(s[i]) || s[i] == '.'))
                        {
                            sb.Append(s[i]);
                            i++;
                        }
                        result.Add(new Token(TokenType.Number, sb.ToString()));
                        continue;
                    }
                }

                // Функции (например, sin, cos, sqrt) — ищем максимальное совпадение
                if (char.IsLetter(c))
                {
                    var sb = new StringBuilder();
                    while (i < length && char.IsLetter(s[i]))
                    {
                        sb.Append(s[i]);
                        i++;
                    }
                    string funcName = sb.ToString();
                    // Проверяем, есть ли такая функция в словаре
                    if (Functions.ContainsKey(funcName))
                    {
                        result.Add(new Token(TokenType.Function, funcName));
                    }
                    // Также проверяем константы (pi, e)
                    else if (Constants.ContainsKey(funcName))
                    {
                        // Заменяем константу на число
                        result.Add(new Token(TokenType.Number, Constants[funcName].ToString(CultureInfo.InvariantCulture)));
                    }
                    else
                    {
                        throw new FormatException($"Unknown function/constant: {funcName}");
                    }
                    continue;
                }

                // Оператор
                if (Operators.ContainsKey(c.ToString()))
                {
                    result.Add(new Token(TokenType.Operator, c.ToString()));
                    i++;
                    continue;
                }

                throw new FormatException($"Unexpected character: {c}");
            }

            return result;
        }

        // Преобразование в ОПЗ (алгоритм сортировочной станции)
        protected virtual List<Token> ToRpn(List<Token> tokens)
        {
            var output = new List<Token>();
            var stack = new Stack<Token>();

            foreach (var token in tokens)
            {
                switch (token.Type)
                {
                    case TokenType.Number:
                        output.Add(token);
                        break;

                    case TokenType.Function:
                        stack.Push(token);
                        break;

                    case TokenType.Operator:
                        // Для операторов: выталкиваем из стека все операторы с большим или равным приоритетом
                        // (учитываем ассоциативность)
                        while (stack.Count > 0 && stack.Peek().Type == TokenType.Operator)
                        {
                            var op = stack.Peek();
                            var opInfo = Operators[op.Text];
                            var currentInfo = Operators[token.Text];

                            if ((!currentInfo.IsRightAssociative && currentInfo.Precedence <= opInfo.Precedence) ||
                                (currentInfo.IsRightAssociative && currentInfo.Precedence < opInfo.Precedence))
                            {
                                output.Add(stack.Pop());
                            }
                            else
                            {
                                break;
                            }
                        }
                        stack.Push(token);
                        break;

                    case TokenType.LeftParenthesis:
                        stack.Push(token);
                        break;

                    case TokenType.RightParenthesis:
                        // Выталкиваем до открывающей скобки
                        while (stack.Count > 0 && stack.Peek().Type != TokenType.LeftParenthesis)
                        {
                            output.Add(stack.Pop());
                        }
                        if (stack.Count == 0 || stack.Peek().Type != TokenType.LeftParenthesis)
                            throw new FormatException("Mismatched parentheses");
                        stack.Pop(); // удаляем '('

                        // Если на вершине стека функция – выталкиваем её
                        if (stack.Count > 0 && stack.Peek().Type == TokenType.Function)
                        {
                            output.Add(stack.Pop());
                        }
                        break;
                }
            }

            // Выталкиваем оставшиеся операторы
            while (stack.Count > 0)
            {
                var top = stack.Pop();
                if (top.Type == TokenType.LeftParenthesis || top.Type == TokenType.RightParenthesis)
                    throw new FormatException("Mismatched parentheses");
                output.Add(top);
            }

            return output;
        }

        // Вычисление ОПЗ
        protected virtual double EvaluateRpn(List<Token> rpn)
        {
            var stack = new Stack<double>();

            foreach (var token in rpn)
            {
                if (token.Type == TokenType.Number)
                {
                    if (!double.TryParse(token.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double val))
                        throw new FormatException($"Invalid number: {token.Text}");
                    stack.Push(val);
                }
                else if (token.Type == TokenType.Operator)
                {
                    var opInfo = Operators[token.Text];
                    if (opInfo.Arity == 2)
                    {
                        if (stack.Count < 2) throw new FormatException("Insufficient operands");
                        double b = stack.Pop();
                        double a = stack.Pop();
                        double res = opInfo.BinaryFunc(a, b);
                        stack.Push(res);
                    }
                    else if (opInfo.Arity == 1)
                    {
                        if (stack.Count < 1) throw new FormatException("Insufficient operands");
                        double a = stack.Pop();
                        double res = opInfo.UnaryFunc(a);
                        stack.Push(res);
                    }
                }
                else if (token.Type == TokenType.Function)
                {
                    var funcInfo = Functions[token.Text];
                    if (stack.Count < funcInfo.Arity)
                        throw new FormatException("Insufficient arguments for function");
                    var args = new double[funcInfo.Arity];
                    for (int i = funcInfo.Arity - 1; i >= 0; i--)
                        args[i] = stack.Pop();
                    double result = funcInfo.Func(args);
                    stack.Push(result);
                }
            }

            if (stack.Count != 1)
                throw new FormatException("Invalid expression");

            return stack.Pop();
        }

        // Вспомогательные методы для регистрации операторов/функций в наследниках
        protected void RegisterOperator(string symbol, OperatorInfo info)
        {
            Operators[symbol] = info;
        }

        protected void RegisterFunction(string name, FunctionInfo info)
        {
            Functions[name] = info;
        }

        protected void RegisterConstant(string name, double value)
        {
            Constants[name] = value;
        }
    }

    // Типы токенов
    public enum TokenType
    {
        Number,
        Operator,
        Function,
        LeftParenthesis,
        RightParenthesis
    }

    public class Token
    {
        public TokenType Type { get; }
        public string Text { get; }

        public Token(TokenType type, string text)
        {
            Type = type;
            Text = text;
        }

        public override string ToString() => Text;
    }
}