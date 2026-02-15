using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.NoteUseCases.GenererCsvNotesUe;
using UniversiteDomain.UseCases.NoteUseCases.Get;
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
        
        // DELETE: api/Note/{etudiantId}/{ueId}
        [HttpDelete("{etudiantId}/{ueId}")]
        public async Task<IActionResult> DeleteNote(long etudiantId, long ueId)
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

            UniversiteDomain.UseCases.NoteUseCases.Delete.DeleteNoteUseCase uc = new UniversiteDomain.UseCases.NoteUseCases.Delete.DeleteNoteUseCase(repositoryFactory);
            if (!uc.IsAuthorized(role, user, etudiantId)) return Unauthorized();

            try
            {
                await uc.ExecuteAsync(etudiantId, ueId);
            }
            catch (Exception ex)
            {
                return ValidationProblem(detail: ex.Message);
            }

            return Ok($"Note supprimée pour l'étudiant {etudiantId} dans l'UE {ueId}.");
        }
        
        // GET: api/Note/{etudiantId}/{ueId}
        [HttpGet("{etudiantId}/{ueId}")]
        public async Task<ActionResult<NoteAvecUeDto>> GetNote(long etudiantId, long ueId)
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

            GetNoteUseCase uc = new GetNoteUseCase(repositoryFactory);
            if (!uc.IsAuthorized(role, user, etudiantId)) return Unauthorized();            
            Note? note;
            try
            {
                note = await uc.ExecuteAsync(etudiantId, ueId);
            }
            catch (Exception ex)
            {
                return ValidationProblem(detail: ex.Message);
            }
            return new NoteAvecUeDto().ToDto(note);
        }
        
        // GET: api/Note/{etudiantId}
        [HttpGet("{etudiantId}")]
        public async Task<ActionResult<List<NoteAvecUeDto>>> GetNote(long etudiantId)
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

            GetNoteUseCase uc = new GetNoteUseCase(repositoryFactory);
            if (!uc.IsAuthorized(role, user, etudiantId)) return Unauthorized();            
            List<Note> note;
            try
            {
                note = await uc.ExecuteAsync(etudiantId);
            }
            catch (Exception ex)
            {
                return ValidationProblem(detail: ex.Message);
            }
            return NoteAvecUeDto.ToDtos(note);
        }
        

        // GET: api/Note
        [HttpGet]
        public async Task<ActionResult<List<NoteAvecUeDto>>> GetToutesLesNotes()
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

            GetToutesLesNotesUseCase uc = new GetToutesLesNotesUseCase(repositoryFactory);
            if (!uc.IsAuthorized(role)) return Unauthorized();
            List<Note> notes;
            try
            {
                notes = await uc.ExecuteAsync();
            }
            catch (Exception ex)
            {
                return ValidationProblem(detail: ex.Message);
            }
            return NoteAvecUeDto.ToDtos(notes);
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
