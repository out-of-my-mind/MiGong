using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MiGongWpf.Commands
{
    public class DelegateCommand :ICommand
    {
        public event EventHandler CanExecuteChanged;
        private Action<object> _executeAction { set; get; }
        public DelegateCommand(Action<object> execute)
        {
            _executeAction = execute;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter) => _executeAction(parameter);

    }
}
