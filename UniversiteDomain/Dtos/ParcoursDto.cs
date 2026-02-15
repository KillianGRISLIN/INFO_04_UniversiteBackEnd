using UniversiteDomain.Entities;

namespace UniversiteDomain.Dtos;

public class ParcoursDto
{
    public long Id { get; set; }
    public string NomParcours { get; set; }
    public int AnneeFormation { get; set; }

    public ParcoursDto ToDto(Parcours parcours)
    {
        this.Id = parcours.Id;
        this.NomParcours = parcours.NomParcours;
        this.AnneeFormation = parcours.AnneeFormation;
        return this;
    }
    
    public static List<ParcoursDto> ToDtos(List<Parcours> parcours)
    {
        List<ParcoursDto> dtos = new List<ParcoursDto>();
        foreach (var p in parcours)
        {
            dtos.Add(new ParcoursDto().ToDto(p));
        }
        return dtos;
    }
    
    public Parcours ToEntity()
    {
        return new Parcours {Id = this.Id, NomParcours = this.NomParcours, AnneeFormation = this.AnneeFormation};
    }
}