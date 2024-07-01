using Sitecore.ContentSearch.SearchTypes;
using System.Linq;

namespace SmartcatPlugin.Extensions
{
    public static class ContentSearchExtension
    {
        public static IQueryable<SearchResultItem> WhereNameContains(this IQueryable<SearchResultItem> query, string searchQuery)
        {
            if (string.IsNullOrEmpty(searchQuery))
            {
                return query;
            }

            return query.Where(item => item.Name.Contains(searchQuery));
        }
    }
}