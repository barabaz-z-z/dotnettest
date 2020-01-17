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
            var sql = "select * from orders";

            var ds = GetData(sql);

            var result = ds.Tables.OfType<DataTable>().FirstOrDefault();

            return result;
        }
        public DataTable GetAllOrdersWithCustomers()
        {
            var sql = @"
select o.OrderId, o.OrderDate, c.CustomerId, c.CustomerFirstName, c.CustomerLastName from orders as o
join customers as c on c.CustomerId = o.OrderCustomerId
";

            var ds = GetData(sql);

            var result = ds.Tables.OfType<DataTable>().FirstOrDefault();

            return result;
        }

        public DataTable GetAllOrdersWithPriceUnder(int price)
        {
            var sql = $@"
select o.*, r.CommonPrice from orders as o
right join  
	(select oi.OrderId, sum(oi.Count * i.ItemPrice) as CommonPrice from ordersitems as oi
	join items as i on oi.itemid = i.itemid
	group by oi.OrderId
	having sum(oi.Count * i.ItemPrice) <= {price}) as r on r.orderid = o.orderid
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
declare @CustomerId int
select @CustomerId = c.customerid from customers c
inner join orders o 
on o.ordercustomerid = c.customerid
where o.orderid = {orderId}

delete oi
from orders o
left join ordersitems oi 
on oi.orderid = o.orderid
where o.ordercustomerid = @CustomerId

delete o from orders o
where o.ordercustomerid = @CustomerId

delete c from customers c
where c.customerid = @CustomerId
";

            Execute(sql);
        }

        internal DataTable GetAllItemsAndTheirOrdersCountIncludingTheItemsWithoutOrders()
        {
            var sql = @"
select i.itemid, i.itemname, count(oi.Count) Count from items i
left join ordersitems oi
on i.itemid = oi.itemid
group by i.itemid, i.itemname
";

            var ds = GetData(sql);

            var result = ds.Tables.OfType<DataTable>().FirstOrDefault();

            return result;
        }
    }
}
