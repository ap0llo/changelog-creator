﻿using System;

namespace Grynwald.ChangeLog.Model.Text
{
    public sealed class ChangeLogEntryReferenceTextElement : ITextElement
    {
        public string Text { get; }

        public ChangeLogEntry Entry { get; }

        public ChangeLogEntryReferenceTextElement(string text, ChangeLogEntry entry)
        {
            if (String.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Value must not be null or whitespace", nameof(text));

            Text = text;
            Entry = entry ?? throw new ArgumentNullException(nameof(entry));
        }
    }
}
