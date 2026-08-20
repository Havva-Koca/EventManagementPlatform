using EventManagement.Data.Model.Entities;
using EventManagement.Data.Model.Enums;
using EventManagement.Data.Repositories.Interfaces;
using EventManagement.Services.Interfaces;
using EventManagement.Services.Results;
using Microsoft.AspNetCore.Identity;

namespace EventManagement.Services.Implementations;

public class OrganizerRequestService : IOrganizerRequestService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;

    public OrganizerRequestService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<OrganizerRequestResult> SubmitRequestAsync(string userId)
    {
        var existingRequests = await _unitOfWork.OrganizerRequests.GetAllAsync();
        bool hasPendingRequest = existingRequests.Any(r =>
            r.UserId == userId && r.Status == RequestStatus.Pending);

        if (hasPendingRequest)
        {
            return OrganizerRequestResult.AlreadyPending;
        }

        var request = new OrganizerRequest
        {
            UserId = userId,
            RequestedAt = DateTime.UtcNow,
            Status = RequestStatus.Pending
        };

        await _unitOfWork.OrganizerRequests.AddAsync(request);
        await _unitOfWork.SaveChangesAsync();

        return OrganizerRequestResult.Success;
    }

    public async Task<OrganizerRequestResult> ApproveRequestAsync(int requestId, string reviewerId)
    {
        var request = await _unitOfWork.OrganizerRequests.GetByIdAsync(requestId);

        if (request == null)
        {
            return OrganizerRequestResult.NotFound;
        }

        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user != null)
        {
            await _userManager.AddToRoleAsync(user, "Organizer");
        }

        request.Status = RequestStatus.Approved;
        request.ReviewedAt = DateTime.UtcNow;
        request.ReviewedByUserId = reviewerId;

        _unitOfWork.OrganizerRequests.Update(request);
        await _unitOfWork.SaveChangesAsync();

        return OrganizerRequestResult.Success;
    }

    public async Task<OrganizerRequestResult> RejectRequestAsync(int requestId, string reviewerId)
    {
        var request = await _unitOfWork.OrganizerRequests.GetByIdAsync(requestId);

        if (request == null)
        {
            return OrganizerRequestResult.NotFound;
        }

        request.Status = RequestStatus.Rejected;
        request.ReviewedAt = DateTime.UtcNow;
        request.ReviewedByUserId = reviewerId;

        _unitOfWork.OrganizerRequests.Update(request);
        await _unitOfWork.SaveChangesAsync();

        return OrganizerRequestResult.Success;
    }
}