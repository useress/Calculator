namespace Calculator.Models.Commands
{
    /// <summary>
    /// Command for clearing the display
    /// </summary>
    public class ClearCommand : ICalculatorCommand
    {
        private readonly CalculatorState _state;
        private string _previousDisplay;

        public bool CanUndo => true;

        public ClearCommand(CalculatorState state)
        {
            _state = state;
        }

        public void Execute()
        {
            _previousDisplay = _state.Display;
            _state.Display = string.Empty;
        }

        public void Undo()
        {
            _state.Display = _previousDisplay;
        }
    }
}
