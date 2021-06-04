using Cake.Common.Build;
using Cake.Common.Build.AzurePipelines.Data;
using Cake.Common.Diagnostics;
using Cake.Common.Tools.ReportGenerator;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Frosting;

namespace Build
{
    [TaskName("PublishTestResults")]
    [Dependency(typeof(TestTask))]
    public class PublishTestResultsTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            if (context.IsAzurePipelines)
            {
                var azurePipelines = context.AzurePipelines();

                context.Log.Information("Publishing Test Results");
                azurePipelines.Commands.PublishTestResults(new()
                {
                    Configuration = context.BuildConfiguration,
                    TestResultsFiles = context.FileSystem.GetFilePaths(context.TestResultsPath, "*.trx", SearchScope.Current)
                });


                // The PublishCodeCoverageResults task only supports publishing a single coverage result file,
                // so the results are merged into a single cobertura file using "ReportGenerator"


                var coverageFiles = context.FileSystem.GetFilePaths(context.TestResultsPath, "coverage.cobertura.xml", SearchScope.Recursive);
                context.Log.Information($"Found {coverageFiles.Count} coverage files");


                context.Log.Information("Merging coverage files");
                context.ReportGenerator(
                    reports: coverageFiles,
                    targetDir: context.TestResultsPath.Combine("Coverage"),
                    new ReportGeneratorSettings()
                    {
                        ReportTypes = new[] { ReportGeneratorReportType.Cobertura }
                    }
                );

                context.Log.Information("Publishing Code Coverage Results");
                azurePipelines.Commands.PublishCodeCoverage(new()
                {
                    CodeCoverageTool = AzurePipelinesCodeCoverageToolType.Cobertura,
                    SummaryFileLocation = context.TestResultsPath.CombineWithFilePath("Coverage/Cobertura.xml")
                });
            }
        }

    }
}
