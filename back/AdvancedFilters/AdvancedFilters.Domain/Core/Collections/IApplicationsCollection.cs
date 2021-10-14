using AdvancedFilters.Domain.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdvancedFilters.Domain.Core.Collections
{
    public interface IApplicationsCollection
    {
        Task<IReadOnlyCollection<Application>> GetAsync();
    }

    public class ApplicationsCollection : IApplicationsCollection
    {
        private static readonly Dictionary<string, string> ApplicationNamesById = new Dictionary<string, string>
        {
            { "WEXPENSES", "Cleemy" },
            { "DIRECTORY", "Collaborateurs" },
            { "WFIGGO", "Figgo" },
            { "WPAGGA", "Pagga" },
            { "WPOPLEESTATS", "Poplee Core RH" },
            { "WPOPLEEREM", "Poplee Rem." },
            { "POPLEETALENT", "Poplee Entretiens & Objectifs" },
            { "WTIMMI", "Timmi" },
            { "WTIMMIPROJECT", "Timmi Project" },
            { "LUCCAFACES", "Lucca Faces" },
            { "CLEEMYBANKING", "Cleemy Banking" },
            { "PROCUREMENT", "Cleemy Achats" },
            { "LUCCANIKONIKO", "Niko niko" },
            { "CLIENT-CENTER", "Espace client" },
            { "BLOOM", "Bloom at Work" },
            { "TALENTTRAINING", "Talent Training" },
            { "MEALVOUCHER", "Titres-restaurant" },
            { "PAYMONITOR", "PayMonitor" },
        };

        public Task<IReadOnlyCollection<Application>> GetAsync()
        {
            var applications = ApplicationNamesById
                .Select(kvp => new Application { Id = kvp.Key, Name = kvp.Value })
                .ToList();

            return Task.FromResult<IReadOnlyCollection<Application>>(applications);
        }
    }
}
