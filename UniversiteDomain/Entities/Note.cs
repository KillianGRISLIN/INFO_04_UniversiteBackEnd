namespace UniversiteDomain.Entities;

public class Note
{
    public long Id { get; set; }
    public long EtudiantId { get; set; }
    public long UeId { get; set; } 
    public Etudiant Etudiant { get; set; } 
    public Ue Ue { get; set; }
    public double Valeur { get; set; }

    public override string ToString()
    {
        return "ID "+Id +" :  Etudiant "+EtudiantId+" - UE "+UeId+" - Valeur "+Valeur; }
}