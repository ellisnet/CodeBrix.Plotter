// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TrackerEventArgs.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Provides data for the tracker event.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace CodeBrix.Plotter; //was previously: OxyPlot;

/// <summary>
/// Provides data for the tracker event.
/// </summary>
public class TrackerEventArgs : EventArgs
{
    /// <summary>
    /// Gets or sets the hit result.
    /// </summary>
    /// <value>The hit result.</value>
    public TrackerHitResult HitResult { get; set; }
}
