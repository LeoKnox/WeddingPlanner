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

    public HomeController(WeddingPlannerContext context)
    {
        dbContext = context;
    }

        [Route("")]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [Route("register")]
        [HttpPost]
        public IActionResult register(User newUser)
        {
            if (ModelState.IsValid)
            {
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                dbContext.Add(newUser);
                dbContext.SaveChanges();
                User oneUser = dbContext.Users.Last();
                int x = oneUser.UserId;
                HttpContext.Session.SetInt32("uid", x);
                return RedirectToAction("guest");
            }
            else
            {
                return View("Index");
            }
        }

        [Route("wedding/{num}")]
        [HttpGet]
        public IActionResult wedding(int num)
        {
            Wedding oneWedding = dbContext.Weddings.Where(we => we.WeddingId == num)
                .Include(guest => guest.Attendee)
                .ThenInclude(person => person.User).FirstOrDefault();
            return View("Wedding", oneWedding);
        }
        
        [Route("ursvp/{num}")]
        [HttpGet]
        public IActionResult ursvp(int num)
        {
            Guest g = dbContext.Guests.FirstOrDefault(gu => gu.WeddingId == num);
            dbContext.Guests.Remove(g);
            dbContext.SaveChanges();
            return RedirectToAction("guest");
        }

        [Route("rsvp/{num}")]
        [HttpGet]
        public IActionResult rsvp(int num)
        {
            Guest g = new Guest();
            int? x  = HttpContext.Session.GetInt32("uid");
            g.UserId = (int)x;
            g.WeddingId = num;
            dbContext.Add(g);
            dbContext.SaveChanges();
            return RedirectToAction("guest");
        }

        [Route("guest")]
        [HttpGet]
        public IActionResult guest()
        {
            int? x  = HttpContext.Session.GetInt32("uid");
            if (x == null) {
                return RedirectToAction("Index");
            }
            ViewBag.uid = HttpContext.Session.GetInt32("uid");
            var AllWeddings = dbContext.Weddings
                .Include(person => person.Attendee)
                .ThenInclude(wed => wed.Wedding)
                .ToList();
            return View("Guest", AllWeddings);
        }

        [Route("login")]
        [HttpPost]
        public IActionResult login(Logger newLogin)
        {
            if (ModelState.IsValid)
            {
                User oneUser = dbContext.Users.Where(log => log.Email == newLogin.lemail).FirstOrDefault();
                var hasher = new PasswordHasher<Logger>();
                var result = hasher.VerifyHashedPassword(newLogin, oneUser.Password, newLogin.lpassword);
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
        public IActionResult createwedding()
        {
            return View("CreateWedding");
        }

        [Route("newwedding")]
        [HttpPost]
        public IActionResult newwedding(Wedding NewWedding)
        {
            if (ModelState.IsValid){
                dbContext.Add(NewWedding);
                dbContext.SaveChanges();
                return View("Wedding");
            }
            else
            {
                return View("CreateWedding");
            }
        }

        [Route("logout")]
        [HttpGet]
        public IActionResult logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
