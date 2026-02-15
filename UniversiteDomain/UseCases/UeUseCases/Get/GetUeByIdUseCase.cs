using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.UeUseCases.Get;

public class GetUeByIdUseCase(IRepositoryFactory factory)
{
    public async Task<Ue?> ExecuteAsync(long idUe)
    {
        await CheckBusinessRules();
        Ue? ue = await factory.UeRepository().FindAsync(idUe);
        return ue;
    }
    private async Task CheckBusinessRules()
    {
        ArgumentNullException.ThrowIfNull(factory);
        IUeRepository ueRepository = factory.UeRepository();
        ArgumentNullException.ThrowIfNull(ueRepository);
    }
    
    public bool IsAuthorized(string role, IUniversiteUser user, long idUe)
    {
        if (role.Equals(Roles.Scolarite) || role.Equals(Roles.Responsable)) return true;
        // Si c'est un étudiant qui est connecté,
        // il ne peut consulter que les UE de son parcours
        return user.Etudiant!=null && role.Equals(Roles.Etudiant) && user.Etudiant.ParcoursSuivi!=null && user.Etudiant.ParcoursSuivi.UesEnseignees!=null && user.Etudiant.ParcoursSuivi.UesEnseignees.Any(ue => ue.Id == idUe);
    }
}