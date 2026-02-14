using UniversiteDomain.DataAdapters.DataAdaptersFactory;

namespace UniversiteDomain.UseCases.EtudiantUseCases.Delete;

public class DeleteEtudiantUseCase
{
    private readonly IRepositoryFactory _repositoryFactory;

    public DeleteEtudiantUseCase(IRepositoryFactory repositoryFactory)
    {
        _repositoryFactory = repositoryFactory;
        throw new NotImplementedException();
    }
    
    public async Task ExecuteAsync(long etudiantId)
    {
        throw new NotImplementedException();
    }
}