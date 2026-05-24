using ExpenseTracker.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;

namespace ExpenseTracker.MVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly IConfiguration configuration;
        public AuthController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel rmodel)
        {
            if (!ModelState.IsValid)
                return View(rmodel);

            string apiUrl = configuration["ApiSettings:BaseUrl"] + "auth/register";

            using (HttpClient client=new HttpClient())
            {
                var jsonData = JsonConvert.SerializeObject(rmodel);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(apiUrl,content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Registration successful. Please login.";
                    return RedirectToAction("Login");
                }
                else
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    ViewBag.Error = responseData;
                    return View(rmodel);
                }
            }
        }

        public IActionResult Login()
        {
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            string apiUrl = configuration["ApiSettings:BaseUrl"] + "auth/login";

            using (HttpClient client =new HttpClient())
            {
                var response = await client.PostAsJsonAsync(apiUrl,model);
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    var authResponse = JsonConvert.DeserializeObject<AuthResponseModel>(result);

                    HttpContext.Session.SetString("JWToken", authResponse.Token);

                    return RedirectToAction("Index", "Expense");
                }
                else
                {
                    ViewBag.Error = "Invalid email or password.";
                    return View(model);
                }
            }
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
