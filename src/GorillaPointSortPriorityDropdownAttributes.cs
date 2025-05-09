using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using Grasshopper.Kernel.Types;

public class GorillaPointSortPriorityDropdownAttributes : GH_ComponentAttributes
{
    private readonly string[] priorityLabels = { "Ignore", "High", "Medium", "Low" };
    private readonly int[] values = { 0, 1, 2, 3 };

    private RectangleF xButton, yButton, zButton;
    private bool isDropdownOpenX, isDropdownOpenY, isDropdownOpenZ;
    private int dropdownIndexX = 0, dropdownIndexY = 0, dropdownIndexZ = 0;

    public GorillaPointSortPriorityDropdownAttributes(GorillaPointSortPriorityDropdown owner) : base(owner) { }

    protected override void Layout()
    {
        base.Layout();

        float buttonHeight = 22;
        float gap = 6;
        float buttonWidth = 76;
        float buttonOffsetX = -55;

        // Determine extra width needed to render dropdown fully
        bool dropdownOpen = isDropdownOpenX || isDropdownOpenY || isDropdownOpenZ;
        bool dropdownOpenZ = isDropdownOpenZ;

        float extraWidth = dropdownOpen ? 50 : 0;

        // Set Bounds to fit buttons and dropdowns
        RectangleF baseRec = GH_Convert.ToRectangle(Bounds);
        float totalHeight = 3 * buttonHeight + 2 * gap + 70;
        float extraHeight = dropdownOpenZ ? 60 : 0;
        baseRec.Width = buttonWidth + buttonOffsetX + extraWidth + 35;
        baseRec.Height = totalHeight + 6 + extraHeight;
        Bounds = baseRec;

        float top = baseRec.Top;

        xButton = new RectangleF(baseRec.X + buttonOffsetX, top + 64, buttonWidth, buttonHeight);
        yButton = new RectangleF(baseRec.X + buttonOffsetX, top + 64 + buttonHeight + gap, buttonWidth, buttonHeight);
        zButton = new RectangleF(baseRec.X + buttonOffsetX, top + 64 + 2 * (buttonHeight + gap), buttonWidth, buttonHeight);
    }

    protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
    {
        base.Render(canvas, graphics, channel);

        if (channel != GH_CanvasChannel.Objects) return;

        var owner = Owner as GorillaPointSortPriorityDropdown;
        if (owner == null) return;

        // 🔄 Sync dropdown indices with component values
        dropdownIndexX = Array.IndexOf(values, owner.xPriority);
        dropdownIndexY = Array.IndexOf(values, owner.yPriority);
        dropdownIndexZ = Array.IndexOf(values, owner.zPriority);

        // Draw X, Y, Z Priority Buttons
        DrawButton(graphics, xButton, $"X: {priorityLabels[dropdownIndexX]}", isDropdownOpenX);
        DrawButton(graphics, yButton, $"Y: {priorityLabels[dropdownIndexY]}", isDropdownOpenY);
        DrawButton(graphics, zButton, $"Z: {priorityLabels[dropdownIndexZ]}", isDropdownOpenZ);

        if (isDropdownOpenX)
            DrawDropdownOptions(graphics, xButton, dropdownIndexX);
        if (isDropdownOpenY)
            DrawDropdownOptions(graphics, yButton, dropdownIndexY);
        if (isDropdownOpenZ)
            DrawDropdownOptions(graphics, zButton, dropdownIndexZ);
    }

    private void DrawButton(Graphics g, RectangleF bounds, string text, bool isDropdownOpen)
    {
        var fill = GH_Skin.palette_normal_standard.Fill;
        var border = GH_Skin.palette_normal_standard.Edge;

        g.FillRectangle(new SolidBrush(fill), bounds);
        g.DrawRectangle(new Pen(border), bounds.X, bounds.Y, bounds.Width, bounds.Height);
        DrawCenteredText(g, text, GH_FontServer.Standard, Brushes.Black, bounds);

        // Draw dropdown triangle
        DrawDropdownArrow(g, bounds, isDropdownOpen);
    }

    private void DrawDropdownArrow(Graphics g, RectangleF bounds, bool isOpen)
    {
        PointF center = new PointF(bounds.Right - 10, bounds.Top + bounds.Height / 2f);
        float size = 4.5f;

        PointF[] triangle;

        if (isOpen)
        {
            // Downward triangle
            triangle = new PointF[]
            {
                new PointF(center.X - size, center.Y - size / 2),
                new PointF(center.X + size, center.Y - size / 2),
                new PointF(center.X, center.Y + size / 1.5f)
            };
        }
        else
        {
            // Rightward triangle
            triangle = new PointF[]
            {
                new PointF(center.X - size / 2, center.Y - size),
                new PointF(center.X - size / 2, center.Y + size),
                new PointF(center.X + size, center.Y)
            };
        }

        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        g.FillPolygon(Brushes.Black, triangle);
    }
    private void DrawDropdownOptions(Graphics g, RectangleF button, int currentIndex)
    {
        var optionHeight = 22;
        float dropdownX = button.Right + 2; // Offsetting to the right of the button
        float dropdownY = button.Top;

        // Prevent overlap by ensuring the dropdown is rendered at the right position
        for (int i = 0; i < priorityLabels.Length; i++)
        {
            RectangleF optionRect = new RectangleF(dropdownX, dropdownY + i * optionHeight, button.Width, optionHeight);
            g.FillRectangle(new SolidBrush(Color.LightGray), optionRect);
            g.DrawRectangle(Pens.Black, optionRect.X, optionRect.Y, optionRect.Width, optionRect.Height);

            if (i == currentIndex)
                g.FillRectangle(new SolidBrush(Color.Gray), optionRect);

            DrawCenteredText(g, priorityLabels[i], GH_FontServer.Standard, Brushes.Black, optionRect);
        }
    }

    private void DrawCenteredText(Graphics g, string text, Font font, Brush brush, RectangleF bounds)
    {
        using (StringFormat format = new StringFormat()
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        })
        {
            g.DrawString(text, font, brush, bounds, format);
        }
    }

    public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
    {
        var owner = Owner as GorillaPointSortPriorityDropdown;
        if (owner == null) return GH_ObjectResponse.Ignore;

        // Toggle dropdowns
        if (xButton.Contains(e.CanvasLocation))
        {
            isDropdownOpenX = !isDropdownOpenX;
            isDropdownOpenY = false;
            isDropdownOpenZ = false;
            owner.ExpireSolution(true);
            return GH_ObjectResponse.Handled;
        }
        if (yButton.Contains(e.CanvasLocation))
        {
            isDropdownOpenX = false;
            isDropdownOpenY = !isDropdownOpenY;
            isDropdownOpenZ = false;
            owner.ExpireSolution(true);
            return GH_ObjectResponse.Handled;
        }
        if (zButton.Contains(e.CanvasLocation))
        {
            isDropdownOpenX = false;
            isDropdownOpenY = false;
            isDropdownOpenZ = !isDropdownOpenZ;
            owner.ExpireSolution(true);
            return GH_ObjectResponse.Handled;
        }

        // Handle option selection
        if (isDropdownOpenX && IsWithinDropdown(e.CanvasLocation, xButton))
        {
            dropdownIndexX = GetSelectedOptionIndex(e.CanvasLocation, xButton);
            owner.xPriority = values[dropdownIndexX];
            isDropdownOpenX = false;
            owner.ExpireSolution(true);
            return GH_ObjectResponse.Handled;
        }

        if (isDropdownOpenY && IsWithinDropdown(e.CanvasLocation, yButton))
        {
            dropdownIndexY = GetSelectedOptionIndex(e.CanvasLocation, yButton);
            owner.yPriority = values[dropdownIndexY];
            isDropdownOpenY = false;
            owner.ExpireSolution(true);
            return GH_ObjectResponse.Handled;
        }

        if (isDropdownOpenZ && IsWithinDropdown(e.CanvasLocation, zButton))
        {
            dropdownIndexZ = GetSelectedOptionIndex(e.CanvasLocation, zButton);
            owner.zPriority = values[dropdownIndexZ];
            isDropdownOpenZ = false;
            owner.ExpireSolution(true);
            return GH_ObjectResponse.Handled;
        }

        return base.RespondToMouseDown(sender, e);
    }

    private bool IsWithinDropdown(PointF mouseLocation, RectangleF buttonBounds)
    {
        float dropdownX = buttonBounds.Right + 2; // Offset for the dropdown area
        float dropdownY = buttonBounds.Top; // Aligned to the top of the button
        float dropdownWidth = buttonBounds.Width; // Dropdown width should match the button's width
        float dropdownHeight = priorityLabels.Length * 22; // Height based on the number of options

        RectangleF dropdownBounds = new RectangleF(dropdownX, dropdownY, dropdownWidth, dropdownHeight);
        return dropdownBounds.Contains(mouseLocation);
    }

    private int GetSelectedOptionIndex(PointF mouseLocation, RectangleF buttonBounds)
    {
        float dropdownY = buttonBounds.Top; // Aligned to the top of the button
        int optionHeight = 22;

        // Calculate the index based on the mouse location
        int index = (int)((mouseLocation.Y - dropdownY) / optionHeight);
        return Math.Max(0, Math.Min(index, priorityLabels.Length - 1));
    }
}