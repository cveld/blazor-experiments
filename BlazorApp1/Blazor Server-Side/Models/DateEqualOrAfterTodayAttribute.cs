using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServerSide.Models
{
    // took from https://stackoverflow.com/questions/46184818/dataanotation-to-validate-a-model-how-do-i-validate-it-so-that-the-date-is-not
    public class DateEqualOrAfterTodayAttribute : ValidationAttribute
    {
        public override string FormatErrorMessage(string name)
        {
            return $"The {name} field should be a future date.";
        }

        protected override ValidationResult IsValid(object objValue,
                                                       ValidationContext validationContext)
        {
            var dateValue = objValue as DateTime? ?? new DateTime();            

            if (dateValue.ToLocalTime().Date < DateTime.UtcNow.Date)
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName), new string[] { validationContext.MemberName });
            }
            return ValidationResult.Success;
        }
    }
}
