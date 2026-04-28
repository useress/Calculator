using System;
using System.Globalization;

namespace Calculator.Models.Commands
{
    /// <summary>
    /// Command for calculating the expression
    /// </summary>
    public class CalculateCommand : ICalculatorCommand
    {
        private readonly CalculatorState _state;
        private readonly EngineeringCalculatorModel _model;
        private string _previousDisplay;

        public bool CanUndo => true;

        public CalculateCommand(CalculatorState state, EngineeringCalculatorModel model)
        {
            _state = state;
            _model = model;
        }

        public void Execute()
        {
            try
            {
                _previousDisplay = _state.Display;

                if (string.IsNullOrWhiteSpace(_state.Display))
                    return;

                double result = _model.Evaluate(_state.Display);

                // Format the result to avoid truncation and scientific notation issues
                if (double.IsInfinity(result) || double.IsNaN(result))
                {
                    _state.Display = "Error";
                    return;
                }

                if (result == 0)
                {
                    _state.Display = "0";
                    return;
                }

                // Use F format with 10 decimal places, then remove trailing zeros
                string formatted = result.ToString("F10", CultureInfo.InvariantCulture);

                // Remove trailing zeros after decimal point
                if (formatted.Contains("."))
                {
                    formatted = formatted.TrimEnd('0').TrimEnd('.');
                }

                // Ensure formatted string is not empty
                if (string.IsNullOrEmpty(formatted) || formatted == "-")
                {
                    formatted = "0";
                }

                // If the result is very large or very small, use G format but with better precision
                if (formatted.Length > 15 || double.Abs(result) > 1e10 || (double.Abs(result) < 1e-5 && result != 0))
                {
                    formatted = result.ToString("G10", CultureInfo.InvariantCulture);
                }

                // Final safety check
                if (string.IsNullOrEmpty(formatted))
                {
                    _state.Display = result.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    _state.Display = formatted;
                }
            }
            catch
            {
                _state.Display = "Error";
            }
        }

        public void Undo()
        {
            _state.Display = _previousDisplay;
        }
    }
}
