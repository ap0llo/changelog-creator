using Cake.Common.Build;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Run;
using Cake.Core;
using Cake.Core.IO;
using Cake.Frosting;
using Cake.GitVersioning;

namespace Build
{
    [TaskName("GenerateChangeLog")]
    [TaskDescription("Generates a change log for the current repository")]
    [Dependency(typeof(BuildTask))]
    public class GenerateChangeLogTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            //
            // Generate change log
            //
            var versionOracle = context.GitVersioningGetVersion();

            context.DotNetCoreRun(
                "./src/ChangeLog/Grynwald.ChangeLog.csproj",
                new ProcessArgumentBuilder()
                    .Append($"--currentVersion {versionOracle.NuGetPackageVersion}")
                    .Append($"--versionRange [{versionOracle.NuGetPackageVersion}]")
                    .Append($"--outputpath")
                    .Append(context.ChangeLogOutputPath.FullPath)
                    .Append($"--template GitHubRelease")
                    .Append($"--verbose"),
                new DotNetCoreRunSettings()
                {
                    Configuration = context.BuildConfiguration,
                    NoBuild = true,
                    NoRestore = true,
                    Framework = "net5.0",
                }
            );

            //
            // Publish change log
            //
            if (context.IsRunningOnAzurePipelines())
            {
                context.AzurePipelines().Commands.UploadArtifact(
                    folderName: "",
                    file: context.ChangeLogOutputPath,
                    artifactName: context.ArtifactNames.ChangeLog
                );
            }
        }
    }
}
