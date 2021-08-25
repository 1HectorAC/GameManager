using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GameManager.Models;
using Microsoft.AspNet.Identity;

namespace GameManager.Controllers
{
    public class GamesController : Controller
    {
        private GameTrackerContext db = new GameTrackerContext();

        /// <summary>
        /// Get List of games by user in database using GET.
        /// </summary>
        /// <returns> A view with a list of games</returns>
        [Authorize]
        public ActionResult Index()
        {
            // Get list of game just by the user.
            var AspNetUserId = User.Identity.GetUserId();
            var games = db.Games.Where(g => g.GameUser.AspNetUserId == AspNetUserId);
            return View(games.ToList());
        }


        /// <summary>
        /// Get details of a Game using GET.
        /// </summary>
        /// <param name="id"> The id of a Game object.</param>
        /// <returns> A view with a passed in Game object.</returns>
        [Authorize]
        public ActionResult Details(int? id)
        {
            // Check if id is null.
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Get game based on id and check if it is null
            Game game = db.Games.Find(id);
            if (game == null)
            {
                return HttpNotFound();
            }

            // Check if current user owns the game entry and redirect if not.
            var AspNetUserId = User.Identity.GetUserId();
            var userId = db.GameUsers.Where(g => g.AspNetUserId == AspNetUserId).First().UserId;
            if (game.UserId != userId)
            {
                return RedirectToAction("Index");
            }

            return View(game);
        }

        /// <summary>
        /// Setup entry for Games using GET.
        /// </summary>
        /// <returns> A view.</returns>
        [Authorize]
        public ActionResult Create()
        {
            // SystemName list for dropdown.
            ViewBag.SystemName = new SelectList(db.GameSystems, "Name", "Name");
            return View();
        }

        /// <summary>
        /// Create entry to Games using POST.
        /// </summary>
        /// <param name="game">A game that will replace the another game.</param>
        /// <returns> A view with a game or redirect to index.</returns>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserId,Title,Price,SystemName,DateOfPurchase,DatePlayed,Borrowed,Physical,Replayed")] Game game)
        {
            //Set the UserId of game to the current users Id.
            var AspNetUserId = User.Identity.GetUserId();
            int userId = db.GameUsers.Where(g => g.AspNetUserId == AspNetUserId).First().UserId;
            game.UserId = userId;

            if (ModelState.IsValid)
            {
                db.Games.Add(game);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // SystemName list for dropdown.
            ViewBag.SystemName = new SelectList(db.GameSystems, "Name", "Name", game.SystemName);

            return View(game);
        }

        /// <summary>
        /// Setup editing a Game entry using GET.
        /// </summary>
        /// <param name="id"> The id of game that will be edited.</param>
        /// <returns> A view with a game.</returns>
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Game game = db.Games.Find(id);

            if (game == null)
            {
                return HttpNotFound();
            }

            // Check if the current user owns the game (has matching userId).
            var AspNetUserId = User.Identity.GetUserId();
            int userId = db.GameUsers.Where(g => g.AspNetUserId == AspNetUserId).First().UserId;
            if (userId != game.UserId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // SystemName list for dropdown.
            ViewBag.SystemName = new SelectList(db.GameSystems, "Name", "Name", game.SystemName);

            return View(game);
        }

        /// <summary>
        /// Edit a game entry using Post.
        /// </summary>
        /// <param name="game"> A Game object that will replace a game.</param>
        /// <returns> A view with a game or redirect to index.</returns>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserId,Title,Price,SystemName,DateOfPurchase,DatePlayed,Borrowed,Physical,Replayed")] Game game)
        {
            //Set the UserId of game to the current users Id.
            var AspNetUserId = User.Identity.GetUserId();
            int userId = db.GameUsers.Where(g => g.AspNetUserId == AspNetUserId).First().UserId;
            game.UserId = userId;

            if (ModelState.IsValid)
            {
                db.Entry(game).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            // SystemName list for dropdown.
            ViewBag.SystemName = new SelectList(db.GameSystems, "Name", "Name", game.SystemName);

            return View(game);
        }

        /// <summary>
        /// Setup deleting a game using GET.
        /// </summary>
        /// <param name="id">The id of the game that will be deleted</param>
        /// <returns>A view with a game.</returns>
        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Game game = db.Games.Find(id);
            if (game == null)
            {
                return HttpNotFound();
            }

            // Check if the current user owns the game (has matching userId).
            var AspNetUserId = User.Identity.GetUserId();
            int userId = db.GameUsers.Where(g => g.AspNetUserId == AspNetUserId).First().UserId;
            if (userId != game.UserId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View(game);
        }

        /// <summary>
        /// Confirm delete a game using POST.
        /// </summary>
        /// <param name="id"> The id of the game that will be deleted</param>
        /// <returns> A redirect to index.</returns>
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Game game = db.Games.Find(id);

            // Check if the current user owns the game (has matching userId).
            var AspNetUserId = User.Identity.GetUserId();
            int userId = db.GameUsers.Where(g => g.AspNetUserId == AspNetUserId).First().UserId;
            if (userId != game.UserId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            db.Games.Remove(game);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Select the year to analyze gaming habits using GET.
        /// </summary>
        /// <returns>A view.</returns>
        [Authorize]
        public ActionResult MoneyAnalysis()
        {
            // Years list setup for dropdown. Years depend on the when(years) the user has played games.
            var AspNetUserId = User.Identity.GetUserId();
            int userId = db.GameUsers.Where(g => g.AspNetUserId == AspNetUserId).First().UserId;
            var yearsList = db.Games.Where(g => g.UserId == 1 && g.DateOfPurchase != null).Select(g => new { Year = g.DateOfPurchase.Value.Year }).Distinct();
            ViewBag.SelectYear = new SelectList(yearsList, "Year", "Year");

            return View();
        }


        /// <summary>
        /// Display analysis of games of given year using POST.
        /// </summary>
        /// <param name="SelectYear"> The year of games to analyze.</param>
        /// <returns> A view with a list of games.</returns>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MoneyAnalysis(int SelectYear)
        {
            ViewBag.Year = SelectYear;
            var AspNetUserId = User.Identity.GetUserId();
            int userId = db.GameUsers.Where(g => g.AspNetUserId == AspNetUserId).First().UserId;

            // Setup ViewBag of total money spent over selected year.
            ViewBag.TotalSpent = db.Games.Where(g => g.UserId == userId && g.DateOfPurchase != null && g.DateOfPurchase.Value.Year == SelectYear).Sum(c => c.Price);

            // Get list of games based off selected year.
            var gamesList = db.Games.Where(g => g.UserId == userId && g.DateOfPurchase != null && g.DateOfPurchase.Value.Year == SelectYear).OrderByDescending(g=>g.DateOfPurchase);

            // Get list of cost by month.
            int[] monthlyCost = {0,0,0,0,0,0,0,0,0,0,0,0};
            foreach (Game g in gamesList.ToList())
            {
                if(g.Price != null)
                    monthlyCost[g.DateOfPurchase.Value.Month -1] += (int)g.Price;
            }

            ViewBag.Months = new string[] { "January", "Feburary", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"};
            ViewBag.MonthlyCost = monthlyCost;

            return View(gamesList);
        }
    }
}
