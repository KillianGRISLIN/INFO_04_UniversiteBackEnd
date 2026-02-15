using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Get;

public class GetTousLesEtudiantsByParcoursIdUseCase(IRepositoryFactory factory)
{
    public async Task<List<Etudiant>?> ExecuteAsync(long idParcours)
    {
        await CheckBusinessRules();
        List<Etudiant> etudiants = await factory.EtudiantRepository().FindByConditionAsync(e => e.ParcoursSuivi.Id == idParcours);
        return etudiants;
    }
    private async Task CheckBusinessRules()
    {
        ArgumentNullException.ThrowIfNull(factory);
        IEtudiantRepository etudiantRepository=factory.EtudiantRepository();
        ArgumentNullException.ThrowIfNull(etudiantRepository);
    }
    
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Scolarite) || role.Equals(Roles.Responsable);
    }
}