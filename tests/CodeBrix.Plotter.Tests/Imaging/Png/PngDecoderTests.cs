// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PngDecoderTests.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using SilverAssertions;
using Xunit;

namespace CodeBrix.Plotter.Tests; //was previously: OxyPlot.Tests;

// ReSharper disable InconsistentNaming
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
public class PngDecoderTests
{
    [Theory]
    [InlineData(@"Imaging/TestImages/test_32bit.png", 137, 59)]
    public void Decode_32bitTestImages(string path, int w, int h)
    {
        var d = new PngDecoder();
        var pixels = d.Decode(File.ReadAllBytes(path));
        pixels.GetLength(0).Should().Be(w);
        pixels.GetLength(1).Should().Be(h);
        pixels.Should().NotBeNull();
        var e = new PngEncoder(new PngEncoderOptions());
        var encodedPixels = e.Encode(pixels);
        File.WriteAllBytes(Path.ChangeExtension(path, "out.png"), encodedPixels);
    }

    [Theory]
    [InlineData(@"Imaging/TestImages/test_8bit.png")]
    public void Decode_8bitTestImages(string path)
    {
        var d = new PngDecoder();

        // Expect exception here, 8bit pngs are not yet supported
        Assert.Throws<NotImplementedException>(() => d.Decode(File.ReadAllBytes(path)));
    }
}
