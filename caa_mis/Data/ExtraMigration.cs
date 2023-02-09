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
                    SELECT s.ID, b.ID as BranchID, b.Name as BranchName, i.Name as ItemName, s.Quantity 
                    FROM Items i
                    LEFT JOIN Stocks s on i.ID = s.ItemID
                    INNER JOIN Branches b on s.BranchID = b.ID
                ");
        }
    }
}
