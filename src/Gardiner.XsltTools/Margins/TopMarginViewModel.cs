﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

using JetBrains.Annotations;

using Microsoft.Language.Xml;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace Gardiner.XsltTools.Margins
{
    public class TopMarginViewModel : INotifyPropertyChanged
    {
        private readonly IWpfTextView _textView;
        private readonly ITextBuffer _dataBuffer;
        private TemplateModel _selectedValue;
        private IList<TemplateModel> _templates;
        private bool _dontUpdateCaret;
        private XmlDocumentSyntax _syntax;

        // Only for testing
        public TopMarginViewModel()
        {
        }

        public TopMarginViewModel(IWpfTextView textView)
        {
            _textView = textView;
            _dataBuffer = textView.TextDataModel.DataBuffer;

            UpdateList();

            _dataBuffer.PostChanged += DataBufferOnPostChanged;
            _textView.Caret.PositionChanged += OnCaretChanged;
        }

        public TemplateModel SelectedValue
        {
            get { return _selectedValue; }
            set
            {
                if (Equals(value, _selectedValue))
                    return;

                Debug.WriteLine($"SelectedValue {value}");
                _selectedValue = value;
                TemplateListSelectionChanged(value);
                OnPropertyChanged();
            }
        }

        private void OnCaretChanged(object sender, CaretPositionChangedEventArgs e)
        {
            // what template are we inside?
            var bufferPosition = e.NewPosition.BufferPosition;
            var position = bufferPosition.Position;

            var elt = GetDescendants(_syntax)
                .OfType<XmlElementSyntax>()
                .FirstOrDefault(n =>
                {
                    var leadingTriviaWidth = n.GetLeadingTriviaWidth();
                    var start = n.Start + leadingTriviaWidth;

                    return n.Name == "template" && start <= position && position <= (n.Start + n.FullWidth);
                });

            TemplateModel item = null;

            // Update selected item in list
            if (elt != null)
            {
                item = CreateTemplateModel(elt.Attributes.ToList(), elt);
            }
            _dontUpdateCaret = true;
            SelectedValue = item;
            _dontUpdateCaret = false;

        }

        private void DataBufferOnPostChanged(object sender, EventArgs eventArgs)
        {
            Debug.WriteLine("DataBufferOnPostChanged");
            UpdateList();
        }

        private void UpdateList()
        {
            try
            {
                var snapshot = _dataBuffer.CurrentSnapshot;

                var text = snapshot.GetText();

                _syntax = Parser.ParseText(text);

                var list = GetDescendants(_syntax)
                    .OfType<XmlElementSyntax>()
                    .Where(n => n.Name == "template")
                    .Select(n => CreateTemplateModel(n.Attributes.ToList(), n))
                    //.Where(m => m.Name != null && m.Mode != null && m.Match != null)
                    .ToList();

                _dontUpdateCaret = true;
                Templates = list;
                _dontUpdateCaret = false;

            }
            catch (Exception ex)
            {
                Telemetry.Log(ex);
            }
        }

        public static IEnumerable<SyntaxNode> GetDescendants(SyntaxNode node)
        {
            var result = new List<SyntaxNode>();
            AddDescendants(node, result);
            return result;
        }

        private static void AddDescendants(SyntaxNode node, ICollection<SyntaxNode> resultList)
        {
            resultList.Add(node);

            foreach (var child in node.ChildNodes)
            {
                AddDescendants(child, resultList);
            }
        }

        private static TemplateModel CreateTemplateModel(List<KeyValuePair<string, string>> attributes, XmlElementSyntax name)
        {
            var item = new TemplateModel()
            {
                Mode = attributes.Where(a => a.Key == "mode").Select(a => a.Value).FirstOrDefault(),
                Name = attributes.Where(a => a.Key == "name").Select(a => a.Value).FirstOrDefault(),
                Match = attributes.Where(a => a.Key == "match").Select(a => a.Value).FirstOrDefault(),
                Start = name.Start
            };
            return item;
        }

        public IList<TemplateModel> Templates
        {
            get { return _templates; }
            set
            {
                if (Equals(value, _templates))
                    return;
                _templates = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void TemplateListSelectionChanged(TemplateModel key)
        {
            if (_dontUpdateCaret || key == null)
                return;

            var snapshotPoint = new SnapshotPoint(_dataBuffer.CurrentSnapshot, key.Start);
            _textView.Caret.MoveTo(snapshotPoint);
        }
    }
}