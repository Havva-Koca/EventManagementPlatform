using EventManagement.Data.Common;
using EventManagement.Data.Model.Entities;
using EventManagement.Services.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventManagement.Services.Interfaces;



public interface IRegistrationService
{
    Task<RegistrationResult> RegisterAsync(int eventId, string userId);
    Task<RegistrationResult> CancelAsync(int eventId, string userId);
    Task<PagedResult<Registration>> GetMyRegistrationsAsync(string userId, int pageNumber, int pageSize);
}
