using System;
using System.Collections.Generic;

namespace Calculator.Models
{
    /// <summary>
    /// Инженерный калькулятор с расширенными функциями и оператором возведения в степень.
    /// </summary>
    public class EngineeringCalculatorModel : CalculatorModel
    {
        public EngineeringCalculatorModel()
        {
            // Регистрация оператора возведения в степень (правоассоциативный, высокий приоритет)
            RegisterOperator("^", new OperatorInfo("^", 3, Math.Pow, true));

            // Регистрация функций
            RegisterFunction("sin", new FunctionInfo("sin", 1, args => Math.Sin(args[0])));
            RegisterFunction("cos", new FunctionInfo("cos", 1, args => Math.Cos(args[0])));
            RegisterFunction("tg", new FunctionInfo("tg", 1, args => Math.Tan(args[0])));
            RegisterFunction("ctg", new FunctionInfo("ctg", 1, args => 1.0 / Math.Tan(args[0])));
            RegisterFunction("sqrt", new FunctionInfo("sqrt", 1, args => Math.Sqrt(args[0])));
            RegisterFunction("lg", new FunctionInfo("lg", 1, args => Math.Log10(args[0])));

            // Опционально: константы
            RegisterConstant("pi", Math.PI);
            RegisterConstant("e", Math.E);
        }

        // Переопределяем, чтобы имена функций могли содержать цифры (например, log10)
        protected override bool IsValidFunctionCharacter(char c) => char.IsLetterOrDigit(c);
    }
}