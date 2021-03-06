﻿using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Gardiner.XsltTools.Classification;
using Gardiner.XsltTools.Utils;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;

using XsltEditor.Commands;

namespace Gardiner.XsltTools.Commands
{
    internal class ImportHrefGoToDefinition : CommandTargetBase<VSConstants.VSStd97CmdID>
    {
        public ImportHrefGoToDefinition(IVsTextView adapter, IWpfTextView textView)
            : base(adapter, textView, VSConstants.VSStd97CmdID.GotoDefn)
        {
        }

        protected override bool Execute(VSConstants.VSStd97CmdID commandId, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            var path = FindReferencedPath();
            if (path == null)
            {
                return false;
            }

            Uri uri;
            if (!Uri.TryCreate(path, UriKind.RelativeOrAbsolute, out uri))
            {
                return false;
            }

            // ignore non-files
            if (uri.IsAbsoluteUri && uri.Scheme != Uri.UriSchemeFile)
            {
                return false;
            }

            var referencedPath = Path.Combine(Path.GetDirectoryName(TextView.TextBuffer.GetFileName()), path);

            
            string file;
            try
            {
                file = Path.GetFullPath(referencedPath);
            }
            catch (IOException e)
            {
                VSPackage.DTE.StatusBar.Text = $"Could not find file: \"{referencedPath}\", Error: {e.Message}";

                return false;
            }

            if (!File.Exists(file))
            {
                VSPackage.DTE.StatusBar.Text = $"Could not find file: \"{file}\"";
                return false; // log warning?
}

            FileHelpers.OpenFileInPreviewTab(file);
            return true;
        }

        private string FindReferencedPath()
        {
            var position = TextView.Caret.Position.BufferPosition;
            var line = position.GetContainingLine();
            int linePos = position - line.Start.Position;

            var match = XsltClassifier.Regex.Matches(line.GetText())
                             .Cast<Match>()
                             .FirstOrDefault(m => m.Index <= linePos && m.Index + m.Length >= linePos);

            return match?.Groups["path"].Value;
        }

        protected override bool IsEnabled()
        {
            return FindReferencedPath() != null;
        }
    }
}