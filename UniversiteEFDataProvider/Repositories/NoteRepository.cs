using Microsoft.EntityFrameworkCore;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;

namespace UniversiteEFDataProvider.Repositories;

public class NoteRepository(UniversiteDbContext context) : Repository<Note>(context), INoteRepository
{
    public async Task<List<Note>> FindNoteWithUe(long etudiantId, long ueId)
    {
        ArgumentNullException.ThrowIfNull(Context.Notes);
        return await Context.Notes.Where(n => n.EtudiantId == etudiantId && n.UeId == ueId).Include(n => n.Ue).ToListAsync();
    }
    
    public async Task<List<Note>> FindNoteWithUe(long etudiantId)
    {
        ArgumentNullException.ThrowIfNull(Context.Notes);
        return await Context.Notes.Where(n => n.EtudiantId == etudiantId).Include(n => n.Ue).ToListAsync();
    }

    public new async Task<List<Note>> FindAllAsync()
    {
        ArgumentNullException.ThrowIfNull(Context.Notes);
        return await Context.Notes.Include(n => n.Ue).ToListAsync();
    }
}