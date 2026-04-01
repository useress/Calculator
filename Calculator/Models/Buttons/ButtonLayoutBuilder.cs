using System.Collections.Generic;

namespace Calculator.Models.Buttons
{
    /// <summary>
    /// Builder class for constructing button layouts using the factory
    /// Implements the Builder Pattern combined with Factory Pattern
    /// </summary>
    public class ButtonLayoutBuilder
    {
        private readonly ButtonFactory _factory;
        private readonly List<Button> _normalLayout;
        private readonly List<Button> _extendedLayout;

        public ButtonLayoutBuilder(ButtonFactory factory)
        {
            _factory = factory;
            _normalLayout = new List<Button>();
            _extendedLayout = new List<Button>();
        }

        /// <summary>
        /// Build the normal calculator layout (4 columns, 5 rows)
        /// </summary>
        public ButtonLayoutBuilder BuildNormalLayout()
        {
            _normalLayout.Clear();

            // Row 1: Control buttons
            _normalLayout.Add(_factory.CreateClearButton());
            _normalLayout.Add(_factory.CreateBackspaceButton());
            _normalLayout.Add(_factory.CreateOperatorButton("^", "^"));
            _normalLayout.Add(_factory.CreateOperatorButton("√", "sqrt("));

            // Row 2: 7, 8, 9, /
            _normalLayout.Add(_factory.CreateDigitButton("7"));
            _normalLayout.Add(_factory.CreateDigitButton("8"));
            _normalLayout.Add(_factory.CreateDigitButton("9"));
            _normalLayout.Add(_factory.CreateOperatorButton("/", "/"));

            // Row 3: 4, 5, 6, *
            _normalLayout.Add(_factory.CreateDigitButton("4"));
            _normalLayout.Add(_factory.CreateDigitButton("5"));
            _normalLayout.Add(_factory.CreateDigitButton("6"));
            _normalLayout.Add(_factory.CreateOperatorButton("*", "*"));

            // Row 4: 1, 2, 3, -
            _normalLayout.Add(_factory.CreateDigitButton("1"));
            _normalLayout.Add(_factory.CreateDigitButton("2"));
            _normalLayout.Add(_factory.CreateDigitButton("3"));
            _normalLayout.Add(_factory.CreateOperatorButton("-", "-"));

            // Row 5: 0, ., +, =
            _normalLayout.Add(_factory.CreateDigitButton("0"));
            _normalLayout.Add(_factory.CreateDigitButton("."));
            _normalLayout.Add(_factory.CreateOperatorButton("+", "+"));
            _normalLayout.Add(_factory.CreateEqualsButton());

            return this;
        }

        /// <summary>
        /// Build the extended calculator layout with scientific functions (6 rows, 5 columns)
        /// </summary>
        public ButtonLayoutBuilder BuildExtendedLayout()
        {
            _extendedLayout.Clear();

            // Row 1: Trigonometric functions
            _extendedLayout.Add(CreateWithMargin(_factory.CreateMathFunctionButton("sin", "sin("), "2"));
            _extendedLayout.Add(CreateWithMargin(_factory.CreateMathFunctionButton("cos", "cos("), "2"));
            _extendedLayout.Add(CreateWithMargin(_factory.CreateMathFunctionButton("tan", "tg("), "2"));
            _extendedLayout.Add(CreateWithMargin(_factory.CreateMathFunctionButton("ctg", "ctg("), "2"));
            _extendedLayout.Add(CreateWithMargin(_factory.CreateMathFunctionButton("lg", "lg("), "2"));

            // Row 2: Control and operators
            _extendedLayout.Add(CreateWithMargin(_factory.CreateClearButton(), "2"));
            _extendedLayout.Add(CreateWithMargin(_factory.CreateBackspaceButton(), "2"));
            _extendedLayout.Add(CreateWithMargin(_factory.CreateOperatorButton("√", "sqrt("), "2"));
            _extendedLayout.Add(CreateWithMargin(_factory.CreateOperatorButton("^", "^"), "2"));
            _extendedLayout.Add(CreateWithMargin(_factory.CreateConstantButton("π", "pi"), "2"));

            // Row 3: 7, 8, 9, /, e
            _extendedLayout.Add(CreateWithMargin(_factory.CreateDigitButton("7"), "2"));
            _extendedLayout.Add(CreateWithMargin(_factory.CreateDigitButton("8"), "2"));
            _extendedLayout.Add(CreateWithMargin(_factory.CreateDigitButton("9"), "2"));
            _extendedLayout.Add(CreateWithMargin(_factory.CreateOperatorButton("/", "/"), "2"));
            _extendedLayout.Add(CreateWithMargin(_factory.CreateConstantButton("e", "e"), "2"));

            // Row 4: 4, 5, 6, *, (
            _extendedLayout.Add(CreateWithMargin(_factory.CreateDigitButton("4"), "2"));
            _extendedLayout.Add(CreateWithMargin(_factory.CreateDigitButton("5"), "2"));
            _extendedLayout.Add(CreateWithMargin(_factory.CreateDigitButton("6"), "2"));
            _extendedLayout.Add(CreateWithMargin(_factory.CreateOperatorButton("*", "*"), "2"));
            _extendedLayout.Add(CreateWithMargin(_factory.CreateParenthesisButton("("), "2"));

            // Row 5: 1, 2, 3, -, )
            _extendedLayout.Add(CreateWithMargin(_factory.CreateDigitButton("1"), "2"));
            _extendedLayout.Add(CreateWithMargin(_factory.CreateDigitButton("2"), "2"));
            _extendedLayout.Add(CreateWithMargin(_factory.CreateDigitButton("3"), "2"));
            _extendedLayout.Add(CreateWithMargin(_factory.CreateOperatorButton("-", "-"), "2"));
            _extendedLayout.Add(CreateWithMargin(_factory.CreateParenthesisButton(")"), "2"));

            // Row 6: 0, ., =, +
            _extendedLayout.Add(CreateWithMargin(_factory.CreateDigitButton("0"), "2"));
            _extendedLayout.Add(CreateWithMargin(_factory.CreateDigitButton("."), "2"));
            _extendedLayout.Add(CreateWithMargin(_factory.CreateEqualsButton(), "2"));
            _extendedLayout.Add(CreateWithMargin(_factory.CreateOperatorButton("+", "+"), "2"));

            return this;
        }

        /// <summary>
        /// Get the normal layout buttons
        /// </summary>
        public List<Button> GetNormalLayout() => _normalLayout;

        /// <summary>
        /// Get the extended layout buttons
        /// </summary>
        public List<Button> GetExtendedLayout() => _extendedLayout;

        /// <summary>
        /// Get a button by its display text from normal layout
        /// </summary>
        public Button GetButtonFromNormalLayout(string displayText)
        {
            return _normalLayout.Find(b => b.DisplayText == displayText);
        }

        /// <summary>
        /// Get a button by its display text from extended layout
        /// </summary>
        public Button GetButtonFromExtendedLayout(string displayText)
        {
            return _extendedLayout.Find(b => b.DisplayText == displayText);
        }

        /// <summary>
        /// Helper method to create a button with a specific margin
        /// </summary>
        private Button CreateWithMargin(Button button, string margin)
        {
            button.Margin = margin;
            return button;
        }
    }
}
