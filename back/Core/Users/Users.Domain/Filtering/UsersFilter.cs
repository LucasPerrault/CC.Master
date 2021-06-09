using Tools;

namespace Users.Domain.Filtering
{
    public class UsersFilter
    {
        public CompareBoolean IsActive { get; set; } = CompareBoolean.Bypass;
        public string Search { get; set; } = null;
    }
}
