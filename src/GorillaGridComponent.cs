using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;

public class GorillaGridComponent : GH_Component
{
    public Bitmap PublicIcon => Icon;

    public HashSet<(int col, int row)> selectedCells = new HashSet<(int col, int row)>();

    public GorillaGridComponent()
      : base("Gorilla Grid", "GorillaGrid",
          "A clickable grid layout UI",
          "Gorilla", "UI")
    {
    }

    public override Guid ComponentGuid => new Guid("F133B3D0-19F3-4F12-9BB4-8B2A6F46E9D3");

    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
        //Debug message for Rhino command line
        Rhino.RhinoApp.WriteLine("🔵 Writing selectedCells...");

        writer.SetInt32("SelectedCount", selectedCells.Count);

        int i = 0;
        foreach (var cell in selectedCells)
        {
            writer.SetInt32($"Cell_{i}_Col", cell.col);
            writer.SetInt32($"Cell_{i}_Row", cell.row);
            i++;
        }

        return base.Write(writer);
    }

    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
        //Debug message for Rhino command line
        Rhino.RhinoApp.WriteLine("🟢 Reading selectedCells...");

        selectedCells.Clear();

        int count = reader.GetInt32("SelectedCount");
        for (int i = 0; i < count; i++)
        {
            int col = reader.GetInt32($"Cell_{i}_Col");
            int row = reader.GetInt32($"Cell_{i}_Row");
            selectedCells.Add((col, row));
        }

        // Optional: force update to outputs after reading
        this.ExpireSolution(true);

        return base.Read(reader);
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddIntegerParameter("Cols", "C", "Number of columns", GH_ParamAccess.item, 3);
        pManager.AddIntegerParameter("Rows", "R", "Number of rows", GH_ParamAccess.item, 3);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddTextParameter("Selected Cells", "S", "Selected cell coordinates", GH_ParamAccess.list);
        pManager.AddIntegerParameter("Column Indexes", "CI", "Indexes of selected columns", GH_ParamAccess.list);  // New output
        pManager.AddIntegerParameter("Row Indexes", "RI", "Indexes of selected rows", GH_ParamAccess.list);  // New output
        pManager.AddGenericParameter("Unselected Cells", "U", "List of unselected cells", GH_ParamAccess.list);  // New output
    }

    public override void CreateAttributes()
    {
        this.m_attributes = new GorillaGridAttributes(this);
    }

    protected override void SolveInstance(IGH_DataAccess DA)
    {
        int cols = 3;
        int rows = 3;
        DA.GetData(0, ref cols);
        DA.GetData(1, ref rows);

        // Preparing the output data for selected cells
        var selectedOutput = new List<string>();
        foreach (var (col, row) in selectedCells)
        {
            selectedOutput.Add($"({col},{row})");
        }
        DA.SetDataList(0, selectedOutput);  // "S" output

        // Preparing the output data for column indexes
        var columnIndexes = new List<int>();
        foreach (var (col, _) in selectedCells)
        {
            columnIndexes.Add(col);
        }
        DA.SetDataList(1, columnIndexes);  // "CI" output

        // Preparing the output data for row indexes
        var rowIndexes = new List<int>();
        foreach (var (_, row) in selectedCells)
        {
            rowIndexes.Add(row);
        }
        DA.SetDataList(2, rowIndexes);  // "RI" output

        // Preparing the output data for unselected cells
        var unselectedCells = new List<(int, int)>();
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                var key = (col, row);
                if (!selectedCells.Contains(key))
                    unselectedCells.Add(key);
            }
        }
        DA.SetDataList(3, unselectedCells);  // "U" output
    }

}

/*
Known Bugs:
1. ClickableGridComponent
    a. When grid cells are selected, but the field is reduced by the C or R input, the grid cells persist even if the node is not rendering them. In other words, hidden cell selections.
    b. Flatten/Graft/Simplify on outputs renders the symbol on top of the letter
*/