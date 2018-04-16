using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using WCFServer.ServiceReference1;

namespace WCFServer {
    public partial class MainWindow: Window {
        public static readonly DependencyProperty EntitiesProperty = 
            DependencyProperty.Register("Entities", typeof(DatabaseEntities), typeof(MainWindow), new PropertyMetadata(null));
        public DatabaseEntities Entities {
            get { return (DatabaseEntities)GetValue(EntitiesProperty); }
            set { SetValue(EntitiesProperty, value); }
        }
        public MainWindow() {
            Entities = new DatabaseEntities(new Uri("http://localhost:62700/WcfDataService.svc/"));
            DataContext = this;
            InitializeComponent();
            helper.PropertiesList.Add("Id");
            helper.PropertiesList.Add("Name");
        }
    }
}