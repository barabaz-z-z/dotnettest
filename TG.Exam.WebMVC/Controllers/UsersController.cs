using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestModels = TG.Exam.WebMVC.Models;

namespace TG.Exam.WebMVC.Controllers
{
    /// <summary>
    /// Actually, I didn't catch how I should accomplish 1D, 2B, 3C options from description.
    /// I did them as I understood.
    /// </summary>
    public class UsersController : Controller
    {
        public ActionResult Index()
        {
            var users = TestModels.User.GetAll();

            return View(users);
        }

        public ActionResult Fetch(TestModels.User user)
        {
            return View(user);
        }
    }
}