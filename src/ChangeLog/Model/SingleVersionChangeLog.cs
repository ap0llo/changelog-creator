﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Grynwald.ChangeLog.Model
{
    public sealed class SingleVersionChangeLog : IEnumerable<ChangeLogEntry>
    {
        private readonly List<ChangeLogEntry> m_Entries = new List<ChangeLogEntry>();


        public VersionInfo Version { get; }

        public IEnumerable<ChangeLogEntry> AllEntries => m_Entries.OrderBy(x => x.Date);

        /// <summary>
        /// Gets all change log entries that contain a breaking change.
        /// </summary>
        public IEnumerable<ChangeLogEntry> BreakingChanges => AllEntries.Where(e => e.ContainsBreakingChanges);


        /// <summary>
        /// Initializes a new instance of <see cref="SingleVersionChangeLog"/>
        /// </summary>
        public SingleVersionChangeLog(VersionInfo version)
        {
            Version = version ?? throw new ArgumentNullException(nameof(version));
        }


        public void Add(ChangeLogEntry entry)
        {
            if (entry is null)
                throw new ArgumentNullException(nameof(entry));

            m_Entries.Add(entry);
        }

        public void Remove(ChangeLogEntry entry)
        {
            if (entry is null)
                throw new ArgumentNullException(nameof(entry));

            m_Entries.Remove(entry);
        }

        public IEnumerator<ChangeLogEntry> GetEnumerator() => AllEntries.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => AllEntries.GetEnumerator();
    }
}
