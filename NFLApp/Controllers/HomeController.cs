using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NFLApp.Models;
using System.Diagnostics;
/*
namespace NFLApp.Controllers
{
    public class HomeController : Controller
    {
        *//*private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }*//*

        private TeamContext context;

        public HomeController(TeamContext ctx)
        {
            context = ctx;
        }

        *//*public ViewResult Index(string activeConf = "all", string activeDiv = "all") //thses params are the default
        {
            // store selected conference and division IDs in view bag
            ViewBag.ActiveConf = activeConf;
            ViewBag.ActiveDiv = activeDiv;

            // store conferences and divisions from database in view bag
            //real divs and confs which are in db
            ViewBag.Conferences = context.Conferences.ToList(); 
            ViewBag.Divisions = context.Divisions.ToList();

            // get teams - filter by conference and division
            IQueryable<Team> query = context.Teams.OrderBy(t => t.Name); //bring all teams from db (using lambda function)
            //IQueryable - query that can write to the database

            if (activeConf != "all") //which would mean you've selected a specific conf, same for div
                query = query.Where(
                    t => t.Conference.ConferenceID.ToLower() ==
                         activeConf.ToLower()); //select all that are selected by user
            //wether the "all" is selected or a speicifc one is selected
            if (activeDiv != "all")
                query = query.Where(
                    t => t.Division.DivisionID.ToLower() ==
                         activeDiv.ToLower());
            //query is now ready with either all divs or confs or All divs or confs!! wawaweewa!!

            // pass list of teams to view as model
            var teams = query.ToList();

            return View(teams);
        }*//*

        public ViewResult Index(TeamsViewModel model)
        {
            //var session = new NFLSession(HttpContext.Session);

            // store selected conference and division in session
            var session = new NFLSession(HttpContext.Session);
            session.SetActiveConf(model.ActiveConf);
            session.SetActiveDiv(model.ActiveDiv);

            // if no count in session, get cookie and restore fave teams 
            int? count = session.GetMyTeamCount();

            if (!count.HasValue)
            {
                var cookies = new NFLCookies(Request.Cookies);
                string[] ids = cookies.GetMyTeamIds();

                model.Conferences = context.Conferences.ToList();
                model.Divisions = context.Divisions.ToList();

                IQueryable<Team> query = context.Teams.OrderBy(t => t.Name);
                if (model.ActiveConf != "all")
                    query = query.Where(t =>
                        t.Conference.ConferenceID.ToLower() ==
                             model.ActiveConf.ToLower());
                if (model.ActiveDiv != "all")
                    query = query.Where(t =>
                        t.Division.DivisionID.ToLower() ==
                             model.ActiveDiv.ToLower());
                model.Teams = query.ToList();

                return View(model);
            }
        }

            [HttpGet]
            public IActionResult Details(string id)
            {
                // get selected conference and division from session
                // and pass them to the view in the view model
                var session = new NFLSession(HttpContext.Session);

                var model = new TeamsViewModel
                {
                    Team = context.Teams
                                .Include(t => t.Conference)
                                .Include(t => t.Division)
                                .FirstOrDefault(t => t.TeamID == id) ?? new Team(),
                    ActiveConf = session.GetActiveConf(),
                    ActiveDiv = session.GetActiveDiv()
                };

                return View(model);
            }

            public IActionResult Privacy()
            {
                return View();
            }

            [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
            public IActionResult Error()
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
    }

//TempData has a longer life span, ViewData and ViewBag info disappears after you change the page
//TempData is used with cookies so that it can travel between pages, multiple lifespans
*/

//TempData has a longer life span, ViewData and ViewBag info disappears after you change the page
//TampeData is used with cookies so that it can travel between pages, multiple lifespans

namespace NFL_APP.Controllers
{
    public class HomeController : Controller
    {
        /*private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }*/

        private TeamContext context;
        public HomeController(TeamContext ctx)
        {
            context = ctx;
        }

        public IActionResult Index(string activeConf = "all", string activeDiv = "all")
        {
            var model = new TeamsViewModel
            {
                ActiveConf = activeConf,
                ActiveDiv = activeDiv,
                Conferences = context.Conferences.ToList(),
                Divisions = context.Divisions.ToList()
            };

            //var session = new NFLSession(HttpContext.Session);
            var session = new NFLSession(HttpContext.Session);
            session.SetActiveConf(model.ActiveConf);
            session.SetActiveDiv(model.ActiveDiv);

            // if no count in session, get cookie and restore fave teams
            int? count = session.GetMyTeamCount();
            if (!count.HasValue)
            {
                var cookies = new NFLCookies(Request.Cookies);
                string[] ids = cookies.GetMyTeamsIds();

                if (ids.Length > 0)
                {
                    var myteams = context.Teams.Include(t => t.Conference)
                    .Include(t => t.Division)
                    .Where(t => ids.Contains(t.TeamID)).ToList();
                    session.SetMyTeams(myteams);
                }
            }

            model.Conferences = context.Conferences.ToList();
            model.Divisions = context.Divisions.ToList();

            IQueryable<Team> query = context.Teams.OrderBy(t => t.Name);
            if (model.ActiveConf != "all")
                query = query.Where(t =>
                    t.Conference.ConferenceID.ToLower() ==
                         model.ActiveConf.ToLower());
            if (model.ActiveDiv != "all")
                query = query.Where(t =>
                    t.Division.DivisionID.ToLower() ==
                         model.ActiveDiv.ToLower());
            model.Teams = query.ToList();
            return View(model);
        }

        public IActionResult Details(string id)
        {
            var session = new NFLSession(HttpContext.Session);
            var model = new TeamsViewModel
            {
                Team = context.Teams
                .Include(t => t.Conference)
                .Include(t => t.Division)
                .FirstOrDefault(t => t.TeamID == id) ?? new Team(),
                ActiveConf = session.GetActiveConf(),
                ActiveDiv = session.GetActiveDiv()
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
