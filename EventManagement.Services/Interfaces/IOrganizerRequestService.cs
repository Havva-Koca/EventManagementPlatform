using EventManagement.Services.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventManagement.Services.Interfaces;

public interface IOrganizerRequestService
{
    Task<OrganizerRequestResult> SubmitRequestAsync(string userId);
    Task<OrganizerRequestResult> ApproveRequestAsync(int requestId, string reviewerId);
    Task<OrganizerRequestResult> RejectRequestAsync(int requestId, string reviewerId);
}
