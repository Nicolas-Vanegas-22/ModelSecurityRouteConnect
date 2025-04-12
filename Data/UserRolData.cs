using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Entity.Model;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class UserRolData
    {
        private readonly DbContext _context;

        public UserRolData(DbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserRol>> GetAllAsync()
        {
            return await _context.Set<UserRol>().ToListAsync();
        }

        public async Task<UserRol?> GetByIdAsync(int id)
        {
            return await _context.Set<UserRol>().FindAsync(id);
        }

        public async Task<UserRol> CreateAsync(UserRol rol)
        {
            _context.Set<UserRol>().Add(rol);
            await _context.SaveChangesAsync();
            return rol;
        }
    }
}

