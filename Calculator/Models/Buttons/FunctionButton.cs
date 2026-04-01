using System.Windows.Input;

namespace Calculator.Models.Buttons
{
    /// <summary>
    /// Button for functions and special operations (sin, cos, C, CE, =, √, etc.)
    /// </summary>
    public class FunctionButton : Button
    {
        /// <summary>
        /// Type of function button (e.g., "Trigonometric", "Special", "Control")
        /// </summary>
        public string ButtonType { get; set; }

        /// <summary>
        /// Create a function button with default styling
        /// </summary>
        public FunctionButton(string displayText, string inputValue, ICommand command, string buttonType = "Function")
            : base(displayText, inputValue, command)
        {
            ButtonType = buttonType;
            // Default style for functions
            StyleKey = "XiaomiFunctionButton";
        }

        /// <summary>
        /// Create a function button with custom styling
        /// </summary>
        public FunctionButton(string displayText, string inputValue, ICommand command, string buttonType, string styleKey)
            : base(displayText, inputValue, command)
        {
            ButtonType = buttonType;
            StyleKey = styleKey;
        }

        public override (string Display, string InputValue, ICommand Command, string Style, string Margin) GetConfiguration()
        {
            return (DisplayText, InputValue, Command, StyleKey, Margin);
        }
    }
}
