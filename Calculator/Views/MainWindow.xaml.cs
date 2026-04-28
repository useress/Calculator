using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
            _viewModel = this.DataContext as CalculatorViewModel;
            if (_viewModel != null)
            {
                _buttonFactory = new ButtonFactory(
                    _viewModel.InputCommand,
                    new ClearCommandWrapper(_viewModel.ClearCommand, this),
                    _viewModel.BackspaceCommand,
                    _viewModel.CalculateCommand
                );

                _layoutBuilder = new ButtonLayoutBuilder(_buttonFactory);
                _layoutBuilder.BuildNormalLayout();
                _layoutBuilder.BuildExtendedLayout();

                PopulateNormalLayout();
                PopulateExtendedLayout();

                this.Focus();
                this.Focusable = true;
            }

            LoadCreators();
        }

        private void LoadCreators()
        {
            var creators = new[]
            {
                new { Name = "Лобань Иван", Role = "Архитектор", Photo = "/resources/photos/creator1.jpg" },
                new { Name = "Алексеев Ярослав", Role = "Стажер frontend", Photo = "/resources/photos/creator2.jpg" },
                new { Name = "Данченко Степан", Role = "Бэкенд, тестирование", Photo = "/resources/photos/creator3.jpg" },
                new { Name = "Самсоненко Виталий", Role = "Сеньор frontend", Photo = "/resources/photos/creator4.jpg" }
            };

            foreach (var c in creators)
            {
                var border = new Border
                {
                    Background = (SolidColorBrush)FindResource("SurfaceDark"),
                    CornerRadius = new CornerRadius(12),
                    Margin = new Thickness(10),
                    Padding = new Thickness(10),
                    Width = 200
                };
                var stack = new StackPanel();

                var img = new Image
                {
                    Source = new BitmapImage(new Uri(c.Photo, UriKind.Relative)),
                    Width = 120,
                    Height = 120,
                    Stretch = Stretch.UniformToFill,
                    Margin = new Thickness(0, 0, 0, 8)
                };

                var nameBlock = new TextBlock
                {
                    Text = c.Name,
                    Foreground = (SolidColorBrush)FindResource("TextPrimary"),
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                var roleBlock = new TextBlock
                {
                    Text = c.Role,
                    Foreground = (SolidColorBrush)FindResource("TextSecondary"),
                    FontSize = 12,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                stack.Children.Add(img);
                stack.Children.Add(nameBlock);
                stack.Children.Add(roleBlock);
                border.Child = stack;
                CreatorsWrapPanel.Children.Add(border);
            }
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CalculatorPanel.Visibility = Visibility.Collapsed;
            AboutPanel.Visibility = Visibility.Visible;
        }

        private void BackToCalculator_Click(object sender, RoutedEventArgs e)
        {
            AboutPanel.Visibility = Visibility.Collapsed;
            CalculatorPanel.Visibility = Visibility.Visible;
        }

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
                _window.Focus();
                Keyboard.Focus(_window);
            }
        }

        private void PopulateNormalLayout()
        {
            NormalButtonsGrid.Children.Clear();
            foreach (var button in _layoutBuilder.GetNormalLayout())
            {
                var xamlButton = CreateXamlButton(button);
                NormalButtonsGrid.Children.Add(xamlButton);
            }
        }

        private void PopulateExtendedLayout()
        {
            ExtendedButtonsGrid.Children.Clear();
            foreach (var button in _layoutBuilder.GetExtendedLayout())
            {
                var xamlButton = CreateXamlButton(button);
                ExtendedButtonsGrid.Children.Add(xamlButton);
            }
        }

        private System.Windows.Controls.Button CreateXamlButton(Calculator.Models.Buttons.Button buttonModel)
        {
            var config = buttonModel.GetConfiguration();
            var button = new System.Windows.Controls.Button
            {
                Content = config.Display,
                Command = config.Command,
                CommandParameter = config.InputValue,
                Margin = new Thickness(double.Parse(config.Margin)),
                Style = (Style)this.Resources[config.Style],
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