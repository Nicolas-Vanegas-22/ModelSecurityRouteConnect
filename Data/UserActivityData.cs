using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Entity.Model;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class UserActivityData
    {
        private readonly DbContext _context;

        public UserActivityData(DbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserActivity>> GetAllAsync()
        {
            return await _context.Set<UserActivity>().ToListAsync();
        }

        public async Task<UserActivity?> GetByIdAsync(int id)
        {
            return await _context.Set<UserActivity>().FindAsync(id);
        }

        public async Task<UserActivity> CreateAsync(UserActivity activity)
        {
            _context.Set<UserActivity>().Add(activity);
            await _context.SaveChangesAsync();
            return activity;
        }
    }
}
