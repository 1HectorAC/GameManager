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

            // Setup ViewBag of total money spent over selected year.
            ViewBag.TotalSpent = gamesListOfYear.Sum(g => g.Price);

            // Setup ViewBag of total games played over selected year.
            ViewBag.TotalGames = gamesListOfYear.Count();

            // Setup View bag of total physical games bought.
            ViewBag.PhysicalTotal = gamesListOfYear.Where(g => g.Physical == true).Count();

            // Setup View bag of total digital games bought.
            ViewBag.DigitalTotal = ViewBag.TotalGames - ViewBag.PhysicalTotal;

            // Get Average amount spent per game.
            ViewBag.AverageSpentPerGame = Math.Round((decimal)(ViewBag.TotalSpent / ViewBag.TotalGames));

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
                if (g.Physical == true)
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
            ViewBag.MonthlyCost = monthlyCost;
            ViewBag.MonthlyCount = monthlyCount;
            ViewBag.PriceRange = priceRange;
            ViewBag.MonthlyPhysicalCount = monthlyPhysicalCount;
            ViewBag.MonthlyDigitalCount = monthlyDigitalCount;


            // Get GameSystem count and names arrays.
            var systemNames = gamesListOfYear.Select(g => g.SystemName).Distinct().ToArray();
            int[] systemCounts = new int[systemNames.Length];
            foreach (string x in gamesListOfYear.Select(g => g.SystemName))
            {
                systemCounts[Array.FindIndex(systemNames, g => g == x)] += 1;
            }
            ViewBag.SystemNames = systemNames;
            ViewBag.SystemCounts = systemCounts;

            // Make selection list for systems currently bought for a drop down.
            ViewBag.SelectSystemName = new SelectList(gamesListOfYear.Select(g => g.SystemName).Distinct());

            ViewBag.Months = new string[] { "January", "Feburary", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
            ViewBag.PriceLabels = new string[] { "<$20", "$20-$29", "$30-$39", "$40-$49", "$50-$59", "$60-$69", "$70-$79", "$80-$89", "$90-$99", ">$100" };

            return View();
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
            gamesList = gamesList.Where(g => g.DatePlayed != null && g.DatePlayed.Value.Year == SelectYear).OrderByDescending(g => g.DatePlayed);

            ViewBag.Count = gamesList.Count();

            // Get count data of Borrowed, Replayed, Physical, and Digital.
            ViewBag.Borrowed = gamesList.Where(g => g.Borrowed == true).Count();
            ViewBag.Replayed = gamesList.Where(g => g.Replayed == true).Count();
            ViewBag.Physical = gamesList.Where(g => g.Physical == true).Count();
            ViewBag.Digital = gamesList.Where(g => g.Physical != true).Count();

            // Get GameSystem count and names arrays.
            var systemNames = gamesList.Select(g => g.SystemName).Distinct().ToArray();
            int[] systemCounts = new int[systemNames.Length];
            foreach (string x in gamesList.Select(g => g.SystemName))
            {
                systemCounts[Array.FindIndex(systemNames, g => g == x)] += 1;
            }
            ViewBag.SystemNames = systemNames;
            ViewBag.SystemCounts = systemCounts;

            // Make selection list for systems currently played for a drop down
            ViewBag.SelectSystemName = new SelectList(gamesList.Select(g => g.SystemName).Distinct());

            int[] monthlyCount = new int[12];
            int[] borrowedMonthlyCount = new int[12];
            int[] replayedMonthlyCount = new int[12];
            int[] physicalMonthlyCount = new int[12];
            int[] digitalMonthlyCount = new int[12];

            // Setup values for monthly data.
            foreach (Game g in gamesList.ToList())
            {
                monthlyCount[g.DatePlayed.Value.Month - 1] += 1;
                if (g.Borrowed == true)
                    borrowedMonthlyCount[g.DatePlayed.Value.Month - 1] += 1;
                if (g.Replayed == true)
                    replayedMonthlyCount[g.DatePlayed.Value.Month - 1] += 1;
                if (g.Physical == true)
                    physicalMonthlyCount[g.DatePlayed.Value.Month - 1] += 1;
                else
                    digitalMonthlyCount[g.DatePlayed.Value.Month - 1] += 1;
            }
            ViewBag.MonthlyCount = monthlyCount;
            ViewBag.BorrowedMonthlyCount = borrowedMonthlyCount;
            ViewBag.ReplayedMonthlyCount = replayedMonthlyCount;
            ViewBag.PhysicalMonthlyCount = physicalMonthlyCount;
            ViewBag.DigitalMonthlyCount = digitalMonthlyCount;
            ViewBag.Months = new string[] { "January", "Feburary", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };


            return View();
        }

        /// <summary>
        /// Get Across Years Analysis view.
        /// </summary>
        /// <returns>A View. </returns>
        [Authorize]
        public ActionResult AcrossYearsAnalysis()
        {
            var option = (Request.QueryString["option"]);
            // Check for current option choice.
            if (option == "Buy" || option == "Play")
            {

                var gamesList = GetUserGamesList(User.Identity.GetUserId());

                // Variable for webpage.
                List<int> yearsList = new List<int>();
                List<string> variableTitles = new List<string>();
                List<List<string>> variableValues = new List<List<string>>();
                List<List<string>> systemTitles = new List<List<string>>();
                List<List<int>> systemValues = new List<List<int>>();


                // Setup Items based on if buy or play option.
                if (option == "Buy")
                {
                    //Check if there are bought games.
                    if (gamesList.Where(g => g.DateOfPurchase != null).FirstOrDefault() == null)
                    {
                        ViewBag.ErrorMessage = "You don't have any bought games.";
                        return View();
                    }

                    // Setup titles for variables.
                    variableTitles = new List<string> { "Total Spent", "Total Bought", "Average Spent/Game", "Physical/Digital" };

                    // Setup data.
                    var data = gamesList.Where(g => g.DateOfPurchase != null && g.Price != null).GroupBy(g => g.DateOfPurchase.Value.Year).Select(g => new
                    {
                        g.Key,
                        totalSpent = g.Sum(g2 => g2.Price),
                        totalBought = g.Count(),
                        avg = Math.Round((decimal)g.Sum(g2 => g2.Price) / g.Count()),
                        totalPhysical = g.Count(g2 => g2.Physical == true)
                    });

                    foreach (var x in data)
                    {
                        yearsList.Add(x.Key);

                        var avg = (int)Math.Round((decimal)(x.totalSpent / x.totalBought));
                        var digital = x.totalBought - x.totalPhysical;

                        variableValues.Add(new List<string> {
                            "$"+x.totalSpent,
                            x.totalBought.ToString(),
                            "$"+ avg,
                            x.totalPhysical+"-"+ digital});
                    };

                    // Setup for system Info.
                    var systemInfo = gamesList.Where(g => g.DateOfPurchase != null).GroupBy(g => g.DateOfPurchase.Value.Year).Select(g => new { g.Key, SystemData = g.GroupBy(g2 => g2.SystemName).Select(g3 => new { g3.Key, total = g3.Count() }) }).Select(x => x.SystemData).ToList();

                    foreach (var x in systemInfo)
                    {
                        List<string> sTitles = new List<string>();
                        List<int> sValue = new List<int>();
                        foreach (var y in x)
                        {
                            sTitles.Add(y.Key);
                            sValue.Add(y.total);
                        }
                        systemTitles.Add(sTitles);
                        systemValues.Add(sValue);
                    };

                }
                else
                {
                    //Check if there are bought games.
                    if (gamesList.Where(g => g.DatePlayed != null).FirstOrDefault() == null)
                    {
                        ViewBag.ErrorMessage = "You don't have any played games.";
                        return View();
                    }

                    // Setup titles for variables.
                    variableTitles = new List<string> { "Total Played", "Total Borrowed", " Total Replayed", "Physical/Digital" };

                    // Setup data.
                    var data = gamesList.Where(g => g.DatePlayed != null).GroupBy(g => g.DatePlayed.Value.Year).Select(g => new
                    {
                        g.Key,
                        totalPlayed = g.Count(),
                        totalBorrowed = g.Count(g2 => g2.Borrowed == true),
                        totalReplayed = g.Count(g2 => g2.Replayed == true),
                        totalPhysical = g.Count(g2 => g2.Physical == true)
                    });

                    foreach (var x in data)
                    {
                        yearsList.Add(x.Key);

                        var digital = x.totalPlayed - x.totalPhysical;

                        variableValues.Add(new List<string> {
                            x.totalPlayed.ToString(),
                            x.totalBorrowed.ToString(),
                            x.totalReplayed.ToString(),
                            x.totalPhysical+"-"+ digital});

                    }

                    // Setup for system Info.
                    var systemInfo = gamesList.Where(g => g.DatePlayed != null).GroupBy(g => g.DatePlayed.Value.Year).Select(g => new { g.Key, SystemData = g.GroupBy(g2 => g2.SystemName).Select(g3 => new { g3.Key, total = g3.Count() }) }).Select(x => x.SystemData).ToList();

                    foreach (var x in systemInfo)
                    {
                        List<string> sTitles = new List<string>();
                        List<int> sValue = new List<int>();
                        foreach (var y in x)
                        {
                            sTitles.Add(y.Key);
                            sValue.Add(y.total);
                        }
                        systemTitles.Add(sTitles);
                        systemValues.Add(sValue);
                    };
                }

                // Setup viewbag variables.
                ViewBag.Option = option;
                ViewBag.YearsList = yearsList;
                ViewBag.VariableTitles = variableTitles;
                ViewBag.VariableValues = variableValues;
                ViewBag.SystemTitles = systemTitles;
                ViewBag.SystemValues = systemValues;
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
