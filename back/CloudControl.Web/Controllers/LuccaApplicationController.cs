using Environments.Domain;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Rights.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rights.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace CloudControl.Web.Controllers
{
    [AllowAnonymous]
    [ApiController, Route("api/application-description")]
    public class LuccaApplicationController
    {
        private static readonly Lazy<List<LuccaOperation>> AllDescriptions
            = new Lazy<List<LuccaOperation>>(() => OperationDescription
                .All()
                .Select(d => new LuccaOperation(d))
                .ToList());

        private static readonly Lazy<LuccaBusinessItemsCollection> BusinessItemsCollection
            = new Lazy<LuccaBusinessItemsCollection>
            (
                () => new LuccaBusinessItemsCollection
                {
                    Label = "Types d'environnement",
                    Items = Enum.GetValues(typeof(EnvironmentPurpose))
                        .Cast<EnvironmentPurpose>()
                        .Select(p => new LuccaBusinessItem(p))
                        .ToList()
                }
            );

        [HttpGet("operations")]
        public Page<LuccaOperation> GetAllOperations() => new Page<LuccaOperation>
        {
            Items = AllDescriptions.Value,
            Count = AllDescriptions.Value.Count
        };

        [HttpGet("operations/v3-compatibility")]
        public RestApiV3CollectionCompatibilityFormat<LuccaOperation> GetAllOperationsInV3() => new RestApiV3CollectionCompatibilityFormat<LuccaOperation>(AllDescriptions.Value);

        [HttpGet("business-items")]
        public LuccaBusinessItemsCollection GetAllBusinessItems() => BusinessItemsCollection.Value;

        [HttpGet("business-items/v3-compatibility")]
        public RestApiV3CompatibilityFormat<LuccaBusinessItemsCollection> GetAllBusinessItemsInV3() => new RestApiV3CompatibilityFormat<LuccaBusinessItemsCollection>(BusinessItemsCollection.Value);
    }

    public class LuccaBusinessItem
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public LuccaBusinessItem(EnvironmentPurpose environmentPurpose)
        {
            Id = (int)environmentPurpose;
            Name = GetName(environmentPurpose);
        }

        private static string GetName(EnvironmentPurpose purpose)
        {
            return purpose switch
            {
                EnvironmentPurpose.Contractual => "Contractuel",
                EnvironmentPurpose.Lucca => "Lucca",
                EnvironmentPurpose.InternalUse => "Usage interne",
                EnvironmentPurpose.QA => "QA",
                EnvironmentPurpose.Virgin => "Vierge",
                EnvironmentPurpose.Cluster => "Test de cluster",
                EnvironmentPurpose.Security => "Sécurité",
                EnvironmentPurpose.InternalTest => "Test interne",
                EnvironmentPurpose.ExternalTest => "Test externe",
                EnvironmentPurpose.UrbaHack => "Hack Urba",
                _ => throw new InvalidEnumArgumentException(nameof(purpose), (int)purpose, typeof(EnvironmentPurpose))
            };
        }
    }

    public class LuccaOperation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CulturedName { get; set; }
        public bool IsBusinessItemSpecific { get; set; }

        public List<LuccaOperationScope> Scopes { get; set; }


        public LuccaOperation(OperationDescription operationDescription)
        {
            Id = operationDescription.Id;
            Name = operationDescription.Name;
            CulturedName = operationDescription.Name;
            IsBusinessItemSpecific = operationDescription.IsBusinessItemSpecific;
            Scopes = operationDescription.Scopes.Select(s => new LuccaOperationScope(s)).ToList();
        }

        public class LuccaOperationScope
        {
            public int Id { get; }
            public Scope Name { get; }
            public LuccaOperationScope(Scope scope)
            {
                Id = (int)scope;
                Name = scope;
            }
        }
    }

    public class LuccaBusinessItemsCollection
    {
        public string Label { get; set; }
        public IReadOnlyCollection<LuccaBusinessItem> Items { get; set; }
    }

    public class RestApiV3CompatibilityFormat<T>
    {
        public RestApiV3Header Header { get; } = new RestApiV3Header();
        public T Data { get; }

        public RestApiV3CompatibilityFormat(T data)
        {
            Data = data;
        }

        public class RestApiV3Header
        {
            public DateTime Generated => DateTime.Now;
        }
    }

    public class RestApiV3Collection<T>
    {
        public ICollection<T> Items { get; set; }
    }

    public class RestApiV3CollectionCompatibilityFormat<T> : RestApiV3CompatibilityFormat<RestApiV3Collection<T>>
    {
        public RestApiV3CollectionCompatibilityFormat(ICollection<T> items) : base(new RestApiV3Collection<T> { Items = items })
        { }
    }
}
