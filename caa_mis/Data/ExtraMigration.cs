using Microsoft.EntityFrameworkCore.Migrations;

namespace caa_mis.Data
{
    public static class ExtraMigration
    {
        public static void Steps(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
             @"
                    Create View StockSummaryByBranch as
                    SELECT s.ID, b.ID as BranchID, b.Name as BranchName, 
                            i.Name as ItemName, i.Cost as ItemCost, s.Quantity, s.MinLevel                        
                    FROM Items i
                    LEFT JOIN Stocks s on i.ID = s.ItemID
                    INNER JOIN Branches b on s.BranchID = b.ID
                ");

            migrationBuilder.Sql(
             @"
                    CREATE VIEW TransactionSummary AS
                    SELECT t.ID, t.EmployeeID, e.FirstName AS EmployeeName,
                           t.OriginID, b1.Name AS OriginName,
                           t.DestinationID, b2.Name AS DestinationName,
                           t.TransactionStatusID, ts.Name AS TransactionStatusName,
                           t.TransactionTypeID, tt.Name AS TransactionTypeName,
                           t.TransactionDate, t.ReceivedDate,
                           t.Description, t.Shipment
                    FROM Transactions t
                    INNER JOIN Employees e ON t.EmployeeID = e.ID
                    INNER JOIN Branches b1 ON t.OriginID = b1.ID
                    INNER JOIN Branches b2 ON t.DestinationID = b2.ID
                    INNER JOIN TransactionStatuses ts ON t.TransactionStatusID = ts.ID
                    INNER JOIN TransactionTypes tt ON t.TransactionTypeID = tt.ID
                ");
            migrationBuilder.Sql(
            @"
                    CREATE VIEW TransactionItemSummary AS
                    SELECT ti.ID, ti.ItemID, i.Name AS ItemName,
                           ti.TransactionID, e.FirstName AS EmployeeName,                            
                           t.OriginID, b1.Name AS OriginName,                           
                           t.DestinationID, b2.Name AS DestinationName,
                           ts.Name AS TransactionStatusName,
                           ti.Quantity
                    FROM TransactionItems ti
                    INNER JOIN Items i ON ti.ItemID = i.ID
                    INNER JOIN Transactions t ON ti.TransactionID = t.ID
                    INNER JOIN Employees e ON t.EmployeeID = e.ID
                    INNER JOIN Branches b1 ON t.OriginID = b1.ID
                    INNER JOIN Branches b2 ON t.DestinationID = b2.ID
                    INNER JOIN TransactionStatuses ts ON t.TransactionStatusID = ts.ID
                ");

        }
    }
}
