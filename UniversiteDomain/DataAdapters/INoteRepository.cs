using UniversiteDomain.Entities;
namespace UniversiteDomain.DataAdapters;

public interface INoteRepository : IRepository<Note>
{
    public Task<List<Note>> FindNoteWithUe(long etudiantId, long ueId);
    public Task<List<Note>> FindNoteWithUe(long etudiantId);
}