using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using Calculator.Models.Buttons;
using Calculator.ViewModels;

namespace Calculator.Views
{
    public partial class MainWindow : Window
    {
        private ButtonFactory _buttonFactory;
        private ButtonLayoutBuilder _layoutBuilder;
        private CalculatorViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Initialize button factory with commands from ViewModel
            _viewModel = this.DataContext as CalculatorViewModel;
            if (_viewModel != null)
            {
                _buttonFactory = new ButtonFactory(
                    _viewModel.InputCommand,
                    new ClearCommandWrapper(_viewModel.ClearCommand, this),
                    _viewModel.BackspaceCommand,
                    _viewModel.CalculateCommand
                );

                // Build layouts using factory
                _layoutBuilder = new ButtonLayoutBuilder(_buttonFactory);
                _layoutBuilder.BuildNormalLayout();
                _layoutBuilder.BuildExtendedLayout();

                // Populate grids with buttons
                PopulateNormalLayout();
                PopulateExtendedLayout();
                
                // Set focus to the window to ensure keyboard input works
                this.Focus();
                this.Focusable = true;
            }
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

            foreach (var button in _layoutBuilder.GetNormalLayout())
            {
                var xamlButton = CreateXamlButton(button);
                NormalButtonsGrid.Children.Add(xamlButton);
            }
        }

        /// <summary>
        /// Populate the extended calculator layout grid with buttons
        /// </summary>
        private void PopulateExtendedLayout()
        {
            ExtendedButtonsGrid.Children.Clear();

            foreach (var button in _layoutBuilder.GetExtendedLayout())
            {
                var xamlButton = CreateXamlButton(button);
                ExtendedButtonsGrid.Children.Add(xamlButton);
            }
        }

        /// <summary>
        /// Create a WPF Button from a Button model
        /// </summary>
        private System.Windows.Controls.Button CreateXamlButton(Models.Buttons.Button buttonModel)
        {
            var config = buttonModel.GetConfiguration();

            var button = new System.Windows.Controls.Button
            {
                Content = config.Display,
                Command = config.Command,
                CommandParameter = config.InputValue,
                Margin = new Thickness(double.Parse(config.Margin)),
                Style = (System.Windows.Style)this.Resources[config.Style],
                // Prevent buttons from taking keyboard focus
                Focusable = false
            };

            return button;
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