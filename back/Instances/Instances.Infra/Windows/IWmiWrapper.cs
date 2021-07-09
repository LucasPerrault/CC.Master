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

    public static class WmiConstants
    {
        public const string ClassCNameType = "MicrosoftDNS_CNAMETYPE";
        public const string MethodCreateInstanceFromPropertyData = "CreateInstanceFromPropertyData";
        public const string PropertyCNameTypeDnsServerName = "DnsServerName";
        public const string PropertyCNameTypeContainerName = "ContainerName";
        public const string PropertyCNameTypeOwnerName = "OwnerName";
        public const string PropertyCNameTypePrimaryName = "PrimaryName";
    }
}
