using System.Collections.Generic;
using System.Management;

namespace Instances.Infra.Windows
{
    public class WmiSessionWrapper : IWmiSessionWrapper
    {
        public WmiSessionWrapper(ManagementScope managementScope)
        {
            ManagementScope = managementScope;
        }

        public ManagementScope ManagementScope { get; }


    }
    public class WmiWrapper : IWmiWrapper
    {
        public IWmiSessionWrapper CreateSession(string sessionPath)
        {
            var session = new ManagementScope(sessionPath)
            {
                Options = new ConnectionOptions { Impersonation = ImpersonationLevel.Impersonate }
            };
            session.Connect();
            return new WmiSessionWrapper(session);
        }

        public void InvokeClassMethod(IWmiSessionWrapper session, string classPath, string methodName, Dictionary<string, object> parameters)
        {
            var man = new ManagementClass(session.ManagementScope, new ManagementPath(classPath), null);
            var vars = man.GetMethodParameters(methodName);
            foreach(var kvp in parameters)
            {
                vars[kvp.Key] = kvp.Value;
            }
            man.InvokeMethod(methodName, vars, null);
        }

        public void QueryAndDeleteObjects(IWmiSessionWrapper session, string query)
        {
            var objectQuery = new ObjectQuery(query);
            var s = new ManagementObjectSearcher(session.ManagementScope, objectQuery);
            var col = s.Get();
            foreach (var o in col)
            {
                ((ManagementObject) o).Delete();
            }
        }
    }
}
