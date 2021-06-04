using System;
using System.Collections.Generic;
using System.Text;
using Cake.Common.Build;
using Cake.Common.Tools.DotNetCore;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Frosting;

namespace Build
{
    [TaskName("SetBuildNumber")]
    [Dependency(typeof(RestoreToolsTask))]
    public class SetBuildNumberTask : FrostingTask
    {
        public override void Run(ICakeContext context)
        {
            if (context.AzurePipelines().IsRunningOnAzurePipelines)
            {
                context.Log.Information("Setting Build Number using nbgv");
                context.DotNetCoreTool("nbgv cloud --all-vars");
            }
        }
    }
}
