using Cake.Frosting;

namespace Build
{
    [TaskName("Default")]
    [Dependency(typeof(BuildTask))]
    public class DefaultTask : FrostingTask
    {
    }
}
