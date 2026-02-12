using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.UeExceptions;

namespace UniversiteDomain.UseCases.UeUseCases.Create;

public class CreateUeUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<Ue> ExecuteAsync(string numeroUe, string intitule)
    {
        var etudiant = new Ue{NumeroUe = numeroUe, Intitule = intitule};
        return await ExecuteAsync(etudiant);
    }
    public async Task<Ue> ExecuteAsync(Ue ue)
    {
        await CheckBusinessRules(ue);
        Ue ueCreee = await repositoryFactory.UeRepository().CreateAsync(ue);
        repositoryFactory.SaveChangesAsync().Wait();
        return ueCreee;
    }
    private async Task CheckBusinessRules(Ue ue)
    {
        ArgumentNullException.ThrowIfNull(ue);
        ArgumentNullException.ThrowIfNull(ue.NumeroUe);
        ArgumentNullException.ThrowIfNull(ue.Intitule);
        ArgumentNullException.ThrowIfNull(repositoryFactory.UeRepository());
        
        // On recherche une UE qui a le même numéro que celui de l'UE à ajouter
        List<Ue> existe = await repositoryFactory.UeRepository().FindByConditionAsync(e=>e.NumeroUe.Equals(ue.NumeroUe));

        // Si la liste n'est pas vide, c'est que le numéro d'UE est déjà utilisé
        if (existe is {Count:>0}) throw new DuplicateNumeroUeException(ue.NumeroUe+ " - ce numéro d'UE est déjà utilisé");
        
        // Le métier définit que les intitulés d'UE doivent contenir au moins 3 lettres
        if (ue.Intitule.Length < 3) throw new InvalidIntituleException(ue.Intitule +" incorrect - L'intitulé d'une UE doit contenir au moins 3 lettres");
    }
}