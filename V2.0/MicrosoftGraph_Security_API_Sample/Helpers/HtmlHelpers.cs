// -----------------------------------------------------------------------
// <copyright file="HtmlHelpers.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MicrosoftGraph_Security_API_Sample.Helpers
{
    public static class StatisticHelper
    {
        public static MvcHtmlString CreateStatisticColumn<T>(this HtmlHelper html, Dictionary<KeyValuePair<string, string>, Dictionary<T, int>> items, string filterKey, string iconName = null) where T : struct, IConvertible
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            var list = new List<TagBuilder>();

            foreach (var item in items)
            {
                // Create wrapper
                TagBuilder wrapperDiv = new TagBuilder("div");
                wrapperDiv.AddCssClass("stat-row stat-lg-row");

                if (!string.IsNullOrWhiteSpace(iconName))
                {
                    // Create icon item
                    TagBuilder iconDiv = new TagBuilder("div");
                    iconDiv.AddCssClass("icon");
                    TagBuilder icon = new TagBuilder("i");
                    icon.AddCssClass(iconName);
                    iconDiv.InnerHtml += icon.ToString();
                    wrapperDiv.InnerHtml += iconDiv.ToString();
                }

                // Create row title
                TagBuilder titleDiv = new TagBuilder("div");
                titleDiv.AddCssClass("title");

                // Create link with filter
                TagBuilder a = new TagBuilder("a");
                if (filterKey.Equals("netconn:destinationaddress"))
                {
                    a.SetInnerText(item.Key.Value);
                }
                else
                {
                    a.SetInnerText(item.Key.Key);
                }
               
                a.MergeAttribute("href", $"/Home/Filter?key={filterKey}&value={HttpUtility.UrlEncode(item.Key.Value)}");
                titleDiv.MergeAttribute("title", item.Key.Value);
                titleDiv.InnerHtml += a.ToString();

                wrapperDiv.InnerHtml += titleDiv.ToString();

                // Create html for each value
                foreach (T severityLevel in (T[])Enum.GetValues(typeof(T)))
                {
                    TagBuilder valueDiv = new TagBuilder("div");
                    valueDiv.AddCssClass($"value {severityLevel.ToString().ToLower()}");
                    valueDiv.SetInnerText(item.Value.ContainsKey(severityLevel) ? item.Value[severityLevel].ToString() : "0");
                    wrapperDiv.InnerHtml += valueDiv.ToString();
                }

                list.Add(wrapperDiv);
            }

            return new MvcHtmlString(string.Join(Environment.NewLine, list.Select(item => item.ToString())));
        }

        public static MvcHtmlString CreateFilterActionLink(this HtmlHelper html, string title, string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
            {
                throw new Exception("Key and value can't be empty");
            }

            // Create link with filter
            TagBuilder a = new TagBuilder("a");
            a.SetInnerText(!string.IsNullOrWhiteSpace(title) ? title : value);
            a.MergeAttribute("href", $"/Home/Filter?key={key}&value={HttpUtility.UrlEncode(value)}");

            return new MvcHtmlString(a.ToString());
        }
    }
}