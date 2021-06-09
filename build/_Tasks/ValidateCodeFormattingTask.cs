using System.Linq;
using Cake.Common.Tools.DotNetCore;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Frosting;

namespace Build
{
    [TaskName("ValidateCodeFormatting")]
    [TaskDescription("Validates that all code files are formatted properly.")]
    [Dependency(typeof(RestoreToolsTask))]
    public class ValidateCodeFormattingTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            foreach (var dir in context.FormattableCodeDirectories)
            {
                context.Log.Debug($"Checking code formatting in '{dir}')");
                context.DotNetCoreTool(
                    new ProcessArgumentBuilder()
                        .Append("format")
                        .Append(dir.FullPath)
                        .Append("--folder")
                        .Append("--check")
                        .Render()
                );
            }
        }
    }
}
