Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Data.Services
Imports System.Data.Services.Common
Imports System.Linq
Imports System.ServiceModel.Web
Imports System.Web

Namespace WCFService
	Public Class WcfDataService
		Inherits DataService(Of DatabaseEntities)
		Public Shared Sub InitializeService(ByVal config As DataServiceConfiguration)
			config.SetEntitySetAccessRule("*", EntitySetRights.All)
			config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V2
		End Sub
		<WebGet> _
		Public Function GetCustomersExtendedData(ByVal extendedDataInfo As String) As String
			Return DevExpress.Xpf.Core.ServerMode.ExtendedDataHelper.GetExtendedData(CurrentDataSource.Items, extendedDataInfo)
		End Function
	End Class
End Namespace