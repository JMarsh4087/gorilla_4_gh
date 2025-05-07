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

        float height = 22 * 3 + 6; // 3 buttons, spacing
        RectangleF baseRec = GH_Convert.ToRectangle(Bounds);
        baseRec.Height += height;

        Bounds = baseRec;

        float buttonWidth = baseRec.Width - 6;
        xButton = new RectangleF(baseRec.X + 3, baseRec.Bottom - height + 0, buttonWidth, 22);
        yButton = new RectangleF(baseRec.X + 3, baseRec.Bottom - height + 24, buttonWidth, 22);
        zButton = new RectangleF(baseRec.X + 3, baseRec.Bottom - height + 48, buttonWidth, 22);
    }

    protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
    {
        base.Render(canvas, graphics, channel);

        if (channel != GH_CanvasChannel.Objects) return;

        var owner = Owner as GorillaPointSortPriorityDropdown;
        if (owner == null) return;

        // Draw X, Y, Z Priority Buttons
        DrawButton(graphics, xButton, $"X: {priorityLabels[dropdownIndexX]}", isDropdownOpenX);
        DrawButton(graphics, yButton, $"Y: {priorityLabels[dropdownIndexY]}", isDropdownOpenY);
        DrawButton(graphics, zButton, $"Z: {priorityLabels[dropdownIndexZ]}", isDropdownOpenZ);

        // If dropdown is open, show options
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

        // Draw dropdown indicator (like an arrow)
        string arrow = isDropdownOpen ? "▼" : "▶";
        g.DrawString(arrow, GH_FontServer.Standard, Brushes.Black, bounds.X + bounds.Width - 20, bounds.Y + 2);
    }

    private void DrawDropdownOptions(Graphics g, RectangleF bounds, int currentIndex)
    {
        var optionHeight = 22;
        for (int i = 0; i < priorityLabels.Length; i++)
        {
            RectangleF optionRect = new RectangleF(bounds.X, bounds.Bottom + i * optionHeight, bounds.Width, optionHeight);
            g.FillRectangle(new SolidBrush(Color.LightGray), optionRect);
            g.DrawRectangle(Pens.Black, optionRect.X, optionRect.Y, optionRect.Width, optionRect.Height);

            // Highlight selected option
            if (i == currentIndex)
            {
                g.FillRectangle(new SolidBrush(Color.Gray), optionRect);
            }

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

        // Toggle dropdowns
        if (xButton.Contains(e.CanvasLocation))
        {
            isDropdownOpenX = !isDropdownOpenX;
            owner?.ExpireSolution(true);
            return GH_ObjectResponse.Handled;
        }
        if (yButton.Contains(e.CanvasLocation))
        {
            isDropdownOpenY = !isDropdownOpenY;
            owner?.ExpireSolution(true);
            return GH_ObjectResponse.Handled;
        }
        if (zButton.Contains(e.CanvasLocation))
        {
            isDropdownOpenZ = !isDropdownOpenZ;
            owner?.ExpireSolution(true);
            return GH_ObjectResponse.Handled;
        }

        // Handle option selection
        if (isDropdownOpenX && IsWithinDropdown(e.CanvasLocation, xButton))
        {
            dropdownIndexX = GetSelectedOptionIndex(e.CanvasLocation, xButton);
            owner?.ExpireSolution(true);
            return GH_ObjectResponse.Handled;
        }
        if (isDropdownOpenY && IsWithinDropdown(e.CanvasLocation, yButton))
        {
            dropdownIndexY = GetSelectedOptionIndex(e.CanvasLocation, yButton);
            owner?.ExpireSolution(true);
            return GH_ObjectResponse.Handled;
        }
        if (isDropdownOpenZ && IsWithinDropdown(e.CanvasLocation, zButton))
        {
            dropdownIndexZ = GetSelectedOptionIndex(e.CanvasLocation, zButton);
            owner?.ExpireSolution(true);
            return GH_ObjectResponse.Handled;
        }

        return base.RespondToMouseDown(sender, e);
    }

    private bool IsWithinDropdown(PointF mouseLocation, RectangleF buttonBounds)
    {
        return mouseLocation.Y > buttonBounds.Bottom &&
               mouseLocation.Y < buttonBounds.Bottom + 22 * priorityLabels.Length;
    }

    private int GetSelectedOptionIndex(PointF mouseLocation, RectangleF buttonBounds)
    {
        int optionHeight = 22;
        int index = (int)((mouseLocation.Y - buttonBounds.Bottom) / optionHeight);
        return Math.Max(0, Math.Min(index, priorityLabels.Length - 1));
    }
}