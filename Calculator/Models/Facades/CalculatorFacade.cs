namespace Calculator.Models.Facades
{
    /// <summary>
    /// Facade Pattern: Provides a unified, simplified interface to the calculator subsystem
    /// Hides complexity of Models, Commands, State, and Factory interactions
    /// </summary>
    public class CalculatorFacade
    {
        private readonly EngineeringCalculatorModel _calculatorModel;
        private readonly Commands.CalculatorState _state;
        private readonly Commands.CommandInvoker _invoker;
        private readonly Buttons.ButtonFactory _buttonFactory;

        public string Display
        {
            get => _state.Display;
            set => _state.Display = value;
        }

        public bool IsExtendedLayout
        {
            get => _state.IsExtendedLayout;
            set => _state.IsExtendedLayout = value;
        }

        public bool CanUndo => _invoker.CanUndo;
        public bool CanRedo => _invoker.CanRedo;

        public event System.EventHandler UndoRedoChanged
        {
            add => _invoker.UndoRedoChanged += value;
            remove => _invoker.UndoRedoChanged -= value;
        }

        public event System.ComponentModel.PropertyChangedEventHandler StateChanged
        {
            add => _state.PropertyChanged += value;
            remove => _state.PropertyChanged -= value;
        }

        /// <summary>
        /// Initialize the calculator facade with required dependencies
        /// </summary>
        public CalculatorFacade(
            EngineeringCalculatorModel calculatorModel,
            Commands.CalculatorState state,
            Commands.CommandInvoker invoker,
            Buttons.ButtonFactory buttonFactory)
        {
            _calculatorModel = calculatorModel;
            _state = state;
            _invoker = invoker;
            _buttonFactory = buttonFactory;
        }

        /// <summary>
        /// Process user input
        /// </summary>
        public void Input(string value)
        {
            if (string.IsNullOrEmpty(value))
                return;

            var command = new Commands.InputCommand(_state, value);
            _invoker.ExecuteCommand(command);
        }

        /// <summary>
        /// Clear the display
        /// </summary>
        public void Clear()
        {
            var command = new Commands.ClearCommand(_state);
            _invoker.ExecuteCommand(command);
        }

        /// <summary>
        /// Perform backspace operation
        /// </summary>
        public void Backspace()
        {
            var command = new Commands.BackspaceCommand(_state);
            _invoker.ExecuteCommand(command);
        }

        /// <summary>
        /// Calculate the current expression
        /// </summary>
        public void Calculate()
        {
            var command = new Commands.CalculateCommand(_state, _calculatorModel);
            _invoker.ExecuteCommand(command);
        }

        /// <summary>
        /// Toggle between normal and extended layout
        /// </summary>
        public void ToggleLayout()
        {
            var command = new Commands.ToggleLayoutCommand(_state);
            _invoker.ExecuteCommand(command);
        }

        /// <summary>
        /// Undo the last operation
        /// </summary>
        public void Undo()
        {
            _invoker.Undo();
        }

        /// <summary>
        /// Redo the last undone operation
        /// </summary>
        public void Redo()
        {
            _invoker.Redo();
        }

        /// <summary>
        /// Get the button factory for creating UI buttons
        /// </summary>
        public Buttons.ButtonFactory GetButtonFactory()
        {
            return _buttonFactory;
        }

        /// <summary>
        /// Get current calculator state (for advanced operations)
        /// </summary>
        public Commands.CalculatorState GetState()
        {
            return _state;
        }

        /// <summary>
        /// Evaluate an expression directly without modifying state
        /// </summary>
        public double EvaluateExpression(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
                return 0;

            try
            {
                return _calculatorModel.Evaluate(expression);
            }
            catch
            {
                return double.NaN;
            }
        }

        /// <summary>
        /// Reset calculator to initial state
        /// </summary>
        public void Reset()
        {
            _state.Display = string.Empty;
            _state.IsExtendedLayout = false;
            _invoker.ClearHistory();
        }
    }
}
