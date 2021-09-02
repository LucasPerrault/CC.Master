using Environments.Domain;
using Lucca.Core.Api.Abstractions.Paging;
using Lucca.Core.Rights.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Rights.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace CloudControl.Web.Controllers
{
    [ApiController, Route("api/application-description")]
    public class LuccaApplicationController
    {
        private static readonly Lazy<List<LuccaOperation>> AllDescriptions
            = new Lazy<List<LuccaOperation>>(() => OperationDescription
                .All()
                .Where(d => !d.IsPrivate)
                .Select(d => new LuccaOperation(d))
                .ToList());

        [HttpGet("operations")]
        public Page<LuccaOperation> GetAll() => new Page<LuccaOperation>
        {
            Items = AllDescriptions.Value,
            Count = AllDescriptions.Value.Count
        };

        [HttpGet("operations/v3-compatibility")]
        public RestApiV3CompatibilityFormat<LuccaOperation> GetAllButInV3() => new RestApiV3CompatibilityFormat<LuccaOperation>(AllDescriptions.Value);
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

    public class RestApiV3CompatibilityFormat<T>
    {
        public RestApiV3Data Data { get; }

        public RestApiV3CompatibilityFormat(ICollection<T> items)
        {
            Data = new RestApiV3Data { Items = items };
        }

        public class RestApiV3Data
        {
            public ICollection<T> Items { get; set; }
        }
    }
}
