using System.Web;

namespace Versus.Api.Tests.Helpers;

public static class QueryStringHelper
{
    public static string AsQueryString<T>(this T obj)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }

        return typeof(T)
            .GetProperties()
            .Where(p => p.GetValue(obj) != null)
            .Select(p => $"{p.Name}={HttpUtility.UrlEncode(p.GetValue(obj)!.ToString())}")
            .Aggregate((current, next) => current + "&" + next);
    }
}