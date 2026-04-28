namespace Calculator.Models.Commands
{
    /// <summary>
    /// Command for input operations (adding digits, operators, functions)
    /// </summary>
    public class InputCommand : ICalculatorCommand
    {
        private readonly CalculatorState _state;
        private readonly string _input;
        private string _previousDisplay;

        public bool CanUndo => true;

        public InputCommand(CalculatorState state, string input)
        {
            _state = state;
            _input = input;
        }

        public void Execute()
        {
            _previousDisplay = _state.Display;

            // If currently showing "Error" or empty display — clear it when user starts typing
            if (_state.Display == "Error" || _state.Display == null)
            {
                _state.Display = string.Empty;
            }

            // Append the token to the current display
            string newDisplay = _state.Display + _input;

            if (!string.IsNullOrEmpty(newDisplay))
            {
                _state.Display = newDisplay;
            }
        }

        public void Undo()
        {
            _state.Display = _previousDisplay;
        }
    }
}
