// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringHelperTests.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using SilverAssertions;
using Xunit;

namespace CodeBrix.Plotter.Tests; //was previously: OxyPlot.Tests;

// ReSharper disable InconsistentNaming
[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
public class StringHelperTests
{
    [Fact]
    public void Format_StandardFormatString()
    {
        StringHelper.Format(CultureInfo.InvariantCulture, "{0}", null, 3.1).Should().Be("3.1");
        StringHelper.Format(CultureInfo.InvariantCulture, "{0:0.00}", null, Math.PI).Should().Be("3.14");
        StringHelper.Format(CultureInfo.InvariantCulture, "PI={0:0.00}", null, Math.PI).Should().Be("PI=3.14");
    }

    [Fact]
    public void Format_Item()
    {
        var item = new Item { Text = "Hello World", Value = 3.14 };
        StringHelper.Format(CultureInfo.InvariantCulture, "{0} {1:0} {Text} {Value:0.000}", item, item.Value, item.Value).Should().Be("3.14 3 Hello World 3.140");
        StringHelper.Format(CultureInfo.InvariantCulture, "{Text}", item).Should().Be("Hello World");
    }

    [Fact]
    public void CreateValidFormatString()
    {
        StringHelper.CreateValidFormatString(null).Should().Be("{0}");
        StringHelper.CreateValidFormatString(string.Empty).Should().Be("{0}");
        StringHelper.CreateValidFormatString("0.00").Should().Be("{0:0.00}");
        StringHelper.CreateValidFormatString("Item {0}").Should().Be("Item {0}");
    }

    [Fact]
    public void FormatIntegers()
    {
        var items = new[] { 1, 2, 3 };
        items.Format(null, null, CultureInfo.InvariantCulture).Should().Equal(new[] { "1", "2", "3" });
        items.Format("", "00", CultureInfo.InvariantCulture).Should().Equal(new[] { "01", "02", "03" });
        items.Format(null, "Item {0}", CultureInfo.InvariantCulture).Should().Equal(new[] { "Item 1", "Item 2", "Item 3" });
    }

    [Fact]
    public void FormatStrings()
    {
        var items = new[] { "One", "Two", "Three" };
        items.Format("Length", null, CultureInfo.InvariantCulture).Should().Equal(new[] { "3", "3", "5" });
        items.Format(null, "Item {0}", CultureInfo.InvariantCulture).Should().Equal(new[] { "Item One", "Item Two", "Item Three" });
    }

    public class Item
    {
        public string Text { get; set; }

        public double Value { get; set; }
    }
}
