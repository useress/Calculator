using System.Windows.Input;

namespace Calculator.Models.Buttons.Decorators
{
    /// <summary>
    /// Decorator Pattern: Abstract decorator for button enhancement
    /// Allows adding functionality to buttons without modifying the Button class
    /// </summary>
    public abstract class ButtonDecorator : Button
    {
        protected readonly Button _decoratedButton;

        protected ButtonDecorator(Button decoratedButton)
            : base(decoratedButton.DisplayText, decoratedButton.InputValue, decoratedButton.Command)
        {
            _decoratedButton = decoratedButton;
            StyleKey = _decoratedButton.StyleKey;
            Margin = _decoratedButton.Margin;
        }

        public override (string Display, string InputValue, ICommand Command, string Style, string Margin) GetConfiguration()
        {
            return _decoratedButton.GetConfiguration();
        }
    }

    /// <summary>
    /// Decorator: Add tooltip functionality to buttons
    /// </summary>
    public class TooltipButtonDecorator : ButtonDecorator
    {
        public string Tooltip { get; }

        public TooltipButtonDecorator(Button button, string tooltip)
            : base(button)
        {
            Tooltip = tooltip;
            DisplayText = $"{button.DisplayText} ({tooltip})";
        }

        public override (string Display, string InputValue, ICommand Command, string Style, string Margin) GetConfiguration()
        {
            var config = base.GetConfiguration();
            return (DisplayText, config.InputValue, config.Command, config.Style, config.Margin);
        }
    }

    /// <summary>
    /// Decorator: Add disabled state to buttons
    /// </summary>
    public class DisabledButtonDecorator : ButtonDecorator
    {
        public bool IsDisabled { get; set; }

        public DisabledButtonDecorator(Button button, bool isDisabled = true)
            : base(button)
        {
            IsDisabled = isDisabled;
        }

        public override (string Display, string InputValue, ICommand Command, string Style, string Margin) GetConfiguration()
        {
            if (IsDisabled)
            {
                var config = base.GetConfiguration();
                return (config.Display, string.Empty, null, config.Style + "Disabled", config.Margin);
            }
            return base.GetConfiguration();
        }
    }

    /// <summary>
    /// Decorator: Add custom margin/padding to buttons
    /// </summary>
    public class MarginButtonDecorator : ButtonDecorator
    {
        public string CustomMargin { get; }

        public MarginButtonDecorator(Button button, string margin)
            : base(button)
        {
            CustomMargin = margin;
            Margin = margin;
        }

        public override (string Display, string InputValue, ICommand Command, string Style, string Margin) GetConfiguration()
        {
            var config = base.GetConfiguration();
            return (config.Display, config.InputValue, config.Command, config.Style, CustomMargin);
        }
    }

    /// <summary>
    /// Decorator: Add custom styling/coloring to buttons
    /// </summary>
    public class StyledButtonDecorator : ButtonDecorator
    {
        public string CustomStyleKey { get; }

        public StyledButtonDecorator(Button button, string styleKey)
            : base(button)
        {
            CustomStyleKey = styleKey;
            StyleKey = styleKey;
        }

        public override (string Display, string InputValue, ICommand Command, string Style, string Margin) GetConfiguration()
        {
            var config = base.GetConfiguration();
            return (config.Display, config.InputValue, config.Command, CustomStyleKey, config.Margin);
        }
    }

    /// <summary>
    /// Decorator: Add size/dimension properties to buttons
    /// </summary>
    public class SizedButtonDecorator : ButtonDecorator
    {
        public string Width { get; }
        public string Height { get; }

        public SizedButtonDecorator(Button button, string width, string height)
            : base(button)
        {
            Width = width;
            Height = height;
        }

        public override (string Display, string InputValue, ICommand Command, string Style, string Margin) GetConfiguration()
        {
            return base.GetConfiguration();
        }

        public (string Width, string Height) GetDimensions()
        {
            return (Width, Height);
        }
    }

    /// <summary>
    /// Decorator: Add confirmation requirement before execution
    /// </summary>
    public class ConfirmableButtonDecorator : ButtonDecorator
    {
        public string ConfirmationMessage { get; }
        public bool RequiresConfirmation { get; set; } = true;

        public ConfirmableButtonDecorator(Button button, string confirmationMessage)
            : base(button)
        {
            ConfirmationMessage = confirmationMessage;
        }

        public override (string Display, string InputValue, ICommand Command, string Style, string Margin) GetConfiguration()
        {
            if (RequiresConfirmation)
            {
                return base.GetConfiguration();
                // In real implementation, would wrap Command with confirmation logic
            }
            return base.GetConfiguration();
        }
    }
}
