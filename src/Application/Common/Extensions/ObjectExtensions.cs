using System.Collections;
using System.Runtime.InteropServices;
using System.Text;

namespace CleanArchitecture.Application.Common.Extensions;

public static class ObjectExtensions
{
    public static string ToQueryString(this object obj)
    {
        var properties = obj.GetType().GetProperties();
        var queryString = new StringBuilder();

        foreach (var property in properties)
        {
            var value = property.GetValue(obj);

            if (value != null)
            {
                if (typeof(IList).IsAssignableFrom(property.PropertyType))
                {
                    var list = (IList)value;
                    foreach (var t in list)
                    {
                        queryString.Append($"{property.Name}={Uri.EscapeDataString(t.ToString() ?? string.Empty)}&");
                    }
                }
                else
                {
                    queryString.Append($"{property.Name}={Uri.EscapeDataString(value.ToString() ?? string.Empty)}&");
                }
            }
        }

        if (queryString.Length > 0)
        {
            queryString.Length--; // Remove the last '&' character
        }

        return queryString.ToString();
    }
}

