using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.UeUseCases.Get;

public class GetToutesLesUesUseCase(IRepositoryFactory factory)
{
    public async Task<List<Ue>> ExecuteAsync()
    {
        await CheckBusinessRules();
        return await factory.UeRepository().FindAllAsync();
    }
    
    private async Task CheckBusinessRules()
    {
        ArgumentNullException.ThrowIfNull(factory);
        IUeRepository ueRepository = factory.UeRepository();
        ArgumentNullException.ThrowIfNull(ueRepository);
    }
    public bool IsAuthorized(string role)
    {
        return role == Roles.Scolarite || role == Roles.Responsable;
    }
}

