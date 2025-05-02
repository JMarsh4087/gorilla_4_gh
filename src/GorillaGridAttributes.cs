using System;
using System.Drawing;
using Grasshopper.GUI.Canvas;
using Grasshopper.GUI;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Attributes;

public class GorillaGridAttributes : GH_ComponentAttributes
{
    private const int cellSize = 18;
    private const int padding = 6;
    private const int headerHeight = 100;

    public GorillaGridAttributes(GorillaGridComponent owner) : base(owner) { }

    protected override void Layout()
    {
        base.Layout();

        var comp = Owner as GorillaGridComponent;

        int cols = GetInputValue(0, 3);
        int rows = GetInputValue(1, 3);

        int gridWidth = cols * cellSize + padding * 2;
        int gridHeight = rows * cellSize + padding * 2;

        Rectangle capsule = GH_Convert.ToRectangle(Bounds);
        capsule.Height = headerHeight;

        // Full bounds includes capsule + grid below
        Rectangle fullBounds = new Rectangle(
            capsule.Left,
            capsule.Top,
            Math.Max(capsule.Width, gridWidth),
            capsule.Height + gridHeight
        );

        Bounds = fullBounds;

        // Place outputs to the right of the actual grid bounds
        float outputX = Bounds.Right - 15;
        float outputYStart = Bounds.Top + 8;

        for (int i = 0; i < Owner.Params.Output.Count; i++)
        {
            AdjustOutputPosition(i, outputX, outputYStart + i * 20);
        }
    }

    private void AdjustOutputPosition(int index, float xPos, float yPos)
    {
        if (index >= Owner.Params.Output.Count) return;

        var outputParam = Owner.Params.Output[index];

        // Force a layout update for the output parameter
        if (outputParam.Attributes is GH_Attributes<IGH_Param> attr)
        {
            attr.Pivot = new PointF(xPos, yPos);
            attr.Bounds = new RectangleF(attr.Pivot, new SizeF(10, 10));
            attr.ExpireLayout();

            outputParam.Attributes.PerformLayout();
        }
    }

    private int GetInputValue(int index, int fallback)
    {
        if (Owner.Params.Input[index].VolatileDataCount > 0)
        {
            var data = Owner.Params.Input[index].VolatileData.get_Branch(0)[0];
            if (data is GH_Integer gInt)
                return gInt.Value;
        }
        return fallback;
    }
    protected override void Render(GH_Canvas canvas, Graphics graphics, GH_CanvasChannel channel)
    {
        base.Render(canvas, graphics, channel);

        var gridComponent = Owner as GorillaGridComponent;
        int cols = GetInputValue(0, 3);
        int rows = GetInputValue(1, 3);
        int offsetX = (int)(Bounds.Left + padding);
        int offsetY = (int)(Bounds.Top + headerHeight);

        int gridWidth = cols * cellSize + padding * 2;
        int gridHeight = rows * cellSize + padding * 2;
        Rectangle gridRect = new Rectangle(offsetX - padding, offsetY - padding, gridWidth, gridHeight);

        if (channel == GH_CanvasChannel.Wires)
        {
            // Draw background behind wires
            graphics.FillRectangle(Brushes.WhiteSmoke, gridRect);
            graphics.DrawRectangle(Pens.Gray, gridRect);
        }

        if (channel == GH_CanvasChannel.Objects)
        {
            // Draw the grid
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    int x = offsetX + col * cellSize;
                    int y = offsetY + row * cellSize;

                    Rectangle cell = new Rectangle(x, y, cellSize - 1, cellSize - 1);
                    bool isSelected = gridComponent.selectedCells.Contains((col, row));

                    graphics.FillRectangle(isSelected ? Brushes.LightBlue : Brushes.White, cell);
                    graphics.DrawRectangle(Pens.Black, cell);
                }
            }

            // Draw the icon in the header center
            if (gridComponent.PublicIcon != null)
            {
                int iconSize = 24;
                int centerX = (int)(Bounds.Left + Bounds.Width / 2 - iconSize / 2);
                int centerY = (int)(Bounds.Top + headerHeight / 2 - iconSize / 2);
                graphics.DrawImage(gridComponent.PublicIcon, new Rectangle(centerX, centerY, iconSize, iconSize));
            }
        }
    }

    public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
    {
        var comp = Owner as GorillaGridComponent;

        int cols = GetInputValue(0, 3);
        int rows = GetInputValue(1, 3);

        int offsetX = (int)(Bounds.Left + padding);
        int offsetY = (int)(Bounds.Top + headerHeight);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                int x = offsetX + col * cellSize;
                int y = offsetY + row * cellSize;

                Rectangle cell = new Rectangle(x, y, cellSize - 1, cellSize - 1);

                if (cell.Contains(Point.Round(e.CanvasLocation)))
                {
                    var key = (col, row);
                    if (comp.selectedCells.Contains(key))
                        comp.selectedCells.Remove(key);
                    else
                        comp.selectedCells.Add(key);

                    comp.ExpireSolution(true);
                    return GH_ObjectResponse.Handled;
                }
            }
        }

        return base.RespondToMouseDown(sender, e);
    }
}