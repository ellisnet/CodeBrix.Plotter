// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Example.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using CodeBrix.Plotter;

namespace CodeBrix.Plotter.Tests.ExampleLibrary; //was previously: ExampleLibrary;

/// <summary>
/// Represents an example.
/// </summary>
public class Example
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Example"/> class.
    /// </summary>
    /// <param name="model">The model.</param>
    /// <param name="controller">The controller.</param>
    public Example(PlotModel model, IPlotController controller = null)
    {
        this.Model = model;
        this.Controller = controller;
    }

    /// <summary>
    /// Gets the controller.
    /// </summary>
    /// <value>
    /// The controller.
    /// </value>
    public IPlotController Controller { get; private set; }

    /// <summary>
    /// Gets the model.
    /// </summary>
    /// <value>
    /// The model.
    /// </value>
    public PlotModel Model { get; private set; }
}
