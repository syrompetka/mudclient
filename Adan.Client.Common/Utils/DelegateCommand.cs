// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DelegateCommand.cs" company="Adamand MUD">
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
    /// Base class for all commands.
    /// </summary>
    public class DelegateCommand : ICommand
    {
        private readonly Action<object> _method;
        private bool _canExecute;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand"/> class.
        /// </summary>
        /// <param name="method">The method.</param>
        public DelegateCommand([NotNull] Action<object> method)
            : this(method, true)
        {
            Assert.ArgumentNotNull(method, "method");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand"/> class.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="canExecute">The can execute.</param>
        public DelegateCommand([NotNull] Action<object> method, bool canExecute)
        {
            Assert.ArgumentNotNull(method, "method");

            _method = method;
            _canExecute = canExecute;
        }

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Gets or sets a value indicating whether this command can be executed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this command can be executed; otherwise, <c>false</c>.
        /// </value>
        public bool CanBeExecuted
        {
            get
            {
                return _canExecute;
            }

            set
            {
                _canExecute = value;
                RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        public bool CanExecute([CanBeNull]object parameter)
        {
            return _canExecute;
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        public void Execute([CanBeNull]object parameter)
        {
            _method.Invoke(parameter);
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
