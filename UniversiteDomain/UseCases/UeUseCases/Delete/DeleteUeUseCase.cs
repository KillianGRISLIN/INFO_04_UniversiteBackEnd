using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.UeUseCases.Delete;

public class DeleteUeUseCase(IRepositoryFactory factory)
{
    public async Task ExecuteAsync(long idUe)
    {
        await CheckBusinessRules();
        await factory.UeRepository().DeleteAsync(idUe);
        await factory.SaveChangesAsync();
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

