using Cake.Frosting;

namespace Build
{
    [TaskName("Validate")]
    [TaskDescription("Validates files under source control")]
    [Dependency(typeof(ValidateCodeFormattingTask))]
    [Dependency(typeof(ValidateDocumentationTask))]
    [Dependency(typeof(ValidateConfigurationSchemaTask))]
    public class ValidateTask : FrostingTask
    {
    }
}
