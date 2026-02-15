using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.ParcoursExceptions;

namespace UniversiteDomain.UseCases.ParcoursUseCases.Create;

public class CreateParcoursUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<Parcours> ExecuteAsync(string nomParcours, int anneeFormation)
    {
        var parcours = new Parcours{ NomParcours = nomParcours, AnneeFormation = anneeFormation};
        return await ExecuteAsync(parcours);
    }
    public async Task<Parcours> ExecuteAsync(Parcours parcours)
    {
        await CheckBusinessRules(parcours);
        Parcours pa = await repositoryFactory.ParcoursRepository().CreateAsync(parcours);
        repositoryFactory.SaveChangesAsync().Wait();
        return pa;
    }
    private async Task CheckBusinessRules(Parcours parcours)
    {
        ArgumentNullException.ThrowIfNull(parcours);
        ArgumentNullException.ThrowIfNull(parcours.NomParcours);
        ArgumentNullException.ThrowIfNull(parcours.AnneeFormation);
        ArgumentNullException.ThrowIfNull(repositoryFactory.ParcoursRepository());
        
        // On recherche un parcours avec le même nom et la même année de formation
        List<Parcours> existe = await repositoryFactory.ParcoursRepository().FindByConditionAsync(p=>p.NomParcours.Equals(parcours.NomParcours) && p.AnneeFormation == parcours.AnneeFormation);

        // Si un parcours existe déjà avec le même nom et la même année de formation, on lève une exception
        if (existe is {Count:>0}) throw new DuplicateParcoursException(parcours.NomParcours + "-" + parcours.AnneeFormation + " existe déjà");
        
        // On vérifie si l'année de formation est valide (entre 1 et 2)
        if (parcours.AnneeFormation < 1 || parcours.AnneeFormation > 2) throw new InvalidAnneeFormationException(parcours.AnneeFormation + " n'est pas une année de formation valide (entre 1 et 2)");
    }
    
    public bool IsAuthorized(string role)
    {
        return role == "Scolarite" || role == "Responsable";
    }
}