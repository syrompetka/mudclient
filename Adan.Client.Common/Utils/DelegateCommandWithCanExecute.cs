// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DelegateCommandWithCanExecute.cs" company="Adamand MUD">
//   Copyright (c) Adamant MUD
// </copyright>
// <summary>
//   Defines the DelegateCommand type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Adan.Client.Common.Utils
{
    using System;
    using System.Windows.Input;

    using CSLib.Net.Annotations;
    using CSLib.Net.Diagnostics;

    /// <summary>
    /// Command that can handle whether it can be executed or not.
    /// </summary>
    public class DelegateCommandWithCanExecute : ICommand
    {
        private readonly Action<object> _method;
        private readonly Func<object, bool> _canExecuteMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommandWithCanExecute"/> class.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="canExecuteMethod">The can execute method.</param>
        public DelegateCommandWithCanExecute([NotNull] Action<object> method, [NotNull] Func<object, bool> canExecuteMethod)
        {
            Assert.ArgumentNotNull(method, "method");
            Assert.ArgumentNotNull(canExecuteMethod, "canExecuteMethod");

            _method = method;
            _canExecuteMethod = canExecuteMethod;
        }

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        public bool CanExecute([CanBeNull]object parameter)
        {
            return _canExecuteMethod(parameter);
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        public void Execute([CanBeNull]object parameter)
        {
            _method.Invoke(parameter);
        }

        /// <summary>
        /// Updates the can execute by invoking <see cref="CanExecuteChanged"/> event.
        /// </summary>
        public void UpdateCanExecute()
        {
            RaiseCanExecuteChanged();
        }

        private void RaiseCanExecuteChanged()
        {
            var canExecuteChanged = CanExecuteChanged;

            if (canExecuteChanged != null)
            {
                canExecuteChanged(this, EventArgs.Empty);
            }
        }
    }
}
