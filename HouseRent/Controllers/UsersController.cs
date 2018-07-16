using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HouseRent.Models;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace HouseRent.Controllers
{
    public class UsersController : Controller
    {
        private const string userString = "ID,Name,Contact,Email,Password,Address,Role,Avatar";
        private readonly HouseRentContext _context;

        public UsersController(HouseRentContext context)
        {
            _context = context;
        }

        //A funtion to return image path from database
        public FileContentResult GetImg(int id)
        {
            var image = _context.User.Find(id).Avatar;

            return image != null ? new FileContentResult(image, "image/png") : null;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            //this means only admin can see the user list
            if(HttpContext.Session.GetString("sRole") != "admin")
            {
                return RedirectToAction("Index", "Home");
            } 
            return View(await _context.User.ToListAsync());
        }       


        public IActionResult Login()
        {
            //this means u cannot reload login page once u logged in
            if(!String.IsNullOrEmpty(HttpContext.Session.GetString("sEmail")))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind(userString)] User user)
        {
            var usr = await _context.User
                       .SingleOrDefaultAsync(u => u.Email.ToUpper() == user.Email.ToUpper()
                       && u.Password == user.Password);
            if (usr == null)
            {
                HttpContext.Session.SetString("loginFailed", "Email & Password didn't matched!");
                return View();
            }
            HttpContext.Session.SetString("sName", usr.Name);
            HttpContext.Session.SetString("sEmail", usr.Email);
            HttpContext.Session.SetString("sRole", usr.Role);
            HttpContext.Session.SetString("sId", usr.ID.ToString());
            HttpContext.Session.Remove("loginFailed");

            return RedirectToAction("Index", "Advertises");

        }

        public IActionResult Logout()
        {
            //all session variables, sets as empty...
            HttpContext.Session.Remove("sName");
            HttpContext.Session.Remove("sEmail");
            HttpContext.Session.Remove("sRole");
            HttpContext.Session.Remove("sId");
            HttpContext.Session.Remove("loginFailed");
            HttpContext.Session.Remove("userExist");

            return RedirectToAction(nameof(Login));
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            //this means u cannot load details page without logged in
            if (String.IsNullOrEmpty(HttpContext.Session.GetString("sEmail")))
            {
                return RedirectToAction("Index", "Home");
            }
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .SingleOrDefaultAsync(m => m.ID == id);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                //this means admins and owners only can see their account details
                if(user.Email != HttpContext.Session.GetString("sEmail")
                    && HttpContext.Session.GetString("sRole") != "admin")
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            //this means u cannot load create/signup page once u logged in
            if (!String.IsNullOrEmpty(HttpContext.Session.GetString("sEmail")))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Users/Create  (Used for sign up)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(userString)] User user, IFormFile img)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //for image : if given an image or not
                    if (img.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            img.CopyTo(ms);
                            var fileBytes = ms.ToArray();
                            user.Avatar = fileBytes;
                        }
                    } 
                    user.Role = "normal"; //others user are either admin or banned
                    _context.Add(user);
                    await _context.SaveChangesAsync();
                    HttpContext.Session.SetString("sName", user.Name);
                    HttpContext.Session.SetString("sEmail", user.Email);
                    HttpContext.Session.SetString("sRole", user.Role);
                    HttpContext.Session.SetString("sId", user.ID.ToString());
                    HttpContext.Session.Remove("userExist");
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    HttpContext.Session.SetString("userExist", user.Email+" Already Exist. Go to Login Page.");
                    return View(user);
                }
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            //this means u cannot load edit page without logged in
            if (String.IsNullOrEmpty(HttpContext.Session.GetString("sEmail")))
            {
                return RedirectToAction("Index", "Home");
            }
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User.SingleOrDefaultAsync(m => m.ID == id);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                //this means owners only can edit their account details
                if (user.Email != HttpContext.Session.GetString("sEmail"))
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind(userString)] User user)
        {
            if (id != user.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            //this means u cannot load delete page without logged in
            if (String.IsNullOrEmpty(HttpContext.Session.GetString("sEmail")))
            {
                return RedirectToAction("Index", "Home");
            }
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .SingleOrDefaultAsync(m => m.ID == id);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                //this means admins and owners can delete an account
                if (user.Email != HttpContext.Session.GetString("sEmail")
                    && HttpContext.Session.GetString("sRole") != "admin")
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.User.SingleOrDefaultAsync(m => m.ID == id);
            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            //if the owner deletes his account, he will be logged out
            if(HttpContext.Session.GetString("sEmail") == user.Email)
            {
                return RedirectToAction(nameof(Logout));
            }
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.ID == id);
        }
    }
}
