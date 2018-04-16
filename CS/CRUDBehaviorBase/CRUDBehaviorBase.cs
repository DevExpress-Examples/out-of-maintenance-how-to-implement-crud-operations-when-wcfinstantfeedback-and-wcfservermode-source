using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Interactivity;
using DevExpress.Xpf.Grid;
using System.Windows.Input;
using System.Windows;
using DevExpress.Xpf.Core.ServerMode;
using System.Data.Linq;
using DevExpress.Xpf.Core;
using System.Windows.Controls;
using DevExpress.Xpf.Bars;
using System.Data.Services.Client;

namespace CRUDBehavior {
    public class CRUDBehaviorBase: Behavior<GridControl> {
        public static readonly DependencyProperty NewRowFormProperty =
            DependencyProperty.Register("NewRowForm", typeof(DataTemplate), typeof(CRUDBehaviorBase), new PropertyMetadata(null));
        public static readonly DependencyProperty EditRowFormProperty =
            DependencyProperty.Register("EditRowForm", typeof(DataTemplate), typeof(CRUDBehaviorBase), new PropertyMetadata(null));
        public static readonly DependencyProperty EntityObjectTypeProperty =
            DependencyProperty.Register("EntityObjectType", typeof(Type), typeof(CRUDBehaviorBase), new PropertyMetadata(null));
        public static readonly DependencyProperty AllowKeyDownActionsProperty =
            DependencyProperty.Register("AllowKeyDownActions", typeof(bool), typeof(CRUDBehaviorBase), new PropertyMetadata(false));
        public static readonly DependencyProperty PrimaryKeyProperty =
            DependencyProperty.Register("PrimaryKey", typeof(string), typeof(CRUDBehaviorBase), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty PropertiesListProperty =
            DependencyProperty.Register("PropertiesList", typeof(List<string>), typeof(CRUDBehaviorBase), new PropertyMetadata(new List<string>()));
        public static readonly DependencyProperty DataServiceContextProperty =
            DependencyProperty.Register("DataServiceContext", typeof(DataServiceContext), typeof(CRUDBehaviorBase), new UIPropertyMetadata(null));
        
        public DataTemplate NewRowForm {
            get { return (DataTemplate)GetValue(NewRowFormProperty); }
            set { SetValue(NewRowFormProperty, value); }
        }
        public DataTemplate EditRowForm {
            get { return (DataTemplate)GetValue(EditRowFormProperty); }
            set { SetValue(EditRowFormProperty, value); }
        }
        public Type EntityObjectType {
            get { return (Type)GetValue(EntityObjectTypeProperty); }
            set { SetValue(EntityObjectTypeProperty, value); }
        }
        public bool AllowKeyDownActions {
            get { return (bool)GetValue(AllowKeyDownActionsProperty); }
            set { SetValue(AllowKeyDownActionsProperty, value); }
        }
        public string PrimaryKey {
            get { return (string)GetValue(PrimaryKeyProperty); }
            set { SetValue(PrimaryKeyProperty, value); }
        }
        public List<string> PropertiesList {
            get { return (List<string>)GetValue(PropertiesListProperty); }
            set { SetValue(PropertiesListProperty, value); }
        }
        public DataServiceContext DataServiceContext {
            get { return (DataServiceContext)GetValue(DataServiceContextProperty); }
            set { SetValue(DataServiceContextProperty, value); }
        }

        public GridControl Grid { get { return AssociatedObject; } }
        public TableView View { get { return Grid != null ? (TableView)Grid.View : null; } }

        #region Commands
        public CustomCommand NewRowCommand { get; private set; }
        public CustomCommand RemoveRowCommand { get; private set; }
        public CustomCommand EditRowCommand { get; private set; }
        protected virtual void ExecuteNewRowCommand() {
            AddNewRow();
        }
        protected virtual bool CanExecuteNewRowCommand() {
            return true;
        }
        protected virtual void ExecuteRemoveRowCommand() {
            RemoveSelectedRows();
        }
        protected virtual bool CanExecuteRemoveRowCommand() {
            return true;
        }
        protected virtual void ExecuteEditRowCommand() {
            EditRow();
        }
        protected virtual bool CanExecuteEditRowCommand() {
            return CanExecuteRemoveRowCommand();
        }
        #endregion

        public CRUDBehaviorBase() {
            NewRowCommand = new CustomCommand(ExecuteNewRowCommand, CanExecuteNewRowCommand);
            RemoveRowCommand = new CustomCommand(ExecuteRemoveRowCommand, CanExecuteRemoveRowCommand);
            EditRowCommand = new CustomCommand(ExecuteEditRowCommand, CanExecuteEditRowCommand);
        }
        public virtual object CreateNewRow() {
            return Activator.CreateInstance(EntityObjectType);
        }
        public virtual void AddNewRow(object newRow) {
            if(DataServiceContext == null)
                return;
            DataServiceContext.AddObject(EntityObjectType.Name + "s", newRow);
            DataServiceContext.SaveChanges();
            UpdateDataSource();
        }
        public virtual void RemoveRow(int rowIndex) {
            if(DataServiceContext == null || PrimaryKey == string.Empty) return;
            object cellValue = Grid.GetFocusedRowCellValue(PrimaryKey);
            if(cellValue == null) return;
            string stringCellValue = cellValue.ToString();
            var en = DataServiceContext.Entities.GetEnumerator();
            while(en.MoveNext()) {
                if(en.Current.Identity.EndsWith(string.Format("({0})", stringCellValue))) {
                    DataServiceContext.DeleteObject(en.Current.Entity);
                    DataServiceContext.SaveChanges();
                    UpdateDataSource();
                    break;
                }
            }
        }
        public virtual void AddNewRow() {
            DXWindow dialog = CreateDialogWindow(CreateNewRow(), false);
            dialog.Closed += OnNewRowDialogClosed;
            dialog.ShowDialog();
        }
        public virtual void RemoveRow() {
            RemoveRow(Grid.GetListIndexByRowHandle(View.FocusedRowHandle));
        }
        public virtual void RemoveSelectedRows() {
            int[] selectedRowsHandles = Grid.GetSelectedRowHandles();
            if(selectedRowsHandles != null || selectedRowsHandles.Length > 0) {
                List<object> rowKeys = new List<object>();
                foreach(int index in selectedRowsHandles)
                    rowKeys.Add(Grid.GetCellValue(index, PrimaryKey));
                foreach(object cellValue in rowKeys) {
                    var en = DataServiceContext.Entities.GetEnumerator();
                    if(cellValue == null) continue;
                    string stringCellValue = cellValue.ToString();
                    int currIndex = 0;
                    while(en.MoveNext()) {
                        if(en.Current.Identity.EndsWith(string.Format("({0})", stringCellValue)))
                            DataServiceContext.DeleteObject(en.Current.Entity);
                        currIndex++;
                    }
                    DataServiceContext.SaveChanges();
                    UpdateDataSource();
                }
            }
            else if(Grid.CurrentItem != null)
                RemoveRow();
        }
        public virtual void EditRow() {
            if(View == null || Grid.CurrentItem == null || PrimaryKey == string.Empty) return;
            var en = DataServiceContext.Entities.GetEnumerator();
            while(en.MoveNext()) {
                if(en.Current.Identity.EndsWith(string.Format("({0})", Grid.GetFocusedRowCellValue(PrimaryKey)))) {
                    DXWindow dialog = CreateDialogWindow(en.Current.Entity, true);
                    dialog.Closed += OnEditRowDialogClosed;
                    dialog.ShowDialog();
                    break;
                }
            }
        }
        protected virtual DXWindow CreateDialogWindow(object content, bool isEditingMode = false) {
            DXDialog dialog = new DXDialog {
                Tag = content,
                Buttons = DialogButtons.OkCancel,
                Title = isEditingMode ? "Edit Row" : "Add New Row",
                SizeToContent = SizeToContent.WidthAndHeight
            };
            ContentControl c = new ContentControl { Content = content };
            if(isEditingMode) {
                dialog.Title = "Edit Row";
                c.ContentTemplate = EditRowForm;
            }
            else {
                dialog.Title = "Add New Row";
                c.ContentTemplate = NewRowForm;
            }
            dialog.Content = c;
            return dialog;
        }
        protected virtual void OnNewRowDialogClosed(object sender, EventArgs e) {
            ((DXWindow)sender).Closed -= OnNewRowDialogClosed;
            if((bool)((DXWindow)sender).DialogResult)
                AddNewRow(((DXWindow)sender).Tag);
        }
        protected virtual void OnEditRowDialogClosed(object sender, EventArgs e) {
            ((DXWindow)sender).Closed -= OnEditRowDialogClosed;
            if((bool)((DXWindow)sender).DialogResult) {
                DataServiceContext.UpdateObject(((Window)sender).Tag);
                DataServiceContext.SaveChanges();
                UpdateDataSource();
            }
            else {
                foreach(string propertyName in PropertiesList)
                    DataServiceContext.LoadProperty(((Window)sender).Tag, propertyName);
                UpdateDataSource();
            }
        }
        protected virtual void OnViewKeyDown(object sender, KeyEventArgs e) {
            if(!AllowKeyDownActions)
                return;
            if(e.Key == Key.Delete) {
                RemoveSelectedRows();
                e.Handled = true;
            }
            if(e.Key == Key.Enter) {
                EditRow();
                e.Handled = true;
            }
        }
        protected virtual void OnViewRowDoubleClick(object sender, RowDoubleClickEventArgs e) {
            EditRow();
            e.Handled = true;
        }
        protected virtual void OnGridLoaded(object sender, RoutedEventArgs e) {
            Grid.Loaded -= OnGridLoaded;
            Initialize();
        }
        protected virtual void UpdateCommands() {
            RemoveRowCommand.RaiseCanExecuteChangedEvent();
            EditRowCommand.RaiseCanExecuteChangedEvent();
        }
        protected override void OnAttached() {
            base.OnAttached();
        }
        protected override void OnDetaching() {
            Uninitialize();
            base.OnDetaching();
        }
        protected virtual void Initialize() {
            View.KeyDown += OnViewKeyDown;
            View.RowDoubleClick += OnViewRowDoubleClick;
            Grid.CurrentItemChanged += OnGridCurrentItemChanged;
            UpdateCommands();
        }

        void OnGridCurrentItemChanged(object sender, CurrentItemChangedEventArgs e) {
            UpdateCommands();
        }
        protected virtual void Uninitialize() {
            View.KeyDown -= OnViewKeyDown;
            View.RowDoubleClick -= OnViewRowDoubleClick;
            Grid.CurrentItemChanged += OnGridCurrentItemChanged;
        }
        protected virtual void UpdateDataSource() {
        }
    }
    public class CustomCommand: ICommand {
        Action _executeMethod;
        Func<bool> _canExecuteMethod;
        public CustomCommand(Action executeMethod, Func<bool> canExecuteMethod) {
            _executeMethod = executeMethod;
            _canExecuteMethod = canExecuteMethod;
        }
        public bool CanExecute(object parameter) {
            return _canExecuteMethod();
        }
        public event EventHandler CanExecuteChanged;
        public void Execute(object parameter) {
            _executeMethod();
        }
        public void RaiseCanExecuteChangedEvent() {
            if(CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }
    }
}