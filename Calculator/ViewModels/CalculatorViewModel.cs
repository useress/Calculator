using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Calculator.Models;

namespace Calculator.ViewModels
{
    public class CalculatorViewModel : INotifyPropertyChanged
    {
        private readonly CalculatorModel _model = new CalculatorModel();

        private string _number1;
        private string _number2;
        private string _result;
        private string _operation;

        public string Number1
        {
            get => _number1;
            set { _number1 = value; OnPropertyChanged(); }
        }

        public string Number2
        {
            get => _number2;
            set { _number2 = value; OnPropertyChanged(); }
        }

        public string Result
        {
            get => _result;
            set { _result = value; OnPropertyChanged(); }
        }

        public string Operation
        {
            get => _operation;
            set { _operation = value; OnPropertyChanged(); }
        }

        public ICommand CalculateCommand { get; }

        public CalculatorViewModel()
        {
            CalculateCommand = new RelayCommand(Calculate);
        }

        private void Calculate(object obj)
        {
            double a = double.Parse(Number1);
            double b = double.Parse(Number2);

            double res = _model.Calculate(a, b, Operation);

            Result = res.ToString();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}