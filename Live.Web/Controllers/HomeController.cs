using System;
using System.Linq;
using Live.Web.Store;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Live.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserStore _userStore;
        private readonly CommentStore _commentStore;
        private readonly IConfiguration _configuration;

        public HomeController(UserStore userStore, IConfiguration configuration, CommentStore commentStore)
        {
            _userStore = userStore;
            _configuration = configuration;
            _commentStore = commentStore;
        }

        public IActionResult Index()
        {
            SetUser();
            return View();
        }

        public IActionResult Room(int id)
        {
            if (id != 1111 && id != 2222 && id != 3333)
                return RedirectToAction("Index");

            SetUser();
            ViewBag.LiveUrl = _configuration["LiveUrl"];
            ViewBag.Comments = _commentStore.Comments.Where(p => p.Group == id.ToString()).ToList();
            return View(id);
        }

        private void SetUser()
        {
            if (HttpContext.Session.TryGetValue("user", out byte[] value))
                return;

            var random = new Random();
            int index = random.Next(0, 10);
            var user = _userStore.Users[index];
            HttpContext.Session.SetString("user", JsonConvert.SerializeObject(user));
        }
    }
}
