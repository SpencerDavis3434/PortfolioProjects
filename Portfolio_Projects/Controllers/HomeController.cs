using Portfolio_Projects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portfolio_Projects.Controllers
{
    public class HomeController : Controller
    {

        private Entities db = new Entities();
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string longLink, DateTime? expiration)
        {
            LinkConverter model = new LinkConverter();
            model.LongLink = longLink;
            //check if the longlink entered has at least 8 chars
            if (longLink.Length > 8)
            {
                //capture the first 8 chars
                string validity = longLink.Substring(0, 8);

                //check if it has an https:// || http:// and add one if not
                if (!validity.Contains("http://") || !validity.Contains("https://"))
                {
                    longLink = "http://" + longLink;
                }

                model.LongLink = longLink;
                //add the expiry if it has one
                if (expiration != null)
                {
                    model.ExpirationDate = (DateTime)expiration;
                }

                //allowed characters for the shortlink
                string chars = "abcdefghijklmnopqrstuvwxyz0123456789";

                //to exit the loop when we find a unique character sequence
                bool unique = true;

                string shortlink = "";

                Random rand = new Random();

                do
                {
                    shortlink = "";

                    //loop 7 times, get 7 characters from the allowed characters
                    //easily modified up and down if necessary
                    for (int i = 0; i < 7; i++)
                    {
                        //take our existing shortlink and append another character
                        shortlink = shortlink + chars[rand.Next(chars.Length - 1)];
                    }

                    //make sure there are no existing results matching the link we made
                    unique = db.LinkConverters.Any(x => x.ShortLinkExtension == shortlink);

                } while (unique);

                //add the chars to the model
                model.ShortLinkExtension = shortlink;

                //save to the database
                db.LinkConverters.Add(model);
                db.SaveChanges();

                return RedirectToAction("List","Home");

            }


            return View(model);
        }

        public ActionResult List()
        {
            var data = db.LinkConverters.ToList().OrderByDescending(x => x.ExpirationDate);
            foreach (var item in data)
            {
                item.ShortLinkExtension = "http://" + Request.Url.Authority.ToString() + "/" + item.ShortLinkExtension;
            }

            return View(data);
        }

        public ActionResult FastTravel(string id)
        {
            string longLink = "";
            if (id != null)
            {
                //find out where we should redirect
                longLink = db.LinkConverters.SingleOrDefault(x => x.ShortLinkExtension == id).LongLink;

            }
            else
            {
                longLink = "http://" + Request.Url.Authority.ToString() + "/Home/List";
            }
            return Redirect(longLink);

        }
    }
}