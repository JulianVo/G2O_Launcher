// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="ProxyCommand.cs" company="Gothic Online Project">
// //   
// // </copyright>
// // --------------------------------------------------------------------------------------------------------------------
namespace G2O_Launcher.Commands
{
    #region

    using System;
    using System.Windows.Input;

    using G2O_Launcher.Annotations;

    #endregion

    internal class ProxyCommand : ICommand
    {
        private readonly Action<object> _Action;

        private readonly Func<object, bool> _CanExecuteCheck;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProxyCommand" /> class that can allways be executed.
        /// </summary>
        /// <param name="action">Action that should be invoked when the command is executed.</param>
        public ProxyCommand([NotNull] Action<object> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            this._CanExecuteCheck = ob => true;
            this._Action = action;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ProxyCommand" /> with a check if the command can be executed and
        ///     a <see cref="Action" /> that should be executed i the command is executed.
        /// </summary>
        /// <param name="canExecuteCheck">Function that can be used to check if the command can be executed.</param>
        /// <param name="action">Action that should be invoked when the command is executed.</param>
        public ProxyCommand(Func<object, bool> canExecuteCheck, [NotNull] Action<object> action)
        {
            if (canExecuteCheck == null)
            {
                throw new ArgumentNullException(nameof(canExecuteCheck));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            this._CanExecuteCheck = canExecuteCheck;
            this._Action = action;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }

            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public bool CanExecute(object parameter)
        {
            return this._CanExecuteCheck(parameter);
        }

        public void Execute(object parameter)
        {
            this._Action(parameter);
        }
    }
}