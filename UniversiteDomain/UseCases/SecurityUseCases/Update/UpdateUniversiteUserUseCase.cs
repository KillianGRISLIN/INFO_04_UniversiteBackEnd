using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;
using UniversiteDomain.Exceptions.SecurityExceptions;
using UniversiteDomain.UseCases.EtudiantUseCases.Get;
using UniversiteDomain.UseCases.SecurityUseCases.Get;

namespace UniversiteDomain.UseCases.SecurityUseCases.Update;

public class UpdateUniversiteUserUseCase(IRepositoryFactory factory)
{
    public async Task<IUniversiteUser> ExecuteAsync(Etudiant etudiant)
    {
        IUniversiteUser user = await CheckBusinessRules(etudiant);
        await factory.UniversiteUserRepository().UpdateAsync(user, etudiant.Email, etudiant.Email);
        await factory.SaveChangesAsync();
        return user;
    }

    public async Task<IUniversiteUser> CheckBusinessRules(Etudiant etudiant)
    {
        ArgumentNullException.ThrowIfNull(etudiant);
        ArgumentNullException.ThrowIfNull(etudiant.Email);
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentNullException.ThrowIfNull(factory.UniversiteUserRepository());

        // Vérification de l'existence de l'étudiant
        GetEtudiantByIdUseCase getEtudiantByIdUseCase = new GetEtudiantByIdUseCase(factory);
        Etudiant? etudiantExiste = await getEtudiantByIdUseCase.ExecuteAsync(etudiant.Id);
        if (etudiantExiste == null)
            throw new EtudiantNotFoundException($"Aucun étudiant avec l'id {etudiant.Id}");

        // Vérification de l'existence du user associé à l'étudiant
        IUniversiteUser? userExiste = await factory.UniversiteUserRepository().FindByEmailAsync(etudiant.Email);
        if (userExiste == null)
            throw new UniversiteUserNotFoundException($"Aucun utilisateur avec l'email {etudiant.Email}");

        return userExiste;
    }
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}