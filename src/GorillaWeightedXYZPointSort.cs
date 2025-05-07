using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;

public class GorillaWeightedXYZPointSort : GH_Component
{
    public GorillaWeightedXYZPointSort()
      : base("Gorilla Sort Points Weighted by XYZ", "Gorilla Sort XYZ",
          "Sorts points by XYZ using either priority or weighted mode",
          "Gorilla", "Points")
    {
    }

    public override Guid ComponentGuid => new Guid("C1234567-89AB-4CDE-ABCD-1234567890FF");

    protected override void RegisterInputParams(GH_InputParamManager p)
    {
        p.AddPointParameter("Points", "Pts", "List of points to sort", GH_ParamAccess.list);
        p.AddBooleanParameter("Use Weights", "W", "Toggle between strict priority (false) and weighted sort (true)", GH_ParamAccess.item, false);
        p.AddIntegerParameter("X Priority", "Xp", "Priority for X (1=high, 3=low, 0=ignore)", GH_ParamAccess.item, 1);
        p.AddIntegerParameter("Y Priority", "Yp", "Priority for Y (1=high, 3=low, 0=ignore)", GH_ParamAccess.item, 2);
        p.AddIntegerParameter("Z Priority", "Zp", "Priority for Z (1=high, 3=low, 0=ignore)", GH_ParamAccess.item, 3);
        p.AddNumberParameter("X Weight", "Xw", "Weight for X if using weighted mode", GH_ParamAccess.item, 1.0);
        p.AddNumberParameter("Y Weight", "Yw", "Weight for Y if using weighted mode", GH_ParamAccess.item, 0.0);
        p.AddNumberParameter("Z Weight", "Zw", "Weight for Z if using weighted mode", GH_ParamAccess.item, 0.0);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager p)
    {
        p.AddPointParameter("Sorted Points", "S", "Sorted point list", GH_ParamAccess.list);
    }

    protected override void SolveInstance(IGH_DataAccess DA)
    {
        List<Point3d> points = new List<Point3d>();
        bool useWeights = false;
        int xPriority = 1, yPriority = 2, zPriority = 3;
        double xWeight = 1.0, yWeight = 0.0, zWeight = 0.0;

        if (!DA.GetDataList(0, points)) return;
        DA.GetData(1, ref useWeights);
        DA.GetData(2, ref xPriority);
        DA.GetData(3, ref yPriority);
        DA.GetData(4, ref zPriority);
        DA.GetData(5, ref xWeight);
        DA.GetData(6, ref yWeight);
        DA.GetData(7, ref zWeight);

        List<Point3d> sorted = new List<Point3d>(points);

        if (!useWeights)
        {
            // Map of multipliers by priority level
            var multipliers = new Dictionary<int, double>
            {
                {1, 1_000_000},
                {2, 10_000},
                {3, 1}
            };

            sorted.Sort((a, b) =>
            {
                double aVal = 0, bVal = 0;
                double mult;

                if (xPriority > 0 && multipliers.TryGetValue(xPriority, out mult))
                {
                    aVal += a.X * mult;
                    bVal += b.X * mult;
                }
                if (yPriority > 0 && multipliers.TryGetValue(yPriority, out mult))
                {
                    aVal += a.Y * mult;
                    bVal += b.Y * mult;
                }
                if (zPriority > 0 && multipliers.TryGetValue(zPriority, out mult))
                {
                    aVal += a.Z * mult;
                    bVal += b.Z * mult;
                }

                return aVal.CompareTo(bVal);
            });

        }
        else
        {
            // Weighted sort
            sorted.Sort((a, b) =>
            {
                double aVal = a.X * xWeight + a.Y * yWeight + a.Z * zWeight;
                double bVal = b.X * xWeight + b.Y * yWeight + b.Z * zWeight;
                return aVal.CompareTo(bVal);
            });
        }

        DA.SetDataList(0, sorted);
    }
}