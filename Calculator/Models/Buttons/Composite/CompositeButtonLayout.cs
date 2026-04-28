using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Calculator.Models.Buttons.Composite
{
    /// <summary>
    /// Composite Pattern: Component interface for individual and composite buttons
    /// </summary>
    public interface IButtonComponent
    {
        string DisplayText { get; }
        string InputValue { get; }
        ICommand Command { get; }
        string StyleKey { get; }
        string Margin { get; }

        (string Display, string InputValue, ICommand Command, string Style, string Margin) GetConfiguration();
    }

    /// <summary>
    /// Composite Pattern: Leaf component - represents a single button
    /// </summary>
    public class ButtonLeaf : IButtonComponent
    {
        private readonly Button _button;

        public string DisplayText => _button.DisplayText;
        public string InputValue => _button.InputValue;
        public ICommand Command => _button.Command;
        public string StyleKey => _button.StyleKey;
        public string Margin => _button.Margin;

        public ButtonLeaf(Button button)
        {
            _button = button ?? throw new ArgumentNullException(nameof(button));
        }

        public (string Display, string InputValue, ICommand Command, string Style, string Margin) GetConfiguration()
        {
            return _button.GetConfiguration();
        }
    }

    /// <summary>
    /// Composite Pattern: Composite component - represents a group of buttons (row/group)
    /// </summary>
    public class ButtonGroup : IButtonComponent
    {
        private readonly List<IButtonComponent> _components = new List<IButtonComponent>();
        private readonly string _groupName;

        public string DisplayText => _groupName;
        public string InputValue => string.Empty;
        public ICommand Command => null;
        public string StyleKey => "ButtonGroup";
        public string Margin => "0";

        public int ComponentCount => _components.Count;

        /// <summary>
        /// Create a button group with a name
        /// </summary>
        public ButtonGroup(string groupName)
        {
            _groupName = groupName ?? "ButtonGroup";
        }

        /// <summary>
        /// Add a component (button or group) to this group
        /// </summary>
        public void Add(IButtonComponent component)
        {
            if (component == null)
                throw new ArgumentNullException(nameof(component));
            _components.Add(component);
        }

        /// <summary>
        /// Remove a component from this group
        /// </summary>
        public void Remove(IButtonComponent component)
        {
            _components.Remove(component);
        }

        /// <summary>
        /// Get a component by index
        /// </summary>
        public IButtonComponent Get(int index)
        {
            if (index < 0 || index >= _components.Count)
                throw new IndexOutOfRangeException($"Index {index} out of range");
            return _components[index];
        }

        /// <summary>
        /// Get all components in this group
        /// </summary>
        public IReadOnlyList<IButtonComponent> GetComponents()
        {
            return _components.AsReadOnly();
        }

        public (string Display, string InputValue, ICommand Command, string Style, string Margin) GetConfiguration()
        {
            return (DisplayText, InputValue, Command, StyleKey, Margin);
        }
    }

    /// <summary>
    /// Composite Pattern: Button layout builder using composite pattern
    /// </summary>
    public class CompositeButtonLayoutBuilder
    {
        private readonly ButtonFactory _factory;

        public CompositeButtonLayoutBuilder(ButtonFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Build normal calculator layout with composite structure
        /// </summary>
        public ButtonGroup BuildNormalLayout()
        {
            var layout = new ButtonGroup("NormalLayout");

            // Row 1: Control buttons
            var row1 = new ButtonGroup("Row1");
            row1.Add(new ButtonLeaf(_factory.CreateClearButton()));
            row1.Add(new ButtonLeaf(_factory.CreateBackspaceButton()));
            row1.Add(new ButtonLeaf(_factory.CreateOperatorButton("^", "^")));
            row1.Add(new ButtonLeaf(_factory.CreateOperatorButton("√", "sqrt(")));
            layout.Add(row1);

            // Row 2: 7, 8, 9, /
            var row2 = new ButtonGroup("Row2");
            row2.Add(new ButtonLeaf(_factory.CreateDigitButton("7")));
            row2.Add(new ButtonLeaf(_factory.CreateDigitButton("8")));
            row2.Add(new ButtonLeaf(_factory.CreateDigitButton("9")));
            row2.Add(new ButtonLeaf(_factory.CreateOperatorButton("/", "/")));
            layout.Add(row2);

            // Row 3: 4, 5, 6, *
            var row3 = new ButtonGroup("Row3");
            row3.Add(new ButtonLeaf(_factory.CreateDigitButton("4")));
            row3.Add(new ButtonLeaf(_factory.CreateDigitButton("5")));
            row3.Add(new ButtonLeaf(_factory.CreateDigitButton("6")));
            row3.Add(new ButtonLeaf(_factory.CreateOperatorButton("*", "*")));
            layout.Add(row3);

            // Row 4: 1, 2, 3, -
            var row4 = new ButtonGroup("Row4");
            row4.Add(new ButtonLeaf(_factory.CreateDigitButton("1")));
            row4.Add(new ButtonLeaf(_factory.CreateDigitButton("2")));
            row4.Add(new ButtonLeaf(_factory.CreateDigitButton("3")));
            row4.Add(new ButtonLeaf(_factory.CreateOperatorButton("-", "-")));
            layout.Add(row4);

            // Row 5: 0, ., +, =
            var row5 = new ButtonGroup("Row5");
            row5.Add(new ButtonLeaf(_factory.CreateDigitButton("0")));
            row5.Add(new ButtonLeaf(_factory.CreateDigitButton(".")));
            row5.Add(new ButtonLeaf(_factory.CreateOperatorButton("+", "+")));
            row5.Add(new ButtonLeaf(_factory.CreateEqualsButton()));
            layout.Add(row5);

            return layout;
        }

        /// <summary>
        /// Build extended calculator layout with composite structure
        /// </summary>
        public ButtonGroup BuildExtendedLayout()
        {
            var layout = new ButtonGroup("ExtendedLayout");

            // Row 1: Trigonometric functions
            var row1 = new ButtonGroup("Row1");
            row1.Add(new ButtonLeaf(_factory.CreateMathFunctionButton("sin", "sin(")));
            row1.Add(new ButtonLeaf(_factory.CreateMathFunctionButton("cos", "cos(")));
            row1.Add(new ButtonLeaf(_factory.CreateMathFunctionButton("tan", "tg(")));
            row1.Add(new ButtonLeaf(_factory.CreateMathFunctionButton("ctg", "ctg(")));
            row1.Add(new ButtonLeaf(_factory.CreateMathFunctionButton("lg", "lg(")));
            layout.Add(row1);

            // Row 2: Control and operators
            var row2 = new ButtonGroup("Row2");
            row2.Add(new ButtonLeaf(_factory.CreateClearButton()));
            row2.Add(new ButtonLeaf(_factory.CreateBackspaceButton()));
            row2.Add(new ButtonLeaf(_factory.CreateOperatorButton("√", "sqrt(")));
            row2.Add(new ButtonLeaf(_factory.CreateOperatorButton("^", "^")));
            row2.Add(new ButtonLeaf(_factory.CreateConstantButton("π", "pi")));
            layout.Add(row2);

            // Row 3: 7, 8, 9, /, e
            var row3 = new ButtonGroup("Row3");
            row3.Add(new ButtonLeaf(_factory.CreateDigitButton("7")));
            row3.Add(new ButtonLeaf(_factory.CreateDigitButton("8")));
            row3.Add(new ButtonLeaf(_factory.CreateDigitButton("9")));
            row3.Add(new ButtonLeaf(_factory.CreateOperatorButton("/", "/")));
            row3.Add(new ButtonLeaf(_factory.CreateConstantButton("e", "e")));
            layout.Add(row3);

            // Row 4: 4, 5, 6, *, (
            var row4 = new ButtonGroup("Row4");
            row4.Add(new ButtonLeaf(_factory.CreateDigitButton("4")));
            row4.Add(new ButtonLeaf(_factory.CreateDigitButton("5")));
            row4.Add(new ButtonLeaf(_factory.CreateDigitButton("6")));
            row4.Add(new ButtonLeaf(_factory.CreateOperatorButton("*", "*")));
            row4.Add(new ButtonLeaf(_factory.CreateParenthesisButton("(")));
            layout.Add(row4);

            // Row 5: 1, 2, 3, -, )
            var row5 = new ButtonGroup("Row5");
            row5.Add(new ButtonLeaf(_factory.CreateDigitButton("1")));
            row5.Add(new ButtonLeaf(_factory.CreateDigitButton("2")));
            row5.Add(new ButtonLeaf(_factory.CreateDigitButton("3")));
            row5.Add(new ButtonLeaf(_factory.CreateOperatorButton("-", "-")));
            row5.Add(new ButtonLeaf(_factory.CreateParenthesisButton(")")));
            layout.Add(row5);

            // Row 6: 0, ., +, =
            var row6 = new ButtonGroup("Row6");
            row6.Add(new ButtonLeaf(_factory.CreateDigitButton("0")));
            row6.Add(new ButtonLeaf(_factory.CreateDigitButton(".")));
            row6.Add(new ButtonLeaf(_factory.CreateOperatorButton("+", "+")));
            row6.Add(new ButtonLeaf(_factory.CreateEqualsButton()));
            layout.Add(row6);

            return layout;
        }
    }
}
