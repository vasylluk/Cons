using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace TestConsoleApp1
{
    class Program
    {
        class Delivery
        {
            public int Id;
            public double Amount;
            public decimal Price;
            public DateTime Date;
            public string Supplier;
        }

        static void Main(string[] args)
        {
            var names = new[]
            {
                new {Id = 1, Name = "Butter"},
                new {Id = 2, Name = "Cheese"},
                new {Id = 3, Name = "Oil"},
                new {Id = 4, Name = "Sausage"},
                new {Id = 6, Name = "Sausage"},
                new {Id = 5, Name = "Cheese"},
            };
            var deliveries = new List<Delivery>()
            {
                new Delivery(){Id = 1, Amount = 100, Price = 25, Date = new DateTime(2018, 5, 10), Supplier = "Supp1"},
                new Delivery(){Id = 4, Amount = 50, Price = 30, Date = new DateTime(2018, 2, 12), Supplier = "Supp2"},
                new Delivery(){Id = 2, Amount = 60, Price = 20, Date = new DateTime(2018, 4, 15), Supplier = "Supp1"},
                new Delivery(){Id = 3, Amount = 60, Price = 100, Date = new DateTime(2017, 4, 15), Supplier = "Supp2"},
                new Delivery(){Id = 1, Amount = 30, Price = 30, Date = new DateTime(2018, 5, 10), Supplier = "Supp1"},
                new Delivery(){Id = 4, Amount = 40, Price = 120, Date = new DateTime(2018, 5, 20), Supplier = "Supp2"},
                new Delivery(){Id = 2, Amount = 80, Price = 26, Date = new DateTime(2018, 5, 18), Supplier = "Supp3"},
                new Delivery(){Id = 5, Amount = 25, Price = 110, Date = new DateTime(2018, 4, 30), Supplier = "Supp3"},
                new Delivery(){Id = 6, Amount = 20, Price = 120, Date = new DateTime(2018, 5, 10), Supplier = "Supp2"}
            };
            var sales = new[]
            {
                new {Id = 4, Amount = 3, Price = 150, Date = new DateTime(2018, 5, 16)},
                new {Id = 1, Amount = 5, Price = 27, Date = new DateTime(2018, 5, 16)},
                new {Id = 4, Amount = 1, Price = 24, Date = new DateTime(2018, 5, 14)},
                new {Id = 2, Amount = 5, Price = 25, Date = new DateTime(2018, 5, 17)},
                new {Id = 5, Amount = 2, Price = 125, Date = new DateTime(2018, 5, 16)},
                new {Id = 2, Amount = 3, Price = 26, Date = new DateTime(2018, 5, 18)},
                new {Id = 6, Amount = 10, Price = 35, Date = new DateTime(2018, 5, 20)}
            };
            Console.WriteLine("Task 1");
            Console.Write("Input id: ");
            var id = int.Parse(Console.ReadLine());
            var query1 = from sale in sales
                         where sale.Id == id
                         select sale.Price * sale.Amount;
            Console.WriteLine($"Average value is {query1.Average()}");
            Console.WriteLine("Task 2");
            var query2 = from delivery in deliveries
                         where (DateTime.Now.Date - delivery.Date).TotalDays <= 30
                         group delivery by delivery.Id into deliveryGroup
                         orderby deliveryGroup.Count()
                         select new { Id = deliveryGroup.Key, Count = deliveryGroup.Count() };
            PrintCollection(query2);
            Console.WriteLine("Task 3");
            var query3 = from n in names
                         join delivery in deliveries on n.Id equals delivery.Id
                         group delivery.Amount by new { n.Name, delivery.Supplier } into group3
                         select new { group3.Key.Name, group3.Key.Supplier, Total = group3.Sum() };
            PrintCollection(query3);
            Console.WriteLine("Task 4");
            var subQuery = from delivery in deliveries
                           group delivery.Price by delivery.Id into deliveryGroup
                           select new { Id = deliveryGroup.Key, MaxPrice = deliveryGroup.Max() };
            var query4 = from sale in sales
                         join item in subQuery on sale.Id equals item.Id
                         where sale.Price < 1.05m * item.MaxPrice
                         select sale;
            PrintCollection(query4);
            Console.WriteLine("Task 5");
            var subQuery5 = from n in names
                            join sale in sales on n.Id equals sale.Id
                            group sale by n.Name into saleGroup
                            select new { Name = saleGroup.Key, Count = saleGroup.Count() };
            var maxCount = subQuery5.Max(x => x.Count);
            var query5 = from item in subQuery5
                         where item.Count == maxCount
                         select item.Name;
            PrintCollection(query5);
            //----------------------------------------------------------------------------------------------------
            Console.WriteLine("Task 6");

            var a = from d in deliveries
                    group d.Price by d.Id into g
                    select new { Id = g.Key, MinPrice = g.Min() };

            var b =  from s in sales
                     group s.Price by s.Id into g
                     select new { Id = g.Key, MaxPrice = g.Max() };

            var c = from q1 in b
                    select q1.Id;

            var e = from n in names
                      where (!c.Contains(n.Id))
                      select new {  n.Id, MaxPrice = -1 };

            b = b.Concat(e);

            var r = from n in names
                    join q1 in b on n.Id equals q1.Id
                    join q2 in a on n.Id equals q2.Id
                    select new { n.Name, q2.MinPrice, q1.MaxPrice };

            PrintCollection(r);
            //----------------------------------------------------------------------------------------------------
            Console.WriteLine("Task 7");
            var query7d = from n in names
                          join delivery in deliveries on n.Id equals delivery.Id
                          group delivery.Amount by n.Name into deliveryGroup
                          select new { Name = deliveryGroup.Key, input = deliveryGroup.Sum() };
            var query7s = from n in names
                          join sale in sales on n.Id equals sale.Id
                          group sale.Amount by n.Name into saleGroup
                          select new { Name = saleGroup.Key, output = saleGroup.Sum() };
            var remainder = (from item1 in query7d
                             join item2 in query7s on item1.Name equals item2.Name
                             select new { item1.Name, Remainder = item1.input - item2.output }).
                         ToDictionary(x => x.Name, x => x.Remainder);
            PrintCollection(remainder);
        }

        static void PrintCollection(IEnumerable collection)
        {
            foreach (var item in collection)
                Console.WriteLine(item);
        }
    }
}
