using Objects;
using Objects.Geometry;
using RoadChainageMarkings;
using Speckle.Automate.Sdk;
using Speckle.Core.Logging;
using Speckle.Core.Models.Extensions;
using Speckle.Core.Models.GraphTraversal;

public static class AutomateFunction
{
	public static async Task Run(
	  AutomationContext automationContext,
	  FunctionInputs functionInputs
	)
	{
		Console.WriteLine("Starting execution");
		_ = typeof(ObjectsKit).Assembly; // INFO: Force objects kit to initialize

		Console.WriteLine("Receiving version");
		var commitObject = await automationContext.ReceiveVersion();

		Console.WriteLine("Received version: " + commitObject);

		IEnumerable<Curve> curves = commitObject
		  .Flatten()
		  .Where(b => b is Curve)
		  .Select(b => (Curve)b);

		IEnumerable<Polyline> polylines = curves
			.Select(x => x.TryGetDisplayValue<Polyline>())
			.SelectMany(x => x ?? new List<Polyline>() { });

		var outputFile = $"out/Aligment Markers.dxf";

		Directory.CreateDirectory("out");

		var @base = polylines.CreateNewModel(functionInputs.Spacing, outputFile);

		automationContext.AttachResultToObjects(
			Speckle.Automate.Sdk.Schema.ObjectResultLevel.Info, 
			"Alignments",
			curves.Select(x => x.id), 
			"Processed curves");

		var version = await automationContext.CreateNewVersionInProject(@base, "Cross Sections", $"Procedurally generated cross sections from version '{automationContext.AutomationRunData.VersionId}'");

		if (version is string str) automationContext.SetContextView(new() { str });

		automationContext.MarkRunSuccess($"Counted {polylines.Count()} polylines with {polylines.SelectMany(x => x.GetPoints()).Count()} total points. When divided by {functionInputs.Spacing}, this resulted in {polylines.SelectMany(x => x.FramesAtDistance(functionInputs.Spacing)).Count()} frames.");
	}
}
