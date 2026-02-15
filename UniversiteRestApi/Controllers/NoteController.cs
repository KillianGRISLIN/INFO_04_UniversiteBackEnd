using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.NoteUseCases.GenererCsvNotesUe;
using UniversiteDomain.UseCases.NoteUseCases.ImporterNotesCsv;
using UniversiteDomain.UseCases.SecurityUseCases.Get;
using UniversiteRestApi.Dtos;

namespace UniversiteRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoteController(IRepositoryFactory repositoryFactory) : ControllerBase
    {
        // GET: api/Note/ExportCsvNotesUe/{ueId}
        [HttpGet("ExportCsvNotesUe/{ueId}")]
        public async Task<IActionResult> ExportCsvNotesUe(long ueId)
        {
            string role = "";
            string email = "";
            IUniversiteUser user = null;
            try
            {
                CheckSecu(out role, out email, out user);
            }
            catch
            {
                return Unauthorized();
            }

            var uc = new GenererCsvNotesUeUseCase(repositoryFactory);
            if (!uc.IsAuthorized(role)) return Unauthorized();

            MemoryStream csvStream;
            try
            {
                csvStream = await uc.ExecuteAsync(ueId);
            }
            catch (Exception ex)
            {
                return ValidationProblem(detail: ex.Message);
            }

            csvStream.Position = 0;
            return File(csvStream, "text/csv", $"notes_ue_{ueId}.csv");
        }
        
        // POST: api/Note/ImportCsvNotesUe
        [HttpPost("ImportCsvNotesUe/{ueId}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ImportCsvNotesUe([FromForm] ImportCsvNotesUeRequest request, long ueId)
        {
            string role = "";
            string email = "";
            IUniversiteUser user = null;
            try
            {
                CheckSecu(out role, out email, out user);
            }
            catch
            {
                return Unauthorized();
            }

            if (request?.File == null || request.File.Length == 0)
                return BadRequest("Aucun fichier fourni.");

            var uc = new ImporterNotesCsvUeUseCase(repositoryFactory);
            
            if (!uc.IsAuthorized(role)) return Unauthorized();

            try
            {
                using var stream = request.File.OpenReadStream();
                await uc.ExecuteAsync(ueId, stream);
            }
            catch (Exception ex)
            {
                return ValidationProblem(detail: ex.Message);
            }

            return Ok("Import des notes effectué avec succès.");
        }
        
        private void CheckSecu(out string role, out string email, out IUniversiteUser user)
        {
            role = "";
            ClaimsPrincipal claims = HttpContext.User;
            if (claims.FindFirst(ClaimTypes.Email)==null) throw new UnauthorizedAccessException();
            email = claims.FindFirst(ClaimTypes.Email).Value;
            if (email==null) throw new UnauthorizedAccessException();
            //user = repositoryFactory.UniversiteUserRepository().FindByEmailAsync(email).Result;
            user = new FindUniversiteUserByEmailUseCase(repositoryFactory).ExecuteAsync(email).Result;
            if (user==null) throw new UnauthorizedAccessException();
            if (claims.Identity?.IsAuthenticated != true) throw new UnauthorizedAccessException();
            var ident = claims.Identities.FirstOrDefault();
            if (ident == null)throw new UnauthorizedAccessException();
            if (claims.FindFirst(ClaimTypes.Role)==null) throw new UnauthorizedAccessException();
            role = ident.FindFirst(ClaimTypes.Role).Value;
            if (role == null) throw new UnauthorizedAccessException();
        }
    }
}
