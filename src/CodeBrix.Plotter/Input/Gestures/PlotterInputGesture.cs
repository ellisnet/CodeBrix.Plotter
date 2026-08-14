// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlotterInputGesture.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Provides an abstract base class for input device gestures.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace CodeBrix.Plotter; //was previously: OxyPlot;

/// <summary>
/// Provides an abstract base class for input device gestures.
/// </summary>
/// <remarks>The input gesture can be bound to a command in a <see cref="PlotController" />.</remarks>
public abstract class PlotterInputGesture : IEquatable<PlotterInputGesture>
{
    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns><c>true</c> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <c>false</c>.</returns>
    public abstract bool Equals(PlotterInputGesture other);
}
