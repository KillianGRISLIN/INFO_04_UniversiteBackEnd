using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.UeUseCases.Get;
using UniversiteDomain.UseCases.UeUseCases.Delete;
using UniversiteDomain.UseCases.UeUseCases.Update;
using UniversiteDomain.UseCases.SecurityUseCases.Get;
using UniversiteDomain.UseCases.UeUseCases.Create;

namespace UniversiteRestApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UeController(IRepositoryFactory repositoryFactory) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<UeDto>>> GetToutesLesUes()
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
        
        GetToutesLesUesUseCase uc = new GetToutesLesUesUseCase(repositoryFactory);
        if (!uc.IsAuthorized(role)) return Unauthorized();
        List<Ue> ues;
        try
        {
            ues = await uc.ExecuteAsync();
        }
        catch (Exception ex)
        {
            return ValidationProblem(detail: ex.Message);
        }
        return UeDto.ToDtos(ues);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUe(long id)
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
        DeleteUeUseCase uc = new DeleteUeUseCase(repositoryFactory);
        if (!uc.IsAuthorized(role)) return Unauthorized();
        try
        {
            await uc.ExecuteAsync(id);
        }
        catch (Exception ex)
        {
            return ValidationProblem(detail: ex.Message);
        }
        return Ok($"UE {id} supprimée.");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUe(long id, [FromBody] UeDto ueDto)
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
        UpdateUeUseCase uc = new UpdateUeUseCase(repositoryFactory);
        if (!uc.IsAuthorized(role)) return Unauthorized();
        try
        {
            await uc.ExecuteAsync(ueDto.ToEntity());
        }
        catch (Exception ex)
        {
            return ValidationProblem(detail: ex.Message);
        }
        return Ok($"UE {id} mise à jour.");
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UeDto>> GetUeById(long id)
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
        GetUeByIdUseCase uc = new GetUeByIdUseCase(repositoryFactory);
        if (!uc.IsAuthorized(role)) return Unauthorized();
        Ue? ue;
        try
        {
            ue = await uc.ExecuteAsync(id);
        }
        catch (Exception ex)
        {
            return ValidationProblem(detail: ex.Message);
        }
        if (ue == null) return NotFound();
        return new UeDto().ToDto(ue);
    }

    [HttpPost]
    public async Task<IActionResult> PostUe([FromBody] UeDto ueDto)
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
        CreateUeUseCase uc = new CreateUeUseCase(repositoryFactory);
        if (!uc.IsAuthorized(role)) return Unauthorized();
        Ue ue;
        try
        {
            ue = await uc.ExecuteAsync(ueDto.ToEntity());
        }
        catch (Exception ex)
        {
            return ValidationProblem(detail: ex.Message);
        }
        return Ok(new UeDto().ToDto(ue));
    }

    private void CheckSecu(out string role, out string email, out IUniversiteUser user)
    {
        role = "";
        ClaimsPrincipal claims = HttpContext.User;
        if (claims.FindFirst(ClaimTypes.Email)==null) throw new UnauthorizedAccessException();
        email = claims.FindFirst(ClaimTypes.Email).Value;
        if (email==null) throw new UnauthorizedAccessException();
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
