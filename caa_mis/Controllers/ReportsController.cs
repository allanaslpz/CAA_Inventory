﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using caa_mis.Data;
using caa_mis.Models;
using SkiaSharp;
using caa_mis.Utilities;
using Microsoft.EntityFrameworkCore.Storage;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Drawing;
using caa_mis.ViewModels;
using Microsoft.CodeAnalysis.Operations;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Newtonsoft.Json;
using Org.BouncyCastle.Utilities;
using System.Collections;
using Microsoft.AspNetCore.Authorization;
using NuGet.Versioning;
using Microsoft.Extensions.Caching.Memory;
using System.Drawing.Printing;
using DNTBreadCrumb.Core;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;

namespace caa_mis.Controllers
{
    [BreadCrumb(Title = "Reports", UseDefaultRouteUrl = true, Order = 0, IgnoreAjaxRequests = true)]
    [Authorize(Roles = "Admin, Supervisor")]
    public class ReportsController : Controller
    {
        private readonly InventoryContext _context;
        private readonly IMemoryCache _cache;
        private readonly int cacheTimer = 10; //default 10 minutes
        public ReportsController(InventoryContext context, IMemoryCache memoryCache)
        {
            _context = context;
            _cache = memoryCache;
        }

        public IActionResult Index()
        {
            //Change colour of the button when filtering by setting this default
            ViewData["Filtering"] = "btn-outline-primary";

            return View();
        }
        [BreadCrumb(Title = "Generate Barcode", Order = 1, IgnoreAjaxRequests = true)]
        public IActionResult GenerateBarcode()
        {
            return View();
        }
        [BreadCrumb(Title = "Generate Barcode", Order = 1, IgnoreAjaxRequests = true, Url = "GenerateBarcode")]
        [BreadCrumb(Title = "Print Barcode", Order = 1, IgnoreAjaxRequests = true)]
        [HttpPost, ActionName("PrintBarcode")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PrintBarcode(string Products, string actionButton)
        {
            var toPrint = await _context.Items.ToListAsync();

            if (actionButton == "Generate Filtered")
            {
                
                if(!String.IsNullOrEmpty(Products))
                {
                    try
                    {
                        List<string> pr = Products.Split(',').ToList();
                        pr = pr.Select(t => t.Trim()).ToList();
                        pr.Remove(" ");
                        var filteredOrders = from order in _context.Items
                                             where pr.Contains(order.SKUNumber)
                                             select order;
                        return View(filteredOrders);
                    }
                    catch
                    {
                        TempData["ErrorMessage"] = "Invalid Format Submitted. SKU must be separated with comma ex. CAA1234, CAA2345, CAA3245";
                        return View();
                    }
                }
               else
                {
                    TempData["ErrorMessage"] = "Empty Field (SKU). SKU must be separated with comma ex. CAA1234, CAA2345, CAA3245";
                    return View();
                }

            }
            else{
                return View(toPrint);
            }
            

            
        }
        [BreadCrumb(Title = "Stock Summary By Branch", Order = 1, IgnoreAjaxRequests = true)]
        public async Task<IActionResult> StockSummaryByBranch(int? page, int? pageSizeID, int[] BranchID, string sortDirectionCheck,
                                           string sortFieldID, string Products, string actionButton, string sortDirection = "asc", string sortField = "BranchName")
        {
            //List of sort options.
            //NOTE: make sure this array has matching values to the column headings
            string[] sortOptions = new[] { "Branch", "Product", "Cost", "Quantity", "Min Level" };

            //Change colour of the button when filtering by setting this default
            ViewData["Filtering"] = "btn-outline-primary";

            IQueryable<StockSummaryByBranchVM> sumQ = _context.StockSummaryByBranch;

            if (BranchID != null && BranchID.Length > 0)
            {
                sumQ = sumQ.Where(s => BranchID.Contains(s.BranchID));
                ViewData["Filtering"] = "btn-danger";
            }


            if (!string.IsNullOrEmpty(Products))
            {
                List<string> pr = Products.Split(',').Select(t => t.Trim()).ToList();
                pr.Remove("");
                try
                {
                    var filteredItems = _context.Items
                    .Where(item => pr.Contains(item.Name))
                    .Select(item => item.Name);
                    sumQ = sumQ.Where(summary => filteredItems.Contains(summary.ItemName));
                }
                catch
                {
                    TempData["ErrorMessage"] = "Invalid Format Submitted. Product must be separated with comma ex. Chair, Table,...";
                    return View();
                }

                ViewData["Filtering"] = "btn-danger";
            }

            ViewData["BranchID"] = BranchList(BranchID);

            //Before we sort, see if we have called for a change of filtering or sorting
            if (!String.IsNullOrEmpty(actionButton)) //Form Submitted!
            {
                page = 1;//Reset page to start

                if (sortOptions.Contains(actionButton))//Change of sort is requested
                {
                    if (actionButton == sortField) //Reverse order on same field
                    {
                        sortDirection = sortDirection == "asc" ? "desc" : "asc";
                    }
                    sortField = actionButton;//Sort by the button clicked
                }
                else //Sort by the controls in the filter area
                {
                    sortDirection = String.IsNullOrEmpty(sortDirectionCheck) ? "asc" : "desc";
                    sortField = sortFieldID;
                }
            }

            //Now we know which field and direction to sort by
            if (sortField == "Branch")
            {
                if (sortDirection == "asc")
                {
                    sumQ = sumQ
                        .OrderBy(p => p.BranchName);
                }
                else
                {
                    sumQ = sumQ
                        .OrderByDescending(p => p.BranchName);
                }
            }
            else if (sortField == "Product")
            {
                if (sortDirection == "asc")
                {
                    sumQ = sumQ
                        .OrderByDescending(p => p.ItemName);
                }
                else
                {
                    sumQ = sumQ
                        .OrderBy(p => p.ItemName);
                }
            }
            else if (sortField == "Cost")
            {
                if (sortDirection == "asc")
                {
                    sumQ = sumQ
                        .OrderByDescending(p => p.ItemCost);
                }
                else
                {
                    sumQ = sumQ
                        .OrderBy(p => p.ItemCost);
                }
            }
            else if (sortField == "Quantity")
            {
                if (sortDirection == "asc")
                {
                    sumQ = sumQ
                        .OrderBy(p => p.Quantity);
                }
                else
                {
                    sumQ = sumQ
                        .OrderByDescending(p => p.Quantity);
                }
            }
            else if (sortField == "Min Level")
            {
                if (sortDirection == "asc")
                {
                    sumQ = sumQ
                        .OrderBy(p => p.MinLevel);
                }
                else
                {
                    sumQ = sumQ
                        .OrderByDescending(p => p.MinLevel);
                }
            }
            else //Sorting by Name
            {
                if (sortDirection == "asc")
                {
                    sumQ = sumQ
                        .OrderBy(p => p.ItemName)
                        .ThenBy(p => p.Quantity);
                }
                else
                {
                    sumQ = sumQ
                        .OrderByDescending(p => p.ItemName)
                        .ThenByDescending(p => p.Quantity);
                }
            }

            // Save filtered data to memory cache
            var toListSumQ = sumQ.ToList();
            _cache.Set("cachedData", toListSumQ, TimeSpan.FromMinutes(cacheTimer));


            //Set sort for next time
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;
            //SelectList for Sorting Options
            //ViewBag.sortFieldID = new SelectList(sortOptions, sortField.ToString());

            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, "StockItemSummary");
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<StockSummaryByBranchVM>.CreateAsync(sumQ.AsNoTracking(), page ?? 1, pageSize);
            return View(pagedData);
        }

        private SelectList BranchList(int[] selectedId)
        {
            return new SelectList(_context.Branches
                .OrderBy(d => d.Name), "ID", "Name", selectedId);
        }

        public IActionResult DownloadStockItems()
        {
            //retrieving data from cache            
            var items = _cache.Get<IEnumerable<StockSummaryByBranchVM>>("cachedData");

            if (items == null)
            {
                // If data is not in cache, retrieve it from the database and add it to the cache
                items = _context.StockSummaryByBranch.AsNoTracking()
                .ToList();
            }

            int numRows = items.Count();

            if (numRows > 0)
            {
                using ExcelPackage excel = new();
                var workSheet = excel.Workbook.Worksheets.Add("StockProducts");

                workSheet.Cells[3, 1].LoadFromCollection(items, true);

                //Set Style and backgound colour of headings
                using (ExcelRange headings = workSheet.Cells[3, 1, 3, 7])
                {
                    headings.Style.Font.Bold = true;
                    var fill = headings.Style.Fill;
                    fill.PatternType = ExcelFillStyle.Solid;
                    fill.BackgroundColor.SetColor(Color.LightCyan);
                }

                //Autofit columns
                workSheet.Cells.AutoFitColumns();

                //Add a title and timestamp at the top of the report
                workSheet.Cells[1, 1].Value = "Stock Product Report";
                using (ExcelRange Rng = workSheet.Cells[1, 1, 1, 7])
                {
                    Rng.Merge = true; //Merge columns start and end range
                    Rng.Style.Font.Bold = true; //Font should be bold
                    Rng.Style.Font.Size = 18;
                    Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }
                //Since the time zone where the server is running can be different, adjust to 
                //Local for us.
                DateTime utcDate = DateTime.UtcNow;
                TimeZoneInfo esTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                DateTime localDate = TimeZoneInfo.ConvertTimeFromUtc(utcDate, esTimeZone);
                using (ExcelRange Rng = workSheet.Cells[2, 7])
                {
                    Rng.Value = "Created: " + localDate.ToShortTimeString() + " on " +
                        localDate.ToShortDateString();

                    Rng.Style.Font.Size = 12;
                    Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                }

                try
                {
                    Byte[] theData = excel.GetAsByteArray();
                    string filename = "StockProducts.xlsx";
                    string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    return File(theData, mimeType, filename);
                }
                catch (Exception)
                {
                    return BadRequest("Could not build and download the file.");
                }
            }
            return NotFound("No data.");
        }
        [BreadCrumb(Title = "Events Summary", Order = 1, IgnoreAjaxRequests = true)]
        public async Task<IActionResult> EventSummary(int? page, int? pageSizeID, int[] BranchID, string sortDirectionCheck,
                                           string sortFieldID, string Products, string actionButton,
                                           string sortDirection = "asc", string sortField = "Branch",
                                           DateTime? eventStartDate = null, DateTime? eventEndDate = null)
        {
            //List of sort options.
            //NOTE: make sure this array has matching values to the column headings
            string[] sortOptions = new[] { "Employee", "Branch", "Transfer Status", "Event",
                                            "Event Date", "Product", "Quantity"};

            //Change colour of the button when filtering by setting this default
            ViewData["Filtering"] = "btn-outline-primary";

            IQueryable<EventSummaryVM> sumQ = _context.EventSummary;

            if (BranchID != null && BranchID.Length > 0)
            {
                sumQ = sumQ.Where(s => BranchID.Contains(s.BranchID));
                ViewData["Filtering"] = "btn-danger";
            }

            if (eventStartDate != null)
            {
                sumQ = sumQ.Where(e => e.EventDate >= eventStartDate.Value);
                ViewData["Filtering"] = "btn-danger";
            }

            if (eventEndDate != null)
            {
                sumQ = sumQ.Where(e => e.EventDate <= eventEndDate.Value);
                ViewData["Filtering"] = "btn-danger";
            }

            if (!string.IsNullOrEmpty(Products))
            {
                List<string> pr = Products.Split(',').Select(t => t.Trim()).ToList();
                pr.Remove("");
                try
                {
                    var filteredItems = _context.Items
                    .Where(item => pr.Contains(item.Name))
                    .Select(item => item.Name);
                    sumQ = sumQ.Where(summary => filteredItems.Contains(summary.ItemName));
                }
                catch
                {
                    TempData["ErrorMessage"] = "Invalid Format Submitted. Product must be separated with comma ex. Chair, Table,...";
                    return View();
                }

                ViewData["Filtering"] = "btn-danger";
            }


            ViewData["BranchID"] = BranchList(BranchID);

            //Before we sort, see if we have called for a change of filtering or sorting
            if (!String.IsNullOrEmpty(actionButton)) //Form Submitted!
            {
                page = 1;//Reset page to start

                if (sortOptions.Contains(actionButton))//Change of sort is requested
                {
                    if (actionButton == sortField) //Reverse order on same field
                    {
                        sortDirection = sortDirection == "asc" ? "desc" : "asc";
                    }
                    sortField = actionButton;//Sort by the button clicked
                }
                else //Sort by the controls in the filter area
                {
                    sortDirection = String.IsNullOrEmpty(sortDirectionCheck) ? "asc" : "desc";
                    sortField = sortFieldID;
                }
            }

            //Now we know which field and direction to sort by
            if (sortField == "Employee")
            {
                if (sortDirection == "asc")
                {
                    sumQ = sumQ
                        .OrderBy(p => p.EmployeeName);
                }
                else
                {
                    sumQ = sumQ
                        .OrderByDescending(p => p.EmployeeName);
                }
            }
            else if (sortField == "Branch")
            {
                if (sortDirection == "asc")
                {
                    sumQ = sumQ
                        .OrderByDescending(p => p.BranchName);
                }
                else
                {
                    sumQ = sumQ
                        .OrderBy(p => p.BranchName);
                }
            }

            else if (sortField == "Transfer Status")
            {
                if (sortDirection == "asc")
                {
                    sumQ = sumQ
                        .OrderBy(p => p.TransactionStatusName);
                }
                else
                {
                    sumQ = sumQ
                        .OrderByDescending(p => p.TransactionStatusName);
                }
            }
            else if (sortField == "Event")
            {
                if (sortDirection == "asc")
                {
                    sumQ = sumQ
                        .OrderBy(p => p.EventName);
                }
                else
                {
                    sumQ = sumQ
                        .OrderByDescending(p => p.EventName);
                }
            }
            else if (sortField == "Event Date")
            {
                if (sortDirection == "asc")
                {
                    sumQ = sumQ
                        .OrderBy(p => p.EventDate);
                }
                else
                {
                    sumQ = sumQ
                        .OrderByDescending(p => p.EventDate);
                }
            }
            else if (sortField == "Product")
            {
                if (sortDirection == "asc")
                {
                    sumQ = sumQ
                        .OrderBy(p => p.ItemName);
                }
                else
                {
                    sumQ = sumQ
                        .OrderByDescending(p => p.ItemName);
                }
            }
            else if (sortField == "Quantity")
            {
                if (sortDirection == "asc")
                {
                    sumQ = sumQ
                        .OrderBy(p => p.EventQuantity);
                }
                else
                {
                    sumQ = sumQ
                        .OrderByDescending(p => p.ItemName);
                }
            }
            else //Sorting by Name
            {
                if (sortDirection == "asc")
                {
                    sumQ = sumQ
                        .OrderBy(p => p.BranchName)
                        .ThenBy(p => p.EventDate);
                }
                else
                {
                    sumQ = sumQ
                        .OrderByDescending(p => p.BranchName)
                        .ThenByDescending(p => p.EventDate);
                }
            }

            var toListSumQ = sumQ.ToList();
            _cache.Set("cachedData", toListSumQ, TimeSpan.FromMinutes(cacheTimer));

            //Set sort for next time
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;

            // Set the ViewData variables
            ViewData["eventStartDate"] = eventStartDate;
            ViewData["eventEndDate"] = eventEndDate;

            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, "EventSummary");
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<EventSummaryVM>.CreateAsync(sumQ.AsNoTracking(), page ?? 1, pageSize);
            return View(pagedData);
        }

        public IActionResult DownloadEventItems()
        {
            var items = _cache.Get<IEnumerable<EventSummaryVM>>("cachedData");

            if (items == null)
            {
                // If data is not in cache, retrieve it from the database and add it to the cache
                items = _context.EventSummary.AsNoTracking()
                .ToList();
            }

            int numRows = items.Count();

            if (numRows > 0)
            {
                using ExcelPackage excel = new();
                var workSheet = excel.Workbook.Worksheets.Add("EventProducts");

                workSheet.Cells[3, 1].LoadFromCollection(items, true);
                workSheet.Column(10).Style.Numberformat.Format = "yyyy-mm-dd";
                //Set Style and backgound colour of headings
                using (ExcelRange headings = workSheet.Cells[3, 1, 3, 13])
                {
                    headings.Style.Font.Bold = true;
                    var fill = headings.Style.Fill;
                    fill.PatternType = ExcelFillStyle.Solid;
                    fill.BackgroundColor.SetColor(Color.LightCyan);
                }

                //Autofit columns
                workSheet.Cells.AutoFitColumns();

                //Add a title and timestamp at the top of the report
                workSheet.Cells[1, 1].Value = "Event Product Report";
                using (ExcelRange Rng = workSheet.Cells[1, 1, 1, 13])
                {
                    Rng.Merge = true; //Merge columns start and end range
                    Rng.Style.Font.Bold = true; //Font should be bold
                    Rng.Style.Font.Size = 18;
                    Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }
                //Since the time zone where the server is running can be different, adjust to 
                //Local for us.
                DateTime utcDate = DateTime.UtcNow;
                TimeZoneInfo esTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                DateTime localDate = TimeZoneInfo.ConvertTimeFromUtc(utcDate, esTimeZone);
                using (ExcelRange Rng = workSheet.Cells[2, 13])
                {
                    Rng.Value = "Created: " + localDate.ToShortTimeString() + " on " +
                        localDate.ToShortDateString();

                    Rng.Style.Font.Size = 12;
                    Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                }

                try
                {
                    Byte[] theData = excel.GetAsByteArray();
                    string filename = "EventProducts.xlsx";
                    string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    return File(theData, mimeType, filename);
                }
                catch (Exception)
                {
                    return BadRequest("Could not build and download the file.");
                }
            }
            return NotFound("No data.");
        }
        [BreadCrumb(Title = "Transferred Products", Order = 1, IgnoreAjaxRequests = true)]
        public async Task<IActionResult> TransactionItemSummary(int? page, int? pageSizeID, int[] OriginID, int[] DestinationID, string sortDirectionCheck,
                                           string sortFieldID, string Products, string actionButton, string sortDirection = "asc", string sortField = "Origin")
        {
            //List of sort options.
            //NOTE: make sure this array has matching values to the column headings
            string[] sortOptions = new[] { "Employee", "Origin", "Destination",
                                            "Transfer Status", "Product", "Quantity"};

            //Change colour of the button when filtering by setting this default
            ViewData["Filtering"] = "btn-outline-primary";

            IQueryable<TransactionItemSummaryVM> sumQ = _context.TransactionItemSummary;

            if (OriginID != null && OriginID.Length > 0)
            {
                sumQ = sumQ.Where(s => OriginID.Contains(s.OriginID));
                ViewData["Filtering"] = "btn-danger";
            }
            if (DestinationID != null && DestinationID.Length > 0)
            {
                sumQ = sumQ.Where(s => DestinationID.Contains(s.OriginID));
                ViewData["Filtering"] = "btn-danger";
            }

            if (!string.IsNullOrEmpty(Products))
            {
                List<string> pr = Products.Split(',').Select(t => t.Trim()).ToList();
                pr.Remove("");
                try
                {
                    var filteredItems = _context.Items
                    .Where(item => pr.Contains(item.Name))
                    .Select(item => item.Name);
                    sumQ = sumQ.Where(summary => filteredItems.Contains(summary.ItemName));
                }
                catch
                {
                    TempData["ErrorMessage"] = "Invalid Format Submitted. Product must be separated with comma ex. Chair, Table,...";
                    return View();
                }

                ViewData["Filtering"] = "btn-danger";
            }

            ViewData["OriginID"] = BranchList(OriginID);
            ViewData["DestinationID"] = BranchList(DestinationID);


            //Before we sort, see if we have called for a change of filtering or sorting
            if (!String.IsNullOrEmpty(actionButton)) //Form Submitted!
            {
                page = 1;//Reset page to start

                if (sortOptions.Contains(actionButton))//Change of sort is requested
                {
                    if (actionButton == sortField) //Reverse order on same field
                    {
                        sortDirection = sortDirection == "asc" ? "desc" : "asc";
                    }
                    sortField = actionButton;//Sort by the button clicked
                }
                else //Sort by the controls in the filter area
                {
                    sortDirection = String.IsNullOrEmpty(sortDirectionCheck) ? "asc" : "desc";
                    sortField = sortFieldID;
                }
            }

            //Now we know which field and direction to sort by
            if (sortField == "Employee")
            {
                if (sortDirection == "asc")
                {
                    sumQ = sumQ
                        .OrderBy(p => p.EmployeeName);
                }
                else
                {
                    sumQ = sumQ
                        .OrderByDescending(p => p.EmployeeName);
                }
            }
            else if (sortField == "Origin")
            {
                if (sortDirection == "asc")
                {
                    sumQ = sumQ
                        .OrderByDescending(p => p.OriginName);
                }
                else
                {
                    sumQ = sumQ
                        .OrderBy(p => p.OriginName);
                }
            }
            else if (sortField == "Destination")
            {
                if (sortDirection == "asc")
                {
                    sumQ = sumQ
                        .OrderByDescending(p => p.DestinationName);
                }
                else
                {
                    sumQ = sumQ
                        .OrderBy(p => p.DestinationName);
                }
            }
            else if (sortField == "Transfer Status")
            {
                if (sortDirection == "asc")
                {
                    sumQ = sumQ
                        .OrderBy(p => p.TransactionStatusName);
                }
                else
                {
                    sumQ = sumQ
                        .OrderByDescending(p => p.TransactionStatusName);
                }
            }
            else if (sortField == "Product")
            {
                if (sortDirection == "asc")
                {
                    sumQ = sumQ
                        .OrderBy(p => p.ItemName);
                }
                else
                {
                    sumQ = sumQ
                        .OrderByDescending(p => p.ItemName);
                }
            }
            else if (sortField == "Quantity")
            {
                if (sortDirection == "asc")
                {
                    sumQ = sumQ
                        .OrderBy(p => p.Quantity);
                }
                else
                {
                    sumQ = sumQ
                        .OrderByDescending(p => p.Quantity);
                }
            }
            else //Sorting by Name
            {
                if (sortDirection == "asc")
                {
                    sumQ = sumQ
                        .OrderBy(p => p.OriginName)
                        .ThenBy(p => p.DestinationName);
                }
                else
                {
                    sumQ = sumQ
                        .OrderByDescending(p => p.OriginName)
                        .ThenByDescending(p => p.DestinationName);
                }
            }

            //Set sort for next time
            ViewData["sortField"] = sortField;
            ViewData["sortDirection"] = sortDirection;
            
            var toListSumQ = sumQ.ToList();
            _cache.Set("cachedData", toListSumQ, TimeSpan.FromMinutes(cacheTimer));


            int pageSize = PageSizeHelper.SetPageSize(HttpContext, pageSizeID, "TransactionItemSummary");
            ViewData["pageSizeID"] = PageSizeHelper.PageSizeList(pageSize);
            var pagedData = await PaginatedList<TransactionItemSummaryVM>.CreateAsync(sumQ.AsNoTracking(), page ?? 1, pageSize);
            return View(pagedData);
        }

        public IActionResult DownloadTransactionItems()
        {
            //retrieving data from cache
            var items = _cache.Get<IEnumerable<TransactionItemSummaryVM>>("cachedData");

            if (items == null)
            {
                // If data is not in cache, retrieve it from the database and add it to the cache
                items = _context.TransactionItemSummary.AsNoTracking()
                .ToList();
            }

            int numRows = items.Count();

            if (numRows > 0)
            {
                using ExcelPackage excel = new();
                var workSheet = excel.Workbook.Worksheets.Add("Transferred Products");

                workSheet.Cells[3, 1].LoadFromCollection(items, true);

                //Set Style and backgound colour of headings
                using (ExcelRange headings = workSheet.Cells[3, 1, 3, 11])
                {
                    headings.Style.Font.Bold = true;
                    var fill = headings.Style.Fill;
                    fill.PatternType = ExcelFillStyle.Solid;
                    fill.BackgroundColor.SetColor(Color.LightCyan);
                }

                //Autofit columns
                workSheet.Cells.AutoFitColumns();

                //Add a title and timestamp at the top of the report
                workSheet.Cells[1, 1].Value = "Transferred Products Report";
                using (ExcelRange Rng = workSheet.Cells[1, 1, 1, 11])
                {
                    Rng.Merge = true; //Merge columns start and end range
                    Rng.Style.Font.Bold = true; //Font should be bold
                    Rng.Style.Font.Size = 18;
                    Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }
                //Since the time zone where the server is running can be different, adjust to 
                //Local for us.
                DateTime utcDate = DateTime.UtcNow;
                TimeZoneInfo esTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                DateTime localDate = TimeZoneInfo.ConvertTimeFromUtc(utcDate, esTimeZone);
                using (ExcelRange Rng = workSheet.Cells[2, 11])
                {
                    Rng.Value = "Created: " + localDate.ToShortTimeString() + " on " +
                        localDate.ToShortDateString();

                    Rng.Style.Font.Size = 12;
                    Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                }

                try
                {
                    Byte[] theData = excel.GetAsByteArray();
                    string filename = "TransferredProducts.xlsx";
                    string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    return File(theData, mimeType, filename);
                }
                catch (Exception)
                {
                    return BadRequest("Could not build and download the file.");
                }
            }
            return NotFound("No data.");
        }
    }
}
