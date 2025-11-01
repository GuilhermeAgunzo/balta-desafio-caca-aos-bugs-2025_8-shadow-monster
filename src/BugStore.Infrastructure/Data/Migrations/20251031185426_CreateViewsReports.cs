using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BugStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateViewsReports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            CREATE VIEW vwCustomerOrderSummary AS
            SELECT
                o.CustomerId,
                c.Name AS CustomerName,
                COUNT(o.Id) AS TotalOrders,
                SUM(ol.Quantity) AS QtProducts,
                SUM(ol.Total) AS TotalSpent
            FROM Orders o
            INNER JOIN Customers c 
	            ON c.Id = o.CustomerId
            INNER JOIN OrderLines ol 
	            ON ol.OrderId = o.Id
            GROUP BY o.CustomerId;
            ");

            migrationBuilder.Sql(@"
            CREATE INDEX IF NOT EXISTS idx_orders_customerid ON Orders(CustomerId);
            CREATE INDEX IF NOT EXISTS idx_orderlines_orderid ON OrderLines(OrderId);
            CREATE INDEX IF NOT EXISTS idx_customers_id ON Customers(Id);
            CREATE INDEX IF NOT EXISTS idx_orders_createdat ON Orders(CreatedAt);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS vw_CustomerOrderSummary;");

            migrationBuilder.Sql("DROP INDEX IF EXISTS idx_orders_customerid;");
            migrationBuilder.Sql("DROP INDEX IF EXISTS idx_orderlines_orderid;");
            migrationBuilder.Sql("DROP INDEX IF EXISTS idx_customers_id;");
            migrationBuilder.Sql("DROP INDEX IF EXISTS idx_orders_createdat;");
        }
    }
}
