﻿using System;
using System.Collections.Generic;

using EnvDTE;

using Microsoft.VisualStudio.Shell.TableManager;

namespace Gardiner.XsltTools.ErrorList
{
    internal class TableEntriesSnapshot : TableEntriesSnapshotBase
    {
        private readonly string _projectName;

        internal TableEntriesSnapshot(AccessibilityResult result)
        {
            _projectName = result.Project;
            Errors.AddRange(result.Violations);
            Url = result.Url;
        }

        public List<Rule> Errors { get; } = new List<Rule>();

        public override int VersionNumber { get; } = 1;

        public override int Count
        {
            get { return Errors.Count; }
        }

        public string Url { get; set; }

        public override bool TryGetValue(int index, string columnName, out object content)
        {
            content = null;

            if ((index >= 0) && (index < Errors.Count))
            {
                if (columnName == StandardTableKeyNames.DocumentName)
                {
                    content = Errors[index].FileName;
                }
                else if (columnName == StandardTableKeyNames.ErrorCategory || columnName == StandardTableKeyNames.ErrorSource || columnName == StandardTableKeyNames.BuildTool)
                {
                    content = Vsix.Name;
                }
                else if (columnName == StandardTableKeyNames.Line)
                {
                    content = Errors[index].Line;
                }
                else if (columnName == StandardTableKeyNames.Column)
                {
                    content = Errors[index].Column;
                }
                else if (columnName == StandardTableKeyNames.FullText || columnName == StandardTableKeyNames.Text)
                {
                    content = Errors[index].Description;
                }
                else if (columnName == StandardTableKeyNames.ErrorSeverity)
                {
                    content = Errors[index].GetSeverity();
                }
                else if (columnName == StandardTableKeyNames.Priority)
                {
                    content = vsTaskPriority.vsTaskPriorityMedium;
                }
                else if (columnName == StandardTableKeyNames.ErrorSource)
                {
                    content = ErrorSource.Other;
                }
                else if (columnName == StandardTableKeyNames.ErrorCode)
                {
                    content = Errors[index].Id;
                }
                else if (columnName == StandardTableKeyNames.ProjectName)
                {
                    content = _projectName;
                }
                else if ((columnName == StandardTableKeyNames.ErrorCodeToolTip) ||
                         (columnName == StandardTableKeyNames.HelpLink))
                {
                    content = Uri.EscapeUriString(Errors[index].HelpUrl);
                }
            }

            return content != null;
        }
    }
}