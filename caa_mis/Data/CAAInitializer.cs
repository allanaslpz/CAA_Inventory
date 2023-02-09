using System;
using Microsoft.CodeAnalysis;
using System.Diagnostics;
using System.Numerics;
using caa_mis.Models;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using Microsoft.CodeAnalysis.Operations;

namespace caa_mis.Data
{
	public static class CAAInitializer
	{
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            InventoryContext context = applicationBuilder.ApplicationServices.CreateScope()
                .ServiceProvider.GetRequiredService<InventoryContext>();
            try
            {
                // Delete database if you need to apply a new Migration //
                //context.Database.EnsureDeleted();
                // Create database if it does not exist and apply the Migration //
                context.Database.Migrate();

                // Transaction Status Seed Data //
                if (!context.TransactionStatuses.Any())
                {
                    context.TransactionStatuses.AddRange(
                    new TransactionStatus
                    {
                        Name = "Open",
                        Description = "Editable Status",
                    },
                    new TransactionStatus
                    {
                        Name = "Released",
                        Description = "In Transit Status",
                    },
                    new TransactionStatus
                    {
                        Name = "Received",
                        Description = "Processed Status",
                    });
                    context.SaveChanges();
                }

                // Transaction Type Seed Data //
                if (!context.TransactionTypes.Any())
                {
                    context.TransactionTypes.AddRange(
                    new TransactionType
                    {
                        Name = "St. Catharines Christmas Expo",
                        Description = "CAA Winter 2022",
                        InOut = InOut.In
                    },
                    new TransactionType
                    {
                        Name = "Thorold Spring Break",
                        Description = "CAA Spring 2023",
                        InOut = InOut.Out
                    },
                    new TransactionType
                    {
                        Name = "Niagara Falls Summer Splash",
                        Description = "CAA Summer 2023",
                        InOut = InOut.In
                    },
                    new TransactionType
                    {
                        Name = "Welland Halloween Party",
                        Description = "CAA Fall 2023",
                        InOut = InOut.Out
                    });
                    context.SaveChanges();
                }
                // Seed data for Item status
                if (!context.ItemStatuses.Any())
                {
                    context.ItemStatuses.AddRange(
                    new ItemStatus
                    {
                        Name = "Active",
                        Description = "Sets the Item/Product to Active"
                    },
                    new ItemStatus
                    {
                        Name = "Discontinued",
                        Description = "Sets the Item/Product to Discontinued/Deactived"
                    });
                    context.SaveChanges();
                }
                if (!context.Categories.Any())
                {
                    context.Categories.AddRange(
                    new Category
                    {
                        Name = "Swag",
                        Description = "Sunglasses, giveaways, pens etc"
                    },
                    new Category
                    {
                        Name = "Printed Marketing Materials",
                        Description = "Marketing Materials and Posters"
                    },
                    new Category
                    {
                        Name = "Equipment",
                        Description = "Tents, tables, table clothes, roll-up posters, technical assests etc)"
                    },
                    new Category
                    {
                        Name = "Event Materials ",
                        Description = "Event Materials"
                    });
                    context.SaveChanges();
                }
                // Employees Seed Data //
                if (!context.Employees.Any())
                {
                    context.Employees.AddRange(
                    new Employee
                    {
                        FirstName = "Edmund Kevin",
                        LastName = "Rone",
                    },
                    new Employee
                    {
                        FirstName = "Allan Antonio",
                        LastName = "Lopez",
                    },
                    new Employee
                    {
                        FirstName = "Michael Laurence",
                        LastName = "Barde",
                    },
                    new Employee
                    {
                        FirstName = "Luisito Jr",
                        LastName = "Villaflor",
                    },
                    new Employee
                    {
                        FirstName = "Tsogt",
                        LastName = "Batjargal",
                    });
                    context.SaveChanges();
                }
                // Employees Seed Data //
                if (!context.Branches.Any())
                {
                    context.Branches.AddRange(
                    new Branch
                    {
                        Name = "CAA Niagara Falls",
                        Location = "Niagara Falls",
                        Address = "6788 Regional Rd 57",
                        PhoneNumber = "800-263-7272",
                    },
                    new Branch
                    {
                        Name = "CAA Welland",
                        Location = "Welland",
                        Address = "800 Niagara St",
                        PhoneNumber = "800-263-7272",
                    },
                    new Branch
                    {
                        Name = "CAA St. Catharines",
                        Location = "St. Catharines",
                        Address = "76 Lake St",
                        PhoneNumber = "800-263-7272"
                    },
                    new Branch
                    {
                        Name = "CAA Thorold",
                        Location = "Thorold",
                        Address = "3271 Schmon Pkwy",
                        PhoneNumber = "905-984-8585",
                    },
                    new Branch
                    {
                        Name = "CAA Grimsby",
                        Location = "Grimsby",
                        Address = "Orchardview Village Square",
                        PhoneNumber = "800-263-7272",
                    });
                    context.SaveChanges();
                }
                // Items Seed Data //
                if (!context.Items.Any())
                {
                    context.Items.AddRange(
                    new Item
                    {
                        Name = "Sunglasses",
                        SKUNumber = "00001",
                        Cost = 20,
                        Description = "Cool and trending sunglasses",
                        ItemStatusID = context.ItemStatuses.FirstOrDefault(s => s.Name == "Active").ID,
                        CategoryID = context.Categories.FirstOrDefault(c => c.Name == "Swag").ID,
                    },
                    new Item
                    {
                        Name = "Pen",
                        SKUNumber = "00002",
                        Cost = 5,
                        Description = "Clickable pen",
                        ItemStatusID = context.ItemStatuses.FirstOrDefault(s => s.Name == "Active").ID,
                        CategoryID = context.Categories.FirstOrDefault(c => c.Name == "Swag").ID,
                    },
                    new Item
                    {
                        Name = "Towel",
                        SKUNumber = "00003",
                        Cost = 15,
                        Description = "100% Cotton Towels",
                        ItemStatusID = context.ItemStatuses.FirstOrDefault(s => s.Name == "Active").ID,
                        CategoryID = context.Categories.FirstOrDefault(c => c.Name == "Swag").ID,
                    },
                    new Item
                    {
                        Name = "T-Shirt",
                        SKUNumber = "00004",
                        Cost = 20,
                        Description = "White T-Shirt with CAA Logo",
                        ItemStatusID = context.ItemStatuses.FirstOrDefault(s => s.Name == "Active").ID,
                        CategoryID = context.Categories.FirstOrDefault(c => c.Name == "Swag").ID,
                    },
                    new Item
                    {
                        Name = "Tent",
                        SKUNumber = "00010",
                        Cost = 200,
                        Description = "Large red tent",
                        ItemStatusID = context.ItemStatuses.FirstOrDefault(s => s.Name == "Active").ID,
                        CategoryID = context.Categories.FirstOrDefault(c => c.Name == "Equipment").ID,
                    },
                    new Item
                    {
                        Name = "Table",
                        SKUNumber = "00020",
                        Cost = 120,
                        Description = "White long foldable table",
                        ItemStatusID = context.ItemStatuses.FirstOrDefault(s => s.Name == "Active").ID,
                        CategoryID = context.Categories.FirstOrDefault(c => c.Name == "Equipment").ID,
                    },
                    new Item
                    {
                        Name = "Table Cloth",
                        SKUNumber = "00030",
                        Cost = 20,
                        Description = "Blue table cloth ",
                        ItemStatusID = context.ItemStatuses.FirstOrDefault(s => s.Name == "Active").ID,
                        CategoryID = context.Categories.FirstOrDefault(c => c.Name == "Equipment").ID,
                    },
                    new Item
                    {
                        Name = "Roll-up Poster",
                        SKUNumber = "00040",
                        Cost = 40,
                        Description = "Large Poster of CAA",
                        ItemStatusID = context.ItemStatuses.FirstOrDefault(s => s.Name == "Active").ID,
                        CategoryID = context.Categories.FirstOrDefault(c => c.Name == "Equipment").ID,
                    },
                    new Item
                    {
                        Name = "Sticker",
                        SKUNumber = "00005",
                        Cost = 5,
                        Description = "Giveaway Stickers with CAA brand",
                        ItemStatusID = context.ItemStatuses.FirstOrDefault(s => s.Name == "Active").ID,
                        CategoryID = context.Categories.FirstOrDefault(c => c.Name == "Swag").ID,
                    },
                    new Item
                    {
                        Name = "Chair",
                        SKUNumber = "00050",
                        Cost = 40,
                        Description = "Foldable Chair",
                        ItemStatusID = context.ItemStatuses.FirstOrDefault(s => s.Name == "Active").ID,
                        CategoryID = context.Categories.FirstOrDefault(c => c.Name == "Equipment").ID,
                    });

                    context.SaveChanges();
                }
                // Stock Seed Data //
                if (!context.Stocks.Any())
                {
                    context.Stocks.AddRange(
                    new Stock
                    {
                        BranchID = context.Branches.FirstOrDefault(b => b.Location == "Welland").ID,
                        ItemID = context.Items.FirstOrDefault(i => i.Name == "Sunglasses").ID,
                        MinLevel = 50,
                        Quantity = 400,

                    },
                    new Stock
                    {
                        BranchID = context.Branches.FirstOrDefault(b => b.Location == "Welland").ID,
                        ItemID = context.Items.FirstOrDefault(i => i.Name == "Pen").ID,
                        MinLevel = 100,
                        Quantity = 150,
                    },
                    new Stock
                    {
                        BranchID = context.Branches.FirstOrDefault(b => b.Location == "Niagara Falls").ID,
                        ItemID = context.Items.FirstOrDefault(i => i.Name == "Towel").ID,
                        MinLevel = 50,
                        Quantity = 250,
                    },
                    new Stock
                    {
                        BranchID = context.Branches.FirstOrDefault(b => b.Location == "Niagara Falls").ID,
                        ItemID = context.Items.FirstOrDefault(i => i.Name == "T-Shirt").ID,
                        MinLevel = 50,
                        Quantity = 40,
                    },
                    new Stock
                    {
                        BranchID = context.Branches.FirstOrDefault(b => b.Location == "St. Catharines").ID,
                        ItemID = context.Items.FirstOrDefault(i => i.Name == "Sticker").ID,
                        MinLevel = 50,
                        Quantity = 20,
                    },
                    new Stock
                    {
                        BranchID = context.Branches.FirstOrDefault(b => b.Location == "Thorold").ID,
                        ItemID = context.Items.FirstOrDefault(i => i.Name == "Tent").ID,
                        MinLevel = 50,
                        Quantity = 100,
                    },
                    new Stock
                    {
                        BranchID = context.Branches.FirstOrDefault(b => b.Location == "Thorold").ID,
                        ItemID = context.Items.FirstOrDefault(i => i.Name == "Chair").ID,
                        MinLevel = 100,
                        Quantity = 500,
                    },
                    new Stock
                    {
                        BranchID = context.Branches.FirstOrDefault(b => b.Location == "Thorold").ID,
                        ItemID = context.Items.FirstOrDefault(i => i.Name == "Table Cloth").ID,
                        MinLevel = 100,
                        Quantity = 200,
                    },
                    new Stock
                    {
                        BranchID = context.Branches.FirstOrDefault(b => b.Location == "Thorold").ID,
                        ItemID = context.Items.FirstOrDefault(i => i.Name == "Table").ID,
                        MinLevel = 50,
                        Quantity = 100,
                    },
                     new Stock
                     {
                         BranchID = context.Branches.FirstOrDefault(b => b.Location == "St. Catharines").ID,
                         ItemID = context.Items.FirstOrDefault(i => i.Name == "Sunglasses").ID,
                         MinLevel = 50,
                         Quantity = 50,

                     },
                    new Stock
                    {
                        BranchID = context.Branches.FirstOrDefault(b => b.Location == "St. Catharines").ID,
                        ItemID = context.Items.FirstOrDefault(i => i.Name == "Pen").ID,
                        MinLevel = 110,
                        Quantity = 150,
                    },
                    new Stock
                    {
                        BranchID = context.Branches.FirstOrDefault(b => b.Location == "St. Catharines").ID,
                        ItemID = context.Items.FirstOrDefault(i => i.Name == "Towel").ID,
                        MinLevel = 51,
                        Quantity = 210,
                    },
                    new Stock
                    {
                        BranchID = context.Branches.FirstOrDefault(b => b.Location == "St. Catharines").ID,
                        ItemID = context.Items.FirstOrDefault(i => i.Name == "T-Shirt").ID,
                        MinLevel = 10,
                        Quantity = 10,
                    },
                    new Stock
                    {
                        BranchID = context.Branches.FirstOrDefault(b => b.Location == "Grimsby").ID,
                        ItemID = context.Items.FirstOrDefault(i => i.Name == "Sticker").ID,
                        MinLevel = 20,
                        Quantity = 60,
                    },
                    new Stock
                    {
                        BranchID = context.Branches.FirstOrDefault(b => b.Location == "Grimsby").ID,
                        ItemID = context.Items.FirstOrDefault(i => i.Name == "Tent").ID,
                        MinLevel = 70,
                        Quantity = 100,
                    },
                    new Stock
                    {
                        BranchID = context.Branches.FirstOrDefault(b => b.Location == "Niagara Falls").ID,
                        ItemID = context.Items.FirstOrDefault(i => i.Name == "Chair").ID,
                        MinLevel = 600,
                        Quantity = 200,
                    },
                    new Stock
                    {
                        BranchID = context.Branches.FirstOrDefault(b => b.Location == "Grimsby").ID,
                        ItemID = context.Items.FirstOrDefault(i => i.Name == "Table Cloth").ID,
                        MinLevel = 10,
                        Quantity = 20,
                    },
                    new Stock
                    {
                        BranchID = context.Branches.FirstOrDefault(b => b.Location == "Grimsby").ID,
                        ItemID = context.Items.FirstOrDefault(i => i.Name == "Table").ID,
                        MinLevel = 5,
                        Quantity = 0,
                    },
                    new Stock
                    {
                        BranchID = context.Branches.FirstOrDefault(b => b.Location == "Grimsby").ID,
                        ItemID = context.Items.FirstOrDefault(i => i.Name == "Roll-up Poster").ID,
                        MinLevel = 60,
                        Quantity = 9,
                    });
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.GetBaseException().Message);
            }
        }
    }
}
