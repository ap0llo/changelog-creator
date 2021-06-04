using Cake.Common.IO;
using Cake.Core.Diagnostics;
using Cake.Frosting;

namespace Build
{
    [TaskName("CleanTestResults")]
    public class CleanTestResultsTask : FrostingTask<BuildContext>
    {
        public override bool ShouldRun(BuildContext context) => context.DirectoryExists(context.TestResultsPath);

        public override void Run(BuildContext context)
        {
            context.Log.Information($"Cleaning test results directory '{context.TestResultsPath}'");
            context.DeleteDirectory(context.TestResultsPath, new() { Force = true, Recursive = true });
        }
    }
}
