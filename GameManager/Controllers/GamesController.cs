﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GameManager.Models;
using GameManager.Models.ViewModels;
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
            var games = db.Games.Where(g => g.GameUser.AspNetUserId == AspNetUserId).OrderBy(g => g.Title);
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

            // Error checks.
            if (game.DateOfPurchase == null && game.DatePlayed == null)
            {
                ViewBag.ErrorMessage = "One date field is required.";
            }
            else if ((game.DateOfPurchase != null && game.Price == null) || (game.DateOfPurchase == null && game.Price != null))
            {
                ViewBag.ErrorMessage = "You can't have a date bought with no price and vice versa.";
            }
            else if (game.DateOfPurchase != null && game.Borrowed == true)
            {
                ViewBag.ErrorMessage = "A game can't have a bought date and be borrowed.";
            }
            else if (ModelState.IsValid)
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

            // Error checks.
            if (game.DateOfPurchase == null && game.DatePlayed == null)
            {
                ViewBag.ErrorMessage = "One date field is required.";
            }
            else if ((game.DateOfPurchase != null && game.Price == null) || (game.DateOfPurchase == null && game.Price != null))
            {
                ViewBag.ErrorMessage = "You can't have a date bought with no price and vice versa.";
            }
            else if (game.DateOfPurchase != null && game.Borrowed == true)
            {
                ViewBag.ErrorMessage = "A game can't have a bought date and be borrowed.";
            }
            else if (ModelState.IsValid)
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
            var yearsList = gamesList.Where(g => g.DateOfPurchase != null).Select(g => g.DateOfPurchase.Value.Year).Distinct();

            // yearsList length check.
            if (yearsList.Count() >= 1)
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
        public ActionResult MoneyAnalysis(int SelectYear = 0)
        {
            if (SelectYear <= 0)
                return View();
            ViewBag.Year = SelectYear;

            // Get list of games based off selected year.
            var gamesList = GetUserGamesList(User.Identity.GetUserId());
            var gamesListOfYear = gamesList.Where(g => g.DateOfPurchase != null && g.Price != null && g.DateOfPurchase.Value.Year == SelectYear).OrderByDescending(g => g.DateOfPurchase);

            // Make variables for buyData fields.
            var totalSpent = (int)gamesListOfYear.Sum(g => g.Price);
            var totalGames = gamesListOfYear.Count();
            var systemInfo = gamesListOfYear.GroupBy(c => c.SystemName);
            int[] monthlyCost = new int[12];
            int[] monthlyCount = new int[12];
            int[] monthlyPhysicalCount = new int[12];
            int[] monthlyDigitalCount = new int[12];
            int[] priceRange = new int[10];
            int index = 0;

            // Setup values for montlyCost, montlyCount, and priceRange
            foreach (Game g in gamesListOfYear.ToList())
            {
                // Increment cost and count.
                monthlyCost[g.DateOfPurchase.Value.Month - 1] += (int)g.Price;
                monthlyCount[g.DateOfPurchase.Value.Month - 1] += 1;

                // Increment montlyPhysicalCount or monthlyDigitalCount.
                if (g.Physical)
                    monthlyPhysicalCount[g.DateOfPurchase.Value.Month - 1] += 1;
                else
                    monthlyDigitalCount[g.DateOfPurchase.Value.Month - 1] += 1;

                // Increment for price range.
                decimal gameCost = g.Price.Value;
                if (gameCost < 20)
                    priceRange[0]++;
                else if (gameCost >= 100)
                    priceRange[9]++;
                else
                    priceRange[(int)Math.Floor(gameCost / 10) - 1]++;
                index++;
            }

            // Setup buyData.
            BuyData buyData = new BuyData
            {
                TotalSpent = totalSpent,
                TotalGames = totalGames,
                AverageSpend = (int)Math.Round((decimal)(totalSpent / totalGames)),
                TotalPhysical = gamesListOfYear.Where(g => g.Physical == true).Count(),
                TotalDigital = gamesListOfYear.Where(g => g.Physical == false).Count(),
                MonthlyTotalSpent = monthlyCost,
                MonthlyTotalGames = monthlyCount,
                MonthlyTotalPhysical = monthlyPhysicalCount,
                MonthlyTotalDigital = monthlyDigitalCount,
                TotalByPriceRange = priceRange,
                SystemNames = systemInfo.Select(c => c.Key).ToArray(),
                SystemCounts = systemInfo.Select(c => c.Count()).ToArray()
            };

            // Make selection list for systems currently bought for a drop down.
            ViewBag.SelectSystemName = new SelectList(gamesListOfYear.Select(g => g.SystemName).Distinct());

            return View(buyData);
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
            var yearsList = gamesList.Where(g => g.DatePlayed != null).Select(g => g.DatePlayed.Value.Year).Distinct();

            // yearsList length check.
            if (yearsList.Count() >= 1)
                ViewBag.SelectYear = new SelectList(yearsList);

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
        public ActionResult TypeAnalysis(int SelectYear = 0)
        {
            if (SelectYear < 1)
                return View();
            ViewBag.Year = SelectYear;

            // Get list of games based off selected year.
            var gamesList = GetUserGamesList(User.Identity.GetUserId());
            var gamesListOfYear = gamesList.Where(g => g.DatePlayed != null && g.DatePlayed.Value.Year == SelectYear).OrderByDescending(g => g.DatePlayed);

            // Make variables for buyData fields.
            var systemInfo = gamesListOfYear.GroupBy(c => c.SystemName);
            int[] monthlyTotalPlayed = new int[12];
            int[] monthlyTotalBorrowed = new int[12];
            int[] monthlyTotalReplayed = new int[12];
            int[] monthlyTotalPhysical = new int[12];
            int[] monthlyTotalDigital = new int[12];

            // Setup values for monthly data.
            foreach (Game g in gamesListOfYear.ToList())
            {
                monthlyTotalPlayed[g.DatePlayed.Value.Month - 1] += 1;
                if (g.Borrowed)
                    monthlyTotalBorrowed[g.DatePlayed.Value.Month - 1] += 1;
                if (g.Replayed)
                    monthlyTotalReplayed[g.DatePlayed.Value.Month - 1] += 1;
                if (g.Physical)
                    monthlyTotalPhysical[g.DatePlayed.Value.Month - 1] += 1;
                else
                    monthlyTotalDigital[g.DatePlayed.Value.Month - 1] += 1;
            }

            var playData = new PlayData
            {
                TotalGames = gamesListOfYear.Count(),
                TotalBorrowed = gamesListOfYear.Where(g => g.Borrowed == true).Count(),
                TotalReplayed = gamesListOfYear.Where(g => g.Replayed == true).Count(),
                TotalPhysical = gamesListOfYear.Where(g => g.Physical == true).Count(),
                TotalDigital = gamesListOfYear.Where(g => g.Physical == false).Count(),
                MonthlyTotalPlayed = monthlyTotalPlayed,
                MonthlyTotalBorrowed = monthlyTotalBorrowed,
                MonthlyTotalReplayed = monthlyTotalReplayed,
                MonthlyTotalPhysical = monthlyTotalPhysical,
                MonthlyTotalDigital = monthlyTotalDigital,
                SystemNames = systemInfo.Select(c => c.Key).ToArray(),
                SystemCounts = systemInfo.Select(c => c.Count()).ToArray()
            };

            // Make selection list for systems currently bought for a drop down.
            ViewBag.SelectSystemName = new SelectList(gamesListOfYear.Select(g => g.SystemName).Distinct());

            return View(playData);
        }

        /// <summary>
        /// Get Across Years Analysis view.
        /// </summary>
        /// <returns>A View. </returns>
        [Authorize]
        public ActionResult AcrossYearsAnalysis()
        {
            var option = (Request.QueryString["option"]);
            var gamesList = GetUserGamesList(User.Identity.GetUserId());

            // Data that will be diplayed. It will change a little depending on option selected.
            AcrossYearsGameData acrossYearsGameData;

            if (option == "Buy")
            {
                // Get user data organized by year.
                var byYearGameData = gamesList.Where(c => c.DateOfPurchase != null).GroupBy(c => c.DateOfPurchase.Value.Year);

                // Check if there are bought games.
                if (byYearGameData.Count() <= 0)
                {
                    ViewBag.ErrorMessage = "You don't have any bought games.";
                    return View();
                }

                var systemInfo = byYearGameData.Select(c => c.GroupBy(c2 => c2.SystemName));

                acrossYearsGameData = new AcrossYearsGameData
                {
                    Years = byYearGameData.Select(c => c.Key).ToArray(),
                    YearlyTotalGames = byYearGameData.Select(c => c.Count()).ToArray(),
                    YearlyTotalSpent = byYearGameData.Select(c => c.Sum(c2 => c2.Price)).ToArray(),
                    YearlyAverageSpent = byYearGameData.Select(c => c.Sum(c2 => c2.Price) / c.Count()).ToArray(),
                    YearlyTotalPhysical = byYearGameData.Select(c => c.Count(c2 => c2.Physical)).ToArray(),
                    YearlyTotalDigital = byYearGameData.Select(c => c.Count(c2 => !c2.Physical)).ToArray(),
                    YearlySystemNames = systemInfo.Select(c => c.Select(c2 => c2.Key).ToList()).ToList(),
                    YearlySystemCounts = systemInfo.Select(c => c.Select(c2 => c2.Count()).ToList()).ToList()
                };
                ViewBag.Option = option;
                return View(acrossYearsGameData);
            }
            else if (option == "Play")
            {
                // Get user data organized by year.
                var byYearGameData = gamesList.Where(c => c.DatePlayed != null).GroupBy(c => c.DatePlayed.Value.Year);

                // Check if there are played games.
                if (byYearGameData.Count() <= 0)
                {
                    ViewBag.ErrorMessage = "You don't have any played games.";
                    return View();
                }

                var systemInfo = byYearGameData.Select(c => c.GroupBy(c2 => c2.SystemName));

                acrossYearsGameData = new AcrossYearsGameData
                {
                    Years = byYearGameData.Select(c => c.Key).ToArray(),
                    YearlyTotalGames = byYearGameData.Select(c => c.Count()).ToArray(),
                    YearlyTotalBorrowed = byYearGameData.Select(c => c.Count(c2 => c2.Borrowed)).ToArray(),
                    YearlyTotalReplayed = byYearGameData.Select(c => c.Count(c2 => c2.Replayed)).ToArray(),
                    YearlyTotalPhysical = byYearGameData.Select(c => c.Count(c2 => c2.Physical)).ToArray(),
                    YearlyTotalDigital = byYearGameData.Select(c => c.Count(c2 => !c2.Physical)).ToArray(),
                    YearlySystemNames = systemInfo.Select(c => c.Select(c2 => c2.Key).ToList()).ToList(),
                    YearlySystemCounts = systemInfo.Select(c => c.Select(c2 => c2.Count()).ToList()).ToList()
                };
                ViewBag.Option = option;
                return View(acrossYearsGameData);
            }
            return View();
        }

        /// <summary>
        /// Get a filtered games list.
        /// </summary>
        /// <param name="Year"> Only games of the given year will be returned.</param>
        ///  <param name="BuyCheck"> Check if looking at bought games or played.</param>
        /// <param name="Type"> The type of games to filter. </param>
        /// <param name="SelectSystemName"> The systemName that will be filted if needed.</param>
        /// <returns> A JsonResult with games data. </returns>
        [Authorize]
        [HttpPost]
        public JsonResult GameTypeList(int Year, bool BuyCheck, string Type, string SelectSystemName)
        {
            // Get list of games based off selected year.
            var gamesList = GetUserGamesList(User.Identity.GetUserId());
            List<int?> priceList;
            List<string> dateList;
            if (BuyCheck == true)
            {
                gamesList = gamesList.Where(g => g.DateOfPurchase != null && g.DateOfPurchase.Value.Year == Year).OrderByDescending(g => g.DateOfPurchase);
                dateList = gamesList.AsEnumerable().Select(g => g.DateOfPurchase.Value.ToString("MM/dd/yy")).ToList();
            }
            else
            {
                gamesList = gamesList.Where(g => g.DatePlayed != null && g.DatePlayed.Value.Year == Year).OrderByDescending(g => g.DatePlayed);
                dateList = gamesList.AsEnumerable().Select(g => g.DatePlayed.Value.ToString("MM/dd/yy")).ToList();
            }

            var title = "";

            // Filter gamesList based on selected type.
            if (Type == "Borrowed")
            {
                title = "Borrowed";
                gamesList = gamesList.Where(g => g.Borrowed == true);
            }
            else if (Type == "Replayed")
            {
                title = "Replayed";
                gamesList = gamesList.Where(g => g.Replayed == true);
            }
            else if (Type == "Physical")
            {
                title = "Physical Copy";
                gamesList = gamesList.Where(g => g.Physical == true);
            }
            else if (Type == "Digital")
            {
                title = "Digital Copy";
                gamesList = gamesList.Where(g => g.Physical != true);
            }
            else if (Type == "Console")
            {
                title = "Console(" + SelectSystemName + ")";
                gamesList = gamesList.Where(g => g.SystemName == SelectSystemName);
            }
            else
            {
                title = "All Games";
            }

            // Add to priceList if actually using bought games.
            if (BuyCheck)
                priceList = gamesList.Select(g => g.Price).ToList();
            else
                priceList = new List<int?> { };

            var data = new
            {
                Title = title,
                TitleList = gamesList.Select(g => g.Title).ToList(),
                DateList = dateList,
                SystemList = gamesList.Select(g => g.SystemName).ToList(),
                PriceList = priceList
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
