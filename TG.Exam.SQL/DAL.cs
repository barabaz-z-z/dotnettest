using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TG.Exam.SQL
{
    public class DAL
    {
        private SqlConnection GetConnection()
        {
            var connectionString = ConfigurationManager.AppSettings["ConnectionString"];

            var con = new SqlConnection(connectionString);

            con.Open();

            return con;
        }

        private DataSet GetData(string sql)
        {
            var ds = new DataSet();

            using (var con = GetConnection())
            {
                using (var cmd = new SqlCommand(sql, con))
                {
                    using (var adp = new SqlDataAdapter(cmd))
                    {
                        adp.Fill(ds);
                    }
                }
            }

            return ds;
        }

        private void Execute(string sql)
        {
            using (var con = GetConnection())
            {
                using (var cmd = new SqlCommand(sql, con))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public DataTable GetAllOrders()
        {
            var sql = @"
SELECT OrderId, OrderCustomerId, OrderDate 
FROM Orders
";

            var ds = GetData(sql);

            var result = ds.Tables.OfType<DataTable>().FirstOrDefault();

            return result;
        }
        public DataTable GetAllOrdersWithCustomers()
        {
            var sql = @"
SELECT o.OrderId, 
       o.OrderDate, 
       c.CustomerId, 
       c.CustomerFirstName, 
       c.CustomerLastName 
FROM   Orders AS o 
       JOIN Customers AS c 
         ON c.Customerid = o.Ordercustomerid 
";

            var ds = GetData(sql);

            var result = ds.Tables.OfType<DataTable>().FirstOrDefault();

            return result;
        }

        public DataTable GetAllOrdersWithPriceUnder(int price)
        {
            var sql = $@"
SELECT o.*, 
       r.CommonPrice 
FROM   Orders AS o 
       RIGHT JOIN (SELECT oi.OrderId, 
                          Sum(oi.Count * i.ItemPrice) AS CommonPrice 
                   FROM   OrdersItems AS oi 
                          JOIN Items AS i 
                            ON oi.ItemId = i.ItemId 
                   GROUP  BY oi.OrderId 
                   HAVING Sum(oi.Count * i.ItemPrice) <= {price}) AS r 
               ON r.OrderId = o.OrderId 
";
            var ds = GetData(sql);

            var result = ds.Tables.OfType<DataTable>().FirstOrDefault();

            return result;
        }

        public void DeleteCustomer(int orderId)
        {
            // 1. search customer id related to the order and put it into variable
            // 2. remove all pairs order-item
            // 3. remove all orders
            // 4. remove customer
            var sql = $@"
DECLARE @CustomerId INT 

SELECT @CustomerId = c.CustomerId 
FROM   Customers c 
       INNER JOIN Orders o 
               ON o.OrderCustomerId = c.CustomerId 
WHERE  o.OrderId = {orderId} 

DELETE oi 
FROM   Orders o 
       LEFT JOIN OrdersItems oi 
              ON oi.OrderId = o.OrderId 
WHERE  o.OrderCustomerId = @CustomerId 

DELETE o 
FROM   Orders o 
WHERE  o.OrderCustomerId = @CustomerId 

DELETE c 
FROM   Customers c 
WHERE  c.CustomerId = @CustomerId 
";

            Execute(sql);
        }

        internal DataTable GetAllItemsAndTheirOrdersCountIncludingTheItemsWithoutOrders()
        {
            var sql = @"
SELECT i.ItemId, 
       i.ItemName, 
       Count(oi.Count) Count 
FROM   Items i 
       LEFT JOIN OrdersItems oi 
              ON i.ItemId = oi.ItemId 
GROUP  BY i.ItemId, 
          i.ItemName 
";

            var ds = GetData(sql);

            var result = ds.Tables.OfType<DataTable>().FirstOrDefault();

            return result;
        }
    }
}
