using GardenTracker.Domain.Enums;
using GardenTracker.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GardenTracker.Web.Controllers;

[Authorize]
public class UserCropsController : Controller
{
    private readonly CropWorkflowService _workflowService;

    public UserCropsController(CropWorkflowService workflowService)
    {
        _workflowService = workflowService;
    }

    private string GetCurrentUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("User not authenticated");

    // GET: UserCrops
    public async Task<IActionResult> Index()
    {
        var userId = GetCurrentUserId();
        var userCrops = await _workflowService.GetAllActiveUserCropsAsync(userId);
        return View(userCrops);
    }

    // GET: UserCrops/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var userId = GetCurrentUserId();
        var nextSteps = await _workflowService.GetNextStepsForCropAsync(id, userId);
        ViewBag.UserCropId = id;
        return View(nextSteps);
    }

    // POST: UserCrops/CompleteStep
    [HttpPost]
    public async Task<IActionResult> CompleteStep(int stepId, string? notes)
    {
        try
        {
            var userId = GetCurrentUserId();
            var step = await _workflowService.TransitionStepAsync(stepId, WorkflowStepTrigger.Complete, userId, notes);
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
            var userId = GetCurrentUserId();
            var step = await _workflowService.TransitionStepAsync(stepId, WorkflowStepTrigger.Start, userId);
            return RedirectToAction(nameof(Details), new { id = step.UserCropId });
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: UserCrops/Archive
    [HttpPost]
    public async Task<IActionResult> Archive(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _workflowService.ArchiveCropAsync(id, userId);
            TempData["Success"] = "Crop archived successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: UserCrops/Archived
    public async Task<IActionResult> Archived()
    {
        var userId = GetCurrentUserId();
        var archivedCrops = await _workflowService.GetArchivedUserCropsAsync(userId);
        return View(archivedCrops);
    }

    // POST: UserCrops/Unarchive
    [HttpPost]
    public async Task<IActionResult> Unarchive(int id)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _workflowService.UnarchiveCropAsync(id, userId);
            TempData["Success"] = "Crop restored successfully!";
            return RedirectToAction(nameof(Archived));
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Archived));
        }
    }
}
