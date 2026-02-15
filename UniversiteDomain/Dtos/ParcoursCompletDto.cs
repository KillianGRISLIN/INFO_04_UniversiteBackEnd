using UniversiteDomain.Entities;

namespace UniversiteDomain.Dtos;

public class ParcoursCompletDto
{
    public long Id { get; set; }
    public string NomParcours { get; set; } = string.Empty;
    public int AnneeFormation { get; set; }
    public List<EtudiantDto> Inscrits { get; set; } = new();
    public List<UeDto> UesEnseignees { get; set; } = new();

    public ParcoursCompletDto ToDto(Parcours parcours)
    {
        Id = parcours.Id;
        NomParcours = parcours.NomParcours;
        AnneeFormation = parcours.AnneeFormation;
        if (parcours.Inscrits != null)
        {
            Inscrits = EtudiantDto.ToDtos(parcours.Inscrits);
        }
        if (parcours.UesEnseignees != null)
        {
            UesEnseignees = UeDto.ToDtos(parcours.UesEnseignees);
        }
        
        return this;
    }
}
