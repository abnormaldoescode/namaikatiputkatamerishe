using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class CarController : Controller
    {
        // GET: Car
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        //GET: Car/List
        public ActionResult List()
        {
            using (var database = new WebApplication1DbContext())
            {
                //Get cars from database
                var cars = database.Cars
                    .Include(a => a.Author)
                    .ToList();

                return View(cars);
            }
        }
        //GET: Car/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            using (var database = new WebApplication1DbContext())
            {
                //Get the car from database

                var car = database.Cars
                    .Where(a => a.Id == id)
                    .Include(a => a.Author)
                    .First();

            if (car == null)
                {
                    return HttpNotFound();
                }

                return View(car);
            }
        }
        //POST: Car/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(Car car)
        {
            if(ModelState.IsValid)
            {
                using (var database = new WebApplication1DbContext())
                {
                    //Get author id
                    var authorId = database.Users
                        .Where(u => u.UserName == this.User.Identity.Name)
                        .First()
                        .Id;

                    //Set cars author
                    car.AuthorId = authorId;

                    //Save car in DB
                    database.Cars.Add(car);
                    database.SaveChanges();

                    return RedirectToAction("Index");
                }
            }

            return View(car);
        }

        //GET: Car/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }
        //
        //GET: Car/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new WebApplication1DbContext())
            {
                //Get car from database
                var car = database.Cars
                    .Where(a => a.Id == id)
                    .Include(a => a.Author)
                    .First();

                if (!IsUserAutorizedToEdit(car))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }

                //Check if car exists
                if (car == null)
                {
                    return HttpNotFound();
                }

                //Pass car to view
                return View(car);
            }
        }

        //POST: Car/Delete
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new WebApplication1DbContext())
            {
                //Get car from database
                var car = database.Cars
                    .Where(a => a.Id == id)
                    .Include(a => a.Author)
                    .First();

                //Check if car exists
                if (car == null)
                {
                    return HttpNotFound();
                }

                //Remove car from db
                database.Cars.Remove(car);
                database.SaveChanges();

                //Redirect to index page
                return RedirectToAction("Index");
            }
        }

        //GET: Article/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new WebApplication1DbContext())
            {
                //Get car from the database
                var car = database.Cars
                    .Where(a => a.Id == id)
                    .First();

                //Check if car exists
                if (car == null)
                {
                    return HttpNotFound();
                }

                //Create the view model
                var model = new CarViewModel();
                model.Id = car.Id;
                model.Title = car.Title;
                model.Content = car.Content;

                //Pass the view model to view
                return View(model);
            }
        }

        //Post: Car/Edit
        [HttpPost]
        public ActionResult Edit(CarViewModel model)
        {
            //Check if model state is valid
            if (ModelState.IsValid)
            {
                using (var database = new WebApplication1DbContext())
                {
                    //Get car from database
                    var car = database.Cars
                        .FirstOrDefault(a => a.Id == model.Id);

                    //Set car properties
                    car.Title = model.Title;
                    car.Content = model.Content;

                    //Save car state in database
                    database.Entry(car).State = EntityState.Modified;
                    database.SaveChanges();

                    //Redirect to the index page
                    return RedirectToAction("Index");

                }
            }
            //If model state is invalid, return the same view
            return View(model);

        }

        private bool IsUserAutorizedToEdit(Car car)
        {
            bool isAdmin = this.User.IsInRole("Admin");
            bool isAuthor = car.IsAuthor(this.User.Identity.Name);

            return isAdmin || isAuthor;
        }
    }
}