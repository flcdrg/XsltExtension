using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using Gardiner.XsltTools.ErrorList;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;

namespace Gardiner.XsltTools
{
    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType("xml")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    class CreationListener : IVsTextViewCreationListener
    {
        [Import]
        public IVsEditorAdaptersFactoryService EditorAdaptersFactoryService { get; set; }

        [Import]
        public ITextDocumentFactoryService TextDocumentFactoryService { get; set; }

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            var textView = EditorAdaptersFactoryService.GetWpfTextView(textViewAdapter);
            ITextDocument document;

            if (TextDocumentFactoryService.TryGetTextDocument(textView.TextDataModel.DocumentBuffer, out document))
            {
                string fileName = Path.GetFileName(document.FilePath);
                Debug.WriteLine(fileName);

                //TableDataSource.Instance.CleanAllErrors();

                var checker = new XsltChecker();
                var result = checker.CheckFile(document.FilePath);

                ErrorListService.ProcessLintingResults(result);
            }
        }
    }
}
