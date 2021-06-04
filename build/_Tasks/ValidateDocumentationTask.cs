using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Run;
using Cake.Core;
using Cake.Core.IO;
using Cake.Frosting;

namespace Build
{
    [TaskName("ValidateDocumentation")]
    [Dependency(typeof(BuildTask))]
    public class ValidateDocumentationTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            context.DotNetCoreRun(
                "./utilities/docs/docs.csproj",
                new ProcessArgumentBuilder()
                    .Append("validate")
                    .Append("./docs")
                    .Append("README.md"),
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
