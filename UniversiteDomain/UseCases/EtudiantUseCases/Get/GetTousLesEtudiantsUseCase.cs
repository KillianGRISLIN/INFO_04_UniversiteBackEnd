using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Get;

public class GetTousLesEtudiantsUseCase(IRepositoryFactory factory)
{
    public async Task<List<Etudiant>?> ExecuteAsync()
    {
        await CheckBusinessRules();
        List<Etudiant>? etudiant = await factory.EtudiantRepository().FindAllAsync();
        return etudiant;
    }
    private async Task CheckBusinessRules()
    {
        ArgumentNullException.ThrowIfNull(factory);
        IEtudiantRepository etudiantRepository=factory.EtudiantRepository();
        ArgumentNullException.ThrowIfNull(etudiantRepository);
    }
    public bool IsAuthorized(string role)
    {
        if (role.Equals(Roles.Scolarite) || role.Equals(Roles.Responsable)) return true;
        return false;
    }
}