using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Tool;
using Cake.Core;
using Cake.Frosting;

namespace Build
{
    [TaskName("CheckFormatting")]
    [Dependency(typeof(RestoreToolsTask))]
    public class CheckFormattingTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            var toolSettings = new DotNetCoreToolSettings()
            {
                WorkingDirectory = context.RootDirectory
            };

            context.DotNetCoreTool("format ./src --folder --check", toolSettings);
            context.DotNetCoreTool("format ./utilities --folder --check", toolSettings);
            context.DotNetCoreTool("format ./build --folder --check", toolSettings);
        }
    }
}
