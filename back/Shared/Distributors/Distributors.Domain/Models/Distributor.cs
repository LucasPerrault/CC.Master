namespace Distributors.Domain.Models
{
    public static class DistributorCodes
    {
        public const string Mprh = "MPRH";
    }

    public class Distributor
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
