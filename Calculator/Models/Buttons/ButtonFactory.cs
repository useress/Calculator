using System.Windows.Input;

namespace Calculator.Models.Buttons
{
    /// <summary>
    /// Factory for creating calculator buttons using the Factory Method Pattern
    /// </summary>
    public class ButtonFactory
    {
        private readonly ICommand _inputCommand;
        private readonly ICommand _clearCommand;
        private readonly ICommand _backspaceCommand;
        private readonly ICommand _calculateCommand;

        /// <summary>
        /// Initialize the factory with commands
        /// </summary>
        public ButtonFactory(
            ICommand inputCommand,
            ICommand clearCommand,
            ICommand backspaceCommand,
            ICommand calculateCommand)
        {
            _inputCommand = inputCommand;
            _clearCommand = clearCommand;
            _backspaceCommand = backspaceCommand;
            _calculateCommand = calculateCommand;
        }

        /// <summary>
        /// Create a digit button (0-9, decimal point)
        /// </summary>
        public DigitButton CreateDigitButton(string digit)
        {
            return new DigitButton(digit, _inputCommand);
        }

        /// <summary>
        /// Create an operator button (+, -, *, /, ^, etc.)
        /// </summary>
        public OperatorButton CreateOperatorButton(string displayText, string inputValue)
        {
            return new OperatorButton(displayText, inputValue, _inputCommand);
        }

        /// <summary>
        /// Create a function button for mathematical functions (sin, cos, tan, etc.)
        /// </summary>
        public FunctionButton CreateMathFunctionButton(string displayText, string inputValue)
        {
            return new FunctionButton(displayText, inputValue, _inputCommand, "MathFunction");
        }

        /// <summary>
        /// Create a special control button (C, CE, ⌫)
        /// </summary>
        public FunctionButton CreateControlButton(string displayText, ICommand command)
        {
            var btn = new FunctionButton(displayText, "", command, "Control", "XiaomiOperatorButton");
            return btn;
        }

        /// <summary>
        /// Create the equals button
        /// </summary>
        public FunctionButton CreateEqualsButton()
        {
            return new FunctionButton("=", "=", _calculateCommand, "Equals", "XiaomiEqualButton");
        }

        /// <summary>
        /// Create the clear button
        /// </summary>
        public FunctionButton CreateClearButton()
        {
            return CreateControlButton("C", _clearCommand);
        }

        /// <summary>
        /// Create the backspace button
        /// </summary>
        public FunctionButton CreateBackspaceButton()
        {
            return CreateControlButton("⌫", _backspaceCommand);
        }

        /// <summary>
        /// Create a special constant button (π, e)
        /// </summary>
        public FunctionButton CreateConstantButton(string displayText, string inputValue)
        {
            return new FunctionButton(displayText, inputValue, _inputCommand, "Constant");
        }

        /// <summary>
        /// Create a parenthesis button
        /// </summary>
        public FunctionButton CreateParenthesisButton(string paren)
        {
            return new FunctionButton(paren, paren, _inputCommand, "Parenthesis");
        }
    }
}
