using System;
using System.Collections.Generic;
using System.Management;
using System.Text;

namespace Instances.Infra.Windows
{
    public interface IWmiWrapper
    {
        IWmiSessionWrapper CreateSession(string sessionPath);
        void InvokeClassMethod(IWmiSessionWrapper session, string classPath, string methodName, Dictionary<string, object> parameters);
        void QueryAndDeleteObjects(IWmiSessionWrapper session, string query);
    }

    public interface IWmiSessionWrapper
    {
        ManagementScope ManagementScope { get; }
    }
}
