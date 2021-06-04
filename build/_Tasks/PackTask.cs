using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.MSBuild;
using Cake.Common.Tools.DotNetCore.Pack;
using Cake.Core.Diagnostics;
using Cake.Frosting;

namespace Build
{
    [TaskName("Pack")]
    [Dependency(typeof(BuildTask))]
    public class PackTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            context.Log.Information($"Running 'dotnet pack' for {context.SolutionPath}");
            context.DotNetCorePack(context.SolutionPath.FullPath, new DotNetCorePackSettings()
            {
                Configuration = context.BuildConfiguration,
                NoBuild = true,
                OutputDirectory = context.PackageOutputPath,
                MSBuildSettings = new()
                {
                    TreatAllWarningsAs = MSBuildTreatAllWarningsAs.Error
                }
            });
        }
    }
}
