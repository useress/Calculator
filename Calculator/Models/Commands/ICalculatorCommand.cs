namespace Calculator.Models.Commands
{
    /// <summary>
    /// Command pattern interface for calculator operations
    /// Allows encapsulating requests as objects, supporting undo/redo operations
    /// </summary>
    public interface ICalculatorCommand
    {
        /// <summary>
        /// Execute the command
        /// </summary>
        void Execute();

        /// <summary>
        /// Undo the command (revert to previous state)
        /// </summary>
        void Undo();

        /// <summary>
        /// Check if the command can be undone
        /// </summary>
        bool CanUndo { get; }
    }
}
