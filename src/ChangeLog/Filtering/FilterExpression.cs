﻿using System;
using Grynwald.ChangeLog.Model;
using Grynwald.Utilities;

namespace Grynwald.ChangeLog.Filtering
{
    /// <summary>
    /// Implements an expression to match a change log entry based on one or more criteria.
    /// </summary>
    /// <remarks>
    /// A <see cref="FilterExpression"/> allows defining criteria for a entry's type and scope.
    /// An entry is considered to match the filter expression if all defined criteria match.
    /// </remarks>
    /// <example>
    /// <code language="csharp">
    /// // Match any entry
    /// var expression1 = new FilterExpression("*", "*");
    ///
    /// // Match entries of type 'feat' without a scope
    /// var expression2 = new FilterExpression("feat", "");
    /// </code>
    /// </example>
    internal sealed class FilterExpression
    {
        private Func<ChangeLogEntry, bool> m_MatchType;
        private Func<ChangeLogEntry, bool> m_MatchScope;


        /// <summary>
        /// Gets the entry type matched by this filter expression
        /// </summary>
        public string? Type { get; }

        /// <summary>
        /// Gets the entry scope matched by this filter expression
        /// </summary>
        public string? Scope { get; }


        /// <summary>
        /// Initializes a new instance of <see cref="FilterExpression"/>.
        /// </summary>
        /// <param name="type">The type to match, supports wildcards. If value is <c>null</c> or empty, matches entries without a type.</param>
        /// <param name="scope">The scope to match, supports wildcards. If value is <c>null</c> or empty, matches entries without a scope.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="type"/> is <c>null</c> or whitespace and not an empty string.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="scope"/> is <c>null</c> or whitespace and not an empty string.</exception>
        public FilterExpression(string? type, string? scope)
        {
            // Type must not be a whitespace-only strings BUT may be an null or empty
            if (!String.IsNullOrEmpty(type) && String.IsNullOrWhiteSpace(type))
                throw new ArgumentException("Value must not be whitespace", nameof(type));

            // Scope must not be a whitespace-only strings BUT may be an null or empty
            if (!String.IsNullOrEmpty(scope) && String.IsNullOrWhiteSpace(scope))
                throw new ArgumentException("Value must not be whitespace", nameof(scope));

            Type = type;
            Scope = scope;

            // when type is an empty string, match entry without type (= type is null or empty)
            if (String.IsNullOrEmpty(type))
            {
                m_MatchType = entry => String.IsNullOrEmpty(entry.Type.Type);
            }
            // otherwise match entry type (with wildcard support)
            else
            {
                var wildcard = new Wildcard(type);
                m_MatchType = entry => wildcard.IsMatch(entry.Type.Type);
            }

            // when scope is an empty string, match entry without scope (= scope is null or empty)
            if (String.IsNullOrEmpty(scope))
            {
                m_MatchScope = entry => String.IsNullOrEmpty(entry.Scope);
            }
            else
            {
                var wildcard = new Wildcard(scope);
                m_MatchScope = entry => wildcard.IsMatch(entry.Scope ?? "");
            }
        }


        /// <summary>
        /// Determines whether the specified entry matches the filter expression.
        /// </summary>
        /// <param name="entry">The entry to evaluate the filter expression for.</param>
        /// <returns>Returns <c>true</c> if the entry matches the filter expression, otherwise returns <c>false</c></returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entry"/> is <c>null</c></exception>
        public bool IsMatch(ChangeLogEntry entry)
        {
            if (entry is null)
                throw new ArgumentNullException(nameof(entry));

            return m_MatchType(entry) && m_MatchScope(entry);
        }
    }
}
