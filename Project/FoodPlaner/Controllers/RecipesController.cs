﻿using FoodPlaner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodPlaner.Controllers
{
    public class RecipesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Recipes
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Show(int id)
        {
            Recipe recipe = db.Recipes.Find(id);
            return View(recipe);
        }
    }
}