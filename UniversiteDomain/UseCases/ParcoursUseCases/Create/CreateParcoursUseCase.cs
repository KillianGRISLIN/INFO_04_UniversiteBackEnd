using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.ParcoursExceptions;
using UniversiteDomain.Util;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Create;

public class CreateParcoursUseCase(IParcoursRepository parcoursRepository)
{
    public async Task<Parcours> ExecuteAsync(string nomParcours, int anneeFormation)
    {
        var parcours = new Parcours{ NomParcours = nomParcours, AnneeFormation = anneeFormation};
        return await ExecuteAsync(parcours);
    }
    public async Task<Parcours> ExecuteAsync(Parcours parcours)
    {
        await CheckBusinessRules(parcours);
        Parcours pa = await parcoursRepository.CreateAsync(parcours);
        parcoursRepository.SaveChangesAsync().Wait();
        return pa;
    }
    private async Task CheckBusinessRules(Parcours parcours)
    {
        ArgumentNullException.ThrowIfNull(parcours);
        ArgumentNullException.ThrowIfNull(parcours.NomParcours);
        ArgumentNullException.ThrowIfNull(parcours.AnneeFormation);
        ArgumentNullException.ThrowIfNull(parcoursRepository);
        
        // On recherche un parcours avec le même nom et la même année de formation
        List<Parcours> existe = await parcoursRepository.FindByConditionAsync(p=>p.NomParcours.Equals(parcours.NomParcours) && p.AnneeFormation == parcours.AnneeFormation);

        // Si un parcours existe déjà avec le même nom et la même année de formation, on lève une exception
        if (existe is {Count:>0}) throw new DuplicateParcoursException(parcours.NomParcours + "-" + parcours.AnneeFormation + " existe déjà");
        
        // On vérifie si l'année de formation est valide (entre 2020 et 2030)
        if (parcours.AnneeFormation < 2020 || parcours.AnneeFormation > 2030) throw new InvalidAnneeFormationException(parcours.AnneeFormation + " n'est pas une année de formation valide (entre 2020 et 2030)");
        
        // On vérifie que le nom du parcours fait au moins 3 caractères
        if (parcours.NomParcours.Length < 3) throw new InvalidNomParcoursException(parcours.NomParcours + " n'est pas un nom de parcours valide (au moins 3 caractères)");
    }
}