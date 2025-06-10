using System.Reflection;
using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.CategoryModels;
using Service.YoutubeVideoService;

namespace PlanningAndSkillsAppBackend.Controllers
{
    public class YoutubeVideoController : Controller
    {
        private readonly IYoutubeVideoService _videoService;
        public YoutubeVideoController(IYoutubeVideoService videoService)
        {
            _videoService = videoService;
        }

        // GET: /Videos?categoryId=5
        public async Task<IActionResult> Index(int categoryId)
        {
            var videos = await _videoService.GetVideosByCategoryId(categoryId);
            ViewData["CategoryId"] = categoryId;
            return View(videos);
        }

        // GET: /Videos/Details/3
        public async Task<IActionResult> Details(int id)
        {
            // Optionally you could add a GetById in your service
            var all = await _videoService.GetVideosByCategoryId(0);
            var video = all.FirstOrDefault(v => v.Id == id);
            if (video == null) return NotFound();
            return View(video);
        }

        // GET: /Videos/Create?categoryId=5
        public IActionResult Create(int categoryId)
        {
            ViewData["CategoryId"] = categoryId;
            return View(new YoutubeVideoModel { CategoryId = categoryId });
        }

        // POST: /Videos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(YoutubeVideoModel model)
        {
            if (!ModelState.IsValid) return View(model);
            await _videoService.AddVideo(model);
            return RedirectToAction(
                actionName: "CategoryDetails",      // the action on CategoriesController
                controllerName: "Categories",       // the controller name (no "Controller" suffix)
                routeValues: new { id = model.CategoryId }
            );
        }

        // GET: /Videos/Edit/3
        public async Task<IActionResult> Edit(int id)
        {
            var all = await _videoService.GetVideosByCategoryId(0);
            var video = all.FirstOrDefault(v => v.Id == id);
            if (video == null) return NotFound();
            return View(video);
        }

        // POST: /Videos/Edit/3
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(YoutubeVideoModel model)
        {
            if (!ModelState.IsValid) return View(model);
            await _videoService.UpdateVideo(model);
            return RedirectToAction(
                actionName: "CategoryDetails",      // the action on CategoriesController
                controllerName: "Categories",       // the controller name (no "Controller" suffix)
                routeValues: new { id = model.CategoryId }
            );
        }

        // GET: /Videos/Delete/3
        public async Task<IActionResult> Delete(int id)
        {
            var all = await _videoService.GetVideosByCategoryId(0);
            var video = all.FirstOrDefault(v => v.Id == id);
            if (video == null) return NotFound();
            return View(video);
        }

        // POST: /Videos/Delete/3
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int categoryId)
        {
            await _videoService.DeleteVideo(id);
            return RedirectToAction(
                actionName: "CategoryDetails",      // the action on CategoriesController
                controllerName: "Categories",       // the controller name (no "Controller" suffix)
                routeValues: new { id = categoryId }
            );
        }
    }
}
