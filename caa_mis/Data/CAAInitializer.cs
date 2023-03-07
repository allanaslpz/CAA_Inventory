using System;
using Microsoft.CodeAnalysis;
using System.Diagnostics;
using System.Numerics;
using caa_mis.Models;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using Microsoft.CodeAnalysis.Operations;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;

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
                // Uncomment to Delete database and apply a new Migration //
                //context.Database.EnsureDeleted();
                // Create database if it does not exist and apply the Migration //
                context.Database.Migrate();
<<<<<<< HEAD

                // Inventory Status Seed Data //
=======
                Random random = new Random();
                // Transaction Status Seed Data //
>>>>>>> origin/DEV
                if (!context.TransactionStatuses.Any())
                {
                    context.TransactionStatuses.AddRange(
                    new TransactionStatus
                    {
                        Name = "Open",
                        Description = "Editable Status",
                        Status = Archived.Enabled
                    },
                    new TransactionStatus
                    {
                        Name = "Released",
                        Description = "In Transit Status",
                        Status = Archived.Enabled
                    },
                    new TransactionStatus
                    {
                        Name = "Received",
                        Description = "Processed Status",
                        Status = Archived.Enabled
                    },
                    new TransactionStatus
                    {
                        Name = "Pending",
                        Description = "Pending Status",
                        Status = Archived.Disabled
                    });
                    context.SaveChanges();
                }

                // Event Type Seed Data //
                if (!context.TransactionTypes.Any())
                {
                    context.TransactionTypes.AddRange(
               
                    new TransactionType
                    {
                        Name = "Stock In",
                        Description = " Generic Stock In",
                        InOut = InOut.In,
                        Status = Archived.Enabled
                    },
                    new TransactionType
                    {
                        Name = "Stock Out",
                        Description = "Generic Stock Out",
                        InOut = InOut.Out,
                        Status = Archived.Enabled
                    },
                    new TransactionType
                    {
                        Name = "Winter Ice",
                        Description = "CAA Winter 2023",
                        InOut = InOut.In,
                        Status = Archived.Enabled
                    },
                    new TransactionType
                    {
                        Name = "Spring Break",
                        Description = "CAA Spring 2023",
                        InOut = InOut.In,
                        Status = Archived.Enabled
                    },
                    new TransactionType
                    {
                        Name = "Summer Splash",
                        Description = "CAA Summer 2023",
                        InOut = InOut.In,
                        Status = Archived.Enabled
                    },
                    new TransactionType
                    {
                        Name = "Halloween Party",
                        Description = "CAA Fall 2023",
                        InOut = InOut.Out,
                        Status = Archived.Enabled
                    },
                    new TransactionType
                    {
                        Name = "Testing Event",
                        Description = "Pending Event Stock In ",
                        InOut = InOut.In,
                        Status = Archived.Disabled
                    });
                    context.SaveChanges();
                }

                // Vendors Seed Data //
                if (!context.Suppliers.Any())
                {
                    context.Suppliers.AddRange(
                    new Supplier
                    {
                        SupplierName = "Walmart",
                        Address1 = "102 Primeway Dr",
                        City = "Welland",
                        Province = "ON",
                        PostalCode = "L3B0A1",
                        Phone = "9057353500",
                        Email = "walmart.welland@gmail.com",
                        Status = Archived.Enabled
                    },
                    new Supplier
                    {
                        SupplierName = "Sobeys",
                        Address1 = "400 Scott St",
                        City = "St. Catharines",
                        Province = "ON",
                        PostalCode = "L2M3W4",
                        Phone = "9059359974",
                        Email = "sobeys.scott.niagara@outlook.com",
                        Status = Archived.Enabled
                    },
                    new Supplier
                    {
                        SupplierName = "Staples",
                        Address1 = "7190 Morrison St",
                        City = "Niagara Falls",
                        Province = "ON",
                        PostalCode = "L2E7K5",
                        Phone = "9053580650",
                        Email = "staples.falls@yahoo.com",
                        Status = Archived.Enabled
                    },
                    new Supplier
                    {
                        SupplierName = "Costco Wholesale",
                        Address1 = "7500 Pin Oak Dr",
                        City = "Niagara Falls",
                        Province = "ON",
                        PostalCode = "L2H2E9",
                        Phone = "3654470200",
                        Email = "costco.niagara@aol.com",
                        Status = Archived.Enabled
                    },
                    new Supplier
                    {
                        SupplierName = "Canadian Tire",
                        Address1 = "158 Primeway Dr",
                        City = "Welland",
                        Province = "ON",
                        PostalCode = "L3B0A1",
                        Phone = "9057327501",
                        Email = "canadian.tire@icloud.com",
                        Status = Archived.Enabled
                    },
                    new Supplier
                    {
                        SupplierName = "RONA",
                        Address1 = "359 S Service Rd",
                        City = "Grimsby",
                        Province = "ON",
                        PostalCode = "L3M4E8",
                        Phone = "9053091959",
                        Email = "rona.grimsby@protonmail.com",
                        Status = Archived.Enabled
                    },
                    new Supplier
                    {
                        SupplierName = "Test CAA Site",
                        Address1 = "3271 Schmon Pkwy",
                        City = "Thorold",
                        Province = "ON",
                        PostalCode = "L2V4Y6",
                        Phone = "9053091959",
                        Email = "test.site@zoho.com",
                        Status = Archived.Disabled
                    });
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
                        Name = "Others (Supplier / Head Office)",
                        Location = "NA",
                        Address = "NA",
                        PhoneNumber = "NA",
                    },
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

                // Transaction Seed Data //
                if (!context.Transactions.Any())
                {
                    context.Transactions.AddRange(
                    new Transaction
                    {
                        EmployeeID = context.Employees.FirstOrDefault(b => b.FirstName == "Allan Antonio").ID,
                        TransactionStatusID = context.TransactionStatuses.FirstOrDefault(b => b.Name == "Open").ID,
                        TransactionTypeID = context.TransactionTypes.FirstOrDefault(b => b.Name == "Thorold Spring Break").ID,
                        OriginID = context.Branches.FirstOrDefault(b => b.Name == "CAA Welland").ID,
                        DestinationID = context.Branches.FirstOrDefault(b => b.Name == "CAA Thorold").ID,
                        TransactionDate = DateTime.Today.AddDays(-20),
                        ReceivedDate = DateTime.Today.AddDays(-6),
                        Description = "House keeping items thorold team",
                        Shipment = "Thorold team will handle it"
                    },
                    new Transaction
                    {
                        EmployeeID = context.Employees.FirstOrDefault(b => b.FirstName == "Edmund Kevin").ID,
                        TransactionStatusID = context.TransactionStatuses.FirstOrDefault(b => b.Name == "Open").ID,
                        TransactionTypeID = context.TransactionTypes.FirstOrDefault(b => b.Name == "Thorold Spring Break").ID,
                        OriginID = context.Branches.FirstOrDefault(b => b.Name == "CAA Thorold").ID,
                        DestinationID = context.Branches.FirstOrDefault(b => b.Name == "CAA Welland").ID,
                        TransactionDate = DateTime.Today.AddDays(-180),
                        ReceivedDate = DateTime.Today.AddDays(-1),
                        Description = "Some swagger items - 3 safety shoes, socks",
                        Shipment = "Thorold team own truck"
                    },
                    new Transaction
                    {
                        EmployeeID = context.Employees.FirstOrDefault(b => b.FirstName == "Michael Laurence").ID,
                        TransactionStatusID = context.TransactionStatuses.FirstOrDefault(b => b.Name == "Received").ID,
                        TransactionTypeID = context.TransactionTypes.FirstOrDefault(b => b.Name == "St. Catharines Christmas Expo").ID,
                        OriginID = context.Branches.FirstOrDefault(b => b.Name == "CAA St. Catharines").ID,
                        DestinationID = context.Branches.FirstOrDefault(b => b.Name == "CAA Welland").ID,
                        TransactionDate = DateTime.Today.AddDays(-220),
                        ReceivedDate = DateTime.Today.AddDays(-6),
                        Description = "Chrismas Events Items",
                        Shipment = "No"
                    },
                    new Transaction
                    {
                        EmployeeID = context.Employees.FirstOrDefault(b => b.FirstName == "Edmund Kevin").ID,
                        TransactionStatusID = context.TransactionStatuses.FirstOrDefault(b => b.Name == "Open").ID,
                        TransactionTypeID = context.TransactionTypes.FirstOrDefault(b => b.Name == "Niagara Falls Summer Splash").ID,
                        OriginID = context.Branches.FirstOrDefault(b => b.Name == "CAA Welland").ID,
                        DestinationID = context.Branches.FirstOrDefault(b => b.Name == "CAA St. Catharines").ID,
                        TransactionDate = DateTime.Today.AddDays(-10),
                        ReceivedDate = DateTime.Today.AddDays(-2),
                        Description = "4 desks for expo event",
                        Shipment = "Test shipment"
                    },
                    new Transaction
                    {
                        EmployeeID = context.Employees.FirstOrDefault(b => b.FirstName == "Tsogt").ID,
                        TransactionStatusID = context.TransactionStatuses.FirstOrDefault(b => b.Name == "Received").ID,
                        TransactionTypeID = context.TransactionTypes.FirstOrDefault(b => b.Name == "Thorold Spring Break").ID,
                        OriginID = context.Branches.FirstOrDefault(b => b.Name == "CAA St. Catharines").ID,
                        DestinationID = context.Branches.FirstOrDefault(b => b.Name == "CAA Grimsby").ID,
                        TransactionDate = DateTime.Today.AddDays(-20),
                        ReceivedDate = DateTime.Today.AddDays(-6),
                        Description = "House keeping items thorold team",
                        Shipment = "Thorold team will handle it"


                    },
                    new Transaction
                    {
                        EmployeeID = context.Employees.FirstOrDefault(b => b.FirstName == "Allan Antonio").ID,
                        TransactionStatusID = context.TransactionStatuses.FirstOrDefault(b => b.Name == "Open").ID,
                        TransactionTypeID = context.TransactionTypes.FirstOrDefault(b => b.Name == "Niagara Falls Summer Splash").ID,
                        OriginID = context.Branches.FirstOrDefault(b => b.Name == "CAA Thorold").ID,
                        DestinationID = context.Branches.FirstOrDefault(b => b.Name == "CAA St. Catharines").ID,
                        TransactionDate = DateTime.Today.AddDays(-20),
                        ReceivedDate = DateTime.Today.AddDays(-6),
                        Description = "stock recharge",
                        Shipment = "Airport transportation"
                    },
                    new Transaction
                    {
                        EmployeeID = context.Employees.FirstOrDefault(b => b.FirstName == "Luisito Jr").ID,
                        TransactionStatusID = context.TransactionStatuses.FirstOrDefault(b => b.Name == "Released").ID,
                        TransactionTypeID = context.TransactionTypes.FirstOrDefault(b => b.Name == "St. Catharines Christmas Expo").ID,
                        OriginID = context.Branches.FirstOrDefault(b => b.Name == "CAA Grimsby").ID,
                        DestinationID = context.Branches.FirstOrDefault(b => b.Name == "CAA Thorold").ID,
                        TransactionDate = DateTime.Today.AddDays(-20),
                        ReceivedDate = DateTime.Today.AddDays(-6),
                        Description = "House keeping items - 1 broom, 2 vacuum cleaner, 2 dawn cleaner, towels ",
                        Shipment = "Bicycle"
                    },
                    new Transaction
                    {
                        EmployeeID = context.Employees.FirstOrDefault(b => b.FirstName == "Allan Antonio").ID,
                        TransactionStatusID = context.TransactionStatuses.FirstOrDefault(b => b.Name == "Released").ID,
                        TransactionTypeID = context.TransactionTypes.FirstOrDefault(b => b.Name == "St. Catharines Christmas Expo").ID,
                        OriginID = context.Branches.FirstOrDefault(b => b.Name == "CAA Welland").ID,
                        DestinationID = context.Branches.FirstOrDefault(b => b.Name == "CAA Grimsby").ID,
                        TransactionDate = DateTime.Today.AddDays(-100),
                        ReceivedDate = DateTime.Today.AddDays(-3),
                        Description = "Disco items - 2 part balloons, 4 tapes, 6 candles",
                        Shipment = "test"
                    });
                    context.SaveChanges();
                }
                //TransactionItem Seed Data
                int[] itemIDs = context.Items.Select(a => a.ID).ToArray();
                int itemIDCount = itemIDs.Length;
                int[] transactionIDs = context.Transactions.Select(a => a.ID).ToArray();
                int transactionIDCount = transactionIDs.Length;
                if (!context.TransactionItems.Any())
                {
                    foreach (int f in itemIDs)
                    {
                        TransactionItem m = new()
                        {
                            ItemID = itemIDs[random.Next(itemIDCount)],
                            TransactionID = transactionIDs[random.Next(transactionIDCount)],
                            Quantity = random.Next(1, 34)
                        };
                        context.TransactionItems.Add(m);
                    }
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
