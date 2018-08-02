using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HouseRent.Models;
using System.IO;                    //*Directory
using Microsoft.AspNetCore.Http;    //*IFormFile
using Microsoft.AspNetCore.Hosting; //*get RootPath

namespace HouseRent.Controllers
{
    public class AdvertisesController : Controller
    {
        private const string AdString = "ID,Heading,UserMail,Phone,PostTime,RentDate,Address,YoutubeLink,FlatSize,FlatType,Category,Rent,FlatDetails,UtilitiesBill,OtherBill";
        private const string CmntString = "ID,AdvertiseID,Commenter,CommentTime,Anonymous,CommentText";
        private readonly HouseRentContext _context;

        public AdvertisesController(HouseRentContext context)
        {
            _context = context;
        }

        //A funtion to return image path from database
        public FileContentResult GetImg(int id)
        {
            var image = _context.Image.Find(id).FlatImage;
            return image != null ? new FileContentResult(image, "image/png") : null;
        }

        //A function to make normal youtube link to embeded youtube link
        public string YTlink(string link)
        {
            try
            {
                int youtu = link.IndexOf("youtu.be");
                if (youtu != -1) // obiviously in youtu.be category
                {
                    link = link.Substring(link.IndexOf("be/") + 3, 11);
                }
                else // in youtube.com category
                {
                    link = link.Substring(link.IndexOf("?v=") + 3, 11);
                }

                return "https://www.youtube.com/embed/" + link;
            }
            catch
            {
                //if given youtube link is not valid, it returns null 
                return null;
            }
        }

        // GET: Advertises
        public async Task<IActionResult> Index(DateTime rentfrom, string area, string type, string rent, string category)
        {
            var Add = from a in _context.Advertise
                      select a;

            //if we do not set, DateTime type returns this date 1/1/0001 12:00:00 AM, or "01-Jan-01 12:00:00 AM"
            if (rentfrom.ToString() != "1/1/0001 12:00:00 AM" && rentfrom.ToString() != "01-Jan-01 12:00:00 AM")
            {
                Add = Add.Where(f => f.RentDate <= rentfrom);
            }

            //now next values are rquared to check it is null or not
            if (!String.IsNullOrEmpty(area))
            {
                Add = Add.Where(a => a.Address.Contains(area));
            }
            if (!String.IsNullOrEmpty(type))
            {
                Add = Add.Where(t => t.FlatType.Contains(type));
            }
            if (!String.IsNullOrEmpty(category))
            {
                Add = Add.Where(c => c.Category.Contains(category));
            }

            //rent need to convert into int type from string
            if (!String.IsNullOrEmpty(rent))
            {
                int low = 1000 * Int32.Parse(rent.Substring(0, 2));
                int up = 1000 * Int32.Parse(rent.Substring(4, 2));
                Add = Add.Where(r => r.Rent >= low
                    && r.Rent <= up);
            }

            return View(await Add.ToListAsync());
        }

        public async Task<IActionResult> MyPosts()
        {
            string usermail = HttpContext.Session.GetString("sEmail");
            //this means u cannot load myposts without being logged in
            if (String.IsNullOrEmpty(usermail))
            {
                return RedirectToAction("Login", "Users");
            }
            var mypost = from mp in _context.Advertise
                         where mp.UserMail == usermail
                         select mp;
            return View(await mypost.ToListAsync());
        }

        // GET: Advertises/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            string tempUser = HttpContext.Session.GetString("sEmail");
            //this means u cannot load details without being logged in
            if (String.IsNullOrEmpty(tempUser))
            {
                return RedirectToAction("Login", "Users");
            }
            else
            {
                //if logged in, user exist or deleted!!

                var userexist = await _context.User.SingleOrDefaultAsync(u => u.Email == tempUser);
                if (userexist == null)
                {
                    return RedirectToAction("Logout", "Users");
                }
            }

            if (id == null)
            {
                return NotFound();
            }

            var advertise = await _context.Advertise
                .SingleOrDefaultAsync(m => m.ID == id);
            if (advertise == null)
            {
                return NotFound();
            }

            //that means normal youtube link converted to embeded youtube link
            advertise.YoutubeLink = YTlink(advertise.YoutubeLink);

            //from image table we will get images
            var images = from i in _context.Image
                         where i.AdvertiseID == id
                         select i;
            advertise.Images = await images.ToListAsync();

            //from comment table we will get comments
            var comments = from c in _context.Comment
                           where c.AdvertiseID == id
                           select c;
            advertise.Comments = await comments.ToListAsync();

            //from review table we will get reviews
            var reviews = from r in _context.Review
                          where r.AdvertiseID == id
                          select r;
            advertise.Reviews = await reviews.ToListAsync();


            return View(advertise);
        }

        //this function is to review
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoReview(Review review)
        {
            string usr = HttpContext.Session.GetString("sEmail");
            //this means u cannot review without being logged in
            if (String.IsNullOrEmpty(usr))
            {
                return RedirectToAction("Login", "Users");
            }
            review.Reviewer = usr;

            //checking if user reviewed once or not...
            var Reviewed = await _context.Review.SingleOrDefaultAsync(r => r.Reviewer == usr && r.AdvertiseID == review.AdvertiseID);

            if (Reviewed == null)
            {
                //if user reviewing for an advertise for the first time 
                _context.Add(review);
                await _context.SaveChangesAsync();
            }
            else
            {
                //if already reviewed it, but want to change review
                Reviewed.ReviewStar = review.ReviewStar;
                _context.Update(Reviewed);
                await _context.SaveChangesAsync();

            }


            return RedirectToAction("Details", new { id = review.AdvertiseID });
        }

        //this function is to make a comment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoComment([Bind(CmntString)] Comment comment)
        {
            string usr = HttpContext.Session.GetString("sEmail");
            //this means u cannot comment without being logged in
            if (String.IsNullOrEmpty(usr))
            {
                return RedirectToAction("Login", "Users");
            }
            comment.CommentTime = DateTime.Now;
            comment.Commenter = usr;
            _context.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = comment.AdvertiseID });

        }
        // Delete a comment
        public async Task<IActionResult> DeleteComment(int id)
        {
            //this means u cannot DeleteComment without being logged in
            if (String.IsNullOrEmpty(HttpContext.Session.GetString("sEmail")))
            {
                return RedirectToAction("Login", "Users");
            }
            var comment = await _context.Comment.SingleOrDefaultAsync(m => m.ID == id);

            //this means only commenter can only delete his comments
            if (HttpContext.Session.GetString("sEmail") == comment.Commenter)
            {
                _context.Comment.Remove(comment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", new { id = comment.AdvertiseID });
        }

        // GET: Advertises/Create
        public async Task<IActionResult> Create()
        {
            string tempUser = HttpContext.Session.GetString("sEmail");

            //this means u cannot Create Advertises without being logged in
            if (String.IsNullOrEmpty(tempUser))
            {
                return RedirectToAction("Login", "Users");
            }
            else
            {
                //if logged in, user exist or deleted!!

                var userexist = await _context.User.SingleOrDefaultAsync(u => u.Email == tempUser);
                if (userexist == null)
                {
                    return RedirectToAction("Logout", "Users");
                }
            }
            return View();
        }

        // POST: Advertises/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(AdString)] Advertise advertise, List<IFormFile> imgs)
        {
            if (ModelState.IsValid)
            {
                //Here we need to add some values not given by user
                advertise.PostTime = DateTime.Now;
                advertise.UserMail = HttpContext.Session.GetString("sEmail");


                _context.Add(advertise);
                await _context.SaveChangesAsync();  //updated the advertise table
                //now image table need to update
                foreach (var img in imgs)
                {

                    if (img.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            img.CopyTo(ms);
                            var fileBytes = ms.ToArray();
                            //string s = Convert.ToBase64String(fileBytes);

                            Image image = new Image();
                            image.AdvertiseID = advertise.ID;
                            image.FlatImage = fileBytes;
                            _context.Add(image);
                            await _context.SaveChangesAsync();

                            // act on the Base64 data
                        }
                    }
                } //all images updated in image table

                return RedirectToAction(nameof(Index));
            }
            return View(advertise);
        }

        // GET: Advertises/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            string tempUser = HttpContext.Session.GetString("sEmail");
            //this means u cannot Edit Advertises without being logged in
            if (String.IsNullOrEmpty(tempUser))
            {
                return RedirectToAction("Login", "Users");
            }
            else
            {
                //if logged in, user exist or deleted!!

                var userexist = await _context.User.SingleOrDefaultAsync(u => u.Email == tempUser);
                if (userexist == null)
                {
                    return RedirectToAction("Logout", "Users");
                }
            }
            if (id == null)
            {
                return NotFound();
            }

            var advertise = await _context.Advertise.SingleOrDefaultAsync(m => m.ID == id);
            if (advertise == null)
            {
                return NotFound();
            }
            else
            {
                //this means owners only can edit their advertises
                if (advertise.UserMail != HttpContext.Session.GetString("sEmail"))
                {
                    return RedirectToAction("Index", "Advertises");
                }
            }
            return View(advertise);
        }

        // POST: Advertises/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind(AdString)] Advertise advertise)
        {
            if (id != advertise.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(advertise);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdvertiseExists(advertise.ID))
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
            return View(advertise);
        }

        // GET: Advertises/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            string tempUser = HttpContext.Session.GetString("sEmail");
            //this means u cannot load delete page without being logged in
            if (String.IsNullOrEmpty(tempUser))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                //if logged in, user exist or deleted!!

                var userexist = await _context.User.SingleOrDefaultAsync(u => u.Email == tempUser);
                if (userexist == null)
                {
                    return RedirectToAction("Logout", "Users");
                }
            }
            if (id == null)
            {
                return NotFound();
            }

            var advertise = await _context.Advertise
                .SingleOrDefaultAsync(m => m.ID == id);
            if (advertise == null)
            {
                return NotFound();
            }
            else
            {
                //this means admins and owners can delete an account
                if (advertise.UserMail != HttpContext.Session.GetString("sEmail")
                    && HttpContext.Session.GetString("sRole") != "admin")
                {
                    return RedirectToAction("Index", "Advertises");
                }
            }

            return View(advertise);
        }

        // POST: Advertises/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var advertise = await _context.Advertise.SingleOrDefaultAsync(m => m.ID == id);
            _context.Advertise.Remove(advertise);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdvertiseExists(int id)
        {
            return _context.Advertise.Any(e => e.ID == id);
        }
    }
}
