using System;
using System.Linq;
using Cake.Common.Build;
using Cake.Common.Build.AzurePipelines.Data;
using Cake.Common.Diagnostics;
using Cake.Common.Tools.DotNetCore;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Frosting;

namespace Build
{
    [TaskName("PublishTestResults")]
    [Dependency(typeof(TestTask))]
    public class PublishTestResultsTask : FrostingTask<BuildContext>
    {
        public override bool ShouldRun(BuildContext context) => context.IsAzurePipelines;

        public override void Run(BuildContext context)
        {
            var azurePipelines = context.AzurePipelines();

            context.Log.Information("Publishing Test Results");
            var testResults = context.FileSystem.GetFilePaths(context.TestResultsPath, "*.trx", SearchScope.Current);

            if (!testResults.Any())
                throw new Exception($"No test results found in '{context.TestResultsPath}'");

            azurePipelines.Commands.PublishTestResults(new()
            {
                Configuration = context.BuildConfiguration,
                TestResultsFiles = testResults,
                TestRunner = AzurePipelinesTestRunnerType.VSTest
            });

            // Azure Pipelines only supports publishing a single coverage result file,
            // so the results are merged into a single cobertura file using "ReportGenerator"

            var coverageFiles = context.FileSystem.GetFilePaths(context.TestResultsPath, "coverage.cobertura.xml", SearchScope.Recursive);
            if (!coverageFiles.Any())
                throw new Exception($"No coverage files found in '{context.TestResultsPath}'");

            context.Log.Information($"Found {coverageFiles.Count} coverage files");

            context.Log.Information("Merging coverage files");


            var htmlReportType = context.IsAzurePipelines ? "HtmlInline_AzurePipelines" : "Html";
            context.DotNetCoreTool(new ProcessArgumentBuilder()
                .Append("tool")
                .Append("run")
                .Append("reportgenerator")
                .Append("--")
                .Append($"-reports:{String.Join(";", coverageFiles)}")
                .Append($"-targetdir:{context.TestResultsPath.Combine("Coverage")}")
                .Append($"-reporttypes:cobertura;{htmlReportType}")
                .Render()
            );

            context.Log.Information("Publishing Code Coverage Results");
            azurePipelines.Commands.PublishCodeCoverage(new()
            {
                CodeCoverageTool = AzurePipelinesCodeCoverageToolType.Cobertura,
                SummaryFileLocation = context.TestResultsPath.CombineWithFilePath("Coverage/Cobertura.xml"),
                ReportDirectory = context.TestResultsPath.Combine("Coverage"),
            });
        }

    }
}
