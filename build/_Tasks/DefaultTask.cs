using Cake.Frosting;

namespace Build
{
    [TaskName("Default")]
    [TaskDescription("Default build (just builds the project)")]
    [Dependency(typeof(BuildTask))]
    public class DefaultTask : FrostingTask
    {
    }
}
