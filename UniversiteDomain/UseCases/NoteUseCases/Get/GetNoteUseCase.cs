using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;
using UniversiteDomain.Exceptions.UeExceptions;
using UniversiteDomain.UseCases.EtudiantUseCases.Get;
using UniversiteDomain.UseCases.UeUseCases.Get;

namespace UniversiteDomain.UseCases.NoteUseCases.Get;

public class GetNoteUseCase(IRepositoryFactory factory)
{
    public async Task<Note?> ExecuteAsync(long etudiantId, long ueId)
    {
        await CheckBusinessRules(etudiantId, ueId);
        List<Note> notes = await factory.NoteRepository().FindNoteWithUe(etudiantId, ueId);
        return notes.FirstOrDefault();
    }
    
    public async Task<List<Note>> ExecuteAsync(long etudiantId)
    {
        await CheckBusinessRules(etudiantId);
        return await factory.NoteRepository().FindNoteWithUe(etudiantId);
    }
    
    public async Task CheckBusinessRules(long etudiantId)
    {
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentNullException.ThrowIfNull(factory.NoteRepository());
        
        GetEtudiantByIdUseCase getEtudiantByIdUseCase = new GetEtudiantByIdUseCase(factory);
        Etudiant? etudiant = await getEtudiantByIdUseCase.ExecuteAsync(etudiantId);
        if (etudiant == null) throw new EtudiantNotFoundException("L'étudiant avec l'ID "+etudiantId+" n'existe pas");
    }
    
    public async Task CheckBusinessRules(long etudiantId, long ueId)
    {
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentNullException.ThrowIfNull(factory.NoteRepository());
        
        GetEtudiantByIdUseCase getEtudiantByIdUseCase = new GetEtudiantByIdUseCase(factory);
        Etudiant? etudiant = await getEtudiantByIdUseCase.ExecuteAsync(etudiantId);
        if (etudiant == null) throw new EtudiantNotFoundException("L'étudiant avec l'ID "+etudiantId+" n'existe pas");
        
        GetUeByIdUseCase getUeByIdUseCase = new GetUeByIdUseCase(factory);
        Ue? ue = await getUeByIdUseCase.ExecuteAsync(ueId);
        if (ue == null) throw new UeNotFoundException("L'UE avec l'ID "+ueId+" n'existe pas");
    }
    
    public bool IsAuthorized(string role, IUniversiteUser user, long idEtudiant)
    {
        if (role.Equals(Roles.Scolarite) || role.Equals(Roles.Responsable)) return true;
        // Si c'est un étudiant qui est connecté,
        // il ne peut consulter que ses propres informations
        return user.Etudiant!=null && role.Equals(Roles.Etudiant) && user.Etudiant.Id==idEtudiant;
    }
}

