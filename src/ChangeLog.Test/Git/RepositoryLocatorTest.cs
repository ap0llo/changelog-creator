﻿using System;
using System.IO;
using System.Threading.Tasks;
using Grynwald.ChangeLog.Git;
using Grynwald.Utilities.IO;
using Xunit;
using Xunit.Abstractions;

namespace Grynwald.ChangeLog.Test.Git
{
    /// <summary>
    /// Tests for <see cref="RepositoryLocator"/>
    /// </summary>
    public class RepositoryLocatorTest
    {
        private readonly ITestOutputHelper m_TestOutputHelper;

        public RepositoryLocatorTest(ITestOutputHelper testOutputHelper)
        {
            m_TestOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
        }


        [Fact]
        public async Task TryGetRepositoryPath_succeeds_when_starting_path_is_a_git_repository()
        {
            // ARRANGE
            using var temporaryDirectory = new TemporaryDirectory();
            var git = new GitWrapper(temporaryDirectory, m_TestOutputHelper);
            await git.InitAsync();

            // ACT 
            var success = RepositoryLocator.TryGetRepositoryPath(temporaryDirectory, out var actualRepositoryPath);

            // ASSERT
            Assert.True(success);
            Assert.Equal(temporaryDirectory, actualRepositoryPath?.TrimEnd(Path.DirectorySeparatorChar));
        }

        [Theory]
        [InlineData("subdir")]
        [InlineData("subdir/dir2")]
        [InlineData("subdir/dir2/dir3")]
        [InlineData("subdir/dir2/./dir3")]
        [InlineData("subdir/dir2/../dir3")]
        public async Task TryGetRepositoryPath_succeeds_when_starting_path_is_a_subdirectory_of_git_repository(string relativePath)
        {
            // ARRANGE
            using var temporaryDirectory = new TemporaryDirectory();
            var git = new GitWrapper(temporaryDirectory, m_TestOutputHelper);
            await git.InitAsync();

            var startingPath = temporaryDirectory.AddSubDirectory(relativePath);

            // ACT 
            var success = RepositoryLocator.TryGetRepositoryPath(startingPath, out var actualRepositoryPath);

            // ASSERT
            Assert.True(success);
            Assert.Equal(temporaryDirectory, actualRepositoryPath?.TrimEnd(Path.DirectorySeparatorChar));
        }

        [Fact]
        public void TryGetRepositoryPath_fails_when_starting_path_is_not_a_git_repository()
        {
            // ARRANGE
            using var temporaryDirectory = new TemporaryDirectory();

            // ACT 
            var success = RepositoryLocator.TryGetRepositoryPath(temporaryDirectory, out var actualRepositoryPath);

            // ASSERT
            Assert.False(success);
            Assert.Null(actualRepositoryPath);
        }

        [Fact]
        public async Task TryGetRepositoryPath_fails_when_starting_path_is_a_bare_repository()
        {
            // ARRANGE
            using var temporaryDirectory = new TemporaryDirectory();
            var git = new GitWrapper(temporaryDirectory, m_TestOutputHelper);
            await git.InitAsync(createBareRepository: true);

            // ACT 
            var success = RepositoryLocator.TryGetRepositoryPath(temporaryDirectory, out var actualRepositoryPath);

            // ASSERT
            Assert.False(success);
            Assert.Null(actualRepositoryPath);
        }
    }
}
