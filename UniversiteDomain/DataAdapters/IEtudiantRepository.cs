using UniversiteDomain.Entities;
namespace UniversiteDomain.DataAdapters;

public interface IEtudiantRepository : IRepository<Etudiant>
{
    Task AffecterParcoursAsync(long idEtudiant, long idParcours); 
    Task AffecterParcoursAsync(Etudiant etudiant, Parcours parcours);
    public Task<Etudiant?> FindEtudiantCompletAsync(long idEtudiant);
    public Task<List<Etudiant>?> FindEtudiantsAvecNoteUeAsync(long idUe);
}