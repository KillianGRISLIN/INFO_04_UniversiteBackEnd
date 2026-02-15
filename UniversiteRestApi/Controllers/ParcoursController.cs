using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Dtos;
using UniversiteDomain.UseCases.ParcoursUseCases.Delete;
using UniversiteDomain.UseCases.ParcoursUseCases.Get;
using UniversiteDomain.UseCases.ParcoursUseCases.Update;
using UniversiteDomain.UseCases.SecurityUseCases.Get;

namespace UniversiteRestApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ParcoursController(IRepositoryFactory repositoryFactory) : ControllerBase
{
    // DELETE: api/Parcours/{etudiantId}/{ueId}
    [HttpDelete("{parcoursId}")]
    public async Task<IActionResult> DeleteParcours(long parcoursId)
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

        DeleteParcoursUseCase uc = new DeleteParcoursUseCase(repositoryFactory);
        if (!uc.IsAuthorized(role)) return Unauthorized();

        try
        {
            await uc.ExecuteAsync(parcoursId);
        }
        catch (Exception ex)
        {
            return ValidationProblem(detail: ex.Message);
        }

        return Ok($"Parcours {parcoursId} supprimée.");
    }
    
    // GET: api/Parcours/{parcoursId}
    [HttpGet("{parcoursId}")]
    public async Task<ActionResult<ParcoursDto>> GetParcours(long parcoursId)
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

        GetParcoursByIdUseCase uc = new GetParcoursByIdUseCase(repositoryFactory);
        if (!uc.IsAuthorized(role)) return Unauthorized();            
        Parcours? parcours;
        try
        {
            parcours = await uc.ExecuteAsync(parcoursId);
        }
        catch (Exception ex)
        {
            return ValidationProblem(detail: ex.Message);
        }
        return new ParcoursDto().ToDto(parcours);
    }

    // GET: api/Parcours
    [HttpGet]
    public async Task<ActionResult<List<ParcoursDto>>> GetTousLesParcours()
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

        GetTousLesParcoursUseCase uc = new GetTousLesParcoursUseCase(repositoryFactory);
        if (!uc.IsAuthorized(role)) return Unauthorized();
        List<Parcours> parcours;
        try
        {
            parcours = await uc.ExecuteAsync();
        }
        catch (Exception ex)
        {
            return ValidationProblem(detail: ex.Message);
        }
        return ParcoursDto.ToDtos(parcours);
    }
    
    // GET api/<ParcoursController>/complet/5
    [HttpGet("complet/{id}")]
    public async Task<ActionResult<ParcoursCompletDto>> GetUnParcoursCompletAsync(long id)
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
        GetParcoursCompletUseCase uc = new GetParcoursCompletUseCase(repositoryFactory);
        if (!uc.IsAuthorized(role)) return Unauthorized();
        Parcours? parcours;
        try
        {
            parcours = await uc.ExecuteAsync(id);
        }
        catch (Exception ex)
        {
            return ValidationProblem(detail: ex.Message);
        }
        if (parcours == null) return NotFound();
        return new ParcoursCompletDto().ToDto(parcours);
    }
    
    // PUT: api/Parcours/{parcoursId}
    [HttpPut("{parcoursId}")]
    public async Task<IActionResult> UpdateParcours(long parcoursId, [FromBody] ParcoursDto parcoursDto)
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

        UpdateParcoursUseCase uc = new UpdateParcoursUseCase(repositoryFactory);
        if (!uc.IsAuthorized(role)) return Unauthorized();
        try
        {
            await uc.ExecuteAsync(parcoursDto.ToEntity());
        }
        catch (Exception ex)
        {
            return ValidationProblem(detail: ex.Message);
        }
        return Ok($"Parcours {parcoursId} mise à jour.");
    }
    
    // POST: api/Parcours
    [HttpPost]
    public async Task<IActionResult> CreateParcours([FromBody] ParcoursDto parcoursDto)
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

        var uc = new UniversiteDomain.UseCases.ParcoursUseCases.Create.CreateParcoursUseCase(repositoryFactory);
        if (!uc.IsAuthorized(role)) return Unauthorized();
        Parcours parcours;
        try
        {
            parcours = await uc.ExecuteAsync(parcoursDto.ToEntity());
        }
        catch (Exception ex)
        {
            return ValidationProblem(detail: ex.Message);
        }
        return Ok(new ParcoursDto().ToDto(parcours));
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
