using AdvancedFilters.Domain.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdvancedFilters.Domain.Core.Collections
{
    public interface IApplicationsCollection
    {
        Task<IReadOnlyCollection<Application>> GetAsync(string search);
    }

    public class ApplicationsCollection : IApplicationsCollection
    {
        public static readonly HashSet<string> SystemApplicationIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "LUCCA", "DIRECTORY", "CLIENT-CENTER"
        };

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
            { "GUI", "GUI" },
            { "LUCCA", "Lucca" },
            { "WCALENDAR", "Calendar" },
            { "WEXTERNE", "Externe" },
            { "WGEDSIMPLE", "Gedsimple" },
            { "WINFOSUGO", "Infosugo" },
            { "WPLANNER", "Planner" },
            { "WPLANNING", "Planning" },
            { "WPM", "Wpm" },
            { "WUCALHEBDO", "Ucalhebdo" },
            { "WURBA", "Urba" },
            { "WUTIMEEXPORT", "Utimeexport" },
        };

        public Task<IReadOnlyCollection<Application>> GetAsync(string search)
        {
            var applications = ApplicationNamesById
                .Select(kvp => new Application { Id = kvp.Key, Name = kvp.Value })
                .Where(e => !SystemApplicationIds.Contains(e.Id))
                .Where(a => string.IsNullOrEmpty(search) || a.Name.ToLowerInvariant().StartsWith(search.ToLowerInvariant()))
                .OrderBy(a => a.Name)
                .ToList();

            return Task.FromResult<IReadOnlyCollection<Application>>(applications);
        }

        public static string GetName(string applicationId)
        {
            if (applicationId is null || !ApplicationNamesById.TryGetValue(applicationId, out var name))
            {
                return null;
            }

            return name;
        }
    }
}
