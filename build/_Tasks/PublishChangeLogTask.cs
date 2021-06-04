using Cake.Common.Build;
using Cake.Frosting;

namespace Build
{
    [TaskName("PublishChangeLog")]
    [Dependency(typeof(GenerateChangeLogTask))]
    public class PublishChangeLogTask : FrostingTask<BuildContext>
    {
        public override bool ShouldRun(BuildContext context) => context.IsAzurePipelines;

        public override void Run(BuildContext context)
        {
            context.AzurePipelines().Commands.UploadArtifact(
                folderName: "",
                file: context.ChangeLogOutputPath,
                artifactName: context.ArtifactNames.ChangeLog
            );
        }
    }
}
