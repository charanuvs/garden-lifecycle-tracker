using GardenTracker.Domain.Enums;
using GardenTracker.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace GardenTracker.Web.Controllers;

public class UserCropsController : Controller
{
    private readonly CropWorkflowService _workflowService;

    public UserCropsController(CropWorkflowService workflowService)
    {
        _workflowService = workflowService;
    }

    // GET: UserCrops
    public async Task<IActionResult> Index()
    {
        var userCrops = await _workflowService.GetAllActiveUserCropsAsync();
        return View(userCrops);
    }

    // GET: UserCrops/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var nextSteps = await _workflowService.GetNextStepsForCropAsync(id);
        ViewBag.UserCropId = id;
        return View(nextSteps);
    }

    // POST: UserCrops/CompleteStep
    [HttpPost]
    public async Task<IActionResult> CompleteStep(int stepId, string? notes)
    {
        try
        {
            var step = await _workflowService.TransitionStepAsync(stepId, WorkflowStepTrigger.Complete, notes);
            return RedirectToAction(nameof(Details), new { id = step.UserCropId });
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: UserCrops/StartStep
    [HttpPost]
    public async Task<IActionResult> StartStep(int stepId)
    {
        try
        {
            var step = await _workflowService.TransitionStepAsync(stepId, WorkflowStepTrigger.Start);
            return RedirectToAction(nameof(Details), new { id = step.UserCropId });
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}
