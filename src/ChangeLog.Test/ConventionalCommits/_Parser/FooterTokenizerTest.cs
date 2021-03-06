﻿using System;
using System.Collections.Generic;
using System.Linq;
using Grynwald.ChangeLog.ConventionalCommits;
using Xunit;

namespace Grynwald.ChangeLog.Test.ConventionalCommits
{
    /// <summary>
    /// Tests for <see cref="FooterTokenizer"/>
    /// </summary>
    public class FooterTokenizerTest
    {
        public static IEnumerable<object[]> TokenizerTestCases()
        {
            static object[] TestCase(string input, params FooterToken[] tokens) =>
                new object[]
                {
                    new XunitSerializableLineToken(LineToken.Line(input, 1)),
                    tokens.Select(t => new XunitSerializableFooterToken(t)).ToArray()
                };

            yield return TestCase("", FooterToken.Eol(1, 1));

            yield return TestCase(
                "Some string",
                FooterToken.String("Some", 1, 1),
                FooterToken.Space(1, 5),
                FooterToken.String("string", 1, 6),
                FooterToken.Eol(1, 12)
            );

            yield return TestCase(
                "type: description",
                FooterToken.String("type", 1, 1),
                FooterToken.Colon(1, 5),
                FooterToken.Space(1, 6),
                FooterToken.String("description", 1, 7),
                FooterToken.Eol(1, 18)
            );

            yield return TestCase(
                "type #description",
                FooterToken.String("type", 1, 1),
                FooterToken.Space(1, 5),
                FooterToken.Hash(1, 6),
                FooterToken.String("description", 1, 7),
                FooterToken.Eol(1, 18)
            );

            yield return TestCase(
                "BREAKING CHANGE: description",
                FooterToken.String("BREAKING", 1, 1),
                FooterToken.Space(1, 9),
                FooterToken.String("CHANGE", 1, 10),
                FooterToken.Colon(1, 16),
                FooterToken.Space(1, 17),
                FooterToken.String("description", 1, 18),
                FooterToken.Eol(1, 29)
            );

            yield return TestCase(
                "BREAKING CHANGE #description",
                FooterToken.String("BREAKING", 1, 1),
                FooterToken.Space(1, 9),
                FooterToken.String("CHANGE", 1, 10),
                FooterToken.Space(1, 16),
                FooterToken.Hash(1, 17),
                FooterToken.String("description", 1, 18),
                FooterToken.Eol(1, 29)
            );

            yield return TestCase(
               "breaking-change: description",
               FooterToken.String("breaking-change", 1, 1),
               FooterToken.Colon(1, 16),
               FooterToken.Space(1, 17),
               FooterToken.String("description", 1, 18),
               FooterToken.Eol(1, 29)
           );

        }

        [Theory]
        [MemberData(nameof(TokenizerTestCases))]
        public void GetTokens_returns_expected_tokens(XunitSerializableLineToken input, IEnumerable<XunitSerializableFooterToken> expectedTokens)
        {
            // ARRANGE
            var inspectors = expectedTokens
                .Select(x => x.Value)
                .Select<FooterToken, Action<FooterToken>>(token => (t => Assert.Equal(token, t)))
                .ToArray();

            // ACT
            var actualTokens = new FooterTokenizer().GetTokens(input.Value).ToArray();

            // ASSERT
            Assert.Collection(actualTokens, inspectors);
        }

        [Fact]
        public void GetTokens_throws_ArgumentException_if_input_token_is_null()
        {
            Assert.Throws<ArgumentNullException>(() => new FooterTokenizer().GetTokens(null!).ToArray());
        }

        [Fact]
        public void GetTokens_throws_ArgumentException_if_input_token_is_invalid()
        {
            Assert.Throws<ArgumentException>(() => new FooterTokenizer().GetTokens(LineToken.Eof(1)).ToArray());
            Assert.Throws<ArgumentException>(() => new FooterTokenizer().GetTokens(LineToken.Blank(1)).ToArray());
        }
    }
}
