using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public class Calculator
    //var (result, error) = calculator.Calculate(expr);

{
    // Делегат для операций
    private delegate double Operation(double a, double b);

    // Словарь для хранения операций с их приоритетом
    private readonly Dictionary<string, (Operation operation, int priority)> _operations;

    public Calculator()
    {
        _operations = new Dictionary<string, (Operation, int)>(StringComparer.OrdinalIgnoreCase)
        {
            { "+", ((a, b) => a + b, 1) },
            { "-", ((a, b) => a - b, 1) },
            { "*", ((a, b) => a * b, 2) },
            { "/", ((a, b) => b != 0 ? a / b : throw new DivideByZeroException("Деление на ноль"), 2) }
        };
    }

    // Метод для добавления новых операций с указанием приоритета
    public void AddOperation(string symbol, Operation operation, int priority = 1)
    {
        if (string.IsNullOrWhiteSpace(symbol))
            throw new ArgumentException("Символ операции не может быть пустым");

        _operations[symbol] = (operation ?? throw new ArgumentNullException(nameof(operation)), priority);
    }

    // Основной метод вычисления сложных выражений
    public (double? result, string error) Calculate(string expression)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(expression))
                return (null, "Выражение не может быть пустым");

            // Удаляем все пробелы
            string cleanExpression = Regex.Replace(expression, @"\s+", "");

            // Токенизируем выражение
            var tokens = Tokenize(cleanExpression);
            if (tokens == null)
                return (null, "Ошибка разбора выражения");

            // Преобразуем в обратную польскую нотацию (алгоритм сортировочной станции)
            var rpn = ConvertToRPN(tokens);
            if (rpn == null)
                return (null, "Ошибка преобразования выражения");

            // Вычисляем результат
            double result = EvaluateRPN(rpn);
            return (result, null);
        }
        catch (DivideByZeroException ex)
        {
            return (null, ex.Message);
        }
        catch (Exception ex)
        {
            return (null, $"Ошибка вычисления: {ex.Message}");
        }
    }

    private List<string> Tokenize(string expression)
    {
        var tokens = new List<string>();
        int i = 0;

        while (i < expression.Length)
        {
            char c = expression[i];

            // Пропускаем пробелы (хотя мы уже удалили, на всякий случай)
            if (char.IsWhiteSpace(c))
            {
                i++;
                continue;
            }

            // Обработка чисел (включая десятичные и отрицательные)
            if (char.IsDigit(c) || c == '.' || (c == '-' && (i == 0 || IsOperator(expression[i - 1].ToString()))))
            {
                string number = "";

                // Если это минус в начале числа
                if (c == '-')
                {
                    number += c;
                    i++;
                    if (i < expression.Length)
                        c = expression[i];
                }

                // Собираем число
                while (i < expression.Length && (char.IsDigit(c) || c == '.'))
                {
                    number += c;
                    i++;
                    if (i < expression.Length)
                        c = expression[i];
                }

                if (number == "-" || string.IsNullOrEmpty(number))
                    return null;

                tokens.Add(number);
                continue;
            }

            // Обработка операторов (включая многосимвольные)
            string foundOp = FindLongestOperator(expression, i);
            if (foundOp != null)
            {
                tokens.Add(foundOp);
                i += foundOp.Length;
                continue;
            }

            // Обработка скобок
            if (c == '(' || c == ')')
            {
                tokens.Add(c.ToString());
                i++;
                continue;
            }

            // Если дошли до сюда - неизвестный символ
            return null;
        }

        return tokens;
    }

    private string FindLongestOperator(string expression, int startIndex)
    {
        // Ищем самый длинный оператор, начиная с текущей позиции
        var possibleOps = _operations.Keys
            .Where(op => startIndex + op.Length <= expression.Length)
            .OrderByDescending(op => op.Length);

        foreach (var op in possibleOps)
        {
            if (expression.Substring(startIndex, op.Length) == op)
                return op;
        }

        return null;
    }

    private bool IsOperator(string token)
    {
        return _operations.ContainsKey(token);
    }

    private int GetOperatorPriority(string op)
    {
        return _operations.TryGetValue(op, out var info) ? info.priority : 0;
    }

    private List<string> ConvertToRPN(List<string> tokens)
    {
        var output = new List<string>();
        var operatorStack = new Stack<string>();

        foreach (var token in tokens)
        {
            if (double.TryParse(token, out _))
            {
                // Число - добавляем в выходную очередь
                output.Add(token);
            }
            else if (token == "(")
            {
                // Открывающая скобка - в стек операторов
                operatorStack.Push(token);
            }
            else if (token == ")")
            {
                // Закрывающая скобка - выталкиваем операторы до открывающей скобки
                while (operatorStack.Count > 0 && operatorStack.Peek() != "(")
                {
                    output.Add(operatorStack.Pop());
                }

                if (operatorStack.Count == 0)
                    return null; // Непарные скобки

                operatorStack.Pop(); // Удаляем "("
            }
            else if (IsOperator(token))
            {
                // Оператор
                while (operatorStack.Count > 0 && IsOperator(operatorStack.Peek()))
                {
                    string topOp = operatorStack.Peek();

                    // Если приоритет текущего оператора <= приоритета оператора на вершине стека
                    if (GetOperatorPriority(token) <= GetOperatorPriority(topOp))
                    {
                        output.Add(operatorStack.Pop());
                    }
                    else
                    {
                        break;
                    }
                }
                operatorStack.Push(token);
            }
            else
            {
                return null; // Неизвестный токен
            }
        }

        // Выталкиваем оставшиеся операторы
        while (operatorStack.Count > 0)
        {
            string op = operatorStack.Pop();
            if (op == "(" || op == ")")
                return null; // Непарные скобки

            output.Add(op);
        }

        return output;
    }

    private double EvaluateRPN(List<string> rpn)
    {
        var stack = new Stack<double>();

        foreach (var token in rpn)
        {
            if (double.TryParse(token, out double number))
            {
                stack.Push(number);
            }
            else if (IsOperator(token))
            {
                if (stack.Count < 2)
                    throw new InvalidOperationException("Недостаточно операндов");

                double b = stack.Pop();
                double a = stack.Pop();

                double result = _operations[token].operation(a, b);
                stack.Push(result);
            }
        }

        if (stack.Count != 1)
            throw new InvalidOperationException("Ошибка в выражении");

        return stack.Pop();
    }

    // Для удобства - перегрузка с возвратом double и исключением
    public double CalculateWithException(string expression)
    {
        var (result, error) = Calculate(expression);

        if (error != null)
            throw new InvalidOperationException(error);

        return result.Value;
    }

    // Получить список всех поддерживаемых операторов
    public IEnumerable<string> GetSupportedOperators()
    {
        return _operations.Keys.ToList();
    }

    // Получить информацию об операторах
    public Dictionary<string, int> GetOperatorsWithPriority()
    {
        return _operations.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.priority);
    }
}

// Пример использования
class Program
{
    static void Main()
    {
        var calculator = new Calculator();

        // Добавляем новые операции с указанием приоритета
        calculator.AddOperation("^", (a, b) => Math.Pow(a, b), 3); // высокий приоритет
        calculator.AddOperation("%", (a, b) => a % b, 2); // такой же приоритет как * и /
        calculator.AddOperation("**", (a, b) => Math.Pow(a, b), 3); // альтернативный синтаксис степени

        Console.WriteLine("Калькулятор сложных выражений");
        Console.WriteLine("Поддерживаются операции: " +
            string.Join(", ", calculator.GetSupportedOperators()));
        Console.WriteLine("\nПриоритеты операций:");
        foreach (var op in calculator.GetOperatorsWithPriority().OrderBy(x => x.Value))
        {
            Console.WriteLine($"  {op.Key} - приоритет {op.Value}");
        }
        Console.WriteLine("\nПримеры: 2+3*4, (2+3)*4, 2^3+4, 10/2-3*2");
        Console.WriteLine("Для выхода введите 'exit'\n");

        // Интерактивный режим
        while (true)
        {
            Console.Write("Введите выражение: ");
            string input = Console.ReadLine();

            if (input?.ToLower() == "exit")
                break;

            var (result, error) = calculator.Calculate(input);

            if (error == null)
                Console.WriteLine($"Результат: {result}\n");
            else
                Console.WriteLine($"Ошибка: {error}\n");
        }

        // Демонстрация различных выражений
        Console.WriteLine("\nДемонстрация сложных выражений:");
        var testExpressions = new[]
        {
            "2+3*4",
            "(2+3)*4",
            "10/2-3*2",
            "2^3+4",
            "2**3+4",
            "10 % 3 * 2",
            "(2+3)*4-6/2",
            "2.5 * 3 + 1.5",
            "-5 + 3 * 2",
            "10 / (2 + 3)",
            "((2+3)*2-1)/3"
        };

        foreach (var expr in testExpressions)
        {
            var (result, error) = calculator.Calculate(expr);

            if (error == null)
                Console.WriteLine($"'{expr}' = {result}");
            else
                Console.WriteLine($"'{expr}' -> Ошибка: {error}");
        }

        Console.WriteLine("\nНажмите любую клавишу для выхода...");
        Console.ReadKey();
    }
}