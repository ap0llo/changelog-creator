﻿using System;
using Grynwald.ChangeLog.Git;
using Grynwald.ChangeLog.Model.Text;
using Xunit;

namespace Grynwald.ChangeLog.Test.Model
{
    /// <summary>
    /// Tests for <see cref="CommitReferenceTextElement"/>
    /// </summary>
    public class CommitReferenceTextElementTest
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("\t")]
        public void Text_must_not_be_null_or_whitespace(string text)
        {
            // ARRANGE

            // ACT 
            var ex = Record.Exception(() => new CommitReferenceTextElement(text, TestGitIds.Id1));

            // ASSERT
            var argumentException = Assert.IsType<ArgumentException>(ex);
            Assert.Equal("text", argumentException.ParamName);
        }

        [Fact]
        public void Commit_id_must_not_be_null()
        {
            // ARRANGE
            var id = default(GitId);

            // ACT 
            var ex = Record.Exception(() => new CommitReferenceTextElement("some text", id));

            // ASSERT
            Assert.NotNull(ex);
            var argumentException = Assert.IsType<ArgumentException>(ex);
            Assert.Equal("id", argumentException.ParamName);
        }
    }
}
