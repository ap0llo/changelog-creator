using System;
using System.Collections.Generic;
using System.Text;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Build;
using Cake.Common.Tools.DotNetCore.MSBuild;
using Cake.Common.Tools.DotNetCore.Pack;
using Cake.Common.Tools.DotNetCore.Restore;
using Cake.Common.Tools.DotNetCore.Test;
using Cake.Core;
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
                NoBuild = true,
                Configuration = context.BuildConfiguration,
                OutputDirectory = context.PackageOutputPath,
                MSBuildSettings = new()
                {
                    TreatAllWarningsAs = MSBuildTreatAllWarningsAs.Error
                }
            });


            //TODO: On Azure Pipelines, use Build.ArtifactStagingDirectory
        }
    }
}
