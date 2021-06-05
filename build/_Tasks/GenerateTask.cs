using Cake.Frosting;

namespace Build
{
    [TaskName("Generate")]
    [TaskDescription("Updates all auto-generated files and applies formatting rules")]
    [Dependency(typeof(FormatCodeTask))]
    [Dependency(typeof(GenerateConfigurationSchemaTask))]
    [Dependency(typeof(GenerateDocumentationTask))]
    public class GenerateTask : FrostingTask<BuildContext>
    {
    }
}
