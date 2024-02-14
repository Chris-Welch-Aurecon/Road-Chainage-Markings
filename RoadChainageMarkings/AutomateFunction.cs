using Objects;
using Objects.Geometry;
using Speckle.Automate.Sdk;
using Speckle.Core.Logging;
using Speckle.Core.Models.Extensions;

static class AutomateFunction
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

    automationContext.MarkRunSuccess($"Counted {polylines.Count()} polylines with {polylines.SelectMany(x => x.GetPoints()).Count()} total points.");
  }
}
