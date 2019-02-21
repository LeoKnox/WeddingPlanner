using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingPlanner.Models;

namespace WeddingPlanner.Controllers
{
    public class HomeController : Controller
    {
        private WeddingPlannerContext dbContext;
        public HomeController(WeddingPlannerContext context) { dbContext = context; } 

        [Route("")]
        [HttpGet]
        public IActionResult Index()
        {
            LogReg auth = new LogReg();
            return View(auth);
        }

        [Route("register")]
        [HttpPost]
        public IActionResult Register(User newUser)
        {
            if (ModelState.IsValid)
            {
                dbContext.CreateUser(newUser, HttpContext);
                return RedirectToAction("guest", "Home");
            }
            else
                return View("Index");
        }

        [Route("wedding/{weddingId}")]
        [HttpGet]
        public IActionResult Wedding(int weddingId)
        {
            Wedding oneWedding = dbContext.Weddings
                .Where(we => we.WeddingId == weddingId)
                .Include(guest => guest.Attendee)
                .ThenInclude(person => person.User)
                .FirstOrDefault();
            return View("Wedding", oneWedding);
        }
        
        [Route("ursvp/{weddingId}")]
        [HttpGet]
        public IActionResult Ursvp(int weddingId)
        {
            Guest g = dbContext.Guests
                .FirstOrDefault(gu => gu.WeddingId == weddingId);
            dbContext.Guests.Remove(g);
            dbContext.SaveChanges();
            return RedirectToAction("guest");
        }

        [Route("rsvp/{weddingId}")]
        [HttpGet]
        public IActionResult Rsvp(int weddingId)
        {
            Guest g = new Guest();
            int? x  = HttpContext.Session.GetInt32("uid");
            g.UserId = (int)x;
            g.WeddingId = weddingId;
            dbContext.Add(g);
            dbContext.SaveChanges();
            return RedirectToAction("guest");
        }

        [Route("guest")]
        [HttpGet]
        public IActionResult Guest()
        {
            int? id  = HttpContext.Session.GetInt32("uid");
            if (id is null) return RedirectToAction("Index");

            User user = dbContext.Users.FirstOrDefault(u => u.UserId == id);
            List<Wedding> AllWeddings = dbContext.Weddings
                .Include(person => person.Attendee)
                .ThenInclude(wed => wed.Wedding)
                .ToList();

            Dashboard dash = new Dashboard();
            dash.User = user;
            dash.Weddings = AllWeddings;
            return View("Guest", dash);
        }

        [Route("login")]
        [HttpPost]
        public IActionResult Login(Logger newLogin)
        {
            if (ModelState.IsValid)
            {
                User oneUser = dbContext.Users
                    .FirstOrDefault(log => log.Email == newLogin.lemail);
                var hasher = new PasswordHasher<Logger>();
                var result = hasher.VerifyHashedPassword(
                    newLogin, oneUser.Password, newLogin.lpassword);
                if (result == 0)
                {
                    ModelState.AddModelError("Password", "Password does not match");
                    return View("Index");
                }
                HttpContext.Session.SetInt32("uid", oneUser.UserId);
                return RedirectToAction("guest");
            }
            return View("Index");
        }

        [Route("createwedding")]
        [HttpGet]
        public IActionResult CreateWedding()
        {
            return View("CreateWedding");
        }

        [Route("newwedding")]
        [HttpPost]
        public IActionResult NewWedding(Wedding newWedding)
        {
            if (ModelState.IsValid){
                dbContext.Add(newWedding);
                dbContext.SaveChanges();
                return View("Wedding");
            }
            else
                return View("CreateWedding");
        }

        [Route("logout")]
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
