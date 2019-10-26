using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServerSide.Models
{    
    public class BookVacationFormModel
    {            
        public string EmailAddress { get; set; }       
        public string PhoneNumberPrefix { get; set; }        
        public string PhoneNumber { get; set; }        
        public DateTime? BookingDate { get; set; }                
    }
}
