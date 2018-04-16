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

namespace WCFServer {
    public class WCFServerModeCRUDBehavior: CRUDBehavior.CRUDBehaviorBase {
        public static readonly DependencyProperty DataSourceProperty =
            DependencyProperty.Register("DataSource", typeof(WcfServerModeDataSource), typeof(WCFServerModeCRUDBehavior), new PropertyMetadata(null));
        public WcfServerModeDataSource DataSource {
            get { return (WcfServerModeDataSource)GetValue(DataSourceProperty); }
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
            DataSource.Reload();
        }
    }
}