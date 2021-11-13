using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EasyShareApp.Services;
using EasyShareApp.Models;
using EasyShareApp.Models.ViewModels;
using EasyShareApp.Utils;

namespace EasyShareApp.Controllers
{
    public class ShareController : Controller
    {
        private readonly DocumentService _documentService;
        private readonly RegisterService _registerService;

        public ShareController(DocumentService documentService, RegisterService registerService)
        {
            _documentService = documentService;
            _registerService = registerService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Donate()
        {
            return View();
        }

        public IActionResult Faq()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Email,Key,Password,Documents")] Register register)
        {
            try
            {
                register.Key = Util.GetUniqueKey();
                if (register.Password.Length >= 6 &&
                    register.Password.Length < 11)
                {
                    register.Password = Util.Encrypt(register.Password.Trim());
                }
                else
                    throw new Exception("A senha deve ter entre 6 a 10 caracteres");
            }
            catch(Exception e)
            {
                return RedirectToAction(nameof(CustomError), new { message = e.Message });
            }

            if (ModelState.IsValid)
            {
                await _registerService.InsertAsync(register);
                return RedirectToAction(nameof(MenuList), register);
            }
            return View(register);
        }

        public async Task<IActionResult> MenuList(int? id)
        {
            var list = _documentService.FindAllByRegisterId((int)id);

            if (list.Any())
            {
                foreach (var item in list)
                {
                    _documentService.CheckFileAvailable(item);
                }
            }
            var register = _registerService.FindByIdAsync((int)id);
            return View(await register);
        }

        public IActionResult Upload(int? id)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(int id, [Bind("Id,Name,InstantCreation,InstantExpiration,Attachment,Extension,File")] Document document)
        {
            try
            {
                document.InstantExpiration = document.InstantExpiration.AddHours(3);
                if (!_documentService.CheckValidDate(document).Item1)
                {
                    throw new Exception(_documentService.CheckValidDate(document).Item2);
                }
                document = _documentService.FormatDocUpload(document);
                var register = await _registerService.FindByIdAsync(id);

                if (ModelState.IsValid)
                {
                    document.RegisterId = register.Id;
                    document.Register = register;
                    await _documentService.InsertAsync(document);
                    //await _registerService.UpdateAsync(register);
                    return RedirectToAction(nameof(MenuList), register);
                }
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(CustomError), new { message = e.Message });
            }
            return View(document);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            var document = await _documentService.FindByIdAsync((int)id);

            try
            {
                if (document == null)
                {
                    throw new Exception("Arquivo não encontrado");
                }
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(CustomError), new { message = e.Message });
            }
            return View(document);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,InstantExpiration")] Document document)
        {
            DateTime now = DateTime.UtcNow;
            document.InstantExpiration = document.InstantExpiration.AddHours(3);
            var myDocument = await _documentService.FindByIdAsync(id);
            try
            {
                if (myDocument == null)
                {
                    throw new Exception("Arquivo não encontrado");
                }
                if (!_documentService.CheckFileAvailable(myDocument))
                {
                    throw new Exception("Arquivo Expirado");
                }
                if (!_documentService.CheckValidDate(document).Item1)
                {
                    throw new Exception(_documentService.CheckValidDate(document).Item2);
                }
                if (string.IsNullOrEmpty(document.Name))
                {
                    document.Name = $"{now.ToString("dd/MM/yyyy HH:mm:ss")}_new file";
                }

                myDocument.Name = document.Name;
                myDocument.InstantExpiration = document.InstantExpiration;
                await _documentService.UpdateAsync(myDocument);
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(CustomError), new { message = e.Message });
            }

            return RedirectToAction(nameof(MenuList), myDocument.Register);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            var document = await _documentService.FindByIdAsync((int)id);

            try
            {
                if (document == null)
                {
                    throw new Exception("Arquivo não encontrado");
                }
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(CustomError), new { message = e.Message });
            }

            return View(document);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var document = await _documentService.FindByIdAsync(id);
            await _documentService.RemoveAsync(id);
            return RedirectToAction(nameof(MenuList), document.Register);
        }

        public async Task<IActionResult> ConfirmationPage(int? id)
        {
            var register = await _registerService.FindByIdAsync((int)id);

            try
            {
                if (register == null)
                {
                    throw new Exception("Registro não encontrado");
                }
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(CustomError), new { message = e.Message });
            }

            return View(register);
        }

        public async Task<IActionResult> DeleteForce(int? id)
        {
            var register = await _registerService.FindByIdAsync((int)id);

            try
            {
                if (register == null)
                {
                    throw new Exception("Registro não econtrado");
                }
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(CustomError), new { message = e.Message });
            }

            return View(register);
        }

        [HttpPost, ActionName("DeleteForce")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteForce(int id)
        {
            await _registerService.RemoveAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Download(int? id)
        {
            var document = await _documentService.FindByIdAsync((int)id);

            try
            {
                if (document == null)
                {
                    throw new Exception("Arquivo não encontrado");
                }

                return _documentService.FormatDocDownload(document);
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(CustomError), new { message = e.Message });
            }
        }

        public IActionResult GetFile()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetFile([Bind("Key,Password")] Authenticator aut)
        {
            if (ModelState.IsValid)
            {
                var register = await _registerService.FindByKeyAsync(aut.Key);

                try
                {
                    if (register == null)
                    {
                        throw new Exception("Chave identificadora não encontrada :(");
                    }

                    string pass = Util.Decrypt(register.Password);
                    if (aut.Password.Trim().Equals(pass))
                    {
                        return RedirectToAction(nameof(MenuList), register);
                    }
                    else
                    {
                        throw new Exception("Senha inválida!");
                    }

                }
                catch (Exception e)
                {
                    return RedirectToAction(nameof(CustomError), new { message = e.Message });
                }
            }
            return View(nameof(Index));
        }

        public IActionResult CustomError(string message)
        {
            var viewModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                Message = message
            };
            return View(viewModel);
        }

    }
}
