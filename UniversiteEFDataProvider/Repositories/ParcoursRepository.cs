using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;

namespace UniversiteEFDataProvider.Repositories;

public class ParcoursRepository(UniversiteDbContext context) : Repository<Parcours>(context), IParcoursRepository
{
    public async Task<Parcours> AddEtudiantAsync(Parcours parcours, Etudiant etudiant)
    {
        Parcours p = (await AddEtudiantAsync(parcours.Id, etudiant.Id))!; 
        return p;
    }

    public async Task<Parcours> AddEtudiantAsync(long idParcours, long idEtudiant)
    {
        ArgumentNullException.ThrowIfNull(Context.Etudiants);
        ArgumentNullException.ThrowIfNull(Context.Parcours);
        Etudiant e = (await Context.Etudiants.FindAsync(idEtudiant))!;
        Parcours p = (await Context.Parcours.FindAsync(idParcours))!;
        p.Inscrits.Add(e);
        await Context.SaveChangesAsync();
        return p;
    }

    public async Task<Parcours> AddEtudiantAsync(Parcours? parcours, List<Etudiant> etudiants)
    {
        long[] idEtudiants = etudiants.Select(e => e.Id).ToArray();
        Parcours p = (await AddEtudiantAsync(parcours.Id, idEtudiants))!; 
        return p;
    }

    public async Task<Parcours> AddEtudiantAsync(long idParcours, long[] idEtudiants)
    {
        ArgumentNullException.ThrowIfNull(Context.Etudiants);
        ArgumentNullException.ThrowIfNull(Context.Parcours);
        Parcours p = (await Context.Parcours.FindAsync(idParcours))!;
        foreach (long idEtudiant in idEtudiants)
        {
            Etudiant e = (await Context.Etudiants.FindAsync(idEtudiant))!;
            p.Inscrits.Add(e);
        } 
        await Context.SaveChangesAsync(); 
        return p;
    }

    public async Task<Parcours> AddUeAsync(Parcours parcours, Ue ue)
    {
        Parcours p = (await AddUeAsync(parcours.Id, ue.Id))!; 
        return p;
    }

    public async Task<Parcours> AddUeAsync(long idParcours, long idUe)
    {
        ArgumentNullException.ThrowIfNull(Context.Parcours);
        ArgumentNullException.ThrowIfNull(Context.Ues); 
        Parcours p = (await Context.Parcours.FindAsync(idParcours))!; 
        Ue u = (await Context.Ues.FindAsync(idUe))!; 
        p.UesEnseignees.Add(u); 
        await Context.SaveChangesAsync(); 
        return p;
    }

    public async Task<Parcours> AddUeAsync(Parcours? parcours, List<Ue> ues)
    {
        long[] idUes = ues.Select(e => e.Id).ToArray(); 
        Parcours p = (await AddUeAsync(parcours.Id, idUes))!; 
        return p;
    }

    public async Task<Parcours> AddUeAsync(long idParcours, long[] idUes)
    {
        ArgumentNullException.ThrowIfNull(Context.Parcours);
        ArgumentNullException.ThrowIfNull(Context.Ues); 
        Parcours p = (await Context.Parcours.FindAsync(idParcours))!;
        foreach (long idUe in idUes)
        {
            Ue u = (await Context.Ues.FindAsync(idUe))!; p.UesEnseignees.Add(u);
        } 
        await Context.SaveChangesAsync(); 
        return p;
    }
}