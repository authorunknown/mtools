using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json;

public class Js
{
    string _value;

    private Js(string value)
    {
        _value = value;
    }

    public override string ToString()
    {
        return _value;
    }

    public static Js Variable(string name, object value)
    {
        return new Js(string.Format("{0} = {1};", name, JsonConvert.SerializeObject(value)));
    }
}

public static class HtmlHelperExtensions
{
    public static MvcHtmlString ScriptBlock(this HtmlHelper html, params Js[] content)
    {
        if (content == null || content.Length == 0)
            return MvcHtmlString.Empty;

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("<script type=\"text/javascript\">");
        sb.AppendLine("//<![CDATA[");
        foreach (Js v in content)
            sb.AppendLine(v.ToString());
        sb.AppendLine("//]]>");
        sb.AppendLine("</script>");
        return MvcHtmlString.Create(sb.ToString());
    }
}