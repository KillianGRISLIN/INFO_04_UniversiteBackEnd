using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;
using UniversiteDomain.Exceptions.ParcoursExceptions;
using UniversiteDomain.Exceptions.UeExceptions;

namespace UniversiteDomain.UseCases.EtudiantUseCases.ParcoursDansEtudiant;

public class ParcoursDansEtudiantUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task ExecuteAsync(Parcours parcours, Etudiant etudiant)
    {
        await ExecuteAsync(parcours.Id, etudiant.Id);
    }
    
    public async Task ExecuteAsync(long idParcours, long idEtudiant)
    {
        await CheckBusinessRules(idParcours, idEtudiant); 
        await repositoryFactory.EtudiantRepository().AffecterParcoursAsync(idEtudiant, idParcours);
    }
    
    public async Task CheckBusinessRules(long idParcours, long idEtudiant)
    {
        ArgumentNullException.ThrowIfNull(repositoryFactory.ParcoursRepository());
        ArgumentNullException.ThrowIfNull(repositoryFactory.EtudiantRepository());
        
        // Vérification de l'existence du parcours
        Parcours? parcoursExiste = await repositoryFactory.ParcoursRepository().FindAsync(idParcours);
        if (parcoursExiste == null)
            throw new ParcoursNotFoundException($"Aucun parcours avec l'id {idParcours}");
        
        // Vérification de l'existence de l'étudiant
        Etudiant? etudiantExiste = await repositoryFactory.EtudiantRepository().FindAsync(idEtudiant);
        if (etudiantExiste == null)
            throw new EtudiantNotFoundException($"Aucun étudiant avec l'id {idEtudiant}");

        // Vérification que l'étudiant n'est pas déjà inscrit dans un parcours
        if (etudiantExiste.ParcoursSuivi != null)
            throw new InscriptionNotFoundException($"L'étudiant avec l'id {idEtudiant} est déjà inscrit dans un parcours (id {etudiantExiste.ParcoursSuivi.Id})");
    }
    
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}