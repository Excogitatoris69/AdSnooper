using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AdSnooperGui.common
{
    public class GenericCommand : ICommand
    {

        private Action<object> _executeAction;
        private Func<object, bool> _canExecuteFunction;

        public GenericCommand(Action<object> executeAction, Func<object, bool> canExecuteFunction)
        {
            _executeAction = executeAction;
            _canExecuteFunction = canExecuteFunction;
        }
        public GenericCommand(Action<object> executeAction, Func<bool> canExecuteFunction)
        {
            _executeAction = executeAction;
            _canExecuteFunction = (x) => canExecuteFunction();
        }

        public GenericCommand(Action executeAction, Func<bool> canExecuteFunction)
        {
            _executeAction = (x) => { executeAction(); };
            _canExecuteFunction = (x) => canExecuteFunction();
        }
        public GenericCommand(Action<object> executeAction)
        {
            _executeAction = executeAction;
            _canExecuteFunction = (x) => { return true; };
        }
        public GenericCommand(Action executeAction)
        {
            _executeAction = (x) => { executeAction(); };
            _canExecuteFunction = (x) => { return true; };
        }


        public bool CanExecute(object parameter)
        {
            return _canExecuteFunction(parameter);
        }

        public void Execute(object parameter)
        {
            _executeAction(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }


    }
}
