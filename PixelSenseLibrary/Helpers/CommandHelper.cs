using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace PixelSenseLibrary.Helpers
{
    /// <summary>
    /// ICommand Extension Methods
    /// </summary>
    public static class CommandHelper
    {
        public static void RaiseCommand(ICommand refCommand, object objArgs, IInputElement eltTriggering,
                                      IInputElement eltTarget)
        {
            if (refCommand == null)
                return;
            if (refCommand is RoutedCommand)
            {
                IInputElement target = eltTarget;
                var routedCommand = (RoutedCommand)refCommand;
                if (eltTarget == null)
                    target = eltTriggering;
                if (!routedCommand.CanExecute(objArgs, target))
                    return;
                routedCommand.Execute(objArgs, target);
            }
            else
            {
                if (!refCommand.CanExecute(objArgs))
                    return;
                refCommand.Execute(objArgs);
            }
        }
    }
}
