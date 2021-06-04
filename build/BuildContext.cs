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

        //TODO
        public DirectoryPath RootDirectory { get; set; } = DirectoryPath.FromString(System.IO.Path.GetFullPath(".."));

        public FilePath SolutionPath => RootDirectory.CombineWithFilePath("ChangeLog.sln");

        public string BuildConfiguration { get; set; }

        public DirectoryPath PackageOutputPath { get; }


        public bool IsAzurePipelines { get; }

        public ArtifactNameSettings ArtifactNames { get; } = new ArtifactNameSettings()
        {
            Binaries = "Binaries"
        };


        public BuildContext(ICakeContext context) : base(context)
        {
            BuildConfiguration = context.Arguments.GetArgument("configuration") ?? "Release";

            IsAzurePipelines = context.AzurePipelines().IsRunningOnAzurePipelines || context.AzurePipelines().IsRunningOnAzurePipelinesHosted;

            PackageOutputPath = IsAzurePipelines
                ? context.AzurePipelines().Environment.Build.ArtifactStagingDirectory.FullPath
                : RootDirectory.Combine(DirectoryPath.FromString($"Binaries/{BuildConfiguration}/packages/")); //TODO



        }
    }
}
