using Tools;

namespace Users.Domain.Filtering
{
    public class UsersFilter
    {
        public BoolCombination IsActive { get; set; } = BoolCombination.Both;

        public static UsersFilter ActiveOnly = new UsersFilter { IsActive = BoolCombination.TrueOnly };
    }
}
