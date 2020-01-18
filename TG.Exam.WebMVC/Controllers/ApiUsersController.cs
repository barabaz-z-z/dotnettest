using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using TestModels = TG.Exam.WebMVC.Models;

namespace Salestech.Exam.WebMVC.Controllers
{
    [Route("api/users")]
    public class ApiUsersController : ApiController
    {
        [HttpGet]
        public async Task<IHttpActionResult> GetUsers()
        {
            var users = await Task.Run(() =>
            {
                var modifiedUsers = TestModels.User.GetAll();
                modifiedUsers.ForEach(u => u.Age += 10);

                return modifiedUsers;
            });

            return Json(users);
        }
    }
}
