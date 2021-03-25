using Distributors.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Instances.Infra.DataDuplication
{

    public class SqlScriptPicker
    {
        public List<Uri> GetForDuplication(TenantDataDuplication duplication)
        {
            var distinctScriptPaths = EnumerateForDuplication(duplication).ToHashSet();

            var absoluteUri = new Uri("jenkins2.lucca.local/finish/me/please");

            return distinctScriptPaths.Select(p => new Uri(absoluteUri, p)).ToList();
        }

        private IEnumerable<string> EnumerateForDuplication(TenantDataDuplication duplication)
        {
            if (duplication.Type == DatabaseType.Demos || duplication.Type == DatabaseType.Training)
            {
                yield return @"Integration\CloudControl\clean_db_for_training.sql";
            }
            if (duplication.Type == DatabaseType.Demos && duplication.Distributor.Code == DistributorCodes.Mprh)
            {
                yield return @"Integration\CloudControl\MPRH_Demo.login.uniqueness.sql";
            }
            if (duplication.Type == DatabaseType.Production)
            {
                yield return @"Integration\CloudControl\clean_db_for_new_instance.sql";
            }
        }
    }
}
