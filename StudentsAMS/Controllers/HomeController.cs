using Microsoft.AspNetCore.Mvc;
using StudentsAMS.Models;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace StudentsAMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> GetStudentsList()
        {
            try
            {
                var firebaseDatabaseUrl = "https://attendance-tracking-system-ft-default-rtdb.firebaseio.com/";
                var dbDocument = "Student";
                string url = $"{firebaseDatabaseUrl}" +
                       $"{dbDocument}.json";
                HttpClient client = new();
                var httpResponseMessage = await client.GetAsync(url);
                List<StudentsViewModel> entries = new();

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var contentStream = await httpResponseMessage.Content.ReadAsStringAsync();
                    if (contentStream != null && contentStream != "null")
                    {
                        var result = JsonSerializer.Deserialize<Dictionary<string, StudentsViewModel>>(contentStream);
                        if (result == null)
                        {
                            //Sending empty data
                            return View(result);
                        }
                        entries = result.Select(x => x.Value).ToList();
                    }
                }

                return View(entries);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public IActionResult CreateStudent()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateStudent(StudentsViewModel studentsViewModel)
        {
            try
            {
                var firebaseDatabaseUrl = "https://attendance-tracking-system-ft-default-rtdb.firebaseio.com/";
                var dbDocument = "Student";
                studentsViewModel.Id = Guid.NewGuid().ToString("N");
                string courseJsonString = JsonSerializer.Serialize(studentsViewModel);

                var payload = new StringContent(courseJsonString, Encoding.UTF8, "application/json");

                string url = $"{firebaseDatabaseUrl}" +
                            $"{dbDocument}/" +
                            $"{studentsViewModel.Id}.json";
                HttpClient client = new();
                var httpResponseMessage = await client.PutAsync(url, payload);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var contentStream = await httpResponseMessage.Content.ReadAsStringAsync();

                    var result = JsonSerializer.Deserialize<StudentsViewModel>(contentStream);
                    return RedirectToAction("GetStudentsList");
                }
                return RedirectToAction("GetStudentsList");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IActionResult> DeleteStudent(string Id)
        {
            try
            {
                var firebaseDatabaseUrl = "https://attendance-tracking-system-ft-default-rtdb.firebaseio.com/";
                var dbDocument = "Student";
                string url = $"{firebaseDatabaseUrl}" +
                      $"{dbDocument}/" +
                      $"{Id}.json";
                HttpClient client = new();
                var httpResponseMessage = await client.DeleteAsync(url);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var contentStream = await httpResponseMessage.Content.ReadAsStringAsync();
                    if (contentStream == "null")
                    {
                        return RedirectToAction("GetStudentsList");
                    }
                }

                return RedirectToAction("GetStudentsList");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
