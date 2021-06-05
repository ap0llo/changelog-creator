using Cake.Common.Build;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.MSBuild;
using Cake.Common.Tools.DotNetCore.Pack;
using Cake.Core.Diagnostics;
using Cake.Frosting;

namespace Build
{
    [TaskName("Pack")]
    [TaskDescription("Generates all NuGet packages")]
    [Dependency(typeof(BuildTask))]
    public class PackTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            // 
            // Pack NuGet Packages
            // 
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

            //
            // Publish Artifacts
            //
            if (context.IsRunningOnAzurePipelines())
            {
                context.Log.Information("Publishing NuGet packages to Azure Pipelines");
                foreach (var file in context.FileSystem.GetFilePaths(context.PackageOutputPath, "*.nupkg"))
                {
                    context.Log.Debug("Publishing '{file}'");
                    context.AzurePipelines().Commands.UploadArtifact("", file, context.ArtifactNames.Binaries);
                }
            }
        }
    }
}
