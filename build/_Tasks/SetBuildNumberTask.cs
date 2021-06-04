﻿using Cake.Common.Tools.DotNetCore;
using Cake.Core.Diagnostics;
using Cake.Frosting;

namespace Build
{
    [TaskName("SetBuildNumber")]
    [Dependency(typeof(RestoreToolsTask))]
    public class SetBuildNumberTask : FrostingTask<BuildContext>
    {
        public override bool ShouldRun(BuildContext context) => context.IsAzurePipelines;

        public override void Run(BuildContext context)
        {
            context.Log.Information("Setting Build Number using nbgv");
            context.DotNetCoreTool("tool run nbgv -- cloud --all-vars");
        }
    }
}
