﻿using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Gardiner.XsltTools.Margins
{
    /// <summary>
    /// Interaction logic for TopMarginUserControl.xaml
    /// </summary>
    public partial class TopMarginUserControl
    {
        public TopMarginUserControl()
        {
            InitializeComponent();
        }

        public TopMarginUserControl(TopMarginViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }
    }
}
