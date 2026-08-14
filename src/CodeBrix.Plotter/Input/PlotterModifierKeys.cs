// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlotterModifierKeys.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Defines the set of modifier keys.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace CodeBrix.Plotter; //was previously: OxyPlot;

/// <summary>
/// Defines the set of modifier keys.
/// </summary>
[Flags]
public enum PlotterModifierKeys
{
    /// <summary>
    /// No modifiers are pressed.
    /// </summary>
    None = 0,

    /// <summary>
    /// The Control key.
    /// </summary>
    Control = 1,

    /// <summary>
    /// The Alt/Menu key.
    /// </summary>
    Alt = 2,

    /// <summary>
    /// The Shift key.
    /// </summary>
    Shift = 4,

    /// <summary>
    /// The Windows key.
    /// </summary>
    Windows = 8
}
