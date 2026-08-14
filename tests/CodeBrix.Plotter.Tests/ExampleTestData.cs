using System.Collections.Generic;
using System.Linq;
using CodeBrix.Plotter.Tests.ExampleLibrary;

namespace CodeBrix.Plotter.Tests;

/// <summary>
/// Theory data for the tests that run over every example model.
/// </summary>
/// <remarks>
/// The upstream NUnit suite passed <see cref="ExampleInfo" /> instances
/// straight into <c>[TestCaseSource]</c>. xUnit needs <c>IEnumerable&lt;object[]&gt;</c>
/// and works best with data it can serialise, so the category and title are
/// passed instead and the example is looked up again inside the test. That also
/// makes each case identifiable by name in the test output.
/// </remarks>
public static class ExampleTestData
{
    /// <summary>
    /// Gets the category and title of every example suitable for automated tests.
    /// </summary>
    public static IEnumerable<object[]> AllForAutomatedTest =>
        Examples.GetListForAutomatedTest()
            .Select(example => new object[] { example.Category, example.Title });

    /// <summary>
    /// Gets the example with the specified category and title.
    /// </summary>
    /// <param name="category">The example category.</param>
    /// <param name="title">The example title.</param>
    /// <returns>The matching example.</returns>
    public static ExampleInfo Get(string category, string title) =>
        Examples.GetListForAutomatedTest()
            .First(example => example.Category == category && example.Title == title);
}
