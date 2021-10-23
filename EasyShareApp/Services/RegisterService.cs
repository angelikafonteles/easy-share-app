using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EasyShareApp.Services.Exceptions;
using EasyShareApp.Data;
using EasyShareApp.Models;

namespace EasyShareApp.Services
{
    public class RegisterService
    {
        private readonly DatabaseContext _context;
        private readonly DocumentService _documentService;

        public RegisterService(DatabaseContext context, DocumentService documentService)
        {
            _context = context;
            _documentService = documentService;
        }

        public async Task<Register> FindByIdAsync(int id)
        {
            return await _context.Register.FindAsync(id);
        }

        public async Task<Register> FindByKeyAsync(string id)
        {
            return await _context.Register.FirstOrDefaultAsync(obj => obj.Key == id);
        }

        public async Task InsertAsync(Register obj)
        {
            _context.Add(obj);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(int id)
        {
            try
            {
                await _documentService.RemoveAllByRegisterIdAsync(id);
                var obj = await _context.Register.FindAsync(id);
                _context.Register.Remove(obj);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                throw new IntegrityException("Registro não pôde ser apagado");
            }
        }

        public async Task UpdateAsync(Register obj)
        {
            bool hasAny = await _context.Register.AnyAsync(x => x.Id == obj.Id);
            if (!hasAny)
            {
                throw new NotFoundException("Id not found");
            }
            try
            {
                _context.Update(obj);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                throw new DbConcurrencyException(e.Message);
            }
        }
        
    }
}
