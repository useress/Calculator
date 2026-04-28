using System;
using System.Collections.Generic;

namespace Calculator.Models.Commands
{
    /// <summary>
    /// Invoker class for the Command pattern
    /// Manages command execution and maintains history for undo/redo operations
    /// </summary>
    public class CommandInvoker
    {
        private readonly Stack<ICalculatorCommand> _undoStack;
        private readonly Stack<ICalculatorCommand> _redoStack;

        public bool CanUndo => _undoStack.Count > 0;
        public bool CanRedo => _redoStack.Count > 0;

        public event EventHandler UndoRedoChanged;

        public CommandInvoker()
        {
            _undoStack = new Stack<ICalculatorCommand>();
            _redoStack = new Stack<ICalculatorCommand>();
        }

        /// <summary>
        /// Execute a command and add it to the undo history
        /// </summary>
        public void ExecuteCommand(ICalculatorCommand command)
        {
            try
            {
                command.Execute();
                
                if (command.CanUndo)
                {
                    _undoStack.Push(command);
                    _redoStack.Clear();  // Clear redo history when new command is executed
                    UndoRedoChanged?.Invoke(this, EventArgs.Empty);
                }
            }
            catch
            {
                // Silently handle exceptions during command execution
            }
        }

        /// <summary>
        /// Undo the last command
        /// </summary>
        public void Undo()
        {
            if (!CanUndo)
                return;

            try
            {
                var command = _undoStack.Pop();
                command.Undo();
                _redoStack.Push(command);
                UndoRedoChanged?.Invoke(this, EventArgs.Empty);
            }
            catch
            {
                // Silently handle exceptions during undo
            }
        }

        /// <summary>
        /// Redo the last undone command
        /// </summary>
        public void Redo()
        {
            if (!CanRedo)
                return;

            try
            {
                var command = _redoStack.Pop();
                command.Execute();
                _undoStack.Push(command);
                UndoRedoChanged?.Invoke(this, EventArgs.Empty);
            }
            catch
            {
                // Silently handle exceptions during redo
            }
        }

        /// <summary>
        /// Clear all history
        /// </summary>
        public void ClearHistory()
        {
            _undoStack.Clear();
            _redoStack.Clear();
            UndoRedoChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
