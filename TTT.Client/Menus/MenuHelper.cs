using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TTT.Common;

namespace TTT.Client.Menus
{
    public static class MenuHelper
    {
        public static Dictionary<string, MethodInfo> GetMenuActions(this Type objectType)
        {
            return objectType.GetMethods()
                .Select(m => new { Attribute = m.GetCustomAttribute<MenuActionAttribute>(), MethodInfo = m })
                .Where(obj => obj.Attribute != null)
                .OrderBy(obj => obj.Attribute.Order)
                .ToDictionary(obj => obj.Attribute.Title, m => m.MethodInfo);
        }
    }
}