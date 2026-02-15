using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.NoteExceptions;
using UniversiteDomain.Exceptions.UeExceptions;

namespace UniversiteDomain.UseCases.NoteUseCases.Update;

public class UpdateNoteUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task ExecuteAsync(Note note)
    {
        Note noteAModifier = await CheckBusinessRules(note);
        ArgumentNullException.ThrowIfNull(note);
        
        noteAModifier.Valeur = note.Valeur;

        await repositoryFactory.NoteRepository().UpdateAsync(noteAModifier);
        await repositoryFactory.SaveChangesAsync();
    }
    
    private async Task<Note> CheckBusinessRules(Note note)
    {
        ArgumentNullException.ThrowIfNull(note);
        ArgumentNullException.ThrowIfNull(note.EtudiantId);
        ArgumentNullException.ThrowIfNull(note.UeId);
        ArgumentNullException.ThrowIfNull(note.Valeur);
        ArgumentNullException.ThrowIfNull(repositoryFactory.NoteRepository());
    
        // On recherche si l'étudiant a déjà une note pour cette UE
        List<Note> existe = await repositoryFactory.NoteRepository().FindByConditionAsync(n=>n.EtudiantId.Equals(note.EtudiantId) && n.UeId.Equals(note.UeId));
        if (existe.Count == 0) throw new NoteNotFoundException(note.EtudiantId + " a déjà une note pour l'UE " + note.UeId);

        // On vérifie que l'étudiant est bien inscrit dans l'UE
        List<Etudiant> etudiantInscrit = await repositoryFactory.EtudiantRepository().FindByConditionAsync(e=>e.Id.Equals(note.EtudiantId) && e.ParcoursSuivi.UesEnseignees.Any(u=>u.Id.Equals(note.UeId)));
        if (etudiantInscrit.Count == 0) throw new InscriptionNotFoundException(note.EtudiantId+ " n'est pas inscrit dans l'UE " + note.UeId);
        
        // Le métier définit que les notes doivent être compris entre 0 et 20
        if (note.Valeur < 0 || note.Valeur > 20) throw new InvalidValeurException(note.Valeur+" incorrect - la valeur d'une note doit être comprise entre 0 et 20");
        
        return existe[0];
    }
}

