using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model.CategoryModels;
using Service.CategoryService;
using Service.SpotifyPodcastService;
using Service.YoutubeVideoService;

namespace PlanningAndSkillsAppBackend.Controllers
{
    public class CategoriesController : Controller
    {
        //    // GET: CategoriesController
        //    public ActionResult Index()
        //    {
        //        return View();
        //    }

        private readonly ICategoryService _categoryService;
        private readonly IYoutubeVideoService _youtubeVideoService;
        private readonly ISpotifyPodcastService _spotifyPodcastService;

        public CategoriesController(ICategoryService categoryService, IYoutubeVideoService youtubeVideoService, ISpotifyPodcastService spotifyPodcastService)
        {
            _categoryService = categoryService;
            _youtubeVideoService = youtubeVideoService;
            _spotifyPodcastService = spotifyPodcastService;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllAsync();
            return View(categories);
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> CategoryDetails(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            // 2) Grab its videos & podcasts however your data layer works:
            var videos = await _youtubeVideoService.GetVideosByCategoryId(id);
            var podcasts = await _spotifyPodcastService.GetPodcastsByCategoryId(id);

            // 3) Assign them into the model:
            category.YoutubeVideoModel = videos;
            category.SpotifyPodcastModel = podcasts;
            return View("CategoryDetails", category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryModel category)
        {
            if (!ModelState.IsValid)
                return View(category);

            await _categoryService.CreateAsync(category);
            return RedirectToAction(nameof(Index));
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            return View(category);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryModel category)
        {
            if (id != category.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(category);

            await _categoryService.UpdateAsync(category);
            return RedirectToAction(nameof(Index));
        }

        // GET: Categories/Delete/5
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var category = await _categoryService.GetByIdAsync(id);
        //    if (category == null)
        //        return NotFound();

        //    return View(category);
        //}

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _categoryService.DeleteAsync(id);

            return RedirectToAction(nameof(Index));
        }
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    await _categoryService.DeleteAsync(id);
        //    return RedirectToAction(nameof(Index));
        //}

        [HttpGet("api/getCategories")]
        public async Task<IActionResult> GetAllCategoriesApi()
        {
            try
            {
                var categories = await _categoryService.GetAllAsync();

                //For each category, fetch its videos & podcasts
                foreach (var cat in categories)
                {
                    cat.YoutubeVideoModel = await _youtubeVideoService.GetVideosByCategoryId(cat.Id);
                    cat.SpotifyPodcastModel = await _spotifyPodcastService.GetPodcastsByCategoryId(cat.Id);
                }

                return Ok(categories);
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        [HttpGet("api/categories/get_categories")]
        public async Task<IActionResult> GetCategoryNames()
        {
            var categories = await _categoryService.GetAllAsync();
            var names = categories.Select(c =>  c.Name ).ToList();
            return Ok(names);
        }
    }
}
