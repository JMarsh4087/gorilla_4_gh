using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.GUI.Canvas;
using Grasshopper.GUI;
using System.Windows.Forms;

public class GorillaPointSortPriorityDropdown : GH_Component
{
    internal int xPriority = 1;
    internal int yPriority = 2;
    internal int zPriority = 3;

    public GorillaPointSortPriorityDropdown()
      : base("Gorilla Point Sort Priority", "Gorilla Point Sort Priority",
          "Right Click to Set XYZ priorities: 0=Ignore, 1=High, 2=Medium, 3=Low",
          "Gorilla", "Util")
    {
    }

    public override Guid ComponentGuid => new Guid("DD123456-ABCD-42DD-9999-000000000001");

    protected override void RegisterInputParams(GH_InputParamManager pManager) { }

    protected override void RegisterOutputParams(GH_OutputParamManager p)
    {
        p.AddIntegerParameter("X Priority", "Xp", "Priority for X", GH_ParamAccess.item);
        p.AddIntegerParameter("Y Priority", "Yp", "Priority for Y", GH_ParamAccess.item);
        p.AddIntegerParameter("Z Priority", "Zp", "Priority for Z", GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess DA)
    {
        DA.SetData(0, xPriority);
        DA.SetData(1, yPriority);
        DA.SetData(2, zPriority);
    }

    protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
    {
        base.AppendAdditionalComponentMenuItems(menu);

        var options = new Dictionary<string, int>
        {
            { "Ignore (0)", 0 },
            { "High (1)", 1 },
            { "Medium (2)", 2 },
            { "Low (3)", 3 }
        };

        ToolStripMenuItem xMenu = new ToolStripMenuItem("Set X Priority");
        ToolStripMenuItem yMenu = new ToolStripMenuItem("Set Y Priority");
        ToolStripMenuItem zMenu = new ToolStripMenuItem("Set Z Priority");

        foreach (var kv in options)
        {
            var xItem = new ToolStripMenuItem(kv.Key, null, (s, e) => { xPriority = kv.Value; ExpireSolution(true); })
            { Checked = xPriority == kv.Value };
            xMenu.DropDownItems.Add(xItem);

            var yItem = new ToolStripMenuItem(kv.Key, null, (s, e) => { yPriority = kv.Value; ExpireSolution(true); })
            { Checked = yPriority == kv.Value };
            yMenu.DropDownItems.Add(yItem);

            var zItem = new ToolStripMenuItem(kv.Key, null, (s, e) => { zPriority = kv.Value; ExpireSolution(true); })
            { Checked = zPriority == kv.Value };
            zMenu.DropDownItems.Add(zItem);
        }

        menu.Items.Add(xMenu);
        menu.Items.Add(yMenu);
        menu.Items.Add(zMenu);
    }

    public override GH_Exposure Exposure => GH_Exposure.secondary;

    public override bool Write(GH_IO.Serialization.GH_IWriter writer)
    {
        writer.SetInt32("XPriority", xPriority);
        writer.SetInt32("YPriority", yPriority);
        writer.SetInt32("ZPriority", zPriority);
        return base.Write(writer);
    }

    public override bool Read(GH_IO.Serialization.GH_IReader reader)
    {
        xPriority = reader.GetInt32("XPriority");
        yPriority = reader.GetInt32("YPriority");
        zPriority = reader.GetInt32("ZPriority");
        return base.Read(reader);
    }
}
