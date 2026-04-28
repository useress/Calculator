using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Calculator.Models.Commands
{
    /// <summary>
    /// Represents the state of the calculator
    /// Used by commands to access and modify calculator state
    /// </summary>
    public class CalculatorState : INotifyPropertyChanged
    {
        private string _display = string.Empty;
        private bool _isExtendedLayout = false;

        public string Display
        {
            get => _display;
            set { _display = value; OnPropertyChanged(); }
        }

        public bool IsExtendedLayout
        {
            get => _isExtendedLayout;
            set { _isExtendedLayout = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
