using Microsoft.EntityFrameworkCore;
using Entity.Model;

namespace Data
{
    public class RolPermissionData
    {
        private readonly DbContext _context;

        public RolPermissionData(DbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RolPermission>> GetAllAsync()
        {
            return await _context.Set<RolPermission>().ToListAsync();
        }

        public async Task<RolPermission?> GetByIdAsync(int id)
        {
            return await _context.Set<RolPermission>().FindAsync(id);
        }

        public async Task<RolPermission> CreateAsync(RolPermission permission)
        {
            _context.Set<RolPermission>().Add(permission);
            await _context.SaveChangesAsync();
            return permission;
        }
    }
}

