﻿using Rights.Domain.Abstractions;
using System;
using System.Threading.Tasks;

namespace Rights.Domain.Filtering
{
    public class RightsFilter
    {
        private readonly IRightsService _rightsService;

        public RightsFilter(IRightsService rightsService)
        {
            _rightsService = rightsService;
        }

        public async Task<AccessRight> FilterByDistributorAsync(Operation operation, int distributorId)
        {
            var currentUserScope = await _rightsService.GetUserOperationHighestScopeAsync(operation);
            return currentUserScope switch
            {
                AccessRightScope.AllDistributors => AccessRight.All,
                AccessRightScope.OwnDistributorOnly => AccessRight.ForDistributor(distributorId),
                _ => throw new ApplicationException($"Unhandled scope : {currentUserScope}")
            };
        }
    }
}
