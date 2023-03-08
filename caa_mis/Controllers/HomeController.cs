﻿using caa_mis.Data;
using caa_mis.Models;
using caa_mis.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using SQLitePCL;
using System.Collections.Immutable;
using System.Diagnostics;

namespace caa_mis.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly InventoryContext _inventoryContext;

        public HomeController(InventoryContext inventoryContext, ILogger<HomeController> logger)
        {
            _logger = logger;
            _inventoryContext = inventoryContext;
        }

        public IActionResult Index()
        {
            //Dashboard information
            var Item = _inventoryContext.Items.Include(s => s.Stocks).ToList();
            var Category = _inventoryContext.Categories.ToList();
            var Stocklist = _inventoryContext.Stocks.Where(s => s.Quantity > 0).ToList();
            var Branch = _inventoryContext.Branches.ToList();

            var tableItems = from item in Item
                             join stock in Stocklist on item.ID equals stock.ItemID
                             group stock by item.Name into t
                             select new
                             {
                                 ItemName = t.Key,
                                 MinLevel = t.Min(s => s.MinLevel),
                                 TotalStock = t.Sum(s => s.Quantity),
                                 Percentage = 1 - ((double)t.Min(s => s.MinLevel)/ (double)t.Sum(s => s.Quantity))
                             };

            ViewBag.TableItem = tableItems.OrderBy(s => s.Percentage).Take(5);

            //PieChart information

            var pieData = from item in Item join category in Category on item.CategoryID equals category.ID
                          join stock in Stocklist on item.ID equals stock.ItemID
                          group new {stock, item} by category into g
                          select new
                          {
                              catName = g.Key.Name,
                              TotalCost = g.Sum(x => x.stock.Quantity * x.item.Cost)
                          };

            var pieLabel = new List<string>();
            var pieValue = new List<decimal>();

            foreach(var item in pieData)
            {
                
                pieLabel.Add(item.catName);
                pieValue.Add(item.TotalCost);
            }
            ViewBag.PieLabel = pieLabel;
            ViewBag.PieValue = pieValue;

            //Bar Graph information
            var barData = from branch in Branch
                          join stock in Stocklist on branch.ID equals stock.BranchID
                          join item in Item on stock.ItemID equals item.ID
                          group new { stock, item } by branch into b
                          select new
                          {
                              brName = b.Key.Location,
                              brExpenses = b.Sum(x => x.stock.Quantity * x.item.Cost)
                          };

            var barLabel = new List<string>();
            var barValue = new List<decimal>();

            foreach (var item in barData)
            {

                barLabel.Add(item.brName);
                barValue.Add(item.brExpenses);
            }
            ViewBag.BarLabel = barLabel;
            ViewBag.BarValue = barValue;


            return View();
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