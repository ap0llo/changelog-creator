using Cake.Frosting;

namespace Build
{
    [TaskName("CI")]
    [TaskDescription("Main entry point for the Continuous Integration Build")]
    [Dependency(typeof(SetBuildNumberTask))]
    [Dependency(typeof(BuildTask))]
    [Dependency(typeof(TestTask))]
    [Dependency(typeof(PackTask))]
    [Dependency(typeof(ValidateTask))]
    [Dependency(typeof(GenerateChangeLogTask))]
    public class CITask : FrostingTask
    {
    }
}
