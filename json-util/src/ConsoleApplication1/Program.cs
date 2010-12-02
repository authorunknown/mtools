using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace ConsoleApplication1
{
    public static class Program
    {
        static void Main(string[] args)
        {
            var result = Html.ScriptBlock(
                Js.Variable("x", 1),
                Js.Variable("y", new { name = "foo", value = "bar" }));
            Console.WriteLine(result.ToString());
        }

        private static HtmlHelper Html;
    }
}
