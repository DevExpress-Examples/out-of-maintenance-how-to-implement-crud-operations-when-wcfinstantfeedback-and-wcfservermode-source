using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Interactivity;
using System.Windows;
using DevExpress.Xpf.Grid;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.ServerMode;

namespace WCFInstant {
    public class WCFInstantModeCRUDBehavior: CRUDBehavior.CRUDBehaviorBase {
        public static readonly DependencyProperty DataSourceProperty =
            DependencyProperty.Register("DataSource", typeof(WcfInstantFeedbackDataSource), typeof(WCFInstantModeCRUDBehavior), new PropertyMetadata(null));
        public WcfInstantFeedbackDataSource DataSource {
            get { return (WcfInstantFeedbackDataSource)GetValue(DataSourceProperty); }
            set { SetValue(DataSourceProperty, value); }
        }
        protected override bool CanExecuteRemoveRowCommand() {
            if(DataSource == null || Grid == null || View == null || Grid.CurrentItem == null) return false;
            return true;
        }
        protected override void OnAttached() {
            base.OnAttached();
            if(View != null && DataSource != null && DataSource.Data != null)
                Initialize();
            else
                Grid.Loaded += OnGridLoaded;
        }
        protected override void Initialize() {
            Grid.ItemsSource = DataSource.Data;
            NewRowCommand.RaiseCanExecuteChangedEvent();
            base.Initialize();
        }
        protected override void UpdateDataSource() {
            DataSource.Refresh();
        }
    }
}