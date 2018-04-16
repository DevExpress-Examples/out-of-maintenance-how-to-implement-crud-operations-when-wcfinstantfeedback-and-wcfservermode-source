Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Windows.Interactivity
Imports System.Windows
Imports DevExpress.Xpf.Grid
Imports System.Windows.Controls
Imports System.Windows.Input
Imports DevExpress.Xpf.Core
Imports DevExpress.Xpf.Core.ServerMode

Namespace WCFInstant
	Public Class WCFInstantModeCRUDBehavior
		Inherits CRUDBehavior.CRUDBehaviorBase
		Public Shared ReadOnly DataSourceProperty As DependencyProperty = DependencyProperty.Register("DataSource", GetType(WcfInstantFeedbackDataSource), GetType(WCFInstantModeCRUDBehavior), New PropertyMetadata(Nothing))
		Public Property DataSource() As WcfInstantFeedbackDataSource
			Get
				Return CType(GetValue(DataSourceProperty), WcfInstantFeedbackDataSource)
			End Get
			Set(ByVal value As WcfInstantFeedbackDataSource)
				SetValue(DataSourceProperty, value)
			End Set
		End Property
		Protected Overrides Function CanExecuteRemoveRowCommand() As Boolean
            If DataSource Is Nothing OrElse Grid Is Nothing OrElse View Is Nothing OrElse Grid.CurrentItem Is Nothing Then
                Return False
            End If
			Return True
		End Function
		Protected Overrides Sub OnAttached()
			MyBase.OnAttached()
			If View IsNot Nothing AndAlso DataSource IsNot Nothing AndAlso DataSource.Data IsNot Nothing Then
				Initialize()
			Else
                AddHandler (Grid.Loaded), AddressOf OnGridLoaded
			End If
		End Sub
		Protected Overrides Sub Initialize()
			Grid.ItemsSource = DataSource.Data
			NewRowCommand.RaiseCanExecuteChangedEvent()
			MyBase.Initialize()
		End Sub
		Protected Overrides Sub UpdateDataSource()
			DataSource.Refresh()
		End Sub
	End Class
End Namespace