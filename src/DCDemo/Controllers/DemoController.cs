using Microsoft.AspNet.Mvc;
using Orders;
using RavenDemo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DCDemo.Controllers
{
    public class DemoController
    {
        [Route("/api/demo")]
        public IList<Order> Get()
        {
            using (var Session = DocumentStoreFactory.Store.OpenSession())
            {
                return Session.Query<Order>().ToList();
            }
        }

        [Route("/api/discounts")]
        public IList<Order> GetDiscounts()
        {
            using (var Session = DocumentStoreFactory.Store.OpenSession())
            {
                return Session.Query<Order>()
                    .Where(o => o.Lines.Any(line => line.Discount > 0))
                    .ToList();
            }
        }

        [Route("/api/create")]
        public dynamic CreateAThing()
        {
            using (var Session = DocumentStoreFactory.Store.OpenSession())
            {
                var order = new Order()
                {
                    Company = "companies/85",
                    Lines = new List<OrderLine>(),
                    OrderedAt = DateTime.Now
                };

                var newThing = new { Demo = "demo" };

                Session.Store(order);
                Session.SaveChanges();
            }

            return new { result = "OK" };
        }

        [Route("/api/deepquery/{id}")]
        public dynamic GetCompanyToo(string id)
        {
            using (var Session = DocumentStoreFactory.Store.OpenSession())
            {
                var order = Session
                    .Include<Order>(o => o.Company)
                    .Load("Orders/" + id);
                var company = Session.Load<Company>(order.Company);

                return new
                {
                    Company = company,
                    Order = order
                };
            }
        }
    }
}
