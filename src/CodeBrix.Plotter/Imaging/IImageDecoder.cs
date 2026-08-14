// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IImageDecoder.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Specifies functionality to decode an image.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CodeBrix.Plotter; //was previously: OxyPlot;

/// <summary>
/// Specifies functionality to decode an image.
/// </summary>
public interface IImageDecoder
{
    /// <summary>
    /// Gets information about the image in the specified byte array.
    /// </summary>
    /// <param name="bytes">The image data.</param>
    /// <returns>An <see cref="PlotterImageInfo" /> structure.</returns>
    PlotterImageInfo GetImageInfo(byte[] bytes);

    /// <summary>
    /// Decodes an image from the specified byte array.
    /// </summary>
    /// <param name="bytes">The image data.</param>
    /// <returns>The 32-bit pixel data. The indexing is [x,y] where [0,0] is top-left.</returns>
    PlotterColor[,] Decode(byte[] bytes);
}
