﻿using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Run;
using Cake.Core;
using Cake.Core.IO;
using Cake.Frosting;

namespace Build
{
    [TaskName("GenerateConfigurationSchema")]
    [TaskDescription("(Re)generates the configuration file JSON schema")]
    [Dependency(typeof(BuildTask))]
    public class GenerateConfigurationSchemaTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            context.DotNetCoreRun(
                "./utilities/schema/schema.csproj",
                new ProcessArgumentBuilder()
                    .Append("generate")
                    .Append("./schemas/configuration/schema.json"),
                new DotNetCoreRunSettings()
                {
                    Configuration = context.BuildConfiguration,
                    NoBuild = true,
                    NoRestore = true,
                }
            );
        }
    }
}
