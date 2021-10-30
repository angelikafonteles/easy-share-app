using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EasyShareApp.Services.Exceptions;
using EasyShareApp.Data;
using EasyShareApp.Models;
using EasyShareApp.Models.Enums;


namespace EasyShareApp.Services
{
    public class DocumentService
    {
        private readonly DatabaseContext _context;

        public DocumentService(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<List<Document>> FindAllAsync()
        {
            return await _context.Document.ToListAsync();
        }

        public async Task<Document> FindByIdAsync(int id)
        {
            return await _context.Document.Include(obj => obj.Register).FirstOrDefaultAsync(obj => obj.Id == id);
        }

        public List<Document> FindAllByRegisterId(int id)
        {
            return _context.Document.Where(item => item.RegisterId == id).ToList();
        }

        public async Task InsertAsync(Document obj)
        {
            _context.Add(obj);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(int id)
        {
            try
            {
                var obj = await _context.Document.FindAsync(id);
                _context.Document.Remove(obj);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                throw new IntegrityException($"Documento não pôde ser apagado. Error: {e.Message}");
            }
        }

        public async Task RemoveAllByRegisterIdAsync(int id)
        {
            try
            {
                var list = FindAllByRegisterId(id);
                foreach (var item in list)
                {
                    _context.Document.Remove(item);
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateException e)
            {
                throw new IntegrityException($"Documento não pôde ser apagado. Error: {e.Message}");
            }
        }

        public async Task UpdateAsync(Document obj)
        {
            bool hasAny = await _context.Document.AnyAsync(x => x.Id == obj.Id);
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

        public Document FormatDocUpload(Document document)
        {
            DateTime now = DateTime.Now.ToLocalTime();
            document.InstantCreation = now;
            document.Id = 0;

            if (string.IsNullOrEmpty(document.Name))
            {
                document.Name = $"{now.ToString("dd/MM/yyyy HH:mm:ss")}_new file";
            }
            
            if (document.File != null)
            {
                var name = document.File.FileName;
                var extension = name.Substring(name.LastIndexOf('.') + 1);

                if (Regex.IsMatch(extension, "pdf", RegexOptions.IgnoreCase))
                    document.Extension = Extension.PDF;
                else if (Regex.IsMatch(extension, "jpeg", RegexOptions.IgnoreCase))
                    document.Extension = Extension.JPEG;
                else if (Regex.IsMatch(extension, "jpg", RegexOptions.IgnoreCase))
                    document.Extension = Extension.JPG;
                else if (Regex.IsMatch(extension, "png", RegexOptions.IgnoreCase))
                    document.Extension = Extension.PNG;
                else if (Regex.IsMatch(extension, "zip", RegexOptions.IgnoreCase))
                    document.Extension = Extension.ZIP;

                using (var target = new MemoryStream())
                {
                    document.File.CopyTo(target);
                    document.Attachment = target.ToArray();
                }
            }

            return document;
        }

        public FileContentResult FormatDocDownload(Document document)
        {
            DateTime now = DateTime.Now.ToLocalTime();

            if (CheckFileAvailable(document))
            {
                byte[] byteArr = document.Attachment;
                var type = document.Extension.ToString().ToLower();
                string mimeType = "application/unknown";

                if (Regex.IsMatch(type, "jpeg|jpg|png", RegexOptions.IgnoreCase))
                    mimeType = $"image/{type}";
                else
                    mimeType = $"application/{type}";

                return new FileContentResult(byteArr, mimeType)
                {
                    FileDownloadName = $"{now.ToString("dd/MM/yyyy HH:mm:ss")}_new download.{type}"
                };
            }
            else
            {
                throw new Exception("Arquivo Expirado");
            }
        }

        public bool CheckFileAvailable(Document document)
        {
            bool result = true;
            DateTime now = DateTime.Now.ToLocalTime();

            if (document.InstantExpiration < now)
            {
                _context.Document.Remove(document);
                _context.SaveChanges();
                result = false;
            }
            return result;
        }

        public (bool, string) CheckValidDate(Document document)
        {
            DateTime now = DateTime.Now.ToLocalTime();

            bool result = true;
            string msg = string.Empty;

            if (document.InstantExpiration < now)
            {
                result = false;
                msg = "Data escolhida menor que data atual";
            }
            if (document.InstantExpiration > now.AddDays(3))
            {
                result = false;
                msg = "Data superior ao limite permitido";
            }
            return (result, msg);
        }
    }
}

