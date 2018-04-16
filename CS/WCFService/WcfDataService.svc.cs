using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel.Web;
using System.Web;

namespace WCFService {
    public class WcfDataService: DataService<DatabaseEntities> {
        public static void InitializeService(DataServiceConfiguration config) {
            config.SetEntitySetAccessRule("*", EntitySetRights.All);
            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V2;
        }
        [WebGet]
        public string GetCustomersExtendedData(string extendedDataInfo) {
            return DevExpress.Xpf.Core.ServerMode.ExtendedDataHelper.GetExtendedData(CurrentDataSource.Items, extendedDataInfo);
        }
    }
}