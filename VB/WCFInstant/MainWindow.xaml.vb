Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Windows
Imports WCFInstant.ServiceReference1

Namespace WCFInstant
	Partial Public Class MainWindow
		Inherits Window
		Public Shared ReadOnly EntitiesProperty As DependencyProperty = DependencyProperty.Register("Entities", GetType(DatabaseEntities), GetType(MainWindow), New UIPropertyMetadata(Nothing))
		Public Property Entities() As DatabaseEntities
			Get
				Return CType(GetValue(EntitiesProperty), DatabaseEntities)
			End Get
			Set(ByVal value As DatabaseEntities)
				SetValue(EntitiesProperty, value)
			End Set
		End Property
		Public Sub New()
			Entities = New DatabaseEntities(New Uri("http://localhost:62700/WcfDataService.svc/"))
			DataContext = Me
			InitializeComponent()
			helper.PropertiesList.Add("Id")
			helper.PropertiesList.Add("Name")
		End Sub
	End Class
End Namespace
