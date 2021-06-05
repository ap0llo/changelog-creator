using System;
using System.Linq;
using Cake.Common.Build;
using Cake.Common.Build.AzurePipelines.Data;
using Cake.Common.IO;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Test;
using Cake.Core;
using Cake.Core.Diagnostics;
using Cake.Core.IO;
using Cake.Frosting;

namespace Build
{
    [TaskName("Test")]
    [TaskDescription("Runs all tests")]
    [Dependency(typeof(BuildTask))]
    public class TestTask : FrostingTask<BuildContext>
    {
        public override void Run(BuildContext context)
        {
            if (context.DirectoryExists(context.TestResultsPath))
            {
                context.Log.Information($"Cleaning test results directory '{context.TestResultsPath}'");
                context.DeleteDirectory(context.TestResultsPath, new() { Force = true, Recursive = true });
            }

            RunTests(context);

            if (context.CollectCodeCoverage)
            {
                GenerateCoverageReport(context);
            }
        }

        public override void OnError(Exception exception, BuildContext context)
        {
            // If test execution failed, publish test results anyways (so the error can be inspected)
            // but do not throw in PublishTestResults() when there are not test results
            PublishTestResults(context, failInMissingTestResults: false);

            base.OnError(exception, context);
        }

        private void RunTests(BuildContext context)
        {
            context.Log.Information($"Running tests for {context.SolutionPath}, Collect Code Coverage: {context.CollectCodeCoverage}");

            // Run tests
            var settings = new DotNetCoreTestSettings()
            {
                Configuration = context.BuildConfiguration,
                NoBuild = true,
                Loggers = new[] { "trx" },
            };

            if (context.CollectCodeCoverage)
            {
                settings.Collectors = new[] { "XPlat Code Coverage" };
            }

            context.DotNetCoreTest(
                context.SolutionPath.FullPath,
                settings
            );

            // Publish Test Resilts
            PublishTestResults(context, failInMissingTestResults: true);
        }

        private static void PublishTestResults(BuildContext context, bool failInMissingTestResults)
        {
            var testResults = context.FileSystem.GetFilePaths(context.TestResultsPath, "*.trx", SearchScope.Current);

            if (!testResults.Any() && failInMissingTestResults)
                throw new Exception($"No test results found in '{context.TestResultsPath}'");

            if (context.IsRunningOnAzurePipelines())
            {
                context.Log.Information("Publishing Test Results to Azure Pipelines");
                var azurePipelines = context.AzurePipelines();

                // Publish test results to Azure Pipelines test UI
                azurePipelines.Commands.PublishTestResults(new()
                {
                    Configuration = context.BuildConfiguration,
                    TestResultsFiles = testResults,
                    TestRunner = AzurePipelinesTestRunnerType.VSTest
                });

                // Publish result files as downloadable artifact
                foreach (var testResult in testResults)
                {
                    context.Log.Debug($"Publishing '{testResult}' as build artifact");
                    azurePipelines.Commands.UploadArtifact(
                        folderName: "",
                        file: testResult,
                        context.ArtifactNames.TestResults
                    );
                }
            }
        }

        private void GenerateCoverageReport(BuildContext context)
        {
            if (context.DirectoryExists(context.CodeCoverageOutputDirectory))
            {
                context.Log.Information($"Cleaning code coveraget output directory '{context.CodeCoverageOutputDirectory}'");
                context.DeleteDirectory(context.CodeCoverageOutputDirectory, new() { Force = true, Recursive = true });
            }

            var coverageFiles = context.FileSystem.GetFilePaths(context.TestResultsPath, "coverage.cobertura.xml", SearchScope.Recursive);

            if (!coverageFiles.Any())
                throw new Exception($"No coverage files found in '{context.TestResultsPath}'");

            context.Log.Information($"Found {coverageFiles.Count} coverage files");

            //
            // Generate Coverage Report and merged code coverage file
            //
            context.Log.Information("Merging coverage files");
            var htmlReportType = context.IsRunningOnAzurePipelines() ? "HtmlInline_AzurePipelines" : "Html";
            context.DotNetCoreTool(new ProcessArgumentBuilder()
                .Append("tool")
                .Append("run")
                .Append("reportgenerator")
                .Append("--")
                .Append($"-reports:{String.Join(";", coverageFiles)}")
                .Append($"-targetdir:{context.CodeCoverageOutputDirectory}")
                .Append($"-reporttypes:cobertura;{htmlReportType}")
                .Append($"-historyDir:{context.CodeCoverageHistoryDirectory}")
                .Render()
            );

            //
            // Publish Code coverage report
            //
            if (context.IsRunningOnAzurePipelines())
            {
                context.Log.Information("Publishing Code Coverage Results to Azure Pipelines");
                context.AzurePipelines().Commands.PublishCodeCoverage(new()
                {
                    CodeCoverageTool = AzurePipelinesCodeCoverageToolType.Cobertura,
                    SummaryFileLocation = context.CodeCoverageOutputDirectory.CombineWithFilePath("Cobertura.xml"),
                    ReportDirectory = context.CodeCoverageOutputDirectory
                });
            }
        }
    }
}
