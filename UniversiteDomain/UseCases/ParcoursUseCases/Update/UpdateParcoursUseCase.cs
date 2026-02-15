using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.ParcoursExceptions;
using UniversiteDomain.UseCases.ParcoursUseCases.Get;

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
        GetParcoursByIdUseCase getParcoursByIdUseCase = new GetParcoursByIdUseCase(repositoryFactory);
        Parcours? parcoursExiste = await getParcoursByIdUseCase.ExecuteAsync(parcours.Id);
        if (parcoursExiste == null) throw new ParcoursNotFoundException(parcours.NomParcours + "-" + parcours.AnneeFormation + " n'existe pas");
        if (parcours.AnneeFormation < 1 || parcours.AnneeFormation > 2) throw new InvalidAnneeFormationException(parcours.AnneeFormation + " n'est pas une année de formation valide (entre 1 et 2)");
    }

    public bool IsAuthorized(string role)
    {
        if (role.Equals(Roles.Scolarite) || role.Equals(Roles.Responsable)) return true;
        return false;
    }
}

