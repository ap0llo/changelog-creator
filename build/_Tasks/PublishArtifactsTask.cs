using System.IO;
using Cake.Common.Build;
using Cake.Common.IO;
using Cake.Frosting;

namespace Build
{
    [TaskName("PublishArtifacts")]
    [Dependency(typeof(PackTask))]
    public class PublishArtifactsTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            if (context.IsAzurePipelines)
            {
                foreach (var file in context.FileSystem.GetFilePaths(context.PackageOutputPath, "*.nupkg"))
                {
                    context.AzurePipelines().Commands.UploadArtifact(context.ArtifactNames.Binaries, file);
                }
            }
        }

    }
}
