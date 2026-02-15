using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.ParcoursExceptions;

namespace UniversiteDomain.UseCases.ParcoursUseCases.Update;

public class UpdateParcoursUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task ExecuteAsync(Parcours parcours)
    {
        await CheckBusinessRules(parcours);
        await repositoryFactory.ParcoursRepository().UpdateAsync(parcours);
        await repositoryFactory.SaveChangesAsync();
    }

    private async Task CheckBusinessRules(Parcours parcours)
    {
        ArgumentNullException.ThrowIfNull(parcours);
        ArgumentNullException.ThrowIfNull(parcours.NomParcours);
        ArgumentNullException.ThrowIfNull(parcours.AnneeFormation);
        ArgumentNullException.ThrowIfNull(repositoryFactory.ParcoursRepository());
        
        // On recherche un parcours avec le même nom et la même année de formation, mais un id différent
        List<Parcours> existe = await repositoryFactory.ParcoursRepository().FindByConditionAsync(p => p.NomParcours.Equals(parcours.NomParcours) && p.AnneeFormation == parcours.AnneeFormation && p.Id != parcours.Id);
        if (existe is {Count:>0}) throw new DuplicateParcoursException(parcours.NomParcours + "-" + parcours.AnneeFormation + " existe déjà");
        if (parcours.AnneeFormation < 1 || parcours.AnneeFormation > 2) throw new InvalidAnneeFormationException(parcours.AnneeFormation + " n'est pas une année de formation valide (entre 1 et 2)");
    }

    public bool IsAuthorized(string role)
    {
        if (role.Equals(Roles.Scolarite) || role.Equals(Roles.Responsable)) return true;
        return false;
    }
}

