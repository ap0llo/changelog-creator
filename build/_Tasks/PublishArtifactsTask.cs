using Cake.Common.Build;
using Cake.Frosting;

namespace Build
{
    [TaskName("PublishArtifacts")]
    [Dependency(typeof(PackTask))]
    public class PublishArtifactsTask : FrostingTask<BuildContext>
    {
        public override bool ShouldRun(BuildContext context) => context.IsAzurePipelines;

        public override void Run(BuildContext context)
        {
            foreach (var file in context.FileSystem.GetFilePaths(context.PackageOutputPath, "*.nupkg"))
            {
                context.AzurePipelines().Commands.UploadArtifact("", file, context.ArtifactNames.Binaries);
            }
        }
    }
}
