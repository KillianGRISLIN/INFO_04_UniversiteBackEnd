using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;
using UniversiteDomain.UseCases.EtudiantUseCases.Get;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Delete;

public class DeleteEtudiantUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task ExecuteAsync(long etudiantId)
    {
        Etudiant etudiant = new Etudiant{Id = etudiantId};
        await ExecuteAsync(etudiant);
    }
    
    public async Task ExecuteAsync(Etudiant etudiant)
    {
        Etudiant etudiantASupprimer = await CheckBusinessRules(etudiant);
        await repositoryFactory.EtudiantRepository().DeleteAsync(etudiantASupprimer);
        await repositoryFactory.SaveChangesAsync();
    }
    private async Task<Etudiant> CheckBusinessRules(Etudiant etudiant)
    {
        ArgumentNullException.ThrowIfNull(etudiant);
        ArgumentNullException.ThrowIfNull(etudiant.Id);
        ArgumentNullException.ThrowIfNull(repositoryFactory.EtudiantRepository());
        
        // Vérification de l'existence de l'étudiant
        GetEtudiantByIdUseCase getEtudiantByIdUseCase = new GetEtudiantByIdUseCase(repositoryFactory);
        Etudiant? etudiantExiste = await getEtudiantByIdUseCase.ExecuteAsync(etudiant.Id);
        if (etudiantExiste == null)
            throw new EtudiantNotFoundException($"Aucun étudiant avec l'id {etudiant.Id}");
        return etudiantExiste;
    }
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}