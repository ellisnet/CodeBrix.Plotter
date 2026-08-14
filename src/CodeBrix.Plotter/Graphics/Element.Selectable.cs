// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Element.Selectable.cs" company="OxyPlot">
//   Copyright (c) 2014 OxyPlot contributors
// </copyright>
// <summary>
//   Provides an abstract base class for elements that support selection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace CodeBrix.Plotter; //was previously: OxyPlot;

/// <summary>
/// Provides an abstract base class for graphics elements.
/// </summary>
public abstract partial class Element
{
    /// <summary>
    /// The selection
    /// </summary>
    private Selection selection;

    /// <summary>
    /// Gets or sets a value indicating whether this element can be selected. The default is <c>true</c>.
    /// </summary>
    public bool Selectable { get; set; } = true;

    /// <summary>
    /// Gets or sets the selection mode of items in this element. The default is <c>SelectionMode.All</c>.
    /// </summary>
    /// <value>The selection mode.</value>
    /// <remarks>This is only used by the select/unselect functionality, not by the rendering.</remarks>
    public SelectionMode SelectionMode { get; set; } = SelectionMode.All;

    /// <summary>
    /// Gets the actual selection color.
    /// </summary>
    /// <value>The actual selection color.</value>
    protected PlotterColor ActualSelectedColor
    {
        get
        {
            if (this.Parent != null)
            {
                return this.Parent.SelectionColor.GetActualColor(Model.DefaultSelectionColor);
            }

            return Model.DefaultSelectionColor;
        }
    }

    /// <summary>
    /// Determines whether any part of this element is selected.
    /// </summary>
    /// <returns><c>true</c> if this element is selected; otherwise, <c>false</c>.</returns>
    public bool IsSelected()
    {
        return this.selection != null;
    }

    /// <summary>
    /// Gets the indices of the selected items in this element.
    /// </summary>
    /// <returns>Enumerator of item indices.</returns>
    public IEnumerable<int> GetSelectedItems()
    {
        this.EnsureSelection();
        return this.selection.GetSelectedItems();
    }

    /// <summary>
    /// Clears the selection.
    /// </summary>
    public void ClearSelection()
    {
        this.selection = null;
    }

    /// <summary>
    /// Unselects all items in this element.
    /// </summary>
    public void Unselect()
    {
        this.selection = null;
    }

    /// <summary>
    /// Determines whether the specified item is selected.
    /// </summary>
    /// <param name="index">The index of the item.</param>
    /// <returns><c>true</c> if the item is selected; otherwise, <c>false</c>.</returns>
    public bool IsItemSelected(int index)
    {
        if (this.selection == null)
        {
            return false;
        }

        if (index == -1)
        {
            return this.selection.IsEverythingSelected();
        }

        return this.selection.IsItemSelected(index);
    }

    /// <summary>
    /// Selects all items in this element.
    /// </summary>
    public void Select()
    {
        this.selection = Selection.Everything;
    }

    /// <summary>
    /// Selects the specified item.
    /// </summary>
    /// <param name="index">The index.</param>
    public void SelectItem(int index)
    {
        if (this.SelectionMode == SelectionMode.All)
        {
            throw new InvalidOperationException("Use the Select() method when using SelectionMode.All");
        }

        this.EnsureSelection();
        if (this.SelectionMode == SelectionMode.Single)
        {
            this.selection.Clear();
        }

        this.selection.Select(index);
    }

    /// <summary>
    /// Unselects the specified item.
    /// </summary>
    /// <param name="index">The index.</param>
    public void UnselectItem(int index)
    {
        if (this.SelectionMode == SelectionMode.All)
        {
            throw new InvalidOperationException("Use the Unselect() method when using SelectionMode.All");
        }

        this.EnsureSelection();
        this.selection.Unselect(index);
    }

    /// <summary>
    /// Gets the selection color if the item is selected, or the specified color if it is not.
    /// </summary>
    /// <param name="originalColor">The unselected color of the element.</param>
    /// <param name="index">The index of the item to check (use -1 for all items).</param>
    /// <returns>A color.</returns>
    protected PlotterColor GetSelectableColor(PlotterColor originalColor, int index = -1)
    {
        // TODO: rename to GetActualColor (33 usages)
        if (originalColor.IsUndefined())
        {
            return PlotterColors.Undefined;
        }

        if (this.IsItemSelected(index))
        {
            return this.ActualSelectedColor;
        }

        return originalColor;
    }

    /// <summary>
    /// Gets the selection fill color it the element is selected, or the specified fill color if it is not.
    /// </summary>
    /// <param name="originalColor">The unselected fill color of the element.</param>
    /// <param name="index">The index of the item to check (use -1 for all items).</param>
    /// <returns>A fill color.</returns>
    protected PlotterColor GetSelectableFillColor(PlotterColor originalColor, int index = -1)
    {
        // TODO: rename to GetActualFillColor (13 usages)
        return this.GetSelectableColor(originalColor, index);
    }

    /// <summary>
    /// Ensures that the selection field is not <c>null</c>.
    /// </summary>
    private void EnsureSelection()
    {
        if (this.selection == null)
        {
            this.selection = new Selection();
        }
    }

}
