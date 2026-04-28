using System.Windows.Input;

namespace Calculator.Models.Buttons.Bridge
{
    /// <summary>
    /// Bridge Pattern: Abstraction for button behavior/implementation
    /// Decouples button abstraction from its platform-specific implementation
    /// </summary>
    public interface IButtonImplementation
    {
        void Render(Button button);
        void OnClick(Button button);
        void OnHover(Button button);
        void SetProperty(string propertyName, object value);
        object GetProperty(string propertyName);
    }

    /// <summary>
    /// Bridge: WPF-specific button implementation
    /// </summary>
    public class WpfButtonImplementation : IButtonImplementation
    {
        private readonly System.Collections.Generic.Dictionary<string, object> _properties 
            = new System.Collections.Generic.Dictionary<string, object>();

        public void Render(Button button)
        {
            // WPF-specific rendering logic
            System.Diagnostics.Debug.WriteLine($"WPF: Rendering button '{button.DisplayText}' with style '{button.StyleKey}'");
        }

        public void OnClick(Button button)
        {
            // WPF-specific click handling
            button.Command?.Execute(button.InputValue);
        }

        public void OnHover(Button button)
        {
            // WPF-specific hover effects
            System.Diagnostics.Debug.WriteLine($"WPF: Hovering over button '{button.DisplayText}'");
        }

        public void SetProperty(string propertyName, object value)
        {
            _properties[propertyName] = value;
        }

        public object GetProperty(string propertyName)
        {
            return _properties.ContainsKey(propertyName) ? _properties[propertyName] : null;
        }
    }

    /// <summary>
    /// Bridge: Console-based button implementation
    /// </summary>
    public class ConsoleButtonImplementation : IButtonImplementation
    {
        private readonly System.Collections.Generic.Dictionary<string, object> _properties 
            = new System.Collections.Generic.Dictionary<string, object>();

        public void Render(Button button)
        {
            // Console-specific rendering logic
            System.Console.WriteLine($"[BUTTON] {button.DisplayText}");
        }

        public void OnClick(Button button)
        {
            // Console-specific click handling
            System.Console.WriteLine($"Click: {button.InputValue}");
            button.Command?.Execute(button.InputValue);
        }

        public void OnHover(Button button)
        {
            // Console-specific hover effects
            System.Console.WriteLine($">> {button.DisplayText}");
        }

        public void SetProperty(string propertyName, object value)
        {
            _properties[propertyName] = value;
        }

        public object GetProperty(string propertyName)
        {
            return _properties.ContainsKey(propertyName) ? _properties[propertyName] : null;
        }
    }

    /// <summary>
    /// Bridge: Web-based button implementation
    /// </summary>
    public class WebButtonImplementation : IButtonImplementation
    {
        private readonly System.Collections.Generic.Dictionary<string, object> _properties 
            = new System.Collections.Generic.Dictionary<string, object>();

        public void Render(Button button)
        {
            // Web-specific rendering (HTML generation)
            var html = $"<button class='{button.StyleKey}' style='margin: {button.Margin}px;'>{button.DisplayText}</button>";
            System.Diagnostics.Debug.WriteLine($"HTML: {html}");
        }

        public void OnClick(Button button)
        {
            // Web-specific click handling (could be AJAX, etc.)
            button.Command?.Execute(button.InputValue);
        }

        public void OnHover(Button button)
        {
            // Web-specific hover effects (CSS transitions, etc.)
            System.Diagnostics.Debug.WriteLine($"CSS: Apply hover effects to '{button.DisplayText}'");
        }

        public void SetProperty(string propertyName, object value)
        {
            _properties[propertyName] = value;
        }

        public object GetProperty(string propertyName)
        {
            return _properties.ContainsKey(propertyName) ? _properties[propertyName] : null;
        }
    }

    /// <summary>
    /// Bridge: Abstraction - Button implementation with selectable platform
    /// </summary>
    public class PlatformAwareButton
    {
        private readonly Button _button;
        private IButtonImplementation _implementation;

        public PlatformAwareButton(Button button, IButtonImplementation implementation)
        {
            _button = button;
            _implementation = implementation;
        }

        /// <summary>
        /// Change the platform implementation at runtime
        /// </summary>
        public void SetImplementation(IButtonImplementation implementation)
        {
            _implementation = implementation;
        }

        /// <summary>
        /// Render the button using current implementation
        /// </summary>
        public void Render()
        {
            _implementation.Render(_button);
        }

        /// <summary>
        /// Trigger click event
        /// </summary>
        public void Click()
        {
            _implementation.OnClick(_button);
        }

        /// <summary>
        /// Trigger hover event
        /// </summary>
        public void Hover()
        {
            _implementation.OnHover(_button);
        }

        /// <summary>
        /// Set implementation-specific property
        /// </summary>
        public void SetProperty(string propertyName, object value)
        {
            _implementation.SetProperty(propertyName, value);
        }

        /// <summary>
        /// Get implementation-specific property
        /// </summary>
        public object GetProperty(string propertyName)
        {
            return _implementation.GetProperty(propertyName);
        }

        public Button GetButton() => _button;
    }
}
