using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;

public class GorillaLineFlipByBoolean : GH_Component
{
    public GorillaLineFlipByBoolean()
      : base("Gorilla Flip Lines By Boolean", "Flip Line Bool",
          "Flips the direction of lines based on boolean list",
          "Gorilla", "Lines")
    {
    }

    public override Guid ComponentGuid => new Guid("D1E31060-F8D4-4B35-B8BD-9BB8E4D1180F");

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddLineParameter("Lines", "L", "List of lines", GH_ParamAccess.list);
        pManager.AddBooleanParameter("Flip", "F", "List of booleans indicating which lines to flip", GH_ParamAccess.list);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddLineParameter("Result", "R", "List of flipped/unchanged lines", GH_ParamAccess.list);
    }

    protected override void SolveInstance(IGH_DataAccess DA)
    {
        List<Line> lines = new List<Line>();
        List<bool> flip = new List<bool>();

        if (!DA.GetDataList(0, lines)) return;
        if (!DA.GetDataList(1, flip)) return;

        if (lines.Count != flip.Count)
        {
            AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Lines and Flip lists must have the same length.");
            return;
        }

        List<Line> result = new List<Line>();

        for (int i = 0; i < lines.Count; i++)
        {
            Line ln = lines[i];
            if (flip[i])
                ln.Flip();

            result.Add(ln);
        }

        DA.SetDataList(0, result);
    }

    protected override System.Drawing.Bitmap Icon => null; // You can assign an icon if desired
}