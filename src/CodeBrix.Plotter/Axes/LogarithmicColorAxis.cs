// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogarithmicColorAxis.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using CodeBrix.Plotter.Axes.Rendering;

namespace CodeBrix.Plotter.Axes; //was previously: OxyPlot.Axes;

/// <summary>
/// Represents a logarithmic color axis.
/// </summary>
public class LogarithmicColorAxis : LogarithmicAxis, INumericColorAxis
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LinearColorAxis" /> class.
    /// </summary>
    public LogarithmicColorAxis()
    {
        this.Position = AxisPosition.None;
        this.AxisDistance = 20;

        this.IsPanEnabled = false;
        this.IsZoomEnabled = false;
        this.Palette = PlotterPalettes.Viridis();

        this.LowColor = PlotterColors.Undefined;
        this.HighColor = PlotterColors.Undefined;
        this.InvalidNumberColor = PlotterColors.Gray;
    }

    /// <inheritdoc />
    public PlotterColor InvalidNumberColor { get; set; }

    /// <inheritdoc />
    public PlotterColor HighColor { get; set; }

    /// <inheritdoc />
    public PlotterColor LowColor { get; set; }

    /// <inheritdoc />
    public PlotterPalette Palette { get; set; }

    /// <inheritdoc />
    public bool RenderAsImage { get; set; }

    /// <inheritdoc />
    public override bool IsXyAxis()
    {
        return false;
    }

    /// <inheritdoc />
    public override void Render(IRenderContext rc, int pass)
    {
        var renderer = new NumericColorAxisRenderer<LogarithmicColorAxis>(rc, this.PlotModel);
        renderer.Render(this, pass);
    }

    PlotterColor IColorAxis.GetColor(int paletteIndex)
    {
        return this.GetColor(paletteIndex);
    }

    /// <inheritdoc />
    public int GetPaletteIndex(double value)
    {
        if (double.IsNaN(value))
        {
            return int.MinValue;
        }

        if (!this.LowColor.IsUndefined() && value < this.ClipMinimum)
        {
            return 0;
        }

        if (!this.HighColor.IsUndefined() && value > this.ClipMaximum)
        {
            return this.Palette.Colors.Count + 1;
        }

        var logValue = this.PreTransform(value);

        int index = 1 + (int)((logValue - this.LogClipMinimum) / (this.LogClipMaximum - this.LogClipMinimum) * this.Palette.Colors.Count);

        if (index < 1)
        {
            index = 1;
        }

        if (index > this.Palette.Colors.Count)
        {
            index = this.Palette.Colors.Count;
        }

        return index;
    }
}
