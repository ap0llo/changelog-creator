using Cake.Common.Tools.DotNetCore;
using Cake.Frosting;

namespace Build
{
    [TaskName("CheckFormatting")]
    [Dependency(typeof(RestoreToolsTask))]
    public class CheckFormattingTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            context.DotNetCoreTool("format ./src --folder --check");
            context.DotNetCoreTool("format ./utilities --folder --check");
            context.DotNetCoreTool("format ./build --folder --check");
        }
    }
}
