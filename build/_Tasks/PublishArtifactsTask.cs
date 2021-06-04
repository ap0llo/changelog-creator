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
            if (context.AzurePipelines().IsRunningOnAzurePipelines)
            {
                var packagesFiles = Directory.GetFiles(context.PackageOutputPath.FullPath, "*.nupkg");

                foreach (var file in packagesFiles)
                {
                    context.AzurePipelines().Commands.UploadArtifact(context.ArtifactNames.Binaries, file);
                }
            }
        }

    }
}
