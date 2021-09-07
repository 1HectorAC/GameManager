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
        /// Select the year to analyze spending habits using GET.
        /// </summary>
        /// <returns>A view.</returns>
        [Authorize]
        public ActionResult MoneyAnalysis()
        {
            // Years list setup for dropdown. Years depend on the when(years) the user has played games.
            var gamesList = GetUserGamesList(User.Identity.GetUserId());
            var yearsList = gamesList.Where(g => g.DateOfPurchase != null).Select(g => g.DateOfPurchase.Value.Year ).Distinct();
            ViewBag.SelectYear = new SelectList(yearsList);

            return View();
        }


        /// <summary>
        /// Display spending analysis of games using POST.
        /// </summary>
        /// <param name="SelectYear"> The year of games to analyze.</param>
        /// <returns> A view with a list of games.</returns>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MoneyAnalysis(int SelectYear)
        {
            ViewBag.Year = SelectYear;
            var gamesList = GetUserGamesList(User.Identity.GetUserId());
          
            // Get list of games based off selected year.
            var gamesListOfYear = gamesList.Where(g => g.DateOfPurchase != null && g.Price != null && g.DateOfPurchase.Value.Year == SelectYear).OrderByDescending(g => g.DateOfPurchase);

            // Setup ViewBag of total money spent over selected year.
            ViewBag.TotalSpent = gamesListOfYear.Sum(g => g.Price);

            // Setup ViewBag of total games played over selected year.
            ViewBag.TotalGames = gamesListOfYear.Count();

            // Get Average amount spent per game.
            ViewBag.AverageSpentPerGame = Math.Round((decimal) (ViewBag.TotalSpent / ViewBag.TotalGames));

            int[] monthlyCost = new int[12];
            int[] monthlyCount = new int[12];
            int[] priceRange = new int[10];
            int index = 0;
            // Setup values for montlyCost, montlyCount, and priceRange
            foreach (Game g in gamesListOfYear.ToList())
            {
                monthlyCost[g.DateOfPurchase.Value.Month - 1] += (int)g.Price;
                monthlyCount[g.DateOfPurchase.Value.Month - 1] += 1;
                
                decimal gameCost = g.Price.Value;
                if (gameCost < 20)
                    priceRange[0]++;
                else if (gameCost >= 100)
                    priceRange[9]++;
                else
                    priceRange[(int)Math.Floor(gameCost/10)-1]++;
                index++;
            }
            ViewBag.MonthlyCost = monthlyCost;
            ViewBag.MonthlyCount = monthlyCount;
            ViewBag.PriceRange = priceRange;


            ViewBag.Months = new string[] { "January", "Feburary", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            ViewBag.PriceLabels = new string[] { "<$20", "$20-$29", "$30-$39", "$40-$49", "$50-$59", "$60-$69", "$70-$79", "$80-$89", "$90-$99", ">$100" };

            return View(gamesListOfYear);
        }

        /// <summary>
        /// Select the year to analyze gaming habits using GET.
        /// </summary>
        /// <returns>A view.</returns>
        [Authorize]
        public ActionResult TypeAnalysis()
        {
            // Years list setup for dropdown. Years depend on the when(years) the user has played games.
            var gamesList = GetUserGamesList(User.Identity.GetUserId());
            var yearsList = gamesList.Where(g => g.DatePlayed != null).Select(g => new { Year = g.DatePlayed.Value.Year }).Distinct();
            ViewBag.SelectYear = new SelectList(yearsList, "Year", "Year");

            // SystemName list for dropdown.
            ViewBag.SystemName = new SelectList(db.GameSystems, "Name", "Name");

            return View();
        }

        /// <summary>
        /// Display type analysis of games using POST.
        /// </summary>
        /// <param name="SelectYear"> The year of games to analyze.</param>
        /// <returns> A view with a list of games.</returns>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TypeAnalysis(int SelectYear)
        {
            ViewBag.Year = SelectYear;

            // Get list of games based off selected year.
            var gamesList = GetUserGamesList(User.Identity.GetUserId());
            gamesList = gamesList.Where(g=>g.DatePlayed != null && g.DatePlayed.Value.Year == SelectYear).OrderByDescending(g => g.DatePlayed);

            ViewBag.Count = gamesList.Count();

            // Get count data of Borrowed, Replayed, Physical, and Digital.
            ViewBag.Borrowed = gamesList.Where(g => g.Borrowed == true).Count();
            ViewBag.Replayed = gamesList.Where(g => g.Replayed == true).Count();
            ViewBag.Physical = gamesList.Where(g => g.Physical == true).Count();
            ViewBag.Digital = gamesList.Where(g => g.Physical != true).Count();

            // Get GameSystem count and names arrays.
            var systemNames = gamesList.Select(g=>g.SystemName).Distinct().ToArray();
            int[] systemCounts = new int[systemNames.Length];
            foreach(string x in gamesList.Select(g => g.SystemName))
            {
                systemCounts[Array.FindIndex(systemNames,g => g == x) ] += 1;
            }
            ViewBag.SystemNames = systemNames;
            ViewBag.SystemCounts = systemCounts;

            // Make selection list for systems currently played for a drop down
            ViewBag.SelectSystemName = new SelectList(gamesList.Select(g => g.SystemName).Distinct());

            return View();
        }

        /// <summary>
        /// Get Some Game variables for a specific type of games.
        /// </summary>
        /// <param name="Year"> Only games of the given year will be returned.</param>
        /// <param name="Type"> The type of games to filter. </param>
        /// <param name="SelectSystemName"> The systemName that will be filted if needed.</param>
        /// <returns> A JsonResult with games data. </returns>
        [Authorize]
        [HttpPost]
        public JsonResult GameTypeList(int Year, int Type, string SelectSystemName)
        {
            // Get list of games based off selected year.
            var gamesList = GetUserGamesList(User.Identity.GetUserId());
            gamesList = gamesList.Where(g => g.DatePlayed != null && g.DatePlayed.Value.Year == Year).OrderByDescending(g => g.DatePlayed);

            var title = "";

            // Filter gamesList based on selected type.
            if (Type == 1)
            {
                title = "Borrowed";
                gamesList = gamesList.Where(g => g.Borrowed == true);
            }
            if (Type == 2)
            {
                title = "Replayed";
                gamesList = gamesList.Where(g => g.Replayed == true);
            }
            if (Type == 3)
            {
                title = "Physical Copy";
                gamesList = gamesList.Where(g => g.Physical == true);
            }
            if (Type == 4)
            {
                title = "Digital Copy";
                gamesList = gamesList.Where(g => g.Physical != true);
            }
            if (Type == 5)
            {
                title = "Console(" + SelectSystemName + ")";
                gamesList = gamesList.Where(g => g.SystemName == SelectSystemName);
            }

            var data = new {
                Title = title,
                TitleList = gamesList.Select(g => g.Title).ToArray(),
                DateList = gamesList.AsEnumerable().Select(g => g.DatePlayed.Value.ToString("MM/dd/yy")).ToArray(),
                SystemList = gamesList.Select(g => g.SystemName).ToArray()
            };
            
            return Json(data, JsonRequestBehavior.AllowGet);
        }

            /// <summary>
            /// Function used to get a users games list.
            /// </summary>
            /// <param name="AspNetUserId">An id connected to a user. </param>
            /// <returns>A list of games.</returns>
            private IQueryable<Game> GetUserGamesList(string AspNetUserId)
        {
            int userId = db.GameUsers.Where(g => g.AspNetUserId == AspNetUserId).First().UserId;
            return db.Games.Where(g => g.UserId == userId);
        }
        
    }
}
