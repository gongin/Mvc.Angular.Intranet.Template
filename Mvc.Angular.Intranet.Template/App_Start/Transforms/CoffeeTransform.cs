using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Optimization;

namespace Mvc.Angular.Intranet.Template
{
    public class CoffeeTransform : IBundleTransform
    {
        public void Process(BundleContext context, BundleResponse bundle)
        {
            context.HttpContext.Response.Cache.SetLastModifiedFromFileDependencies();

            var coffeeEngine = new CoffeeSharp.CoffeeScriptEngine();
            StringBuilder compiledScript = new StringBuilder(bundle.Content.Length);
            foreach (var bundleFile in bundle.Files)
            {
                using (var stream = new StreamReader(bundleFile.VirtualFile.Open()))
                {
                    string source = stream.ReadToEnd();
                    if (!bundleFile.VirtualFile.Name.EndsWith("coffee"))
                    {
                        compiledScript.Append(source);
                    }
                    else
                    {
                        compiledScript.Append(coffeeEngine.Compile(source, bare: true));
                    }

                    compiledScript.AppendLine();
                }
            }

            bundle.ContentType = "text/javascript";
            bundle.Content = compiledScript.ToString();
        }
    }
}