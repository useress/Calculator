using System.Windows.Input;

namespace Calculator.Models.Buttons
{
    /// <summary>
    /// Button for numeric digits (0-9) and decimal point
    /// </summary>
    public class DigitButton : Button
    {
        public DigitButton(string digit, ICommand command)
            : base(digit, digit, command)
        {
            StyleKey = "XiaomiNumberButton";
        }

        public override (string Display, string InputValue, ICommand Command, string Style, string Margin) GetConfiguration()
        {
            return (DisplayText, InputValue, Command, StyleKey, Margin);
        }
    }
}
