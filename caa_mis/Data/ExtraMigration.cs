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
                    Create View TransactionSummaryVM as
                    SELECT t.ID, t.EmployeeID as EmployeeID, e.Name as EmployeeName,
                            t.OrigingID, b.Name as OriginName,
                            t.DestinationID, b.Name as DestinationName,
                            t.TransactionStatusID, ts.Name as TransactionStatusName,
                            t.TransactionTypeID, tt.Name as TransactionTypeName,
                            t.TransactionDate, t.ReceivedDate,
                            t.Description, t.Shipment
                    FROM Transactions t
                    LEFT JOIN Employees e on t.EmployeeID = e.ID
                    LEFT JOIN Branches b on t.OriginID = b.ID
                    LEFT JOIN Branches b on t.DestinationID = b.ID
                    LEFT JOIN TransactionStatuses ts on t.TransactionStatusID = ts.ID
                    LEFT JOIN TransactionTypes tt on t.TransactionTypeID = tt.ID
                ");

        }
    }
}
