using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NetBots.WebServer.Data.MsSql;
using NetBots.WebServer.Model;

namespace NetBotsHostProject.Controllers
{
    public class PlayerBotsController : Controller
    {
        private NetBotsDbContext db = new NetBotsDbContext();

        // GET: PlayerBots
        public ActionResult Index()
        {
            return View(db.PlayerBots.ToList());
        }

        // GET: PlayerBots/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlayerBot playerBot = db.PlayerBots.Find(id);
            if (playerBot == null)
            {
                return HttpNotFound();
            }
            return View(playerBot);
        }

        // GET: PlayerBots/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PlayerBots/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Owner,URL")] PlayerBot playerBot)
        {
            if (ModelState.IsValid)
            {
                db.PlayerBots.Add(playerBot);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(playerBot);
        }

        // GET: PlayerBots/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlayerBot playerBot = db.PlayerBots.Find(id);
            if (playerBot == null)
            {
                return HttpNotFound();
            }
            return View(playerBot);
        }

        // POST: PlayerBots/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Owner,URL")] PlayerBot playerBot)
        {
            if (ModelState.IsValid)
            {
                db.Entry(playerBot).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(playerBot);
        }

        // GET: PlayerBots/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlayerBot playerBot = db.PlayerBots.Find(id);
            if (playerBot == null)
            {
                return HttpNotFound();
            }
            return View(playerBot);
        }

        // POST: PlayerBots/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PlayerBot playerBot = db.PlayerBots.Find(id);
            db.PlayerBots.Remove(playerBot);
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
    }
}
