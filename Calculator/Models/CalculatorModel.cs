using System;

namespace Calculator.Models
{
    public class CalculatorModel
    {
        public double Calculate(double a, double b, string op)
        {
            switch (op)
            {
                case "+":
                    return a + b;

                case "-":
                    return a - b;

                case "*":
                    return a * b;

                case "/":
                    if (b == 0)
                        throw new DivideByZeroException();
                    return a / b;

                default:
                    return 0;
            }
        }
    }
}