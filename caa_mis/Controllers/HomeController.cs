using caa_mis.Data;
using caa_mis.Models;
using caa_mis.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.Style;
using SQLitePCL;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Drawing;

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

            ViewBag.Branches = Branch.Select(b => new SelectListItem
            {
                Value = b.ID.ToString(),
                Text = b.Location
            }).ToList();
            ViewBag.Branches.Insert(0, new SelectListItem { Value = "0", Text = "All" });

            int branchID;
            bool result = int.TryParse(Request.Query["branch"], out branchID);
            if (!result)
            {
                branchID = 0;
            }

            var tableItems = from item in Item
                             join stock in Stocklist on item.ID equals stock.ItemID
                             where (branchID == 0 || stock.BranchID == branchID)
                             group stock by item.Name into t
                             let minLevel = t.Min(s => s.MinLevel)
                             let totalStock = t.Sum(s => s.Quantity)
                             where totalStock < minLevel // Filter out items not low in stock
                             select new
                             {
                                 ItemName = t.Key,
                                 MinLevel = minLevel,
                                 TotalStock = totalStock,
                                 Percentage = 1 - ((double)minLevel / (double)totalStock)
                             };
            ViewBag.CurrentBranchID = branchID;
            ViewBag.TableItem = tableItems.OrderBy(s => s.Percentage);

            //PieChart information

            var pieData = from item in Item join category in Category on item.CategoryID equals category.ID
                          join stock in Stocklist on item.ID equals stock.ItemID
                          where (branchID == 0 || stock.BranchID == branchID)
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

        public ActionResult LowInStock(int branch = 0)
        {
            var Item = _inventoryContext.Items.Include(s => s.Stocks).ToList();
            var Stocklist = _inventoryContext.Stocks.Where(s => s.Quantity > 0);
            if (branch != 0)
            {
                Stocklist = Stocklist.Where(s => s.BranchID == branch);
            }
            var Category = _inventoryContext.Categories.ToList();
            var Branch = _inventoryContext.Branches.ToList();

            ViewBag.Branches = Branch.Select(b => new SelectListItem
            {
                Value = b.ID.ToString(),
                Text = b.Location
            }).ToList();
            ViewBag.Branches.Insert(0, new SelectListItem { Value = "0", Text = "All" });

            var tableItems = from item in Item
                             join stock in Stocklist on item.ID equals stock.ItemID
                             group stock by item.Name into t
                             let minLevel = t.Min(s => s.MinLevel)
                             let totalStock = t.Sum(s => s.Quantity)
                             where totalStock < minLevel // Filter out items not low in stock
                             select new
                             {
                                 ItemName = t.Key,
                                 MinLevel = minLevel,
                                 TotalStock = totalStock,
                                 Percentage = 1 - ((double)minLevel / (double)totalStock)
                             };
            ViewBag.CurrentBranchID = branch;
            ViewBag.TableItem = tableItems.OrderBy(s => s.Percentage);

            return View(tableItems);
        }

        public IActionResult DownloadLowInStock(int branch = 0)
        {
            var items = _inventoryContext.Items.Include(s => s.Stocks).ToList();
            var stocklist = _inventoryContext.Stocks.Where(s => s.Quantity > 0);
            if (branch != 0)
            {
                stocklist = stocklist.Where(s => s.BranchID == branch);
            }
            var categories = _inventoryContext.Categories.ToList();
            var branches = _inventoryContext.Branches.ToList();

            var tableItems = from item in items
                             join stock in stocklist on item.ID equals stock.ItemID
                             group stock by item.Name into t
                             let minLevel = t.Min(s => s.MinLevel)
                             let totalStock = t.Sum(s => s.Quantity)
                             where totalStock < minLevel // Filter out items not low in stock
                             select new
                             {
                                 ItemName = t.Key,
                                 MinLevel = minLevel,
                                 TotalStock = totalStock
                                 //Percentage = 1 - ((double)minLevel / (double)totalStock)
                             };

            using var excel = new ExcelPackage();
            var worksheet = excel.Workbook.Worksheets.Add("Low In Stock Items");
            worksheet.Cells["A1"].Value = "Low In Stock Items Report";

            worksheet.Cells["A3"].LoadFromCollection(tableItems, true);

            worksheet.Cells[3, 1, 3, 3].Style.Font.Bold = true;
            worksheet.Cells[3, 1, 3, 3].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[3, 1, 3, 3].Style.Fill.BackgroundColor.SetColor(Color.LightCyan);

            worksheet.Cells.AutoFitColumns();

            //Since the time zone where the server is running can be different, adjust to 
            //Local for us.
            DateTime utcDate = DateTime.UtcNow;
            TimeZoneInfo esTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            DateTime localDate = TimeZoneInfo.ConvertTimeFromUtc(utcDate, esTimeZone);

            worksheet.Cells[1, 1].Value = "Low In Stock Items Report";
            worksheet.Cells[1, 1].Style.Font.Bold = true;
            worksheet.Cells[1, 1].Style.Font.Size = 14;
            worksheet.Cells[2, 1].Value = "Created: " + localDate.ToShortTimeString() + " on " +
                localDate.ToShortDateString();

            var data = excel.GetAsByteArray();
            var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var fileName = "LowInStockItems.xlsx";

            return File(data, contentType, fileName);
        }
    }
}