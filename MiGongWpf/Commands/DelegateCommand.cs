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

        public bool CanExecute(object parameter)
        {
            if (CanExecuteFunc == null) return true;
            return this.CanExecuteFunc(parameter);
        }

        public void Execute(object parameter)
        {
            if (ExecuteAction == null) return;
            ExecuteAction(parameter);
        }

        public Action<object> ExecuteAction { set; get; }
        public Func<object, bool> CanExecuteFunc { set; get; }
    }
}
