using Cake.Frosting;

namespace Build
{
    [TaskName("CI")]
    [Dependency(typeof(SetBuildNumberTask))]
    [Dependency(typeof(CheckFormattingTask))]
    [Dependency(typeof(BuildTask))]
    [Dependency(typeof(TestTask))]
    [Dependency(typeof(PublishTestResultsTask))]
    [Dependency(typeof(PackTask))]
    [Dependency(typeof(PublishArtifactsTask))]
    [Dependency(typeof(ValidateDocumentationTask))]
    [Dependency(typeof(ValidateConfigurationSchemaTask))]
    public class CITask : FrostingTask
    {
    }
}
