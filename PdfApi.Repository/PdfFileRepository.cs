using Microsoft.EntityFrameworkCore;
using PdfApi.Data;
using PdfApi.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace PdfApi.Repository
{
    public class PdfFileRepository : IPdfFileRepository
    {
        private readonly PdfApiDbContext _context;

        public PdfFileRepository(PdfApiDbContext context)
        {
            _context = context;
        }

        public Task<int> CountAsync()
            => _context.Files.CountAsync();

        public async Task<PdfFileModel> AddAsync(PdfFileModel model)
        {
            var entry = await _context.Files.AddAsync(model);
            Debug.Assert(entry.State == EntityState.Added);

            return entry.Entity;
        }

        public async Task<IEnumerable<PdfFileModel>> GetAsync(
            int page,
            int itemsPerPage,
            Func<IQueryable<PdfFileModel>, IOrderedQueryable<PdfFileModel>> orderBy)
        {
            var query = _context.Files.AsNoTracking();

            if (orderBy is null)
            {
                query = query.OrderBy(m => m.Name);
            }
            else
            {
                query = orderBy(query);
            }

            return await query
                .Skip((page - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToListAsync();
        }

        public async Task<PdfFileModel> DeleteAsync(string fileName)
        {
            var file = await _context.Files.FindAsync(fileName);

            if (file is null)
            {
                return null;
            }

            var entry = _context.Remove(file);
            Debug.Assert(entry.State == EntityState.Deleted);

            return entry.Entity;
        }

        public Task<int> SaveAsync()
            => _context.SaveChangesAsync();
    }
}
