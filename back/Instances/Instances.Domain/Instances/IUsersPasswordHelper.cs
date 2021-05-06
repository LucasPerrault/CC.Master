namespace Instances.Domain.Instances
{
    public interface IUsersPasswordHelper
    {
        void ThrowIfInvalid(string password);
        string Generate();
    }
}
