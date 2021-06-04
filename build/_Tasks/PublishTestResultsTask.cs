using System.Linq;
using Cake.Common.Build;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Frosting;

namespace Build
{
    [TaskName("PublishTestResults")]
    [Dependency(typeof(TestTask))]
    public class PublishTestResultsTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            if (context.IsAzurePipelines)
            {
                var azurePipelines = context.AzurePipelines();

                context.Log.Information("Publishing Test Results");
                azurePipelines.Commands.PublishTestResults(new()
                {
                    Configuration = context.BuildConfiguration,
                    TestResultsFiles = context.FileSystem.GetDirectory(context.TestResultsPath).GetFiles("*.trx", SearchScope.Current).Select(x => x.Path).ToArray()
                });
            }
        }

    }
}
