using Entity.Model;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class DestinationActivityData
    {
        private readonly DbContext _context;

        public DestinationActivityData(DbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DestinationActivity>> GetAllAsync()
        {
            return await _context.Set<DestinationActivity>().ToListAsync();
        }

        public async Task<DestinationActivity?> GetByIdAsync(int id)
        {
            return await _context.Set<DestinationActivity>().FindAsync(id);
        }

        public async Task<DestinationActivity> CreateAsync(DestinationActivity activity)
        {
            _context.Set<DestinationActivity>().Add(activity);
            await _context.SaveChangesAsync();
            return activity;
        }
    }
}
