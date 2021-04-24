﻿using System;
using System.Collections.Generic;
using Grynwald.ChangeLog.Configuration;
using Grynwald.ChangeLog.Model;

namespace Grynwald.ChangeLog.Templates.ViewModel
{
    internal class ChangeLogEntryViewModel
    {
        private readonly ChangeLogConfiguration m_Configuration;
        private readonly ChangeLogEntry m_Model;


        // TODO: Remove public property once all templates only access viewmodel properties
        public ChangeLogEntry Model => m_Model;


        public bool HasScope => !String.IsNullOrEmpty(Scope);

        public string? Scope => m_Model.Scope;

        public string? ScopeDisplayName => HasScope ? m_Model.GetScopeDisplayName(m_Configuration) : null;

        public string Summary => m_Model.Summary;

        public bool ContainsBreakingChanges => m_Model.ContainsBreakingChanges;

        public IEnumerable<string> BreakingChangeDescriptions => m_Model.BreakingChangeDescriptions;

        public IEnumerable<string> Body => m_Model.Body;

        //TODO: Add ChangeLogEntryFooterViewModel
        public IReadOnlyList<ChangeLogEntryFooter> Footers => m_Model.Footers;


        public ChangeLogEntryViewModel(ChangeLogConfiguration configuration, ChangeLogEntry model)
        {
            m_Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            m_Model = model ?? throw new ArgumentNullException(nameof(model));
        }

    }
}
