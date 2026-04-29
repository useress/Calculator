using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using Calculator.Models.Buttons;
using Calculator.Models.Buttons.Adapters;
using Calculator.Models.Buttons.Bridge;
using Calculator.Models.Buttons.Composite;
using Calculator.Models.Buttons.Decorators;
using Calculator.Models.Commands;
using Calculator.Models.Facades;
using System.Collections.Generic;
using Calculator.ViewModels;

namespace Calculator.Views
{
    public partial class MainWindow : Window
    {
        private ButtonFactory _buttonFactory = null!;
        private CompositeButtonLayoutBuilder _layoutBuilder = null!;
        private CalculatorViewModelRefactored _viewModel = null!;
        private ButtonGroup _normalLayout = null!;
        private ButtonGroup _extendedLayout = null!;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Build runtime object graph through facade to keep UI layer simple.
            var model = new Calculator.Models.EngineeringCalculatorModel();
            var state = new CalculatorState();
            var invoker = new CommandInvoker();
            var facade = new CalculatorFacade(model, state, invoker);

            _viewModel = new CalculatorViewModelRefactored(facade);
            DataContext = _viewModel;

            _buttonFactory = new ButtonFactory(
                _viewModel.InputCommand,
                new ClearCommandWrapper(_viewModel.ClearCommand, this),
                _viewModel.BackspaceCommand,
                _viewModel.CalculateCommand
            );
            facade.SetButtonFactory(_buttonFactory);

            // Build layouts through Composite pattern.
            _layoutBuilder = new CompositeButtonLayoutBuilder(_buttonFactory);
            _normalLayout = _layoutBuilder.BuildNormalLayout();
            _extendedLayout = _layoutBuilder.BuildExtendedLayout();

            PopulateNormalLayout();
            PopulateExtendedLayout();

            // Set focus to the window to ensure keyboard input works
            this.Focus();
            this.Focusable = true;
        }

        /// <summary>
        /// Wrapper command that clears and maintains keyboard focus
        /// </summary>
        private class ClearCommandWrapper : ICommand
        {
            private readonly ICommand _innerCommand;
            private readonly Window _window;

            public ClearCommandWrapper(ICommand innerCommand, Window window)
            {
                _innerCommand = innerCommand;
                _window = window;
            }

            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }

            public bool CanExecute(object parameter) => _innerCommand.CanExecute(parameter);

            public void Execute(object parameter)
            {
                _innerCommand.Execute(parameter);
                // Restore focus to window after clearing to ensure keyboard input works
                _window.Focus();
                System.Windows.Input.Keyboard.Focus(_window);
            }
        }

        /// <summary>
        /// Populate the normal calculator layout grid with buttons
        /// </summary>
        private void PopulateNormalLayout()
        {
            NormalButtonsGrid.Children.Clear();

            foreach (var button in FlattenButtons(_normalLayout))
            {
                var xamlButton = CreateXamlButton(button, isExtendedLayout: false);
                NormalButtonsGrid.Children.Add(xamlButton);
            }
        }

        /// <summary>
        /// Populate the extended calculator layout grid with buttons
        /// </summary>
        private void PopulateExtendedLayout()
        {
            ExtendedButtonsGrid.Children.Clear();

            foreach (var button in FlattenButtons(_extendedLayout))
            {
                var xamlButton = CreateXamlButton(button, isExtendedLayout: true);
                ExtendedButtonsGrid.Children.Add(xamlButton);
            }
        }

        /// <summary>
        /// Create a WPF Button from a Button model
        /// </summary>
        private System.Windows.Controls.Button CreateXamlButton(Models.Buttons.Button buttonModel, bool isExtendedLayout)
        {
            // Decorator pattern: add runtime visual behavior without touching base button types.
            var decoratedButton = ApplyDecorators(buttonModel, isExtendedLayout);

            // Adapter pattern: convert button model into UI-friendly contract.
            var adapter = ButtonAdapterFactory.CreateAdapter(decoratedButton, ButtonAdapterFactory.AdapterType.Wpf);

            // Bridge pattern: keep rendering/action pipeline abstract from concrete UI platform.
            var platformAwareButton = new PlatformAwareButton(decoratedButton, new WpfButtonImplementation());
            platformAwareButton.SetProperty("Layout", isExtendedLayout ? "Extended" : "Normal");
            platformAwareButton.Render();

            var button = new System.Windows.Controls.Button
            {
                Content = adapter.GetDisplayText(),
                Command = adapter.GetCommand(),
                CommandParameter = adapter.GetInputValue(),
                Margin = new Thickness(double.Parse(adapter.GetMargin())),
                Style = (System.Windows.Style)this.Resources[adapter.GetStyleIdentifier()],
                // Prevent buttons from taking keyboard focus
                Focusable = false
            };

            return button;
        }

        private Models.Buttons.Button ApplyDecorators(Models.Buttons.Button button, bool isExtendedLayout)
        {
            Models.Buttons.Button decorated = button;

            if (isExtendedLayout)
            {
                decorated = new MarginButtonDecorator(decorated, "2");
            }

            // Sized decorator is used to keep optional size metadata for future dynamic layout scenarios.
            var sizedDecorator = new SizedButtonDecorator(decorated, isExtendedLayout ? "56" : "64", "60");
            _ = sizedDecorator.GetDimensions();
            decorated = sizedDecorator;

            return decorated;
        }

        private IEnumerable<Models.Buttons.Button> FlattenButtons(ButtonGroup root)
        {
            foreach (var component in root.GetComponents())
            {
                foreach (var button in FlattenComponent(component))
                {
                    yield return button;
                }
            }
        }

        private IEnumerable<Models.Buttons.Button> FlattenComponent(IButtonComponent component)
        {
            if (component is ButtonLeaf leaf)
            {
                yield return leaf.GetButton();
                yield break;
            }

            if (component is ButtonGroup group)
            {
                foreach (var child in group.GetComponents())
                {
                    foreach (var button in FlattenComponent(child))
                    {
                        yield return button;
                    }
                }
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                this.DragMove();
        }

        private void MinimizeWindow_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}