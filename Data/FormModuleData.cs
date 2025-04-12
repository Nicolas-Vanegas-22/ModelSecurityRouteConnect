using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Entity.Model;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class FormModuleData
    {
        private readonly DbContext _context;

        public FormModuleData(DbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FormModule>> GetAllAsync()
        {
            return await _context.Set<FormModule>().ToListAsync();
        }

        public async Task<FormModule?> GetByIdAsync(int id)
        {
            return await _context.Set<FormModule>().FindAsync(id);
        }

        public async Task<FormModule> CreateAsync(FormModule formModule)
        {
            _context.Set<FormModule>().Add(formModule);
            await _context.SaveChangesAsync();
            return formModule;
        }
    }
}

