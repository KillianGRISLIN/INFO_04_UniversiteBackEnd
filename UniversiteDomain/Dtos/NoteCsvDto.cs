using UniversiteDomain.Entities;

namespace UniversiteDomain.Dtos;

public class NoteCsvDto
{
    public string NumEtud { get; set; }
    public string Nom { get; set; }
    public string Prenom { get; set; }
    public long NumeroUe { get; set; }
    public string Intitule { get; set; }
    public float? Note { get; set; }
    
    public NoteCsvDto ToDto(Etudiant etudiant, Ue ue)
    {
        Note note = etudiant.NotesObtenues?.FirstOrDefault(n => n.UeId == ue.Id);
        this.NumEtud = etudiant.NumEtud;
        this.Nom = etudiant.Nom;
        this.Prenom = etudiant.Prenom;
        this.NumeroUe = ue.Id;
        this.Intitule = ue.Intitule;
        if (note != null) this.Note = note.Valeur;
        return this;
    }
    
    public Note ToEntity()
    {
        return new Note { EtudiantId = 0, UeId = this.NumeroUe, Valeur = this.Note ?? 0 };
    }
}