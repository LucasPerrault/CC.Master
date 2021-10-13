using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Instances.Domain.Github.Models
{
    public class GithubApiCommit
    {
        public string Sha { get; set; }
        public string Message { get; set; }
        public string Commiter { get; set; }
        public DateTime CommitedOn
        {
            get
            {
                return commitedOn;
            }
            set
            {
                commitedOn = value.ToLocalTime();
            }
        }
        private DateTime commitedOn { get; set; }
    }
}
