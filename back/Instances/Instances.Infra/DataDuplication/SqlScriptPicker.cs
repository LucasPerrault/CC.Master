using Distributors.Domain.Models;
using Instances.Domain.Instances;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Instances.Infra.DataDuplication
{
    public class SqlScriptPicker : ISqlScriptPicker
    {
        public List<Uri> GetForDuplication(InstanceDuplication duplication)
        {
            var distinctScriptPaths = EnumerateForDuplication(duplication).ToHashSet();

            var absoluteUri = new Uri("http://jenkins2.lucca.local/finish/me/please");

            return distinctScriptPaths.Select(p => new Uri(absoluteUri, p)).ToList();
        }

        private IEnumerable<string> EnumerateForDuplication(InstanceDuplication duplication)
        {
            if (duplication.Type == InstanceDuplicationType.Demos || duplication.Type == InstanceDuplicationType.Training)
            {
                yield return @"Integration\CloudControl\clean_db_for_training.sql";
            }
            if (duplication.Type == InstanceDuplicationType.Demos && duplication.DistributorId == DistributorIds.Mprh)
            {
                yield return @"Integration\CloudControl\MPRH_Demo.login.uniqueness.sql";
            }
            if (duplication.Type == InstanceDuplicationType.Production)
            {
                yield return @"Integration\CloudControl\clean_db_for_new_instance.sql";
            }
        }
    }
}
