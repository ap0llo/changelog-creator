using Cake.Common.Tools.DotNetCore;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Frosting;

namespace Build
{
    [TaskName("RestoreTools")]
    public class RestoreToolsTask : FrostingTask
    {
        public override void Run(ICakeContext context)
        {
            context.Log.Information("Restoring .NET Local Tools");
            context.DotNetCoreTool("tool restore");
        }
    }
}
