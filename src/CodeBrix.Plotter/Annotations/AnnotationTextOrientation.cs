// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnnotationTextOrientation.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Specifies the orientation of the text in an annotation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CodeBrix.Plotter.Annotations; //was previously: OxyPlot.Annotations;

/// <summary>
/// Specifies the orientation of the text in an annotation.
/// </summary>
public enum AnnotationTextOrientation
{
    /// <summary>
    /// Horizontal text.
    /// </summary>
    Horizontal,

    /// <summary>
    /// Vertical text.
    /// </summary>
    Vertical,

    /// <summary>
    /// Oriented along the line.
    /// </summary>
    AlongLine
}
