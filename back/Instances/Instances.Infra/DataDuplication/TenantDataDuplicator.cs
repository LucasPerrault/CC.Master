using System.Threading.Tasks;

namespace Instances.Infra.DataDuplication
{
    public interface ITenantDataDuplicator
    {
        Task DuplicateOnRemoteAsync(TenantDataDuplication duplication);
    }

    public class TenantDataDuplicator : ITenantDataDuplicator
    {
        private readonly SqlScriptPicker _scriptPicker;

        public TenantDataDuplicator(SqlScriptPicker scriptPicker)
        {
            _scriptPicker = scriptPicker;
        }

        public async Task DuplicateOnRemoteAsync(TenantDataDuplication duplication)
        {
            var scripts = _scriptPicker.GetForDuplication(duplication);

            // request db and sgf duplication on remote and wait for the result
        }
    }
}
