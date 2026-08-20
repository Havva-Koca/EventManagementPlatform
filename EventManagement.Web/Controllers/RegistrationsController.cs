using EventManagement.Data.Model.Entities;
using EventManagement.Data.Model.Enums;
using EventManagement.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EventManagement.Web.Controllers;

[Authorize]
public class RegistrationsController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public RegistrationsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(int eventId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var eventItem = await _unitOfWork.Events.GetByIdAsync(eventId);
        if (eventItem == null)
            return NotFound();

        // Organizer, kendi düzenlediği event'e katılımcı olarak kayıt olamaz.
        if (eventItem.OrganizerId == userId)
            return Forbid();

        var existingRegistration = await _unitOfWork.Registrations.GetByEventAndUserAsync(eventId, userId);

        // Zaten aktif bir kaydı varsa tekrar işlem yapmaya gerek yok.
        if (existingRegistration != null && existingRegistration.Status == RegistrationStatus.Confirmed)
        {
            return RedirectToAction("Details", "Events", new { id = eventId });
        }

        var confirmedCount = await _unitOfWork.Registrations.GetConfirmedCountAsync(eventId);
        if (confirmedCount >= eventItem.Capacity)
        {
            TempData["Message"] = "This event is no longer accepting registrations — it's full.";
            return RedirectToAction("Details", "Events", new { id = eventId });
        }

        if (existingRegistration != null)
        {
            // Daha önce iptal edilmiş bir kayıt var — unique index yüzünden yeni satır
            // ekleyemiyoruz, mevcut satırı tekrar Confirmed'e çeviriyoruz.
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

        return RedirectToAction("Details", "Events", new { id = eventId });
    }
    public IActionResult Index()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int eventId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var registration = await _unitOfWork.Registrations.GetByEventAndUserAsync(eventId, userId);

        // Kayıt hiç yoksa ya da zaten iptal edilmişse, iptal edecek bir şey yok.
        if (registration == null || registration.Status != RegistrationStatus.Confirmed)
            return NotFound();

        registration.Status = RegistrationStatus.Cancelled;
        _unitOfWork.Registrations.Update(registration);
        await _unitOfWork.SaveChangesAsync();

        return RedirectToAction("Details", "Events", new { id = eventId });
    }
}
