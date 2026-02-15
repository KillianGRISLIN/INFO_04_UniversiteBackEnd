using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.UeExceptions;
using UniversiteDomain.UseCases.UeUseCases.Get;

namespace UniversiteDomain.UseCases.UeUseCases.Update;

public class UpdateUeUseCase(IRepositoryFactory factory)
{
    public async Task ExecuteAsync(Ue ue)
    {
        await CheckBusinessRules(ue);
        await factory.UeRepository().UpdateAsync(ue);
        await factory.SaveChangesAsync();
    }

    private async Task CheckBusinessRules(Ue ue)
    {
        ArgumentNullException.ThrowIfNull(ue);
        ArgumentNullException.ThrowIfNull(ue.NumeroUe);
        ArgumentNullException.ThrowIfNull(ue.Intitule);
        ArgumentNullException.ThrowIfNull(factory.UeRepository());

        // Vérifier que l'UE existe
        GetUeByIdUseCase getUeByIdUseCase = new GetUeByIdUseCase(factory);
        Ue? existingUe = await getUeByIdUseCase.ExecuteAsync(ue.Id);
        if (existingUe == null)
            throw new UeNotFoundException($"L'UE avec l'id {ue.Id} n'existe pas");

        // Vérifier unicité du numéro d'UE (hors soi-même)
        List<Ue> uesAvecMemeNumero = await factory.UeRepository().FindByConditionAsync(u => u.NumeroUe == ue.NumeroUe && u.Id != ue.Id);
        if (uesAvecMemeNumero is { Count: >0 })
            throw new DuplicateNumeroUeException($"Le numéro d'UE {ue.NumeroUe} existe déjà");

        // Vérifier que l'intitulé n'est pas vide
        if (ue.Intitule.Length < 3) throw new InvalidIntituleException(ue.Intitule +" incorrect - L'intitulé d'une UE doit contenir au moins 3 lettres");
    }

    public bool IsAuthorized(string role)
    {
        return role == Roles.Scolarite || role == Roles.Responsable;
    }
}
