﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using caa_mis.Data;

#nullable disable

namespace caa_mis.Data.CAAMigrations
{
    [DbContext(typeof(InventoryContext))]
    partial class InventoryContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.14");

            modelBuilder.Entity("caa_mis.Models.Branch", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.ToTable("Branches");
                });

            modelBuilder.Entity("caa_mis.Models.Bulk", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("BranchID")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<int>("EmployeeID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TransactionStatusID")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("BranchID");

                    b.HasIndex("EmployeeID");

                    b.HasIndex("TransactionStatusID");

                    b.ToTable("Bulks");
                });

            modelBuilder.Entity("caa_mis.Models.BulkItem", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("BulkID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ItemID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Quantity")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("BulkID");

                    b.HasIndex("ItemID");

                    b.ToTable("BulkItems");
                });

            modelBuilder.Entity("caa_mis.Models.Category", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("caa_mis.Models.Employee", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("caa_mis.Models.Event", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("BranchID")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Date")
                        .HasColumnType("TEXT");

                    b.Property<int>("EmployeeID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<int>("TransactionStatusID")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("BranchID");

                    b.HasIndex("EmployeeID");

                    b.HasIndex("TransactionStatusID");

                    b.HasIndex("Name", "Date")
                        .IsUnique();

                    b.ToTable("Events");
                });

            modelBuilder.Entity("caa_mis.Models.EventItem", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("EventID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ItemID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Quantity")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("EventID");

                    b.HasIndex("ItemID");

                    b.ToTable("EventItems");
                });

            modelBuilder.Entity("caa_mis.Models.Item", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CategoryID")
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("Cost")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<int>("ItemStatusID")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("ManufacturerID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MinLevel")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("SKUNumber")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("Scale")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("CategoryID");

                    b.HasIndex("ItemStatusID");

                    b.HasIndex("ManufacturerID");

                    b.HasIndex("SKUNumber")
                        .IsUnique();

                    b.ToTable("Items");
                });

            modelBuilder.Entity("caa_mis.Models.ItemPhoto", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("Content")
                        .HasColumnType("BLOB");

                    b.Property<int>("ItemID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("MimeType")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("ItemID")
                        .IsUnique();

                    b.ToTable("ItemPhotos");
                });

            modelBuilder.Entity("caa_mis.Models.ItemStatus", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.ToTable("ItemStatuses");
                });

            modelBuilder.Entity("caa_mis.Models.ItemSupplier", b =>
                {
                    b.Property<int>("SupplierID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ItemID")
                        .HasColumnType("INTEGER");

                    b.HasKey("SupplierID", "ItemID");

                    b.HasIndex("ItemID");

                    b.ToTable("ItemSuppliers");
                });

            modelBuilder.Entity("caa_mis.Models.ItemThumbnail", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("Content")
                        .HasColumnType("BLOB");

                    b.Property<int>("ItemID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("MimeType")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("ItemID")
                        .IsUnique();

                    b.ToTable("ItemThumbnails");
                });

            modelBuilder.Entity("caa_mis.Models.Manufacturer", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("Manufacturers");
                });

            modelBuilder.Entity("caa_mis.Models.Stock", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("BranchID")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("ItemID")
                        .IsRequired()
                        .HasColumnType("INTEGER");

                    b.Property<int>("MinLevel")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Quantity")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("BranchID");

                    b.HasIndex("ItemID");

                    b.ToTable("Stocks");
                });

            modelBuilder.Entity("caa_mis.Models.Supplier", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Address1")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("Address2")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("TEXT");

                    b.Property<string>("PostalCode")
                        .IsRequired()
                        .HasMaxLength(6)
                        .HasColumnType("TEXT");

                    b.Property<string>("Province")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SupplierName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("Suppliers");
                });

            modelBuilder.Entity("caa_mis.Models.Transaction", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<int?>("DestinationID")
                        .IsRequired()
                        .HasColumnType("INTEGER");

                    b.Property<int>("EmployeeID")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("OriginID")
                        .IsRequired()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("ReceivedDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Shipment")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("TransactionDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("TransactionStatusID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TransactionTypeID")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("DestinationID");

                    b.HasIndex("EmployeeID");

                    b.HasIndex("OriginID");

                    b.HasIndex("TransactionStatusID");

                    b.HasIndex("TransactionTypeID");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("caa_mis.Models.TransactionItem", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool?>("IsEdited")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ItemID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Quantity")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("ReceivedQuantity")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("StockID")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TransactionID")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.HasIndex("ItemID");

                    b.HasIndex("StockID");

                    b.HasIndex("TransactionID");

                    b.ToTable("TransactionItems");
                });

            modelBuilder.Entity("caa_mis.Models.TransactionStatus", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.ToTable("TransactionStatuses");
                });

            modelBuilder.Entity("caa_mis.Models.TransactionType", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<int>("InOut")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.ToTable("TransactionTypes");
                });

            modelBuilder.Entity("caa_mis.ViewModels.EventSummaryVM", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("BranchID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("BranchName")
                        .HasColumnType("TEXT");

                    b.Property<int>("EmployeeID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("EmployeeName")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("EventDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("EventID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("EventName")
                        .HasColumnType("TEXT");

                    b.Property<int>("EventQuantity")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ItemID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ItemName")
                        .HasColumnType("TEXT");

                    b.Property<int>("TransactionStatusID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("TransactionStatusName")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToView("EventSummary");
                });

            modelBuilder.Entity("caa_mis.ViewModels.StockSummaryByBranchVM", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("BranchID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("BranchName")
                        .HasColumnType("TEXT");

                    b.Property<double>("ItemCost")
                        .HasColumnType("REAL");

                    b.Property<string>("ItemName")
                        .HasColumnType("TEXT");

                    b.Property<int>("MinLevel")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Quantity")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.ToView("StockSummaryByBranch");
                });

            modelBuilder.Entity("caa_mis.ViewModels.TransactionItemSummaryVM", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("DestinationID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("DestinationName")
                        .HasColumnType("TEXT");

                    b.Property<string>("EmployeeName")
                        .HasColumnType("TEXT");

                    b.Property<int>("ItemID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ItemName")
                        .HasColumnType("TEXT");

                    b.Property<int>("OriginID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("OriginName")
                        .HasColumnType("TEXT");

                    b.Property<int>("Quantity")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TransactionID")
                        .HasColumnType("INTEGER");

                    b.Property<string>("TransactionStatusName")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToView("TransactionItemSummary");
                });

            modelBuilder.Entity("caa_mis.Models.Bulk", b =>
                {
                    b.HasOne("caa_mis.Models.Branch", "Branch")
                        .WithMany("Bulks")
                        .HasForeignKey("BranchID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("caa_mis.Models.Employee", "Employee")
                        .WithMany("Bulks")
                        .HasForeignKey("EmployeeID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("caa_mis.Models.TransactionStatus", "TransactionStatus")
                        .WithMany("Bulks")
                        .HasForeignKey("TransactionStatusID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Branch");

                    b.Navigation("Employee");

                    b.Navigation("TransactionStatus");
                });

            modelBuilder.Entity("caa_mis.Models.BulkItem", b =>
                {
                    b.HasOne("caa_mis.Models.Bulk", "Bulk")
                        .WithMany("BulkItems")
                        .HasForeignKey("BulkID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("caa_mis.Models.Item", "Item")
                        .WithMany("BulkItems")
                        .HasForeignKey("ItemID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Bulk");

                    b.Navigation("Item");
                });

            modelBuilder.Entity("caa_mis.Models.Event", b =>
                {
                    b.HasOne("caa_mis.Models.Branch", "Branch")
                        .WithMany("Events")
                        .HasForeignKey("BranchID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("caa_mis.Models.Employee", "Employee")
                        .WithMany("Events")
                        .HasForeignKey("EmployeeID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("caa_mis.Models.TransactionStatus", "TransactionStatus")
                        .WithMany("Events")
                        .HasForeignKey("TransactionStatusID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Branch");

                    b.Navigation("Employee");

                    b.Navigation("TransactionStatus");
                });

            modelBuilder.Entity("caa_mis.Models.EventItem", b =>
                {
                    b.HasOne("caa_mis.Models.Event", "Event")
                        .WithMany("EventItems")
                        .HasForeignKey("EventID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("caa_mis.Models.Item", "Item")
                        .WithMany("EventItems")
                        .HasForeignKey("ItemID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Event");

                    b.Navigation("Item");
                });

            modelBuilder.Entity("caa_mis.Models.Item", b =>
                {
                    b.HasOne("caa_mis.Models.Category", "Category")
                        .WithMany("Items")
                        .HasForeignKey("CategoryID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("caa_mis.Models.ItemStatus", "ItemStatus")
                        .WithMany("Items")
                        .HasForeignKey("ItemStatusID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("caa_mis.Models.Manufacturer", "Manufacturer")
                        .WithMany("Items")
                        .HasForeignKey("ManufacturerID");

                    b.Navigation("Category");

                    b.Navigation("ItemStatus");

                    b.Navigation("Manufacturer");
                });

            modelBuilder.Entity("caa_mis.Models.ItemPhoto", b =>
                {
                    b.HasOne("caa_mis.Models.Item", "Item")
                        .WithOne("ItemPhoto")
                        .HasForeignKey("caa_mis.Models.ItemPhoto", "ItemID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Item");
                });

            modelBuilder.Entity("caa_mis.Models.ItemSupplier", b =>
                {
                    b.HasOne("caa_mis.Models.Item", "Item")
                        .WithMany("ItemSuppliers")
                        .HasForeignKey("ItemID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("caa_mis.Models.Supplier", "Supplier")
                        .WithMany("ItemSuppliers")
                        .HasForeignKey("SupplierID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Item");

                    b.Navigation("Supplier");
                });

            modelBuilder.Entity("caa_mis.Models.ItemThumbnail", b =>
                {
                    b.HasOne("caa_mis.Models.Item", "Item")
                        .WithOne("ItemThumbnail")
                        .HasForeignKey("caa_mis.Models.ItemThumbnail", "ItemID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Item");
                });

            modelBuilder.Entity("caa_mis.Models.Stock", b =>
                {
                    b.HasOne("caa_mis.Models.Branch", "Branch")
                        .WithMany("Stocks")
                        .HasForeignKey("BranchID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("caa_mis.Models.Item", "Item")
                        .WithMany("Stocks")
                        .HasForeignKey("ItemID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Branch");

                    b.Navigation("Item");
                });

            modelBuilder.Entity("caa_mis.Models.Transaction", b =>
                {
                    b.HasOne("caa_mis.Models.Branch", "Destination")
                        .WithMany("Destinations")
                        .HasForeignKey("DestinationID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("caa_mis.Models.Employee", "Employee")
                        .WithMany("Transactions")
                        .HasForeignKey("EmployeeID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("caa_mis.Models.Branch", "Origin")
                        .WithMany("Origins")
                        .HasForeignKey("OriginID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("caa_mis.Models.TransactionStatus", "TransactionStatus")
                        .WithMany("Transactions")
                        .HasForeignKey("TransactionStatusID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("caa_mis.Models.TransactionType", "TransactionType")
                        .WithMany("Transactions")
                        .HasForeignKey("TransactionTypeID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Destination");

                    b.Navigation("Employee");

                    b.Navigation("Origin");

                    b.Navigation("TransactionStatus");

                    b.Navigation("TransactionType");
                });

            modelBuilder.Entity("caa_mis.Models.TransactionItem", b =>
                {
                    b.HasOne("caa_mis.Models.Item", "Item")
                        .WithMany("TransactionItems")
                        .HasForeignKey("ItemID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("caa_mis.Models.Stock", null)
                        .WithMany("TransactionItems")
                        .HasForeignKey("StockID");

                    b.HasOne("caa_mis.Models.Transaction", "Transaction")
                        .WithMany("TransactionItems")
                        .HasForeignKey("TransactionID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Item");

                    b.Navigation("Transaction");
                });

            modelBuilder.Entity("caa_mis.Models.Branch", b =>
                {
                    b.Navigation("Bulks");

                    b.Navigation("Destinations");

                    b.Navigation("Events");

                    b.Navigation("Origins");

                    b.Navigation("Stocks");
                });

            modelBuilder.Entity("caa_mis.Models.Bulk", b =>
                {
                    b.Navigation("BulkItems");
                });

            modelBuilder.Entity("caa_mis.Models.Category", b =>
                {
                    b.Navigation("Items");
                });

            modelBuilder.Entity("caa_mis.Models.Employee", b =>
                {
                    b.Navigation("Bulks");

                    b.Navigation("Events");

                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("caa_mis.Models.Event", b =>
                {
                    b.Navigation("EventItems");
                });

            modelBuilder.Entity("caa_mis.Models.Item", b =>
                {
                    b.Navigation("BulkItems");

                    b.Navigation("EventItems");

                    b.Navigation("ItemPhoto");

                    b.Navigation("ItemSuppliers");

                    b.Navigation("ItemThumbnail");

                    b.Navigation("Stocks");

                    b.Navigation("TransactionItems");
                });

            modelBuilder.Entity("caa_mis.Models.ItemStatus", b =>
                {
                    b.Navigation("Items");
                });

            modelBuilder.Entity("caa_mis.Models.Manufacturer", b =>
                {
                    b.Navigation("Items");
                });

            modelBuilder.Entity("caa_mis.Models.Stock", b =>
                {
                    b.Navigation("TransactionItems");
                });

            modelBuilder.Entity("caa_mis.Models.Supplier", b =>
                {
                    b.Navigation("ItemSuppliers");
                });

            modelBuilder.Entity("caa_mis.Models.Transaction", b =>
                {
                    b.Navigation("TransactionItems");
                });

            modelBuilder.Entity("caa_mis.Models.TransactionStatus", b =>
                {
                    b.Navigation("Bulks");

                    b.Navigation("Events");

                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("caa_mis.Models.TransactionType", b =>
                {
                    b.Navigation("Transactions");
                });
#pragma warning restore 612, 618
        }
    }
}
