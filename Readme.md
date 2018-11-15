<!-- default file list -->
*Files to look at*:

* [CRUDBehaviorBase.cs](./CS/CRUDBehaviorBase/CRUDBehaviorBase.cs) (VB: [CRUDBehaviorBase.vb](./VB/CRUDBehaviorBase/CRUDBehaviorBase.vb))
* [WCFInstantModeCRUDBehavior.cs](./CS/CRUDBehaviorBase/WCFInstantModeCRUDBehavior.cs) (VB: [WCFInstantModeCRUDBehavior.vb](./VB/CRUDBehaviorBase/WCFInstantModeCRUDBehavior.vb))
* [WCFServerModeCRUDBehavior.cs](./CS/CRUDBehaviorBase/WCFServerModeCRUDBehavior.cs) (VB: [WCFServerModeCRUDBehavior.vb](./VB/CRUDBehaviorBase/WCFServerModeCRUDBehavior.vb))
* [MainWindow.xaml](./CS/WCFInstant/MainWindow.xaml) (VB: [MainWindow.xaml](./VB/WCFInstant/MainWindow.xaml))
* [MainWindow.xaml.cs](./CS/WCFInstant/MainWindow.xaml.cs) (VB: [MainWindow.xaml](./VB/WCFInstant/MainWindow.xaml))
* [Reference.cs](./CS/WCFInstant/Service References/ServiceReference1/Reference.cs) (VB: [Reference.vb](./VB/WCFInstant/Service References/ServiceReference1/Reference.vb))
* [MainWindow.xaml](./CS/WCFServer/MainWindow.xaml) (VB: [MainWindow.xaml.vb](./VB/WCFServer/MainWindow.xaml.vb))
* [MainWindow.xaml.cs](./CS/WCFServer/MainWindow.xaml.cs) (VB: [MainWindow.xaml.vb](./VB/WCFServer/MainWindow.xaml.vb))
* [Reference.cs](./CS/WCFServer/Service References/ServiceReference1/Reference.cs) (VB: [Reference.vb](./VB/WCFServer/Service References/ServiceReference1/Reference.vb))
* [WcfDataService.svc.cs](./CS/WCFService/WcfDataService.svc.cs) (VB: [WcfDataService.svc.vb](./VB/WCFService/WcfDataService.svc.vb))
<!-- default file list end -->
# How to implement CRUD operations when WcfInstantFeedback and WcfServerMode sources are used


<p>This example shows how to use WcfInstantFeedbackDataSource or WcfServerModeDataSource with DXGrid, and how to implement CRUD operations (e.g., add, remove, edit) in your application via special behavior.<br />The test sample requires the SQL Express service to be installed on your machine.<br /><br />We have created the WCFServerModeCRUDBehavior and WCFInstantModeCRUDBehavior attached behaviors for GridControl. For instance:</p>


```xaml
<dxg:GridControl>
   <i:Interaction.Behaviors>
       <crud:WCFServerModeCRUDBehavior ...>
           <crud:WCFServerModeCRUDBehavior.DataSource/>
               <dxsm:LinqServerModeDataSource .../>
           </crud:WCFServerModeCRUDBehavior.DataSource>
       </crud:WCFServerModeCRUDBehavior>
   </i:Interaction.Behaviors>
</dxg:GridControl>
```


<p> </p>
<p>The WCFServerModeCRUDBehavior and WCFInstantModeCRUDBehavior classes contain the NewRowForm and EditRowForm properties to provide the "Add Row" and "Edit Row" actions. With these properties, you can create the Add and Edit forms according to your requirements:</p>


```xml
<DataTemplate x:Key="EditRecordTemplate">
   <StackPanel Margin="8" MinWidth="200">
       <Grid>
           <Grid.ColumnDefinitions>
               <ColumnDefinition/>
               <ColumnDefinition/>
           </Grid.ColumnDefinitions>
           <Grid.RowDefinitions>
               <RowDefinition/>
               <RowDefinition/>
           </Grid.RowDefinitions>
           <TextBlock Text="ID:" VerticalAlignment="Center" Grid.Row="0" Grid.Column="0" Margin="0,0,6,4" />
           <dxe:TextEdit x:Name="txtID" Grid.Row="0" Grid.Column="1" EditValue="{Binding Path=Id, Mode=TwoWay}" Margin="0,0,0,4" />
           <TextBlock Text="Name:" VerticalAlignment="Center" Grid.Row="1" Grid.Column="0" Margin="0,0,6,4" />
           <dxe:TextEdit x:Name="txtCompany" Grid.Row="1" Grid.Column="1" EditValue="{Binding Path=Name, Mode=TwoWay}" Margin="0,0,0,4" />
       </Grid>
   </StackPanel>
</DataTemplate>
<crud:WCFServerModeCRUDBehavior NewRowForm="{StaticResource ResourceKey=EditRecordTemplate}" EditRowForm="{StaticResource ResourceKey=EditRecordTemplate}"/> 

```


<p>This Behavior classes require the following information from your data model:<br />- EntityObjectType - the type of rows;<br />- DataServiceContext - an object of the DataServiceContext type;<br />- PropertiesList - the table columns' list;<br />- PrimaryKey - the primary key of the database table;<br />- DataSource - an object of the WcfInstantFeedbackDataSource or WcfServerModeDataSource type.</p>


```xml
<dxg:GridControl>
   <i:Interaction.Behaviors>
       <crud:WCFInstantModeCRUDBehavior EntityObjectType="{x:Type sr:Item}" DataSource="{Binding ElementName=wcfInstantSource}" DataServiceContext="{Binding DataSource.DataServiceContext, RelativeSource={RelativeSource Self}}"/>
   </i:Interaction.Behaviors>
</dxg:GridControl>

```




```cs
helper.PropertiesList.Add("Id");
helper.PropertiesList.Add("Name");
```


<p>See the <a href="http://documentation.devexpress.com/#WPF/clsDevExpressXpfCoreServerModeWcfInstantFeedbackDataSourcetopic"><u>WcfInstantFeedbackDataSource</u></a> and <a href="http://documentation.devexpress.com/#WPF/clsDevExpressXpfCoreServerModeWcfServerModeDataSourcetopic"><u>WcfServerModeDataSource</u></a> classes to learn more about WcfInstantFeedbackDataSource and WcfServerModeDataSource .</p>
<p>Behavior class descendants support the following commands: NewRowCommand, RemoveRowCommand, EditRowCommand. You can bind your interaction controls with these commands with ease. For instance:</p>


```xml
<crud:WCFServerModeCRUDBehavior x:Name="helper"/>
<StackPanel Grid.Row="1" Orientation="Horizontal" HorizontalAlignment="Center">
   <Button Height="22" Width="60" Command="{Binding Path=NewRowCommand, ElementName=helper}">Add</Button>
   <Button Height="22" Width="60" Command="{Binding Path=RemoveRowCommand, ElementName=helper}" Margin="6,0,6,0">Remove</Button>
   <Button Height="22" Width="60" Command="{Binding Path=EditRowCommand, ElementName=helper}">Edit</Button>
</StackPanel>
```


<p>By default, the WCFServerModeCRUDBehavior and WCFInstantModeCRUDBehavior solutions support the following end-user interaction capabilities:<br />1. An end-user can edit selected row values by double-clicking on a grid row or by pressing the Enter key if the AllowKeyDownActions property is True.<br />2. An end-user can remove selected rows via the Delete key press if the AllowKeyDownActions property is True.<br />3. An end-user can add new rows, remove and edit them via the NewRowCommand, RemoveRowCommand, and EditRowCommand commands.</p>

<br/>


