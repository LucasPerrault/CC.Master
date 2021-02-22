using Lucca.Core.Rights.RightsHelper;
using Rights.Domain;
using Rights.Domain.Abstractions;
using Rights.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Rights.Infra.Services
{
    public class RightsService : IRightsService
    {
        private readonly ClaimsPrincipalRightsHelper _rightsHelper;
        private readonly ClaimsPrincipal _principal;

        public RightsService(ClaimsPrincipalRightsHelper rightsHelper, ClaimsPrincipal principal)
        {
            _rightsHelper = rightsHelper ?? throw new ArgumentNullException(nameof(rightsHelper));
            _principal = principal ?? throw new ArgumentNullException(nameof(principal));
        }

        public async Task ThrowIfAnyOperationIsMissingAsync(params Operation[] operations)
        {
            var missingOps = new List<Operation>();
            foreach (var operation in operations)
            {
                if (!await HasOperationAsync(operation))
                {
                    missingOps.Add(operation);
                }
            }

            if (missingOps.Any())
            {
                throw new MissingOperationsException(missingOps.ToArray());
            }
        }

        public async Task ThrowIfAllOperationsAreMissingAsync(params Operation[] operations)
        {
            foreach (var operation in operations)
            {
                if (await HasOperationAsync(operation))
                {
                    return;
                }
            }
            throw new MissingOperationsException(operations);
        }

        public async Task<bool> HasOperationAsync(Operation operation)
        {
            return await _rightsHelper.HasOperationAsync(_principal, RightsHelper.CloudControlAppInstanceId, (int)operation);
        }
    }
}