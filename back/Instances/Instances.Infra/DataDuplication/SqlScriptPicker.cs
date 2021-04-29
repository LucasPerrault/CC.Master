using Distributors.Domain.Models;
using Instances.Domain.Instances;
using Instances.Domain.Instances.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Instances.Infra.DataDuplication
{
    public class SqlScriptPickerConfiguration
    {
        public Uri JenkinsBaseUri { get; set; }
        public string MonolithJobPath { get; set; }
    }

    public class SqlScriptPicker : ISqlScriptPicker
    {
        private readonly SqlScriptPickerConfiguration _configuration;

        public SqlScriptPicker(SqlScriptPickerConfiguration configuration)
        {
            _configuration = configuration;
        }
        public List<Uri> GetForDuplication(InstanceDuplication duplication)
        {
            var distinctScriptPaths = EnumerateForDuplication(duplication).ToHashSet();
            // En attendant d'avoir la gestion des sources de code, on prend le lastSuccessfulBuild de master
            var absoluteUri = new Uri(_configuration.JenkinsBaseUri,$"{_configuration.MonolithJobPath}/job/master/lastSuccessfulBuild/artifact/");

            return distinctScriptPaths.Select(p => new Uri(absoluteUri, p)).ToList();
        }

        private IEnumerable<string> EnumerateForDuplication(InstanceDuplication duplication)
        {
            if (duplication.TargetType == InstanceType.Training || duplication.TargetType == InstanceType.Demo)
            {
                yield return @"Integration\CloudControl\clean_db_for_training.sql";
            }
            if (duplication.TargetType == InstanceType.Demo && duplication.DistributorId == DistributorIds.PeopleSphere)
            {
                yield return @"Integration\CloudControl\MPRH_Demo.login.uniqueness.sql";
            }
            if (duplication.TargetType == InstanceType.Prod)
            {
                yield return @"Integration\CloudControl\clean_db_for_new_instance.sql";
            }
        }
    }
}
