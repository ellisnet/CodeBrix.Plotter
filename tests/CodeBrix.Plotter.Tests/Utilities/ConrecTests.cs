// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConrecTests.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using SilverAssertions;
using Xunit;

namespace CodeBrix.Plotter.Tests; //was previously: OxyPlot.Tests;

// ReSharper disable InconsistentNaming
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
public class ConrecTests
{
    [Fact]
    public void Contour()
    {
        var x = ArrayBuilder.CreateVector(0, 10, 100);
        var y = ArrayBuilder.CreateVector(0, 10, 100);
        var z = ArrayBuilder.CreateVector(-1, 1, 20);
        var data = ArrayBuilder.Evaluate((x1, y1) => Math.Sin(x1 * y1), x, y);
        int segments = 0;
        Conrec.Contour(data, x, y, z, (x1, y1, x2, y2, elev) => { segments++; });
        segments.Should().Be(134068);
    }
}
