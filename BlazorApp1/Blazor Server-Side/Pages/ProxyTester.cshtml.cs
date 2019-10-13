using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlazorServerSide.Pages
{
    public class ProxyTesterModel : PageModel
    {
        public string Result { get; set; }
        static HttpClient httpClient = new HttpClient();
        public async Task OnGetAsync()
        {
            var result = await httpClient.GetAsync("https://www.nu.nl/");
            Result = "Requesting external url succeeded";
        }
    }
}
