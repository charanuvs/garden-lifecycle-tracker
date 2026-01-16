using GardenTracker.Domain.Interfaces;
using GardenTracker.Infrastructure.Services;
using GardenTracker.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace GardenTracker.Web.Controllers;

public class CropsController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly CropWorkflowService _workflowService;

    public CropsController(IUnitOfWork unitOfWork, CropWorkflowService workflowService)
    {
        _unitOfWork = unitOfWork;
        _workflowService = workflowService;
    }

    // GET: Crops
    public async Task<IActionResult> Index()
    {
        var crops = await _unitOfWork.CropDefinitions.GetAllAsync();
        return View(crops);
    }

    // GET: Crops/Start/5
    public async Task<IActionResult> Start(int id)
    {
        var crop = await _unitOfWork.CropDefinitions.GetByIdAsync(id);
        if (crop == null)
        {
            return NotFound();
        }

        var viewModel = new StartCropViewModel
        {
            CropDefinitionId = crop.Id,
            CropName = crop.Name,
            StartDate = DateTime.Today
        };

        return View(viewModel);
    }

    // POST: Crops/Start
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Start(StartCropViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var userCrop = await _workflowService.StartCropAsync(
                model.CropDefinitionId,
                model.Nickname,
                model.StartDate);

            return RedirectToAction("Details", "UserCrops", new { id = userCrop.Id });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
    }
}

