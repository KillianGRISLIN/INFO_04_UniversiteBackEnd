using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.EtudiantUseCases.Get;
using UniversiteDomain.UseCases.UeUseCases.Get;

namespace UniversiteDomain.UseCases.ParcoursUseCases.Delete;

public class DeleteParcoursUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task ExecuteAsync(long parcoursId)
    {
        await CheckBusinessRules(parcoursId);

        // Retirer le parcours de chaque étudiant inscrit
        GetTousLesEtudiantsByParcoursIdUseCase getTousLesEtudiantsByParcoursIdUseCase = new GetTousLesEtudiantsByParcoursIdUseCase(repositoryFactory);
        List<Etudiant?> etudiants = await getTousLesEtudiantsByParcoursIdUseCase.ExecuteAsync(parcoursId);
        foreach (Etudiant etudiant in etudiants)
        {
            etudiant.ParcoursSuivi = null;
            await repositoryFactory.EtudiantRepository().UpdateAsync(etudiant);
        }

        // Retirer le parcours des UE associées
        GetToutesLesUesByParcoursIdUseCase getTousLesUesByParcoursIdUseCase = new GetToutesLesUesByParcoursIdUseCase(repositoryFactory);
        List<Ue?> ues = await getTousLesUesByParcoursIdUseCase.ExecuteAsync(parcoursId);
        foreach (Ue ue in ues)
        {
            ue.EnseigneeDans = ue.EnseigneeDans.Where(p => p.Id != parcoursId).ToList();
            await repositoryFactory.UeRepository().UpdateAsync(ue);
        }

        // Supprimer le parcours
        await repositoryFactory.ParcoursRepository().DeleteAsync(parcoursId);
    }
    
    public async Task CheckBusinessRules(long parcoursId)
    {
        ArgumentNullException.ThrowIfNull(parcoursId);
        ArgumentNullException.ThrowIfNull(repositoryFactory.ParcoursRepository());
        
        // Vérification de l'existence du parcours
        Parcours parcours = await repositoryFactory.ParcoursRepository().FindAsync(parcoursId);
        if (parcours == null)
            throw new Exception($"Aucun parcours avec l'id {parcoursId}");
    }
    
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}