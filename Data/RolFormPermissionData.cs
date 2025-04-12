using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Entity.Model;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class RolFormPermissionData
    {
        private readonly DbContext _context;

        public RolFormPermissionData(DbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RolFormPermission>> GetAllAsync()
        {
            return await _context.Set<RolFormPermission>().ToListAsync();
        }

        public async Task<RolFormPermission?> GetByIdAsync(int id)
        {
            return await _context.Set<RolFormPermission>().FindAsync(id);
        }

        public async Task<RolFormPermission> CreateAsync(RolFormPermission formPermission)
        {
            _context.Set<RolFormPermission>().Add(formPermission);
            await _context.SaveChangesAsync();
            return formPermission;
        }
    }
}
