using Cake.Frosting;

namespace Build
{
    [TaskName("CI")]
    [Dependency(typeof(SetBuildNumberTask))]
    [Dependency(typeof(CheckFormattingTask))]
    [Dependency(typeof(BuildTask))]
    [Dependency(typeof(TestTask))]
    [Dependency(typeof(PackTask))]
    [Dependency(typeof(PublishArtifactsTask))]
    public class CITask : FrostingTask
    {
    }
}
