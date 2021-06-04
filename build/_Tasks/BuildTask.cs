using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Build;
using Cake.Common.Tools.DotNetCore.MSBuild;
using Cake.Common.Tools.DotNetCore.Restore;
using Cake.Core.Diagnostics;
using Cake.Frosting;

namespace Build
{
    [TaskName("Build")]
    public class BuildTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            context.Log.Information("Restoring NuGet Packages");
            context.DotNetCoreRestore(context.SolutionPath.FullPath, new DotNetCoreRestoreSettings()
            {
                MSBuildSettings = new()
                {
                    TreatAllWarningsAs = MSBuildTreatAllWarningsAs.Error
                }
            });

            context.Log.Information($"Building {context.SolutionPath}");
            context.DotNetCoreBuild(context.SolutionPath.FullPath, new DotNetCoreBuildSettings()
            {
                Configuration = context.BuildConfiguration,
                NoRestore = true,
                MSBuildSettings = new()
                {
                    TreatAllWarningsAs = MSBuildTreatAllWarningsAs.Error
                },
            });

        }
    }
}
