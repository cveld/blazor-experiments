using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServerSide.Models
{
    public class Vacation
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string OriginalPrice { get; set; }
        public string Price { get; set; }
        public string Rating { get; set; }
        public int Stars { get; set; }
        public ICollection<User> Likes { get; set; }
        public int Persons { get; set; }
        public int SleepingRooms { get; set; }
        public string Location { get; set; }
        public string ImageUrl { get; set; }
        public string PriceUnit { get; set; }
        public Vacation()
        {
            //Likes = new HashSet<User>();
        }
    }
}
