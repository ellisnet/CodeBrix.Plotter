// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FractionHelperTests.cs" company="OxyPlot">
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
public class FractionHelperTests
{
    [Fact]
    public void ConvertToFractionString()
    {
        FractionHelper.ConvertToFractionString(0.75).Should().Be("3/4");
    }

    [Fact]
    public void ConvertToFractionString_WithUnit()
    {
        FractionHelper.ConvertToFractionString(Math.PI * 2, Math.PI, "pi").Should().Be("2pi");
    }
}
