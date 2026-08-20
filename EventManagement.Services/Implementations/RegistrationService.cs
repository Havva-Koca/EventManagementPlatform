using EventManagement.Data.Model.Entities;
using EventManagement.Data.Model.Enums;
using EventManagement.Data.Repositories.Interfaces;
using EventManagement.Services.Interfaces;
using EventManagement.Services.Results;

namespace EventManagement.Services.Implementations;

public class RegistrationService : IRegistrationService
{
    private readonly IUnitOfWork _unitOfWork;

    public RegistrationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<RegistrationResult> RegisterAsync(int eventId, string userId)
    {
        var eventItem = await _unitOfWork.Events.GetByIdAsync(eventId);
        if (eventItem == null)
        {
            return RegistrationResult.EventNotFound;
        }

        if (eventItem.OrganizerId == userId)
        {
            return RegistrationResult.OwnByOrganizer;
        }

        var existingRegistration = await _unitOfWork.Registrations.GetByEventAndUserAsync(eventId, userId);

        if (existingRegistration != null && existingRegistration.Status == RegistrationStatus.Confirmed)
        {
            return RegistrationResult.AlreadyRegistered;
        }

        var confirmedCount = await _unitOfWork.Registrations.GetConfirmedCountAsync(eventId);
        if (confirmedCount >= eventItem.Capacity)
        {
            return RegistrationResult.EventFull;
        }

        if (existingRegistration != null)
        {
            existingRegistration.Status = RegistrationStatus.Confirmed;
            _unitOfWork.Registrations.Update(existingRegistration);
        }
        else
        {
            var registration = new Registration
            {
                EventId = eventId,
                UserId = userId,
                Status = RegistrationStatus.Confirmed
            };
            await _unitOfWork.Registrations.AddAsync(registration);
        }

        await _unitOfWork.SaveChangesAsync();

        return RegistrationResult.Success;
    }

    public async Task<RegistrationResult> CancelAsync(int eventId, string userId)
    {
        var registration = await _unitOfWork.Registrations.GetByEventAndUserAsync(eventId, userId);

        if (registration == null || registration.Status != RegistrationStatus.Confirmed)
        {
            return RegistrationResult.RegistrationNotFound;
        }

        registration.Status = RegistrationStatus.Cancelled;
        _unitOfWork.Registrations.Update(registration);
        await _unitOfWork.SaveChangesAsync();

        return RegistrationResult.Success;
    }
}