﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace ChangeLogCreator.ConventionalCommits
{
    /// <summary>
    /// Enumerates the types of tokens emitted by <see cref="HeaderTokenizer"/>
    /// </summary>
    public enum HeaderTokenKind
    {
        String,             // any string value
        OpenParenthesis,    // '('
        CloseParenthesis,   // ')'
        Colon,              // ':'
        Space,              // ' '
        ExclamationMark,    // '!'
        Eol                 // end of input / last token
    }

    public sealed class HeaderToken : Token<HeaderTokenKind>
    {
        // Constructor should be private but internal for testing
        internal HeaderToken(HeaderTokenKind kind, string? value, int lineNumber, int columnNumber) : base(kind, value, lineNumber, columnNumber)
        { }


        public static HeaderToken String(string value, int lineNumber, int columnNumber) =>
            new HeaderToken(HeaderTokenKind.String, value ?? throw new ArgumentNullException(nameof(value)), lineNumber, columnNumber);

        public static HeaderToken OpenParenthesis(int lineNumber, int columnNumber) =>
            new HeaderToken(HeaderTokenKind.OpenParenthesis, "(", lineNumber, columnNumber);

        public static HeaderToken CloseParenthesis(int lineNumber, int columnNumber) =>
            new HeaderToken(HeaderTokenKind.CloseParenthesis, ")", lineNumber, columnNumber);

        public static HeaderToken Colon(int lineNumber, int columnNumber) =>
            new HeaderToken(HeaderTokenKind.Colon, ":", lineNumber, columnNumber);

        public static HeaderToken Space(int lineNumber, int columnNumber) =>
            new HeaderToken(HeaderTokenKind.Space, " ", lineNumber, columnNumber);

        public static HeaderToken ExclamationMark(int lineNumber, int columnNumber) =>
            new HeaderToken(HeaderTokenKind.ExclamationMark, "!", lineNumber, columnNumber);

        public static HeaderToken Eol(int lineNumber, int columnNumber) =>
            new HeaderToken(HeaderTokenKind.Eol, null, lineNumber, columnNumber);
    }

    /// <summary>
    /// Tokenizer that splits the input into a sequence of <see cref="HeaderToken"/> values to be parsed by <see cref="HeaderParser"/>
    /// </summary>
    public static class HeaderTokenizer
    {
        public static IEnumerable<HeaderToken> GetTokens(LineToken input)
        {
            if (input is null)
                throw new ArgumentNullException(nameof(input));

            if (input.Kind != LineTokenKind.Line)
                throw new ArgumentException($"Input must be line token of kind '{LineTokenKind.Line}', but is kind '{input.Kind}'");


            var currentValue = new StringBuilder();
            var startColumn = 1;

            if (input.Value!.Length == 0)
            {
                yield return HeaderToken.Eol(input.LineNumber, startColumn);
                yield break;
            }

            for (var i = 0; i < input.Value.Length; i++)
            {
                var currentChar = input.Value[i];
                if (TryMatchChar(currentChar, input.LineNumber, i + 1, out var matchedToken))
                {
                    if (currentValue.Length > 0)
                    {
                        var value = currentValue.GetValueAndClear();
                        yield return HeaderToken.String(value, input.LineNumber, startColumn);
                        startColumn += value.Length;
                    }
                    yield return matchedToken;
                    startColumn += 1;
                }
                else
                {
                    currentValue.Append(currentChar);
                }
            }

            // if any input is left in currentValueBuilder, return it as StringToken
            if (currentValue.Length > 0)
            {
                var value = currentValue.GetValueAndClear();
                yield return HeaderToken.String(value, input.LineNumber, startColumn);
                startColumn += value.Length;
            }

            yield return HeaderToken.Eol(input.LineNumber, startColumn);
        }

        private static bool TryMatchChar(char value, int lineNumber, int columnNumber, [NotNullWhen(true)] out HeaderToken? token)
        {
            token = value switch
            {
                '(' => HeaderToken.OpenParenthesis(lineNumber, columnNumber),
                ')' => HeaderToken.CloseParenthesis(lineNumber, columnNumber),
                ':' => HeaderToken.Colon(lineNumber, columnNumber),
                ' ' => HeaderToken.Space(lineNumber, columnNumber),
                '!' => HeaderToken.ExclamationMark(lineNumber, columnNumber),
                _ => null
            };

            return token != null;
        }
    }
}
