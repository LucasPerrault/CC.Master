using System.Threading.Tasks;

namespace Instances.Infra.DbDuplication
{
    public interface IDatabaseDuplicator
    {
        Task DuplicateOnRemoteAsync(DatabaseDuplication duplication);
    }

    public class DatabaseDuplicator : IDatabaseDuplicator
    {
        private readonly SqlScriptPicker _scriptPicker;

        public DatabaseDuplicator(SqlScriptPicker scriptPicker)
        {
            _scriptPicker = scriptPicker;
        }

        public async Task DuplicateOnRemoteAsync(DatabaseDuplication duplication)
        {
            var scripts = _scriptPicker.GetForDuplication(duplication);

            // request db duplication on remote and wait for the result
        }
    }
}
