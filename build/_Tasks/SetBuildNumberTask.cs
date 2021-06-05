using Cake.Common.Tools.DotNetCore;
using Cake.Core.Diagnostics;
using Cake.Frosting;

namespace Build
{
    [TaskName("SetBuildNumber")]
    [TaskDescription("Sets the build number when running in a CI system")]
    [Dependency(typeof(RestoreToolsTask))]
    public class SetBuildNumberTask : FrostingTask<BuildContext>
    {
        public override bool ShouldRun(BuildContext context) => context.IsRunningOnAzurePipelines();

        public override void Run(BuildContext context)
        {
            context.Log.Information("Setting Build Number using nbgv");
            context.DotNetCoreTool("tool run nbgv -- cloud --all-vars");
        }
    }
}
