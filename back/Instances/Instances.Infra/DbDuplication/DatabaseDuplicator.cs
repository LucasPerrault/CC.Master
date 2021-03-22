using System.Threading.Tasks;

namespace Instances.Infra.DbDuplication
{
    public class DatabaseDuplicator
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
