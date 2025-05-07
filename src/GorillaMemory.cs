using System;
using System.Windows.Forms;
using Grasshopper.Kernel;

public class GorillaMemory : GH_Component
{
    // The value that is stored
    private int storedValue = 0;

    public GorillaMemory()
      : base("Gorilla Memory", "Gorilla Mem",
          "Stores a number that persists inside the GH file.",
          "Gorilla", "Data")
    {
    }

    public override Guid ComponentGuid => new Guid("F1E31060-F8D4-4B35-B8BD-9BB8E4D1181A");

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        pManager.AddIntegerParameter("Input", "I", "User input value", GH_ParamAccess.item);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddIntegerParameter("Stored Value", "S", "Persistent stored value", GH_ParamAccess.item);
    }

    protected override void SolveInstance(IGH_DataAccess DA)
    {
        // Attempt to get the input value (if provided by the user)
        int inputValue = storedValue;  // Default to the stored value

        if (DA.GetData(0, ref inputValue)) // If user provided an input, use that value
        {
            storedValue = inputValue; // Store the value
        }

        DA.SetData(0, storedValue); // Output the stored value
    }

    // Use the OverridePersistentData method to save the data persistently
    protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
    {
        base.AppendAdditionalComponentMenuItems(menu);

        // Create a new ToolStripMenuItem for resetting the value
        ToolStripMenuItem resetMenuItem = new ToolStripMenuItem("Reset");
        resetMenuItem.Click += (sender, args) =>
        {
            storedValue = 0; // Reset to default value
        };

        // Add the item to the menu
        menu.Items.Add(resetMenuItem);
    }
}