// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlotterImageTests.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using SilverAssertions;
using Xunit;

namespace CodeBrix.Plotter.Tests; //was previously: OxyPlot.Tests;

// ReSharper disable InconsistentNaming
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
public class PlotterImageTests
{
    [Theory]
    [InlineData("test_8bit.png", ImageFormat.Png)]
    [InlineData("test_32bit.png", ImageFormat.Png)]
    [InlineData("test_8bit.bmp", ImageFormat.Bmp)]
    [InlineData("test_24bit.bmp", ImageFormat.Bmp)]
    [InlineData("test_32bit.bmp", ImageFormat.Bmp)]
    //// [TestCase("test.jpg", ImageFormat.Jpeg)]
    public void GetFormat_TestFiles_(string fileName, ImageFormat expectedImageFormat)
    {
        var image = new PlotterImage(File.ReadAllBytes(@"Imaging/TestImages/" + fileName));
        image.Format.Should().Be(expectedImageFormat);
        image.Width.Should().Be(137);
        image.Height.Should().Be(59);
        Math.Round(image.DpiX).Should().Be(72);
        Math.Round(image.DpiY).Should().Be(72);
    }

    [Fact]
    public void Create_SmallImageToPng()
    {
        var data = new PlotterColor[2, 4];
        data[1, 0] = PlotterColors.Blue;
        data[1, 1] = PlotterColors.Green;
        data[1, 2] = PlotterColors.Red;
        data[1, 3] = PlotterColors.White;
        data[0, 0] = PlotterColor.FromAColor(127, PlotterColors.Yellow);
        data[0, 1] = PlotterColor.FromAColor(127, PlotterColors.Orange);
        data[0, 2] = PlotterColor.FromAColor(127, PlotterColors.Pink);
        data[0, 3] = PlotterColors.Transparent;
        var img = PlotterImage.Create(data, ImageFormat.Png);
        var bytes = img.GetData();
        File.WriteAllBytes(@"Imaging/SmallImage.png", bytes);
    }

    [Fact]
    public void Create_LargeImageToPng()
    {
        int w = 266;
        int h = 40;
        var data = new PlotterColor[w, h];
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                data[x, y] = PlotterColor.FromHsv((double)x / w, 1, 1);
            }
        }

        var img = PlotterImage.Create(data, ImageFormat.Png);
        var bytes = img.GetData();
        File.WriteAllBytes(@"Imaging/LargeImage.png", bytes);
    }

    [Fact]
    public void Create_SmallImageToBmp()
    {
        var data = new PlotterColor[2, 4];
        data[1, 0] = PlotterColors.Blue;
        data[1, 1] = PlotterColors.Green;
        data[1, 2] = PlotterColors.Red;
        data[1, 3] = PlotterColors.White;
        data[0, 0] = PlotterColor.FromAColor(127, PlotterColors.Yellow);
        data[0, 1] = PlotterColor.FromAColor(127, PlotterColors.Orange);
        data[0, 2] = PlotterColor.FromAColor(127, PlotterColors.Pink);
        data[0, 3] = PlotterColors.Transparent;
        var img = PlotterImage.Create(data, ImageFormat.Bmp, new BmpEncoderOptions());
        var bytes = img.GetData();
        File.WriteAllBytes(@"Imaging/SmallImage.bmp", bytes);
    }

    [Fact]
    public void Create_Indexed8bitToBmp()
    {
        var data = new byte[,] { { 0, 1, 2, 3 }, { 4, 5, 6, 7 } };

        var palette = new PlotterColor[8];
        palette[4] = PlotterColors.Blue;
        palette[5] = PlotterColors.Green;
        palette[6] = PlotterColors.Red;
        palette[7] = PlotterColors.White;
        palette[0] = PlotterColor.FromAColor(127, PlotterColors.Yellow);
        palette[1] = PlotterColor.FromAColor(127, PlotterColors.Orange);
        palette[2] = PlotterColor.FromAColor(127, PlotterColors.Pink);
        palette[3] = PlotterColors.Transparent;
        var img = PlotterImage.Create(data, palette, ImageFormat.Bmp);
        var bytes = img.GetData();
        File.WriteAllBytes(@"Imaging/FromIndexed8.bmp", bytes);
    }

    [Fact]
    public void Discussion453825_100()
    {
        var data = new byte[100, 100];
        int k = 0;
        for (int i = 0; i < 100; i++)
        {
            for (int j = 0; j < 100; j++)
            {
                data[i, j] = (byte)(k++ % 256);
            }
        }

        var palette = PlotterPalettes.Gray(256).Colors.ToArray();
        var im = PlotterImage.Create(data, palette, ImageFormat.Bmp);
        File.WriteAllBytes(@"Imaging/Discussion453825.bmp", im.GetData());
    }

    [Fact]
    public void Discussion453825_199()
    {
        int w = 199;
        int h = 256;
        var data = new byte[h, w];
        var data2 = new byte[h * w];
        for (int i = 0; i < h; i++)
        {
            for (int j = 0; j < w; j++)
            {
                data[i, j] = (byte)(i % 256);
                data2[(i * w) + j] = (byte)(i % 256);
            }
        }

        var palette1 = PlotterPalettes.Gray(256).Colors.ToArray();
        var im1 = PlotterImage.Create(data, palette1, ImageFormat.Bmp);
        var bytes1 = im1.GetData();

        // The images should show a gradient image 199x256 - black at the bottom, white at the top
        File.WriteAllBytes(@"Imaging/Discussion453825_199a.bmp", bytes1);
    }
}
