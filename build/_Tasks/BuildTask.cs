using System;
using System.Collections.Generic;
using System.Text;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Build;
using Cake.Common.Tools.DotNetCore.MSBuild;
using Cake.Common.Tools.DotNetCore.Restore;
using Cake.Core;
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
                WorkingDirectory = context.RootDirectory,
                MSBuildSettings = new()
                {
                    TreatAllWarningsAs = MSBuildTreatAllWarningsAs.Error
                }
            });

            context.Log.Information($"Building {context.SolutionPath}");
            context.DotNetCoreBuild(context.SolutionPath.FullPath, new DotNetCoreBuildSettings()
            {
                WorkingDirectory = context.RootDirectory,
                NoRestore = true,
                MSBuildSettings = new()
                {
                    TreatAllWarningsAs = MSBuildTreatAllWarningsAs.Error
                },
                Configuration = context.BuildConfiguration
            });

        }
    }
}
