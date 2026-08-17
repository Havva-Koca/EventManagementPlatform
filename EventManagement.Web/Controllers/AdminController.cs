using EventManagement.Data.Model.Enums;
using EventManagement.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EventManagement.Data.Model.Entities;

namespace EventManagement.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }
    public async Task<IActionResult> PendingRequests()
    {
        var pendingRequests = await _unitOfWork.OrganizerRequests.GetPendingRequestsWithUserAsync();
        return View(pendingRequests);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApproveRequest(int id)
    {
        var request = await _unitOfWork.OrganizerRequests.GetByIdAsync(id);

        if (request == null)
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user != null)
        {
            await _userManager.AddToRoleAsync(user, "Organizer");
        }

        request.Status = RequestStatus.Approved;
        request.ReviewedAt = DateTime.UtcNow;
        request.ReviewedByUserId = _userManager.GetUserId(User);

        _unitOfWork.OrganizerRequests.Update(request);
        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction("PendingRequests");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RejectRequest(int id)
    {
        var request = await _unitOfWork.OrganizerRequests.GetByIdAsync(id);

        if (request == null)
        {
            return NotFound();
        }

        request.Status = RequestStatus.Rejected;
        request.ReviewedAt = DateTime.UtcNow;
        request.ReviewedByUserId = _userManager.GetUserId(User);

        _unitOfWork.OrganizerRequests.Update(request);
        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction("PendingRequests");
    }
}