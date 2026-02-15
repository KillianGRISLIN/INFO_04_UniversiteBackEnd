using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.NoteUseCases.Get;

public class GetToutesLesNotesUseCase(IRepositoryFactory factory)
{
    public async Task<List<Note>> ExecuteAsync()
    {
        await CheckBusinessRules();
        return await factory.NoteRepository().FindAllAsync();
    }
    
    public async Task CheckBusinessRules()
    {
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentNullException.ThrowIfNull(factory.NoteRepository());
    }
    
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Scolarite) || role.Equals(Roles.Responsable);
    }
}

