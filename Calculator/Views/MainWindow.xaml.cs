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

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Initialize button factory with commands from ViewModel
            var viewModel = this.DataContext as CalculatorViewModel;
            if (viewModel != null)
            {
                _buttonFactory = new ButtonFactory(
                    viewModel.InputCommand,
                    viewModel.ClearCommand,
                    viewModel.BackspaceCommand,
                    viewModel.CalculateCommand
                );

                // Build layouts using factory
                _layoutBuilder = new ButtonLayoutBuilder(_buttonFactory);
                _layoutBuilder.BuildNormalLayout();
                _layoutBuilder.BuildExtendedLayout();

                // Populate grids with buttons
                PopulateNormalLayout();
                PopulateExtendedLayout();
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
                Style = (System.Windows.Style)this.Resources[config.Style]
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