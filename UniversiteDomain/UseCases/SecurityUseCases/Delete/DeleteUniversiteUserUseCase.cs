using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;
using UniversiteDomain.Exceptions.SecurityExceptions;
using UniversiteDomain.UseCases.EtudiantUseCases.Get;
using UniversiteDomain.UseCases.SecurityUseCases.Get;

namespace UniversiteDomain.UseCases.SecurityUseCases.Delete;

public class DeleteUniversiteUserUseCase(IRepositoryFactory factory)
{
    public async Task ExecuteAsync(long id)
    {
        await CheckBusinessRules(id);
        await factory.UniversiteUserRepository().DeleteAsync(id);
        await factory.SaveChangesAsync();
    }
    private async Task CheckBusinessRules(long id)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentNullException.ThrowIfNull(factory.UniversiteUserRepository());

        // Récupérer l'étudiant par son id
        GetEtudiantByIdUseCase getEtudiantByIdUseCase = new GetEtudiantByIdUseCase(factory);
        Etudiant? etudiantExiste = await getEtudiantByIdUseCase.ExecuteAsync(id);
        if (etudiantExiste == null)
            throw new EtudiantNotFoundException($"Aucun étudiant avec l'id {id}");
        // Utiliser l'email de l'étudiant pour retrouver l'user
        FindUniversiteUserByEmailUseCase findUniversiteUserByEmailUseCase = new FindUniversiteUserByEmailUseCase(factory);
        IUniversiteUser? user = await findUniversiteUserByEmailUseCase.ExecuteAsync(etudiantExiste.Email);
        if (user == null)
            throw new UniversiteUserNotFoundException($"Aucun utilisateur avec l'email {etudiantExiste.Email}");
    }
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}