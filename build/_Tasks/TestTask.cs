using System;
using System.Collections.Generic;
using System.Text;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Build;
using Cake.Common.Tools.DotNetCore.MSBuild;
using Cake.Common.Tools.DotNetCore.Restore;
using Cake.Common.Tools.DotNetCore.Test;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Frosting;

namespace Build
{
    [TaskName("Test")]
    [Dependency(typeof(BuildTask))]
    public class TestTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            context.Log.Information($"Running tests for {context.SolutionPath}");
            context.DotNetCoreTest(context.SolutionPath.FullPath, new DotNetCoreTestSettings()
            {
                NoBuild = true,
                Configuration = context.BuildConfiguration,
                Collectors = new[] { "XPlat Code Coverage" },
            });

            //TODO: On Azure Pipelines, publish test results and coverage results
        }
    }
}
