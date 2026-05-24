using ExpenseTracker.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Reflection.Metadata;

namespace ExpenseTracker.MVC.Controllers
{
    public class ExpenseController : Controller
    {
        private readonly IConfiguration configuration;
        public ExpenseController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public async Task<IActionResult> Index()
        {
            string token=HttpContext.Session.GetString("JWToken");
            if(String.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login","Auth");
            }

            string apiurl = configuration["ApiSettings:BaseUrl"] + "categories";

            using (HttpClient client =new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token);
                var response=await client.GetAsync(apiurl);
                if(response.IsSuccessStatusCode)
                {
                    var result =await response.Content.ReadAsStringAsync();
                    var categories=JsonConvert.DeserializeObject<List<CategoryModel>>(result);
                    ViewBag.CategoryList = new SelectList(categories, "CategoryId","Name");
                }
            }
            return View(new ExpenseModel());
        }
    }
}
