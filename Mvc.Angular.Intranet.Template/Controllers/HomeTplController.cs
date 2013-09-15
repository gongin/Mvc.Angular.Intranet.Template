using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mvc.Angular.Intranet.Template.Controllers
{
    public class HomeTplController : Controller
    {
        public ActionResult Index()
        {
            return PartialView();
        }
    }
}
