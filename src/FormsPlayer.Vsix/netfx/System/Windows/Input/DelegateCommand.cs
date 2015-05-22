#region BSD License
/* 
Copyright (c) 2010, NETFx
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.

* Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.

* Neither the name of Clarius Consulting nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Input;

/// <summary>
/// Generic <see cref="ICommand"/> implementation that receives delegates/lambdas 
/// for its implementation.
/// </summary>
[DebuggerStepThrough]
internal class DelegateCommand : ICommand
{
    private Func<bool> canExecute;
    private Action execute;

    /// <summary>
    /// See <see cref="ICommand.CanExecuteChanged"/>.
    /// </summary>
    public event EventHandler CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    /// <summary>
    /// Initializes the command with the action to execute when invoked.
    /// </summary>
    public DelegateCommand(Action execute)
        : this(execute, () => true)
    {
    }

    /// <summary>
    /// Initializes the command with the action to execute when invoked 
    /// and the condition function that determines if execution is available.
    /// </summary>
    public DelegateCommand(Action execute, Func<bool> canExecute)
    {
        Guard.NotNull(() => execute, execute);
        Guard.NotNull(() => canExecute, canExecute);

        this.execute = execute;
        this.canExecute = canExecute;
    }

    /// <summary>
    /// See <see cref="ICommand.CanExecute"/>.
    /// </summary>
    public bool CanExecute(object parameter)
    {
        return this.canExecute();
    }

    /// <summary>
    /// See <see cref="ICommand.Execute"/>.
    /// </summary>
    public void Execute(object parameter)
    {
        if (this.CanExecute(parameter))
        {
            this.execute();
        }
    }
}

/// <summary>
/// Generic <see cref="ICommand"/> implementation that receives delegates/lambdas 
/// for its implementation and has a typed argument for the command.
/// </summary>
[DebuggerStepThrough]
public class DelegateCommand<T> : ICommand
{
    private Func<T, bool> canExecute;
    private Action<T> execute;

    /// <summary>
    /// See <see cref="ICommand.CanExecuteChanged"/>.
    /// </summary>
    public event EventHandler CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    /// <summary>
    /// Initializes the command with the action to execute when invoked.
    /// </summary>
    public DelegateCommand(Action<T> execute)
        : this(execute, parameter => true)
    {
    }

    /// <summary>
    /// Initializes the command with the action to execute when invoked 
    /// and the condition function that determines if execution is available.
    /// </summary>
    public DelegateCommand(Action<T> execute, Func<T, bool> canExecute)
    {
        Guard.NotNull(() => execute, execute);
        Guard.NotNull(() => canExecute, canExecute);

        this.execute = execute;
        this.canExecute = canExecute;
    }

    /// <summary>
    /// See <see cref="ICommand.CanExecute"/>.
    /// </summary>
    public bool CanExecute(object parameter)
    {
        return this.canExecute((T)parameter);
    }

    /// <summary>
    /// See <see cref="ICommand.Execute"/>.
    /// </summary>
    public void Execute(object parameter)
    {
        if (this.CanExecute(parameter))
        {
            this.execute((T)parameter);
        }
    }
}
