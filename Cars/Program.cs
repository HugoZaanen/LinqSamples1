using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cars
{
    class Program
    {
        static void Main(string[] args)
        {
            var cars = ProcessFile("fuel.csv");
            var manufacturers = ProcessManufacturer("manufacturers.cvs");

            var query =
                from car in cars
                join manufacturer in manufacturers 
                    on car.Manufacturer equals manufacturer.Name
                orderby car.Combined descending, car.Name ascending
                select new
                {
                    manufacturer.Headquarters,
                    car.Name,
                    car.Combined
                };

            var query2 =
                cars.Join(manufacturers,
                    c => c.Manufacturer,
                    m => m.Name,
                    (c, m) => new
                    {
                        Car = c,
                        Manufacturer = m
                    })
                .OrderByDescending(c => c.Car.Combined)
                .ThenBy(c => c.Car.Name)
                .Select(c => new
                {
                    c.Manufacturer.Headquarters,
                    c.Car.Name,
                    c.Car.Combined
                });

            //var top =
            //        cars.OrderByDescending(c => c.Combined)
            //        .ThenBy(c => c.Name)
            //        .Select(c => c)
            //        .First(c => c.Manufacturer == "BMW" && c.Year == 2016); 

            foreach (var car in query.Take(10))
            {
                Console.WriteLine($"{car.Headquarters} {car.Name}: {car.Combined}");
            }
        }

        private static List<Manufacturer> ProcessManufacturer(string path)
        {
            var query =
                File.ReadAllLines(path)
                .Where(l => l.Length > 1)
                .Select(l =>
                {
                    var columns = l.Split(',');
                    return new Manufacturer
                    {
                        Name = columns[0],
                        Headquarters = columns[1],
                        Year = int.Parse(columns[2])
                    };
                });

            return query.ToList();
        }

        private static List<Car> ProcessFile(string path)
        {
            var query =
                File.ReadAllLines(path)
                .Skip(1)
                .Where(l => l.Length > 1)
                .ToCar();

            return query.ToList();
        }      
    }

    public static class CarExtensions
    {
        public static IEnumerable<Car> ToCar(this IEnumerable<string> source)
        {
            foreach(var lines in source)
            {
                var columns = lines.Split(',');

                yield return new Car
                {
                    Year = int.Parse(columns[0]),
                    Manufacturer = columns[1],
                    Name = columns[2],
                    Displacement = double.Parse(columns[3]),
                    Cylinders = int.Parse(columns[4]),
                    City = int.Parse(columns[5]),
                    Highway = int.Parse(columns[6]),
                    Combined = int.Parse(columns[7])
                };
            }
        }
    }
}
