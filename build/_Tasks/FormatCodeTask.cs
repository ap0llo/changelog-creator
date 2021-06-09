using Cake.Common.Tools.DotNetCore;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Frosting;

namespace Build
{
    [TaskName("FormatCode")]
    [TaskDescription("Applies formatting rules to all code files")]
    [Dependency(typeof(RestoreToolsTask))]
    public class FormatCodeTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            foreach (var dir in context.FormattableCodeDirectories)
            {
                context.Log.Debug($"Formatting code '{dir}')");
                context.DotNetCoreTool(
                    new ProcessArgumentBuilder()
                        .Append("format")
                        .Append(dir.FullPath)
                        .Append("--folder")
                        .Render()
                );
            }

        }
    }
}
