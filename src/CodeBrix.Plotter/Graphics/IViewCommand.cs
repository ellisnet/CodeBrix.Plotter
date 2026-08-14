// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IViewCommand.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Specifies functionality to execute a command on a view.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CodeBrix.Plotter; //was previously: OxyPlot;

/// <summary>
/// Specifies functionality to execute a command on a view.
/// </summary>
public interface IViewCommand
{
    /// <summary>
    /// Executes the command on the specified plot.
    /// </summary>
    /// <param name="view">The view.</param>
    /// <param name="controller">The controller.</param>
    /// <param name="args">The <see cref="PlotterInputEventArgs" /> instance containing the event data.</param>
    void Execute(IView view, IController controller, PlotterInputEventArgs args);
}
