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

            var vacations = new Vacation[]
            {
                new Vacation{Title="My vacation"}
            };
            foreach (var v in vacations)
            {
                context.Vacations.Add(v);
            }
            context.SaveChanges();
        }
    }
}
