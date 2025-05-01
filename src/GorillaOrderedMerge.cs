using System;
using System.Collections;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel.Parameters;

public class GorillaOrderedMergeComponent : GH_Component, IGH_VariableParameterComponent
{
    public GorillaOrderedMergeComponent()
      : base("Gorilla Ordered Merge", "GorillaOrderedMerge",
          "Merges inputs in the order they are connected, like the Merge node but preserving plug order.",
          "Gorilla", "Util")
    { }

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
        // Start with two default dynamic inputs
        pManager.AddGenericParameter("Item 0", "Item 0", "An input item", GH_ParamAccess.tree);
        pManager.AddGenericParameter("Item 1", "Item 1", "An input item", GH_ParamAccess.tree);
        pManager.AddGenericParameter("Item 2", "Item 2", "An input item", GH_ParamAccess.tree);
        pManager[0].Optional = true;
        pManager[1].Optional = true;
        pManager[2].Optional = true;

        // Add Output Mode as dropdown
        var outputMode = new Param_Integer
        {
            Name = "Output Mode",
            NickName = "O",
            Description = "Controls output structure:\n0 = Preserve Tree\n1 = Flatten\n2 = Rebuild Tree",
            Access = GH_ParamAccess.item,
            Optional = true
        };
        outputMode.AddNamedValue("Preserve Tree", 0);
        outputMode.AddNamedValue("Flatten", 1);
        outputMode.AddNamedValue("Rebuild Tree", 2);
        pManager.AddParameter(outputMode);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
        pManager.AddGenericParameter("Merged", "M", "Merged output preserving order and structure", GH_ParamAccess.tree);
    }

    public override void AddedToDocument(GH_Document document)
    {
        base.AddedToDocument(document);
        EnsureTrailingEmptyInput();
    }

    public void VariableParameterMaintenance()
    {
        int outputModeIndex = Params.Input.FindIndex(p => p.Name == "Output Mode");

        // Rename all dynamic inputs to "Item 0", "Item 1", etc.
        for (int i = 0; i < outputModeIndex; i++)
        {
            var param = Params.Input[i];
            param.Name = $"Item {i}";
            param.NickName = $"Item {i}";
        }

        EnsureTrailingEmptyInput();
    }

    private void EnsureTrailingEmptyInput()
    {
        int outputModeIndex = Params.Input.FindIndex(p => p.Name == "Output Mode");
        int dynamicInputCount = outputModeIndex;

        // Check if the last dynamic input has connections
        if (dynamicInputCount == 0 || Params.Input[dynamicInputCount - 1].Sources.Count > 0)
        {
            var newParam = new Param_GenericObject
            {
                Name = $"Item {dynamicInputCount}",
                NickName = $"Item {dynamicInputCount}",
                Description = "An input item",
                Access = GH_ParamAccess.tree,
                Optional = true
            };

            // Insert the new dynamic input right before Output Mode
            Params.RegisterInputParam(newParam, outputModeIndex);
            Params.OnParametersChanged();
        }
    }

    protected override void SolveInstance(IGH_DataAccess DA)
    {
        var outputTree = new GH_Structure<IGH_Goo>();
        int outputMode = 0;
        DA.GetData(Params.Input.Count - 1, ref outputMode); // last param is Output Mode

        int inputCount = Params.Input.Count - 1; // exclude Output Mode
        int pathIndex = 0;

        for (int i = 0; i < inputCount; i++)
        {
            var param = Params.Input[i];
            if (param.Sources.Count == 0)
                continue;

            if (!DA.GetDataTree<IGH_Goo>(i, out GH_Structure<IGH_Goo> dataTree))
                continue;

            foreach (GH_Path path in dataTree.Paths)
            {
                IList rawItems = dataTree.get_Branch(path);
                var castedItems = new List<IGH_Goo>();

                foreach (object obj in rawItems)
                {
                    if (obj is IGH_Goo goo)
                        castedItems.Add(goo);
                }

                switch (outputMode)
                {
                    case 0: // Preserve tree structure
                        outputTree.AppendRange(castedItems, path);
                        break;

                    case 1: // Flatten
                        foreach (var item in castedItems)
                            outputTree.Append(item, new GH_Path(0));
                        break;

                    case 2: // Rebuild tree (ordered)
                        outputTree.AppendRange(castedItems, new GH_Path(pathIndex++));
                        break;

                    default:
                        // Fallback to rebuild behavior if invalid mode
                        outputTree.AppendRange(castedItems, new GH_Path(pathIndex++));
                        break;
                }
            }
        }

        DA.SetDataTree(0, outputTree);
    }

    public bool CanInsertParameter(GH_ParameterSide side, int index)
    {
        return side == GH_ParameterSide.Input && index < Params.Input.Count - 1;
    }

    public bool CanRemoveParameter(GH_ParameterSide side, int index)
    {
        return side == GH_ParameterSide.Input && index < Params.Input.Count - 1;
    }

    public IGH_Param CreateParameter(GH_ParameterSide side, int index)
    {
        if (side == GH_ParameterSide.Input)
        {
            return new Param_GenericObject
            {
                Name = "Item",
                NickName = "Item",
                Description = "An input item",
                Access = GH_ParamAccess.tree,
                Optional = true
            };
        }
        return null;
    }

    public bool DestroyParameter(GH_ParameterSide side, int index) => true;

    public override Guid ComponentGuid => new Guid("fb124470-12a4-4579-babc-dc5d02bdf557");
}
