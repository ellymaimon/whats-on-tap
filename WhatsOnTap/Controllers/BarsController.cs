using System.Collections.Generic;
using System.Linq;
using System;
using Microsoft.AspNetCore.Mvc;
using WhatsOnTap.Models;
using WhatsOnTap.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WhatsOnTap.Controllers
{
    public class BarsController : Controller
    {
        private readonly WhatsOnTapContext _db;

        public BarsController(WhatsOnTapContext context)
        {
            _db = context;
        }

        [HttpGet("/bars")]
        public ActionResult Index() => View(_db.Bars.ToList());

        [HttpGet("bars/{id}")]
        public ActionResult Details(int id)
        {
            BarDetailsViewModel viewModel = new BarDetailsViewModel(_db, id);
            viewModel.FindBarBeers(id);
            return View(viewModel);
        }

        [HttpGet("/bars/{id}/edit")]
        public ActionResult Edit(int id)
        {
            BarDetailsViewModel viewModel = new BarDetailsViewModel(_db, id);
            viewModel.FindAllBeers();
            return View(viewModel);
        }

        [HttpPost("/bars/{id}/edit")]
        public ActionResult Edit(List<int> BeerId, Bar bar, int id)
        {
          _db.Entry(bar).State = EntityState.Modified;
          BarDetailsViewModel viewModel = new BarDetailsViewModel(_db, id);
          var barsToRemove = _db.Taplists.Where(entry => entry.BarId == id).ToList();
          foreach (var barz in barsToRemove)
          {
            if (barz != null)
            {
              _db.Taplists.Remove(barz);
            }
          }

          foreach (int beerId in BeerId)
          {
              Taplist newTaplist = new Taplist(beerId, viewModel.CurrentBar.BarId);
              _db.Taplists.Add(newTaplist);
          }
          _db.SaveChanges();

          return RedirectToAction("Index");
        }

        [HttpGet("/bars/new")]
        public ActionResult Create() => View(_db.Beers.ToList());

        [HttpPost("/bars/new")]
        public ActionResult Create(List<int> BeerId, Bar bar)
        {
            _db.Add(bar);
            foreach (int beerId in BeerId)
            {
                Taplist newTaplist = new Taplist(beerId, bar.BarId);
                _db.Taplists.Add(newTaplist);
            }
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet("/bars/{id}/delete")]
        public ActionResult Delete(int id)
        {
            Bar bar = _db.Bars.FirstOrDefault(bars => bars.BarId == id);
            Taplist joinEntry = _db.Taplists.FirstOrDefault(entry => entry.BarId == id);
            if (joinEntry != null)
            {
              _db.Taplists.Remove(joinEntry);
            }
            _db.Bars.Remove(bar);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
