using EventManagement.Data.Common;
using EventManagement.Data.Model.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventManagement.Data.Repositories.Interfaces;

public interface IRegistrationRepository :IGenericRepository<Registration>
{
    Task<Registration?>GetByEventAndUserAsync(int eventId, string userId);
   
    Task<int> GetConfirmedCountAsync(int eventId);
    Task<PagedResult<Registration>> GetByUserIdAsync(string userId, int pageNumber, int pageSize);
}
