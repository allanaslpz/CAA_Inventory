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

                Random random = new Random();

                // Inventory Status Seed Data //
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
                    });
                    context.SaveChanges();
                }

                // Vendors Seed Data //
                if (!context.Manufacturers.Any())
                {
                    context.Manufacturers.AddRange(
                    new Manufacturer
                    {
                        Name = "Walmart",
                        Address1 = "102 Primeway Dr",
                        City = "Welland",
                        Province = "ON",
                        PostalCode = "L3B0A1",
                        Phone = "9057353500",
                        Email = "walmart.welland@gmail.com",
                        Status = Archived.Enabled
                    },
                    new Manufacturer
                    {
                        Name = "Sobeys",
                        Address1 = "400 Scott St",
                        City = "St. Catharines",
                        Province = "ON",
                        PostalCode = "L2M3W4",
                        Phone = "9059359974",
                        Email = "sobeys.scott.niagara@outlook.com",
                        Status = Archived.Enabled
                    },
                    new Manufacturer
                    {
                        Name = "Staples",
                        Address1 = "7190 Morrison St",
                        City = "Niagara Falls",
                        Province = "ON",
                        PostalCode = "L2E7K5",
                        Phone = "9053580650",
                        Email = "staples.falls@yahoo.com",
                        Status = Archived.Enabled
                    },
                    new Manufacturer
                    {
                        Name = "Costco Wholesale",
                        Address1 = "7500 Pin Oak Dr",
                        City = "Niagara Falls",
                        Province = "ON",
                        PostalCode = "L2H2E9",
                        Phone = "3654470200",
                        Email = "costco.niagara@aol.com",
                        Status = Archived.Enabled
                    },
                    new Manufacturer
                    {
                        Name = "Canadian Tire",
                        Address1 = "158 Primeway Dr",
                        City = "Welland",
                        Province = "ON",
                        PostalCode = "L3B0A1",
                        Phone = "9057327501",
                        Email = "canadian.tire@icloud.com",
                        Status = Archived.Enabled
                    },
                    new Manufacturer
                    {
                        Name = "RONA",
                        Address1 = "359 S Service Rd",
                        City = "Grimsby",
                        Province = "ON",
                        PostalCode = "L3M4E8",
                        Phone = "9053091959",
                        Email = "rona.grimsby@protonmail.com",
                        Status = Archived.Enabled
                    },
                    new Manufacturer
                    {
                        Name = "Test CAA Site",
                        Address1 = "3271 Schmon Pkwy",
                        City = "Thorold",
                        Province = "ON",
                        PostalCode = "L2V4Y6",
                        Phone = "9059848585",
                        Email = "test.site@zoho.com",
                        Status = Archived.Disabled
                    });
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
                        Email = "admin@outlook.com"
                    },
                    new Employee
                    {
                        FirstName = "Allan Antonio",
                        LastName = "Lopez",
                        Email = "super@outlook.com"
                    },
                    new Employee
                    {
                        FirstName = "Michael Laurence",
                        LastName = "Barde",
                        Email = "user@outlook.com"
                    },
                    new Employee
                    {
                        FirstName = "Luisito Jr",
                        LastName = "Villaflor",
                        Email = "lui@outlook.com"
                    },
                    new Employee
                    {
                        FirstName = "Tsogt",
                        LastName = "Batjargal",
                        Email = "tsogo@outlook.com"
                    });
                    context.SaveChanges();
                }
                // Branch Seed Data //
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
                        SKUNumber = "CAA685349",
                        Cost = 20,
                        Description = "Cool and trending sunglasses",
                        ItemStatusID = context.ItemStatuses.FirstOrDefault(s => s.Name == "Active").ID,
                        CategoryID = context.Categories.FirstOrDefault(c => c.Name == "Swag").ID,
                        ManufacturerID = context.Manufacturers.FirstOrDefault(m => m.Name == "Walmart").ID,
                    },
                    new Item
                    {
                        Name = "Pen",
                        SKUNumber = "CAA780017",
                        Cost = 5,
                        Description = "Clickable pen",
                        ItemStatusID = context.ItemStatuses.FirstOrDefault(s => s.Name == "Active").ID,
                        CategoryID = context.Categories.FirstOrDefault(c => c.Name == "Swag").ID,
                        ManufacturerID = context.Manufacturers.FirstOrDefault(m => m.Name == "Staples").ID,
                    },
                    new Item
                    {
                        Name = "Towel",
                        SKUNumber = "CAA814972",
                        Cost = 15,
                        Description = "100% Cotton Towels",
                        ItemStatusID = context.ItemStatuses.FirstOrDefault(s => s.Name == "Active").ID,
                        CategoryID = context.Categories.FirstOrDefault(c => c.Name == "Swag").ID,
                        ManufacturerID = context.Manufacturers.FirstOrDefault(m => m.Name == "Costco Wholesale").ID,
                    },
                    new Item
                    {
                        Name = "T-Shirt",
                        SKUNumber = "CAA929919",
                        Cost = 20,
                        Description = "White T-Shirt with CAA Logo",
                        ItemStatusID = context.ItemStatuses.FirstOrDefault(s => s.Name == "Active").ID,
                        CategoryID = context.Categories.FirstOrDefault(c => c.Name == "Swag").ID,
                        ManufacturerID = context.Manufacturers.FirstOrDefault(m => m.Name == "Walmart").ID,
                    },
                    new Item
                    {
                        Name = "Tent",
                        SKUNumber = "CAA183952",
                        Cost = 200,
                        Description = "Large red tent",
                        ItemStatusID = context.ItemStatuses.FirstOrDefault(s => s.Name == "Active").ID,
                        CategoryID = context.Categories.FirstOrDefault(c => c.Name == "Equipment").ID,
                        ManufacturerID = context.Manufacturers.FirstOrDefault(m => m.Name == "Canadian Tire").ID,
                    },
                    new Item
                    {
                        Name = "Table",
                        SKUNumber = "CAA399773",
                        Cost = 120,
                        Description = "White long foldable table",
                        ItemStatusID = context.ItemStatuses.FirstOrDefault(s => s.Name == "Active").ID,
                        CategoryID = context.Categories.FirstOrDefault(c => c.Name == "Equipment").ID,
                        ManufacturerID = context.Manufacturers.FirstOrDefault(m => m.Name == "RONA").ID,
                    },
                    new Item
                    {
                        Name = "Table Cloth",
                        SKUNumber = "CAA30812",
                        Cost = 20,
                        Description = "Blue table cloth ",
                        ItemStatusID = context.ItemStatuses.FirstOrDefault(s => s.Name == "Active").ID,
                        CategoryID = context.Categories.FirstOrDefault(c => c.Name == "Equipment").ID,
                        ManufacturerID = context.Manufacturers.FirstOrDefault(m => m.Name == "Sobeys").ID,
                    },
                    new Item
                    {
                        Name = "Roll-up Poster",
                        SKUNumber = "CAA610986",
                        Cost = 40,
                        Description = "Large Poster of CAA",
                        ItemStatusID = context.ItemStatuses.FirstOrDefault(s => s.Name == "Active").ID,
                        CategoryID = context.Categories.FirstOrDefault(c => c.Name == "Equipment").ID,
                        ManufacturerID = context.Manufacturers.FirstOrDefault(m => m.Name == "Test CAA Site").ID,
                    },
                    new Item
                    {
                        Name = "Sticker",
                        SKUNumber = "CAA216285",
                        Cost = 5,
                        Description = "Giveaway Stickers with CAA brand",
                        ItemStatusID = context.ItemStatuses.FirstOrDefault(s => s.Name == "Active").ID,
                        CategoryID = context.Categories.FirstOrDefault(c => c.Name == "Swag").ID,
                        ManufacturerID = context.Manufacturers.FirstOrDefault(m => m.Name == "Staples").ID,
                    },
                    new Item
                    {
                        Name = "Chair",
                        SKUNumber = "CAA975524",
                        Cost = 40,
                        Description = "Foldable Chair",
                        ItemStatusID = context.ItemStatuses.FirstOrDefault(s => s.Name == "Active").ID,
                        CategoryID = context.Categories.FirstOrDefault(c => c.Name == "Equipment").ID,
                        ManufacturerID = context.Manufacturers.FirstOrDefault(m => m.Name == "Costco Wholesale").ID,
                    },
                    new Item
                    {
                        Name = "Laptop",
                        SKUNumber = "CAA480919",
                        Cost = 1500,
                        Description = "2023 Lenovo T480 ThinkPad",
                        ItemStatusID = context.ItemStatuses.FirstOrDefault(s => s.Name == "Active").ID,
                        CategoryID = context.Categories.FirstOrDefault(c => c.Name == "Equipment").ID,
                        ManufacturerID = context.Manufacturers.FirstOrDefault(m => m.Name == "Staples").ID,
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
                    },
                    new Stock
                    {
                        BranchID = context.Branches.FirstOrDefault(b => b.Location == "Thorold").ID,
                        ItemID = context.Items.FirstOrDefault(i => i.Name == "Laptop").ID,
                        MinLevel = 1,
                        Quantity = 5,
                    });
                    context.SaveChanges();
                }

                //Transaction Seed Data
                byte rowCount = 150;
                int[] employeeIDs = context.Employees.Select(a => a.ID).ToArray();
                int employeeIDCount = employeeIDs.Length;
                int[] transactionStatusIDs = context.TransactionStatuses.Select(a => a.ID).ToArray();
                int transactionStatusIDCount = transactionStatusIDs.Length;
                int[] transactionTypeIDs = context.TransactionTypes.Select(a => a.ID).ToArray();
                int transactionTypeIDCount = transactionTypeIDs.Length;
                int[] branchIDs = context.Branches.Select(a => a.ID).ToArray();
                int branchIDCount = branchIDs.Length;
                string[] shipment = new string[] { "Bus", "Train", "Bicycle", "Taxi", "Own Ride" };
                int shipCount = shipment.Length;
                string[] description = new string[] { "Lead Guitar", "Base Guitar", "Drums", "Keyboards", "Lead Vocals", "Harmonica", "Didgeridoo" };
                int descCount = description.Length;

                if (!context.Transactions.Any())
                {
                    for (byte i = 1; i <= rowCount; i++)
                    {
                        Transaction t = new()
                        {
                            EmployeeID = employeeIDs[random.Next(employeeIDCount)],
                            TransactionStatusID = transactionStatusIDs[random.Next(transactionStatusIDCount)],
                            TransactionTypeID = transactionTypeIDs[random.Next(transactionTypeIDCount)],
                            OriginID = branchIDs[random.Next(branchIDCount - 1)],
                            DestinationID = branchIDs[random.Next(branchIDCount - 1)],
                            TransactionDate = DateTime.Today.AddDays(random.Next(-100, 6)),
                            ReceivedDate = DateTime.Today.AddDays(random.Next(-100, 6)),
                            Description = description[random.Next(descCount)] + " " + description[random.Next(descCount)],
                            Shipment = shipment[random.Next(shipCount)]
                        };
                        context.Transactions.Add(t);
                    }
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

                //Event Seed Data
                string[] eventName = new string[] { "National Indigenous Day", "Canada Day", "Funding",
                                                    "Saint-Patricks Day", "Niagara College Graduation", "Canada 150",
                "Games of La Francophonie", "Canadian Multiculturalism Day", "Boxing Day"};
                int eventNameCount = eventName.Length;
                if (!context.Events.Any())
                {
                    for (byte i = 0; i < eventNameCount; i++)
                    {
                        Event e = new()
                        {
                            Name = eventName[i],
                            EmployeeID = employeeIDs[random.Next(employeeIDCount)],
                            TransactionStatusID = transactionStatusIDs[random.Next(transactionStatusIDCount)],
                            BranchID = branchIDs[random.Next(branchIDCount - 1)],
                            Date = DateTime.Today.AddDays(random.Next(-100, 6))
                        };
                        context.Events.Add(e);
                    }
                    context.SaveChanges();
                }

                //EventItem Seed Data
                int[] eventIDs = context.Events.Select(a => a.ID).ToArray();
                int eventIDCount = eventIDs.Length;
                if (!context.EventItems.Any())
                {
                    for (byte i = 0; i < eventNameCount; i++)
                    {
                        EventItem ei = new()
                        {
                            ItemID = itemIDs[random.Next(itemIDCount)],
                            EventID = eventIDs[i],
                            Quantity = random.Next(1, 34)
                        };
                        context.EventItems.Add(ei);

                        EventItem ei2 = new()
                        {
                            ItemID = itemIDs[random.Next(itemIDCount)],
                            EventID = eventIDs[i],
                            Quantity = random.Next(1, 34)
                        };
                        context.EventItems.Add(ei2);


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
