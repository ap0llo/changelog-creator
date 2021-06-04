using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Run;
using Cake.Core;
using Cake.Core.IO;
using Cake.Frosting;
using Cake.GitVersioning;

namespace Build
{
    [TaskName("GenerateChangeLog")]
    [Dependency(typeof(BuildTask))]
    public class GenerateChangeLogTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
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
        }
    }
}
