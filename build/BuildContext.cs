using Cake.Common.Build;
using Cake.Core;
using Cake.Core.IO;
using Cake.Frosting;


namespace Build
{
    public class BuildContext : FrostingContext
    {
        public class ArtifactNameSettings
        {
            public string Binaries { get; init; } = "";
        }

        public DirectoryPath RootDirectory { get; set; }

        public FilePath SolutionPath => RootDirectory.CombineWithFilePath("ChangeLog.sln");

        public string BuildConfiguration { get; set; }

        public DirectoryPath BaseOutputPath { get; set; }

        public DirectoryPath PackageOutputPath { get; }

        public DirectoryPath TestResultsPath { get; }

        public bool IsAzurePipelines { get; }


        public ArtifactNameSettings ArtifactNames { get; } = new ArtifactNameSettings()
        {
            Binaries = "Binaries"
        };


        public BuildContext(ICakeContext context) : base(context)
        {
            RootDirectory = context.Environment.WorkingDirectory;

            BuildConfiguration = context.Arguments.GetArgument("configuration") ?? "Release";

            IsAzurePipelines = context.AzurePipelines().IsRunningOnAzurePipelines || context.AzurePipelines().IsRunningOnAzurePipelinesHosted;

            //TODO: This is duplicates in Directory.Build.props
            BaseOutputPath = IsAzurePipelines
                ? context.AzurePipelines().Environment.Build.BinariesDirectory.FullPath
                : RootDirectory.CombineWithFilePath("Binaries").FullPath;

            PackageOutputPath = IsAzurePipelines
                ? context.AzurePipelines().Environment.Build.ArtifactStagingDirectory.FullPath
                : BaseOutputPath.Combine(DirectoryPath.FromString($"{BuildConfiguration}/packages/"));

            TestResultsPath = BaseOutputPath.CombineWithFilePath("TestResults").FullPath;
        }
    }
}
