// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlotterTouchEventArgs.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Provides data for touch events.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CodeBrix.Plotter; //was previously: OxyPlot;

/// <summary>
/// Provides data for touch events.
/// </summary>
public class PlotterTouchEventArgs : PlotterInputEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PlotterTouchEventArgs" /> class.
    /// </summary>
    public PlotterTouchEventArgs()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlotterTouchEventArgs" /> class.
    /// </summary>
    /// <param name="currentTouches">The current touches.</param>
    /// <param name="previousTouches">The previous touches.</param>
    public PlotterTouchEventArgs(ScreenPoint[] currentTouches, ScreenPoint[] previousTouches)
    {
        this.CurrentTouches = currentTouches;
        this.PreviousTouches = previousTouches;
        this.Position = currentTouches[0];

        if (currentTouches.Length == previousTouches.Length)
        {
            this.DeltaTranslation = currentTouches[0] - previousTouches[0];
        }

        double scale = 1;
        if (currentTouches.Length > 1 && currentTouches.Length == previousTouches.Length)
        {
            var currentDistance = (currentTouches[1] - currentTouches[0]).Length;
            var previousDistance = (previousTouches[1] - previousTouches[0]).Length;
            scale = currentDistance / previousDistance;

            if (scale < 0.5)
            {
                scale = 0.5;
            }

            if (scale > 2)
            {
                scale = 2;
            }
        }

        this.DeltaScale = new ScreenVector(scale, scale);
    }

    /// <summary>
    /// Gets or sets the position of the touch.
    /// </summary>
    /// <value>The position.</value>
    public ScreenPoint Position { get; set; }

    /// <summary>
    /// Gets or sets the relative change in scale.
    /// </summary>
    /// <value>The scale change.</value>
    public ScreenVector DeltaScale { get; set; }

    /// <summary>
    /// Gets or sets the change in x and y direction.
    /// </summary>
    /// <value>The translation.</value>
    public ScreenVector DeltaTranslation { get; set; }

    /// <summary>
    /// Gets or sets the positions of all current touches, in the order the touches went down.
    /// </summary>
    /// <value>The current touch positions, or <c>null</c> when the arguments were not created from touch arrays.</value>
    /// <remarks>
    /// Set by the <see cref="PlotterTouchEventArgs(ScreenPoint[], ScreenPoint[])" /> constructor. A manipulator can use
    /// these to find the centre of a pinch; <see cref="Position" /> is always the first touch only.
    /// </remarks>
    public ScreenPoint[] CurrentTouches { get; set; }

    /// <summary>
    /// Gets or sets the positions of all touches before this event, in the order the touches went down.
    /// </summary>
    /// <value>The previous touch positions, or <c>null</c> when the arguments were not created from touch arrays.</value>
    /// <remarks>
    /// Set by the <see cref="PlotterTouchEventArgs(ScreenPoint[], ScreenPoint[])" /> constructor. When its length differs
    /// from <see cref="CurrentTouches" />, a touch went down or up and the event carries no translation or scale.
    /// </remarks>
    public ScreenPoint[] PreviousTouches { get; set; }
}
