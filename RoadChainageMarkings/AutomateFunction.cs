using Objects;
using Objects.Geometry;
using RoadChainageMarkings;
using Speckle.Automate.Sdk;
using Speckle.Core.Logging;
using Speckle.Core.Models.Extensions;

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

		var polylines = commitObject
		  .Flatten()
		  .Where(b => b is Polyline)
		  .Cast<Polyline>();

		var outputFile = $"out/Aligment Markers.dxf";

		Directory.CreateDirectory("out");

		polylines.SaveToDxf(functionInputs.Spacing, outputFile);

		Console.WriteLine("Storing file");
		await automationContext.StoreFileResult(outputFile);

		automationContext.AttachResultToObjects(Speckle.Automate.Sdk.Schema.ObjectResultLevel.Info, "Alignments", polylines.Select(x => x.id), "Processed curves");

		automationContext.MarkRunSuccess($"Counted {polylines.Count()} polylines with {polylines.SelectMany(x => x.GetPoints()).Count()} total points. When divided by {functionInputs.Spacing}, this resulted in {polylines.SelectMany(x => x.FramesAtDistance(functionInputs.Spacing)).Count()} frames.");
	}
}
