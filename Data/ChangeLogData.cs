using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Entity.Model;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class ChangeLogData
    {
        private readonly DbContext _context;

        public ChangeLogData(DbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ChangeLog>> GetAllAsync()
        {
            return await _context.Set<ChangeLog>().ToListAsync();
        }

        public async Task<ChangeLog?> GetByIdAsync(int id)
        {
            return await _context.Set<ChangeLog>().FindAsync(id);
        }

        public async Task<ChangeLog> CreateAsync(ChangeLog changeLog)
        {
            _context.Set<ChangeLog>().Add(changeLog);
            await _context.SaveChangesAsync();
            return changeLog;
        }
    }
}
