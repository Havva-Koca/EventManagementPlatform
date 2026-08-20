using EventManagement.Data.Model.Entities;
using EventManagement.Data.Repositories.Interfaces;
using EventManagement.Web.Models.CategoryViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManagement.Web.Controllers;

[Authorize(Roles = "Admin")]
public class CategoriesController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoriesController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<IActionResult> Index()
    {
        var categories = await _unitOfWork.Categories.GetAllAsync();
        return View(categories);
    }
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var category = new Category
        {
            Name = model.Name,
            Description = model.Description
        };

        await _unitOfWork.Categories.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction("Index");
    }
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);

        if (category == null)
        {
            return NotFound();
        }

        var model = new CategoryViewModel
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };

        return View(model);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CategoryViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var category = await _unitOfWork.Categories.GetByIdAsync(model.Id);

        if (category == null)
        {
            return NotFound();
        }

        category.Name = model.Name;
        category.Description = model.Description;

        _unitOfWork.Categories.Update(category);
        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction("Index");
    }
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);

        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);

        if (category == null)
        {
            return NotFound();
        }

        _unitOfWork.Categories.Delete(category);
        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction("Index");
    }
}