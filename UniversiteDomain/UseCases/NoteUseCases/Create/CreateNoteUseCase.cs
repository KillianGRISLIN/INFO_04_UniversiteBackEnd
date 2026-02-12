using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.NoteExceptions;
using UniversiteDomain.Exceptions.UeExceptions;

namespace UniversiteDomain.UseCases.NoteUseCases.Create;

public class CreateNoteUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<Note> ExecuteAsync(Etudiant etudiant, Ue ue, double valeur) 
    {
        var note = new Note{Etudiant = etudiant, Ue = ue, Valeur = valeur };
        return await ExecuteAsync(note);
    }
    public async Task<Note> ExecuteAsync(Note note)
    {
        await CheckBusinessRules(note);
        Note noteCree = await repositoryFactory.NoteRepository().CreateAsync(note);
        repositoryFactory.SaveChangesAsync().Wait();
        //On met à jour les listes de notes de l'étudiant et de l'UE
        note.Ue.Notes.Add(noteCree);
        note.Etudiant.Notes.Add(noteCree);
        return noteCree;
    }
    private async Task CheckBusinessRules(Note note)
    {
        ArgumentNullException.ThrowIfNull(note);
        ArgumentNullException.ThrowIfNull(note.Etudiant);
        ArgumentNullException.ThrowIfNull(note.Ue);
        ArgumentNullException.ThrowIfNull(note.Valeur);
        ArgumentNullException.ThrowIfNull(repositoryFactory.NoteRepository());
    
        // On recherche si l'étudiant a déjà une note pour cette UE
        List<Note> existe = await repositoryFactory.NoteRepository().FindByConditionAsync(n=>n.Etudiant.Id.Equals(note.Etudiant.Id) && n.Ue.Id.Equals(note.Ue.Id));
        if (existe.Count > 0) throw new DuplicateNoteException(note.Etudiant.Id + "a déjà une note pour l'UE " + note.Ue.Id);

        // On vérifie que l'étudiant est bien inscrit dans l'UE
        List<Etudiant> etudiantInscrit = await repositoryFactory.EtudiantRepository().FindByConditionAsync(e=>e.Id.Equals(note.Etudiant.Id) && e.ParcoursSuivi.UesEnseignees.Any(u=>u.Id.Equals(note.Ue.Id)));
        if (etudiantInscrit.Count == 0) throw new InscriptionNotFoundException(note.Etudiant.Id + " n'est pas inscrit dans l'UE " + note.Ue.Id);
        
        // Le métier définit que les notes doivent être compris entre 0 et 20
        if (note.Valeur < 0 || note.Valeur > 20) throw new InvalidValeurException(note.Valeur+" incorrect - la valeur d'une note doit être comprise entre 0 et 20");
    }
}