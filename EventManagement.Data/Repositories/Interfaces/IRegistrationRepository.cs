using System;
using System.Collections.Generic;
using System.Text;
using EventManagement.Data.Model.Entities;

namespace EventManagement.Data.Repositories.Interfaces;

public interface IRegistrationRepository :IGenericRepository<Registration>
{
    Task<Registration?>GetByEventAndUserAsync(int eventId, string userId);
   
    Task<int> GetConfirmedCountAsync(int eventId);
}
