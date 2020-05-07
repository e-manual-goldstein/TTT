using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace TTT.Common
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class MenuActionAttribute : Attribute
    {
        object _order;
        string _title;
        public string Order => _order?.ToString();
        public string Title => _title;
        
        public MenuActionAttribute(object order, string customTitle = null, [CallerMemberName] string title = null) : this(customTitle, title)
        {
            _order = order ?? "";
        }

        public  MenuActionAttribute(string customTitle = null, [CallerMemberName] string title = null)
        {
            _title = customTitle ?? SplitTitleCase(title);
        }

        private string SplitTitleCase(string title)
        {
            return Regex.Replace(title, @"([A-Z])", @" $1");
        }
    }
}
