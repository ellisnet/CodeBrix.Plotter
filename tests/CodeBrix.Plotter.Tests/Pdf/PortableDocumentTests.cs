// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PortableDocumentTests.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using SilverAssertions;
using Xunit;

namespace CodeBrix.Plotter.Tests; //was previously: OxyPlot.Tests;

// ReSharper disable InconsistentNaming
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
public class PortableDocumentTests
{
    private const string Folder = @"PortableDocumentTests/";

    [Fact]
    public void AddPage_EmptyDocument()
    {
        var doc = new PortableDocument();
        doc.AddPage(PageSize.A4);
        doc.Save(Folder + "EmptyDocument.pdf");
    }

    [Fact]
    public void AddProperties_EmptyDocument()
    {
        var doc = new PortableDocument
        {
            Title = "The title",
            Author = "the author",
            Subject = "the subject",
            Keywords = "key;words",
            Creator = "the creator",
            Producer = "the producer"
        };
        doc.AddPage(PageSize.A4);
        doc.Save(Folder + "Properties.pdf");
    }

    [Fact]
    public void DrawText_HelloWorld()
    {
        var doc = new PortableDocument();
        doc.AddPage(PageSize.A4);
        doc.SetFont("Arial", 96);
        doc.DrawText(50, 400, "Hello world!");
        doc.Save(Folder + "DrawText.pdf");
    }

    [Fact(Skip = "Not supported")]
    public void DrawText_SpecialCharacters()
    {
        var doc = new PortableDocument();
        doc.AddPage(PageSize.A4);
        doc.SetFont("Arial", 96);
        var s = "π";
        doc.DrawText(50, 400, s);
        doc.Save(Folder + "DrawText_SpecialCharacters.pdf");
        (s[0] > 255).Should().BeTrue();
    }

    [Fact]
    public void DrawText_TopLeftCoordinateSystem()
    {
        var doc = new PortableDocument();
        doc.AddPage(PageSize.A4);
        doc.Transform(1, 0, 0, -1, 0, doc.PageHeight);
        doc.SetHorizontalTextScaling(-100);

        // Note: negative font size
        doc.SetFont("Arial", -20);
        doc.DrawText(5, 25, "Hello world!");

        doc.SetColor(PlotterColors.Blue);
        doc.DrawCross(5, 25);

        doc.SetColor(PlotterColors.Blue);
        doc.SetFillColor(PlotterColors.LightBlue);
        doc.DrawEllipse(50, 100, 50, 40, true);

        doc.Save(Folder + "DrawText_TopLeftCoordinateSystem.pdf");
    }

    [Fact]
    public void DrawText_CharacterMap()
    {
        var doc = new PortableDocument();
        doc.AddPage(20 * 17, 20 * 17);
        doc.SetFont("Arial", 18);
        var sb = new StringBuilder();
        for (int i = 32; i < 256; i++)
        {
            double x = 10 + (20 * (i % 16));
            double y = doc.PageHeight - 10 - (20 * (i / 16));
            var s = ((char)i).ToString(CultureInfo.InvariantCulture);
            doc.DrawText(x, y, s);
            sb.Append(s);
            if (i % 16 == 15)
            {
                sb.AppendLine();
            }
        }

        doc.Save(Folder + "DrawText_CharacterMap.pdf");
        File.WriteAllText(Folder + "DrawText_CharacterMap.txt", sb.ToString(), Encoding.UTF8);
    }

    [Fact]
    public void MeasureText()
    {
        var doc = new PortableDocument();
        doc.AddPage(PageSize.A4);

        doc.SetFont("Arial", 96);

        var text = "qjQJKæ";
        double width, height;
        doc.MeasureText(text, out width, out height);
        double y = doc.PageHeight - 400 - height;
        doc.SetColor(0, 0, 1);
        doc.DrawRectangle(50, y, width, height);
        doc.SetFillColor(0, 0, 0);
        doc.DrawText(50, y, text);
        doc.Save(Folder + "MeasureText.pdf");
    }

    [Fact]
    public void DrawText_Rotated()
    {
        var doc = new PortableDocument();
        doc.AddPage(200, 200);
        doc.SetFont("Arial", 12);
        for (int i = 0; i <= 360; i += 30)
        {
            doc.SaveState();
            doc.RotateAt(100, 100, i);
            doc.DrawText(100, 100, "Hello world!");
            doc.RestoreState();
        }

        doc.Save(Folder + "DrawText_Rotated.pdf");
    }

    [Fact]
    public void DrawText_Rotated2()
    {
        var doc = new PortableDocument();
        doc.AddPage(200, 200);
        doc.SetFont("Arial", 12);
        for (int i = 0; i <= 360; i += 30)
        {
            doc.SaveState();
            doc.Translate(100, 100);
            doc.Rotate(i);
            doc.DrawText(0, 0, "Hello world!");
            doc.RestoreState();
        }

        doc.Save(Folder + "DrawText_Rotated2.pdf");
    }

    [Fact]
    public void DrawCircle()
    {
        var doc = new PortableDocument();
        doc.AddPage(200, 200);
        doc.SetColor(PlotterColors.Blue);
        doc.SetFillColor(PlotterColors.LightBlue);
        doc.DrawCircle(100, 100, 95, true);
        doc.DrawCircle(185, 185, 5);
        doc.Save(Folder + "DrawCircle.pdf");
    }

    [Fact]
    public void FillCircle()
    {
        var doc = new PortableDocument();
        doc.AddPage(200, 200);
        doc.SetColor(PlotterColors.Blue);
        doc.SetFillColor(PlotterColors.LightBlue);
        doc.FillCircle(100, 100, 95);
        doc.Save(Folder + "FillCircle.pdf");
    }

    [Fact]
    public void DrawEllipse()
    {
        var doc = new PortableDocument();
        doc.AddPage(200, 100);
        doc.SetColor(PlotterColors.Orange);
        doc.SetFillColor(PlotterColors.LightGreen);
        doc.DrawEllipse(5, 5, 190, 90, true);
        doc.DrawEllipse(175, 85, 20, 10);
        doc.Save(Folder + "DrawEllipse.pdf");
    }

    [Fact]
    public void FillEllipse()
    {
        var doc = new PortableDocument();
        doc.AddPage(200, 100);
        doc.SetColor(PlotterColors.Orange);
        doc.SetFillColor(PlotterColors.LightGreen);
        doc.FillEllipse(5, 5, 190, 90);
        doc.Save(Folder + "FillEllipse.pdf");
    }

    [Fact]
    public void DrawLine()
    {
        var doc = new PortableDocument();
        doc.AddPage(200, 100);
        doc.DrawLine(50, 50, 100, 70);
        doc.Save(Folder + "DrawLine.pdf");
    }

    [Fact]
    public void DrawLine_Colors()
    {
        var doc = new PortableDocument();
        doc.AddPage(100, 100);
        double x = 0;
        double y0 = 78;
        double y1 = 10;

        doc.DrawLine(10, 95, 10, 80);

        doc.SetColor(0, 0, 0);
        doc.DrawLine(x += 10, y0, x, y1);

        doc.SetColor(1, 1, 1);
        doc.DrawLine(x += 10, y0, x, y1);

        doc.SetColor(1, 0, 0);
        doc.DrawLine(x += 10, y0, x, y1);

        doc.SetColor(0, 1, 0);
        doc.DrawLine(x += 10, y0, x, y1);

        doc.SetColor(0, 0, 1);
        doc.DrawLine(x += 10, y0, x, y1);

        doc.SetColor(1, 1, 0);
        doc.DrawLine(x += 10, y0, x, y1);

        doc.Save(Folder + "DrawLine_Colors.pdf");
    }

    [Fact]
    public void DrawLine_LineWidths()
    {
        var doc = new PortableDocument();
        doc.AddPage(100, 100);
        double x = 0;
        double y0 = 78;
        double y1 = 10;

        doc.DrawLine(20, 95, 20, 80);

        doc.SetLineWidth(0.1);
        doc.DrawLine(x += 10, y0, x, y1);

        doc.SetLineWidth(1);
        doc.DrawLine(x += 10, y0, x, y1);

        doc.SetLineWidth(2);
        doc.DrawLine(x += 10, y0, x, y1);

        doc.SetLineWidth(3);
        doc.DrawLine(x += 10, y0, x, y1);

        doc.Save(Folder + "DrawLine_LineWidths.pdf");
    }

    [Fact]
    public void DrawLine_LineDashPatterns()
    {
        var doc = new PortableDocument();
        doc.AddPage(100, 100);
        double x0 = 10;
        double x1 = 40;
        double y = 100;
        double dy = -5;

        doc.SetLineWidth(1);

        // default dash pattern
        doc.DrawLine(x0, y += dy, x1, y);

        doc.SetLineDashPattern(new double[] { }, 0);
        doc.DrawLine(x0, y += dy, x1, y);

        doc.SetLineDashPattern(new[] { 3d }, 0);
        doc.DrawLine(x0, y += dy, x1, y);

        doc.SetLineDashPattern(new[] { 2d }, 1);
        doc.DrawLine(x0, y += dy, x1, y);

        doc.SetLineDashPattern(new[] { 2d, 1 }, 0);
        doc.DrawLine(x0, y += dy, x1, y);

        doc.SetLineDashPattern(new[] { 3d, 5 }, 6);
        doc.DrawLine(x0, y += dy, x1, y);

        doc.SetLineDashPattern(new[] { 2d, 3 }, 11);
        doc.DrawLine(x0, y += dy, x1, y);

        doc.Save(Folder + "DrawLine_LineDashPatterns.pdf");
    }

    [Fact]
    public void Stroke_LineJoins()
    {
        var doc = new PortableDocument();
        doc.AddPage(100, 100);

        doc.SetLineWidth(3);
        doc.MoveTo(10, 10);
        doc.LineTo(50, 60);
        doc.LineTo(90, 10);
        doc.Stroke(false);

        doc.SetColor(1, 0, 0);
        doc.SetLineJoin(LineJoin.Bevel);
        doc.MoveTo(10, 20);
        doc.LineTo(50, 70);
        doc.LineTo(90, 20);
        doc.Stroke(false);

        doc.SetColor(0, 1, 0);
        doc.SetLineJoin(LineJoin.Miter);
        doc.MoveTo(10, 30);
        doc.LineTo(50, 80);
        doc.LineTo(90, 30);
        doc.Stroke(false);

        doc.SetColor(0, 0, 1);
        doc.SetLineJoin(LineJoin.Round);
        doc.MoveTo(10, 40);
        doc.LineTo(50, 90);
        doc.LineTo(90, 40);
        doc.Stroke(false);

        doc.Save(Folder + "Stroke_LineJoins.pdf");
    }

    [Fact]
    public void Stroke_LineCaps()
    {
        var doc = new PortableDocument();
        doc.AddPage(100, 100);

        doc.SetColor(0.5, 0.5, 0.5);
        doc.SetLineWidth(3);
        doc.MoveTo(10, 10);
        doc.LineTo(50, 60);
        doc.LineTo(90, 10);
        doc.Stroke(false);

        doc.SetColor(1, 0, 0);
        doc.SetLineCap(LineCap.Butt);
        doc.MoveTo(10, 20);
        doc.LineTo(50, 70);
        doc.LineTo(90, 20);
        doc.Stroke(false);

        doc.SetColor(0, 1, 0);
        doc.SetLineCap(LineCap.ProjectingSquare);
        doc.MoveTo(10, 30);
        doc.LineTo(50, 80);
        doc.LineTo(90, 30);
        doc.Stroke(false);

        doc.SetColor(0, 0, 1);
        doc.SetLineCap(LineCap.Round);
        doc.MoveTo(10, 40);
        doc.LineTo(50, 90);
        doc.LineTo(90, 40);
        doc.Stroke(false);

        doc.SetColor(0, 0, 0);
        doc.SetLineWidth(0.1);
        doc.MoveTo(10, 10);
        doc.LineTo(50, 60);
        doc.LineTo(90, 10);
        doc.MoveTo(10, 20);
        doc.LineTo(50, 70);
        doc.LineTo(90, 20);
        doc.MoveTo(10, 30);
        doc.LineTo(50, 80);
        doc.LineTo(90, 30);
        doc.MoveTo(10, 40);
        doc.LineTo(50, 90);
        doc.LineTo(90, 40);
        doc.Stroke(false);

        doc.Save(Folder + "Stroke_LineCaps.pdf");
    }

    [Fact]
    public void DrawPolygon()
    {
        var doc = new PortableDocument();
        doc.AddPage(200, 100);

        doc.MoveTo(50, 30);
        doc.LineTo(170, 30);
        doc.LineTo(100, 70);
        doc.SetColor(PlotterColors.Orange);
        doc.SetFillColor(PlotterColors.LightGreen);
        doc.FillAndStroke();

        doc.MoveTo(5, 5);
        doc.LineTo(5, 25);
        doc.LineTo(25, 5);
        doc.Fill();

        doc.MoveTo(195, 95);
        doc.LineTo(175, 95);
        doc.LineTo(195, 75);
        doc.Stroke();

        doc.Save(Folder + "DrawPolygon.pdf");
    }

    [Fact]
    public void DrawRectangle()
    {
        var doc = new PortableDocument();
        doc.AddPage(200, 100);
        doc.SetColor(PlotterColors.Navy);
        doc.SetFillColor(PlotterColors.Gainsboro);
        doc.DrawRectangle(5, 5, 100, 70, true);
        doc.DrawRectangle(185, 85, 10, 10);
        doc.Save(Folder + "DrawRectangle.pdf");
    }

    [Fact]
    public void FillRectangle()
    {
        var doc = new PortableDocument();
        doc.AddPage(200, 100);
        doc.SetColor(PlotterColors.Gainsboro);
        doc.SetFillColor(PlotterColors.Navy);
        doc.FillRectangle(5, 5, 100, 70);
        doc.Save(Folder + "FillRectangle.pdf");
    }

    [Fact(Skip = "Not implemented")]
    public void DrawImage()
    {
        var doc = new PortableDocument();
        doc.AddPage(200, 100);
        //// var image = new PortableDocument.Image() { };
        //// doc.DrawImage(image);
        doc.Save(Folder + "DrawImage.pdf");
    }

    [Fact]
    public void SetClippingRectangle()
    {
        var doc = new PortableDocument();
        doc.AddPage(200, 200);
        doc.SetColor(PlotterColors.Blue);
        doc.SetFillColor(PlotterColors.LightBlue);
        doc.SaveState();
        doc.SetClippingRectangle(5, 5, 50, 50);
        doc.DrawCircle(100, 100, 95, true);
        doc.RestoreState();
        doc.DrawCircle(120, 120, 70);
        doc.Save(Folder + "SetClippingRectangle.pdf");
    }

    [Fact]
    public void Translate()
    {
        var doc = new PortableDocument();
        doc.AddPage(200, 200);

        doc.SaveState();
        doc.SetColor(1, 0, 0);
        doc.Translate(20, 10);
        doc.DrawRectangle(10, 10, 100, 70);
        doc.RestoreState();

        doc.DrawRectangle(10, 10, 100, 70);

        doc.Save(Folder + "Translate.pdf");
    }

    [Fact]
    public void Rotate()
    {
        var doc = new PortableDocument();
        doc.AddPage(200, 200);

        doc.SaveState();
        doc.SetColor(1, 0, 0);
        doc.Rotate(30);
        doc.DrawRectangle(50, 20, 100, 15);
        doc.RestoreState();

        doc.DrawRectangle(50, 20, 100, 15);

        doc.Save(Folder + "Rotate.pdf");
    }

    [Fact]
    public void RotateAt()
    {
        var doc = new PortableDocument();
        doc.AddPage(200, 200);

        doc.SaveState();
        doc.SetColor(1, 0, 0);
        doc.RotateAt(50, 20, 30);
        doc.DrawRectangle(50, 20, 100, 15);
        doc.RestoreState();

        doc.DrawRectangle(50, 20, 100, 15);

        doc.Save(Folder + "RotateAt.pdf");
    }

    [Fact]
    public void FontFaces()
    {
        var doc = new PortableDocument();
        doc.AddPage(PageSize.A4);
        double x = 20 / 25.4 * 72;
        double dy = 10 / 25.4 * 72;
        double y = doc.PageHeight - dy;
        doc.DrawText(x, y -= dy, "This is the default font.");
        doc.SetFont("Courier", 12);
        doc.DrawText(x, y -= dy, "This is courier normal.");
        doc.SetFont("Times", 12, false, true);
        doc.DrawText(x, y -= dy, "This is times italic.");
        doc.SetFont("Helvetica", 12, true);
        doc.DrawText(x, y -= dy, "This is helvetica bold.");
        doc.SetFont("Courier", 12, true, true);
        doc.DrawText(x, y, "This is courier bolditalic.");
        doc.Save(Folder + "FontFaces.pdf");
    }

    [Theory]
    [InlineData("Helvetica")]
    [InlineData("Times")]
    [InlineData("Courier")]
    public void FontFace(string fontName)
    {
        var doc = new PortableDocument();
        doc.AddPage(PageSize.A4);
        doc.SetFont(fontName, 12);
        double x = 20 / 25.4 * 72;
        double dy = 10 / 25.4 * 72;
        double y = doc.PageHeight - dy;
        doc.DrawText(x, y -= dy, "This is 12pt " + fontName + " regular.");
        doc.SetFont(fontName, 12, true);
        doc.DrawText(x, y -= dy, "This is 12pt " + fontName + " bold.");
        doc.SetFont(fontName, 12, false, true);
        doc.DrawText(x, y -= dy, "This is 12pt " + fontName + " italic.");
        doc.SetFont(fontName, 12, true, true);
        doc.DrawText(x, y, "This is 12pt " + fontName + " bold and italic.");
        doc.Save(Folder + "FontFace_" + fontName + ".pdf");
    }

    [Fact]
    public void Transparency()
    {
        var doc = new PortableDocument();
        doc.AddPage(220, 100);
        doc.SetFillColor(PlotterColors.Black);
        doc.FillRectangle(0, 45, 220, 10);
        for (int i = 0; i <= 10; i++)
        {
            doc.SetFillColor(PlotterColor.FromAColor((byte)(255d * i / 10), PlotterColors.Gold));
            doc.FillEllipse((i * 20) + 1, 41, 18, 18);
        }

        doc.Save(Folder + "Transparency.pdf");
    }
}
