namespace Calculator.Models.Commands
{
    /// <summary>
    /// Command for toggling between normal and extended layout
    /// </summary>
    public class ToggleLayoutCommand : ICalculatorCommand
    {
        private readonly CalculatorState _state;
        private bool _previousLayout;

        public bool CanUndo => true;

        public ToggleLayoutCommand(CalculatorState state)
        {
            _state = state;
        }

        public void Execute()
        {
            _previousLayout = _state.IsExtendedLayout;
            _state.IsExtendedLayout = !_state.IsExtendedLayout;
        }

        public void Undo()
        {
            _state.IsExtendedLayout = _previousLayout;
        }
    }
}
