using SkiaSharp;

namespace CodeBrix.Plotter.Skia;

/// <summary>
/// Resolves a font family and weight to a typeface, replacing the system font lookup of a
/// <see cref="SkiaRenderContext"/> completely.
/// </summary>
/// <param name="fontFamily">The font family name to resolve.</param>
/// <param name="fontWeight">The font weight to resolve (400 is normal, 700 is bold).</param>
/// <returns>
/// The resolved typeface. The resolver keeps ownership of it: the render context never disposes
/// a resolver-supplied typeface. A resolver that cannot resolve the family should return its own
/// default typeface rather than <c>null</c> - the render context never falls back to a system
/// font lookup while a resolver is assigned, and treats a <c>null</c> return as
/// <see cref="SKTypeface.Default"/>.
/// </returns>
public delegate SKTypeface TypefaceResolver(string fontFamily, double fontWeight);
