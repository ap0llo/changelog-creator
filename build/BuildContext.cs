using System;
using System.Collections.Generic;
using Cake.Common;
using Cake.Common.IO;
using Cake.Core;
using Cake.Core.IO;
using Cake.Frosting;

namespace Build
{
    public class BuildContext : FrostingContext
    {
        public class ArtifactNameSettings
        {
            /// <summary>
            /// The name of the main artifact on Azure Pipelines
            /// </summary>
            public string Binaries => "Binaries";

            /// <summary>
            /// The Azure Pipelines artifact name under which to save the auto-generated change log
            /// </summary>
            public string ChangeLog => "ChangeLog";

            /// <summary>
            /// The Azure Pipelines artifact name under which to save test result files
            /// </summary>
            public string TestResults => "TestResults";
        }

        /// <summary>
        /// Gets the path of the repository's root directory
        /// </summary>
        public DirectoryPath RootDirectory { get; set; }

        /// <summary>
        /// Gets the path of the Visual Studio solution to build/test
        /// </summary>
        public FilePath SolutionPath => RootDirectory.CombineWithFilePath("ChangeLog.sln");

        /// <summary>
        /// Gets the configuration to build (Debug/Relesae)
        /// </summary>
        public string BuildConfiguration { get; set; }

        /// <summary>
        /// Gets the base output path for the build
        /// </summary>
        public DirectoryPath BaseOutputPath { get; set; }

        /// <summary>
        /// Gets the output path for NuGet packages
        /// </summary>
        public DirectoryPath PackageOutputPath { get; }

        /// <summary>
        /// Gets the output path for test results
        /// </summary>
        public DirectoryPath TestResultsPath { get; }

        /// <summary>
        /// Gets the output path for the auto-generated change log
        /// </summary>
        public FilePath ChangeLogOutputPath => BaseOutputPath.CombineWithFilePath("changelog.md");

        /// <summary>
        /// Gets whether test execution should collect code coverage
        /// </summary>
        public bool CollectCodeCoverage { get; }

        /// <summary>
        /// Gets the names for Azure Pipelines artifacts
        /// </summary>
        public ArtifactNameSettings ArtifactNames { get; } = new();

        /// <summary>
        /// Gets the path of the directory to store code coverage history
        /// </summary>
        public DirectoryPath CodeCoverageHistoryDirectory => BaseOutputPath.Combine("CoverageHistory");

        /// <summary>
        /// Gets the path of the directory to save code coverage to (including the coverage HTML report)
        /// </summary>
        public DirectoryPath CodeCoverageOutputDirectory => TestResultsPath.Combine("Coverage");

        /// <summary>
        /// Gets the directories to apply code formatting rules to.
        /// </summary>
        public IEnumerable<DirectoryPath> FormattableCodeDirectories
        {
            get
            {
                yield return RootDirectory.Combine("build");
                yield return RootDirectory.Combine("src");
                yield return RootDirectory.Combine("utilities");
            }
        }


        public BuildContext(ICakeContext context) : base(context)
        {
            RootDirectory = context.Environment.WorkingDirectory;

            BuildConfiguration = context.Argument("configuration", "Release");
            CollectCodeCoverage = context.Argument("collect-code-coverage", true);

            // Values are defined in Directory.Build.props and embedded into the build assembly
            // using the 'ThisAssembly.Project' source generator
            BaseOutputPath = ThisAssembly.Project.BaseOutputPath;
            PackageOutputPath = ThisAssembly.Project.PackageOutputPath;
            TestResultsPath = ThisAssembly.Project.VSTestResultsDirectory;

            Validate();
        }


        private void Validate()
        {
            if (!this.DirectoryExists(RootDirectory))
            {
                throw new Exception($"Repository root directory '{RootDirectory}' does not exist");
            }

            if (!BuildConfiguration.Equals("Release", StringComparison.OrdinalIgnoreCase) &&
               !BuildConfiguration.Equals("Debug", StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception($"Unsupported build configuration '{BuildConfiguration}'");
            }
        }
    }
}
