using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using log4net;
using log4net.Config;

namespace TG.Exam.Refactoring
{
    public class OrderService : IOrderService
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(OrderService));

        private readonly Stopwatch stopWatch = new Stopwatch();
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["OrdersDBConnectionString"].ConnectionString;

        // it is better to use ConcurrentDictionary because it is intended for threadsafe manipulation
        // made it private, if it is public, so name should start with capital letter and it should be property instead of field
        private ConcurrentDictionary<string, Order> cache = new ConcurrentDictionary<string, Order>();

        public OrderService()
        {
            BasicConfigurator.Configure();
        }

        public Order LoadOrder(string orderId)
        {
            try
            {
                Debug.Assert(null != orderId && orderId != "");

                return Benchmark(LoadOrderInternal, orderId);
            }
            catch (SqlException ex)
            {
                logger.Error(ex.Message);
                throw new ApplicationException("Error");
            }
        }

        private Order LoadOrderInternal(string orderId)
        {
            // made code more friendly calling the method TryGetValue instead of the method ContainsKey, 
            if (cache.TryGetValue(orderId, out Order value))
                return value;

            // simplified it a bit using string interpolation
            string query = $@"
SELECT OrderId, OrderCustomerId, OrderDate
    FROM dbo.Orders where OrderId='{orderId}'
";

            // wrap command by 'using' because at least SqlConnection implements IDisposable
            // also calling the Dispose method inside of 'using' closes connection implicitly
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // wrap command by 'using' because at least SqlCommand implements IDisposable
                using (var command = new SqlCommand(query, connection))
                // wrap reader by 'using' because at least SqlDataReader implements IDisposable 
                // also calling the Dispose method inside of 'using' closes reader implicitly
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // applied index to avoid magic numbers and for convenience
                        var index = 0;
                        var order = new Order
                        {
                            // use explicit relevant methods instead of using cast, it looks more friendly
                            OrderId = reader.GetInt32(index),
                            OrderCustomerId = reader.GetInt32(++index),
                            OrderDate = reader.GetDateTime(++index)
                        };

                        // simplified code for caching process, made it more friendly
                        cache.TryAdd(orderId, order);

                        return order;
                    }
                }
            }

            return null;
        }

        // extracted stopwatch stuff into separate method to comply DRY approach
        private TResult Benchmark<T, TResult>(Func<T, TResult> func, T parameter)
        {
            stopWatch.Start();
            TResult result = func(parameter);
            stopWatch.Stop();
            logger.InfoFormat("Elapsed - {0}", stopWatch.Elapsed);

            return result;
        }
    }
}
