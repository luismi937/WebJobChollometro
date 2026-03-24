using Microsoft.AspNetCore.Mvc;
using MvcNetCoreChollometro.Models;

namespace MvcNetCoreChollometro.Controllers
{
    public class ChollosController : Controller
    {
        private Repositories.RepositoryChollometro repo;
        public ChollosController(Repositories.RepositoryChollometro repo)
        {
            this.repo = repo;
        }
        public async Task<IActionResult> Index()
        {
            List<Chollo> chollos = await this.repo.GetChollosAsync();
            return View(chollos);
        }
    }
}
