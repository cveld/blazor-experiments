using BlazorServerSide.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServerSide.Data
{
    public class VacationsDbInitializer
    {
        public static void Initialize(VacationContext context)
        {
            context.Database.EnsureCreated();

            // Look for any vacations.
            if (context.Vacations.Any())
            {
                return;   // DB has been seeded
            }

            var vacations = new VacationModel[]
            {
                new VacationModel
                {
                    Title = "8 persoons vakantie huis in Humble",
                    ImageUrl = "artiom-vallat-H59WxBNSQY8-unsplash.jpg",
                    Price = "359",
                    Rating = "...",
                    PriceUnit = "1 week va.",
                    Location = "Humble, Denemarken",
                    Persons = 8,
                    SleepingRooms = 4,
                    Stars = 3
                },
                new VacationModel
                {
                    Title = "Modern strandhuis in Bergen aan Zee nabij het centrum", 
                    ImageUrl = "andrew-ruiz-fmz-B9At9iQ-unsplash.jpg",
                    Price = "845", 
                    OriginalPrice = "926",
                    Rating = "8.6",
                    Stars = 3,
                    Persons = 10,
                    SleepingRooms = 5,
                    Location = "Bergen aan Zee, Nederland",
                    PriceUnit = "1 week va."
                }
            };
            foreach (var v in vacations)
            {
                context.Vacations.Add(v);
            }
            context.SaveChanges();
        }
    }
}
