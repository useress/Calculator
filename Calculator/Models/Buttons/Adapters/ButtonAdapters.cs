using System.Windows.Input;

namespace Calculator.Models.Buttons.Adapters
{
    /// <summary>
    /// Adapter Pattern: Adapts Button to a standardized interface
    /// Allows buttons to work with different UI frameworks or contexts
    /// </summary>
    public interface IButtonAdapter
    {
        string GetDisplayText();
        string GetInputValue();
        ICommand GetCommand();
        string GetStyleIdentifier();
        string GetMargin();
        object GetNativeButton();
    }

    /// <summary>
    /// Adapter: WPF Button Adapter
    /// </summary>
    public class WpfButtonAdapter : IButtonAdapter
    {
        private readonly Button _button;

        public WpfButtonAdapter(Button button)
        {
            _button = button;
        }

        public string GetDisplayText() => _button.DisplayText;
        public string GetInputValue() => _button.InputValue;
        public ICommand GetCommand() => _button.Command;
        public string GetStyleIdentifier() => _button.StyleKey;
        public string GetMargin() => _button.Margin;
        public object GetNativeButton() => _button;
    }

    /// <summary>
    /// Adapter: Generic Data Model Adapter
    /// Adapts Button to a simple data model for binding
    /// </summary>
    public class DataModelButtonAdapter : IButtonAdapter
    {
        private readonly Button _button;

        public DataModelButtonAdapter(Button button)
        {
            _button = button;
        }

        public string GetDisplayText() => _button.DisplayText;
        public string GetInputValue() => _button.InputValue;
        public ICommand GetCommand() => _button.Command;
        public string GetStyleIdentifier() => _button.StyleKey;
        public string GetMargin() => _button.Margin;
        public object GetNativeButton() => CreateDataModel();

        private object CreateDataModel()
        {
            return new
            {
                Display = _button.DisplayText,
                Input = _button.InputValue,
                Style = _button.StyleKey,
                Margin = _button.Margin
            };
        }
    }

    /// <summary>
    /// Adapter: Configuration Dictionary Adapter
    /// Converts button to dictionary for flexible usage
    /// </summary>
    public class DictionaryButtonAdapter : IButtonAdapter
    {
        private readonly Button _button;
        private readonly System.Collections.Generic.Dictionary<string, string> _configDict;

        public DictionaryButtonAdapter(Button button)
        {
            _button = button;
            _configDict = new System.Collections.Generic.Dictionary<string, string>
            {
                { "display", button.DisplayText },
                { "input", button.InputValue },
                { "style", button.StyleKey },
                { "margin", button.Margin }
            };
        }

        public string GetDisplayText() => _button.DisplayText;
        public string GetInputValue() => _button.InputValue;
        public ICommand GetCommand() => _button.Command;
        public string GetStyleIdentifier() => _button.StyleKey;
        public string GetMargin() => _button.Margin;
        public object GetNativeButton() => _configDict;

        public System.Collections.Generic.Dictionary<string, string> GetConfigDictionary()
        {
            return new System.Collections.Generic.Dictionary<string, string>(_configDict);
        }
    }

    /// <summary>
    /// Adapter: JSON Serializable Adapter
    /// Adapts button to JSON-friendly format
    /// </summary>
    public class JsonButtonAdapter : IButtonAdapter
    {
        private readonly Button _button;

        public class ButtonJson
        {
            public string Display { get; set; }
            public string Input { get; set; }
            public string Style { get; set; }
            public string Margin { get; set; }
        }

        public JsonButtonAdapter(Button button)
        {
            _button = button;
        }

        public string GetDisplayText() => _button.DisplayText;
        public string GetInputValue() => _button.InputValue;
        public ICommand GetCommand() => _button.Command;
        public string GetStyleIdentifier() => _button.StyleKey;
        public string GetMargin() => _button.Margin;

        public object GetNativeButton()
        {
            return new ButtonJson
            {
                Display = _button.DisplayText,
                Input = _button.InputValue,
                Style = _button.StyleKey,
                Margin = _button.Margin
            };
        }

        public ButtonJson GetJsonRepresentation()
        {
            return (ButtonJson)GetNativeButton();
        }
    }

    /// <summary>
    /// Adapter Factory: Creates appropriate adapter for given context
    /// </summary>
    public class ButtonAdapterFactory
    {
        public enum AdapterType
        {
            Wpf,
            DataModel,
            Dictionary,
            Json
        }

        public static IButtonAdapter CreateAdapter(Button button, AdapterType adapterType)
        {
            return adapterType switch
            {
                AdapterType.Wpf => new WpfButtonAdapter(button),
                AdapterType.DataModel => new DataModelButtonAdapter(button),
                AdapterType.Dictionary => new DictionaryButtonAdapter(button),
                AdapterType.Json => new JsonButtonAdapter(button),
                _ => new WpfButtonAdapter(button)
            };
        }
    }
}
