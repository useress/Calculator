using System.Windows.Input;

namespace Calculator.Models.Buttons
{
    /// <summary>
    /// Abstract base class for all calculator buttons
    /// </summary>
    public abstract class Button
    {
        /// <summary>
        /// Display text shown on the button
        /// </summary>
        public string DisplayText { get; set; }

        /// <summary>
        /// Value to be input when button is pressed
        /// </summary>
        public string InputValue { get; set; }

        /// <summary>
        /// The command to execute when button is pressed
        /// </summary>
        public ICommand Command { get; set; }

        /// <summary>
        /// Style key for XAML styling
        /// </summary>
        public string StyleKey { get; protected set; }

        /// <summary>
        /// Margin for the button (default "3")
        /// </summary>
        public string Margin { get; set; } = "3";

        protected Button(string displayText, string inputValue, ICommand command)
        {
            DisplayText = displayText;
            InputValue = inputValue;
            Command = command;
        }

        /// <summary>
        /// Get button configuration as a tuple (Display, InputValue, Command, StyleKey, Margin)
        /// </summary>
        public abstract (string Display, string InputValue, ICommand Command, string Style, string Margin) GetConfiguration();
    }
}
