using System.Windows.Input;

namespace Calculator.Models.Buttons
{
    /// <summary>
    /// Button for mathematical operators (+, -, *, /, ^, etc.)
    /// </summary>
    public class OperatorButton : Button
    {
        public OperatorButton(string displayText, string inputValue, ICommand command)
            : base(displayText, inputValue, command)
        {
            StyleKey = "XiaomiOperatorButton";
        }

        public override (string Display, string InputValue, ICommand Command, string Style, string Margin) GetConfiguration()
        {
            return (DisplayText, InputValue, Command, StyleKey, Margin);
        }
    }
}
