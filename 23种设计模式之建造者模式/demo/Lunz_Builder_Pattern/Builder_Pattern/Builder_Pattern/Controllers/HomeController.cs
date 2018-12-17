using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Builder_Pattern.Models;
using Builder_Pattern.Libs;

namespace Builder_Pattern.Controllers
{
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult About()
		{
			ViewData["Message"] = "Your application description page.";

			return View();
		}

		public IActionResult Contact()
		{
			ViewData["Message"] = "Your contact page.";

			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}


		#region Builder-Pattern
		public ActionResult DCProject_Client()
		{
			ZRProjectBuilder builder = new ConcreteBuilder_DatacenterProject();
			ZRProjectDirector director = new ZRProjectDirector(builder);
			ZRProject project = director.BuldZRProject();
			project.DeliveryProject();

			return View();
		}

		public ActionResult LunzsharpProject_Client()
		{
			ZRProjectBuilder builder = new  ConcreteBuilder_LunzsharpProject();
			ZRProjectDirector director = new ZRProjectDirector(builder);
			ZRProject project = director.BuldZRProject();
			project.DeliveryProject();

			return View();
		}
		#endregion

	}
}
