using EventManagement.Data.Model.Entities;
using EventManagement.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using EventManagement.Data.Model.Enums;


namespace EventManagement.Data.Repositories.Implementations
{
    public class OrganizerRequestRepository : GenericRepository<OrganizerRequest>, IOrganizerRequestRepository
    {
        public OrganizerRequestRepository(EventManagementDbContext context) : base(context)
        {
        }

        public async Task<List<OrganizerRequest>> GetPendingRequestsWithUserAsync()
        {
            return await _dbSet
                .Include(r => r.User)
                .Where(r => r.Status == RequestStatus.Pending)
                .ToListAsync();
        }
    }
}
