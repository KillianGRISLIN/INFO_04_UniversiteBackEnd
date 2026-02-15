using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;
using UniversiteDomain.UseCases.EtudiantUseCases.Get;
using UniversiteDomain.Util;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Update;

public class UpdateEtudiantUseCase(IRepositoryFactory repositoryFactory)
{
    
    public async Task<Etudiant> ExecuteAsync(Etudiant etudiant)
    {
        Etudiant etudiantAModifier = await CheckBusinessRules(etudiant);
        etudiantAModifier.Email = etudiant.Email;
        etudiantAModifier.Nom = etudiant.Nom;
        etudiantAModifier.Prenom = etudiant.Prenom;
        etudiantAModifier.NumEtud = etudiant.NumEtud;
        await repositoryFactory.EtudiantRepository().UpdateAsync(etudiantAModifier);
        await repositoryFactory.SaveChangesAsync();
        return etudiant;
    }
    private async Task<Etudiant> CheckBusinessRules(Etudiant etudiant)
    {
        ArgumentNullException.ThrowIfNull(etudiant);
        ArgumentNullException.ThrowIfNull(etudiant.NumEtud);
        ArgumentNullException.ThrowIfNull(etudiant.Email);
        ArgumentNullException.ThrowIfNull(repositoryFactory.EtudiantRepository());
        
        // Vérification de l'existence de l'étudiant
        GetEtudiantByIdUseCase getEtudiantByIdUseCase = new GetEtudiantByIdUseCase(repositoryFactory);
        Etudiant? etudiantExiste = await getEtudiantByIdUseCase.ExecuteAsync(etudiant.Id);
        if (etudiantExiste == null)
            throw new EtudiantNotFoundException($"Aucun étudiant avec l'id {etudiant.Id}");
        
        // Vérification du format du mail
        if (!CheckEmail.IsValidEmail(etudiant.Email)) throw new InvalidEmailException(etudiant.Email + " - Email mal formé");
        
        // On recherche un étudiant avec le même numéro étudiant
        List<Etudiant> existe = await repositoryFactory.EtudiantRepository().FindByConditionAsync(e=>e.NumEtud.Equals(etudiant.NumEtud) && e.Id != etudiant.Id);
        // Si un étudiant avec le même numéro étudiant existe déjà, on lève une exception personnalisée
        if (existe is {Count:>0}) throw new DuplicateNumEtudException(etudiant.NumEtud+ " - ce numéro d'étudiant est déjà affecté à un étudiant");
        
        existe = await repositoryFactory.EtudiantRepository().FindByConditionAsync(e=>e.Email.Equals(etudiant.Email) && e.Id != etudiant.Id);
        if (existe is {Count:>0}) throw new DuplicateEmailException(etudiant.Email +" est déjà affecté à un étudiant");
        // Le métier définit que les nom doite contenir plus de 3 lettres
        if (etudiant.Nom.Length < 3) throw new InvalidNomEtudiantException(etudiant.Nom +" incorrect - Le nom d'un étudiant doit contenir plus de 3 caractères");

        return etudiantExiste;
    }
    
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}