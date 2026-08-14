using EventManagement.Data.Model.Entities;
using EventManagement.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventManagement.Data.Repositories.Implementations;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(EventManagementDbContext context) : base(context)
    {
    }
}
