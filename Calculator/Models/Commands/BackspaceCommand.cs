namespace Calculator.Models.Commands
{
    /// <summary>
    /// Command for backspace operation (removing last character)
    /// </summary>
    public class BackspaceCommand : ICalculatorCommand
    {
        private readonly CalculatorState _state;
        private string _previousDisplay;

        public bool CanUndo => true;

        public BackspaceCommand(CalculatorState state)
        {
            _state = state;
        }

        public void Execute()
        {
            _previousDisplay = _state.Display;

            if (!string.IsNullOrEmpty(_state.Display) && _state.Display.Length > 0)
            {
                _state.Display = _state.Display.Substring(0, _state.Display.Length - 1);
            }
        }

        public void Undo()
        {
            _state.Display = _previousDisplay;
        }
    }
}
