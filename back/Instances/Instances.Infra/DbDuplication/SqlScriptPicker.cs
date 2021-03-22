﻿using Distributors.Domain.Models;
using System.Collections.Generic;
using System.Linq;

namespace Instances.Infra.DbDuplication
{

    public class SqlScriptPicker
    {
        public HashSet<string> GetForDuplication(DbDuplication.DatabaseDuplication duplication)
        {
            return EnumerateForDuplication(duplication).ToHashSet();
        }

        private IEnumerable<string> EnumerateForDuplication(DbDuplication.DatabaseDuplication duplication)
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
