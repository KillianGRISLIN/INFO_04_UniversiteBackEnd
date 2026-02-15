using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.UeUseCases.Get;

public class GetToutesLesUesByParcoursIdUseCase(IRepositoryFactory factory)
{
    public async Task<List<Ue>?> ExecuteAsync(long idParcours)
    {
        await CheckBusinessRules();
        List<Ue?> ues = await factory.UeRepository().FindByConditionAsync(ue => ue.EnseigneeDans != null && ue.EnseigneeDans.Any(p => p.Id == idParcours));
        return ues;
    }
    private async Task CheckBusinessRules()
    {
        ArgumentNullException.ThrowIfNull(factory);
        IUeRepository ueRepository = factory.UeRepository();
        ArgumentNullException.ThrowIfNull(ueRepository);
    }
    
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Scolarite) || role.Equals(Roles.Responsable);
    }
}