using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.CategoryModels;
using Service.SpotifyPodcastService;

namespace PlanningAndSkillsAppBackend.Controllers
{
    public class SpotifyPodcastController : Controller
    {
        private readonly ISpotifyPodcastService _podcastService;
        public SpotifyPodcastController(ISpotifyPodcastService podcastService)
        {
            _podcastService = podcastService;
        }

        // GET: /Podcasts?categoryId=5
        public async Task<IActionResult> Index(int categoryId)
        {
            var pods = await _podcastService.GetPodcastsByCategoryId(categoryId);
            ViewData["CategoryId"] = categoryId;
            return View(pods);
        }

        // GET: /Podcasts/Details/4
        public async Task<IActionResult> Details(int id)
        {
            var all = await _podcastService.GetPodcastsByCategoryId(0);
            var pod = all.FirstOrDefault(p => p.Id == id);
            if (pod == null) return NotFound();
            return View(pod);
        }

        // GET: /Podcasts/Create?categoryId=5
        public IActionResult Create(int categoryId)
        {
            ViewData["CategoryId"] = categoryId;
            return View(new SpotifyPodcastModel { CategoryId = categoryId });
        }

        // POST: /Podcasts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SpotifyPodcastModel model)
        {
            if (!ModelState.IsValid) return View(model);
            await _podcastService.AddPodcast(model);
            return RedirectToAction(
                actionName: "CategoryDetails",      // the action on CategoriesController
                controllerName: "Categories",       // the controller name (no "Controller" suffix)
                routeValues: new { id = model.CategoryId }
            );
        }

        // GET: /Podcasts/Edit/4
        public async Task<IActionResult> Edit(int id)
        {
            var all = await _podcastService.GetPodcastsByCategoryId(0);
            var pod = all.FirstOrDefault(p => p.Id == id);
            if (pod == null) return NotFound();
            return View(pod);
        }

        // POST: /Podcasts/Edit/4
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SpotifyPodcastModel model)
        {
            if (!ModelState.IsValid) return View(model);
            await _podcastService.UpdatePodcast(model);
            return RedirectToAction(
                actionName: "CategoryDetails",      // the action on CategoriesController
                controllerName: "Categories",       // the controller name (no "Controller" suffix)
                routeValues: new { id = model.CategoryId }
            );
        }

        // GET: /Podcasts/Delete/4
        public async Task<IActionResult> Delete(int id)
        {
            var all = await _podcastService.GetPodcastsByCategoryId(0);
            var pod = all.FirstOrDefault(p => p.Id == id);
            if (pod == null) return NotFound();
            return View(pod);
        }

        // POST: /Podcasts/Delete/4
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int categoryId)
        {
            await _podcastService.DeletePodcast(id);
            return RedirectToAction(
                actionName: "CategoryDetails",      // the action on CategoriesController
                controllerName: "Categories",       // the controller name (no "Controller" suffix)
                routeValues: new { id = categoryId }
            );
        }
    }
}
