using Cake.Common.IO;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Test;
using Cake.Core.Diagnostics;
using Cake.Frosting;

namespace Build
{
    [TaskName("Test")]
    [Dependency(typeof(BuildTask))]
    [Dependency(typeof(CleanTestResultsTask))]
    public class TestTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            context.Log.Information($"Running tests for {context.SolutionPath}");
            context.DotNetCoreTest(context.SolutionPath.FullPath, new DotNetCoreTestSettings()
            {
                Configuration = context.BuildConfiguration,
                NoBuild = true,
                Loggers = new[]
                {
                    "trx"
                },
                Collectors = new[] { "XPlat Code Coverage" },
            });
        }
    }
}
