using System;

namespace AdvancedFilters.Infra.Storage.DAO
{
    public interface IFacetValueDao
    {
        public int Id { get; set; }
        public int FacetId { get; set; }
        public int EnvironmentId { get; set; }

        public int? IntValue { get; set; }
        public DateTime? DateTimeValue { get; set; }
        public decimal? DecimalValue { get; set; }
        public string StringValue { get; set; }
    }

    internal static class FacetValueDaoExtensions
    {
        public static TDao Fill<TDao, T>(this TDao dao, T value) where TDao : IFacetValueDao
        {
            switch (value)
            {
                case int i:
                    dao.IntValue = i;
                    break;
                case string s:
                    dao.StringValue = s;
                    break;
                case decimal d:
                    dao.DecimalValue = d;
                    break;
                case DateTime d:
                    dao.DateTimeValue = d;
                    break;
                default:
                    throw new ArgumentException(); // TODO
            }

            return dao;
        }
    }
}
