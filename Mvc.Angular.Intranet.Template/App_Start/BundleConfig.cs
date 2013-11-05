using System.Web;
using System.Web.Optimization;

namespace Mvc.Angular.Intranet.Template
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            var cssBundle = new StyleBundle("~/Content/css")
                .Include("~/Vendor/bootstrap/dist/css/bootstrap.css");
            cssBundle.Transforms.Add(new CssMinify());
            bundles.Add(cssBundle);

            var vendorBundle = new ScriptBundle("~/Scripts/vendor")
                .Include("~/Vendor/angular/angular.js")
                .Include("~/Vendor/angular-bootstrap/ui-bootstrap-tpls.js")
                .Include("~/Vendor/angular-ui-router/angular-ui-router.js")
                .Include("~/Vendor/stringjs/lib/string.min.js")
                .Include("~/Vendor/underscore/underscore-min.js")
                .Include("~/Vendor/placeholders/angular-placeholders-0.0.1-SNAPSHOT.min.js");
            vendorBundle.Transforms.Add(new JsMinify());
            bundles.Add(vendorBundle);

            var lessBundle = new Bundle("~/Content/less")
                .Include("~/App/Less/variables.less")
                .Include("~/App/Less/main.less")
                .IncludeDirectory("~/App", "*.less", true);
            lessBundle.Transforms.Add(new LessTransform());
            lessBundle.Transforms.Add(new CssMinify());
            bundles.Add(lessBundle);

            var jsBundle = new ScriptBundle("~/Scripts/App")
                .IncludeDirectory("~/App", "*.js", true)
                .IncludeDirectory("~/App", "*.coffee", true);
            jsBundle.Transforms.Add(new CoffeeTransform());
            jsBundle.Transforms.Add(new JsMinify());
            bundles.Add(jsBundle);

            bundles.IgnoreList.Ignore("*.spec.js");
            bundles.IgnoreList.Ignore("*.spec.coffee");

            // If you'd like to test the optimization locally,
            // you can use this line to force it.
            BundleTable.EnableOptimizations = true;
        }
    }
}