// GenererCsvNotesUeUseCase.cs
using CsvHelper;
using System.Globalization;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.UeExceptions;
using UniversiteDomain.UseCases.EtudiantUseCases.Get;
using UniversiteDomain.UseCases.UeUseCases.Get;

namespace UniversiteDomain.UseCases.NoteUseCases.GenererCsvNotesUe;

public class GenererCsvNotesUeUseCase(IRepositoryFactory factory)
{
    public async Task<MemoryStream> ExecuteAsync(long ueId)
    {
        Ue? ue = await CheckBusinessRules(ueId);

        GetTousLesEtudiantsByUeIdUseCase getTousLesEtudiantsByUeIdUseCase = new GetTousLesEtudiantsByUeIdUseCase(factory);
        List<Etudiant>? etudiants = await getTousLesEtudiantsByUeIdUseCase.ExecuteAsync(ueId);
    
        // On créer une liste de NoteCsvDto à partir des étudiants et de leurs notes dans l'UE
        List<NoteCsvDto> records = new List<NoteCsvDto>();
        if (etudiants != null)
        {
            foreach (Etudiant etudiant in etudiants)
            {
                records.Add(new NoteCsvDto().ToDto(etudiant, ue));
            }
        }
    
        // On utilise CsvHelper pour écrire les données dans un MemoryStream
        MemoryStream stream = new MemoryStream();
        using (StreamWriter writer = new StreamWriter(stream, leaveOpen: true))
        using (CsvWriter csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(records);
        }
        stream.Position = 0;
        return stream;
    }
    
    public async Task<Ue> CheckBusinessRules(long ueId)
    {
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentNullException.ThrowIfNull(factory.UeRepository());
        ArgumentNullException.ThrowIfNull(factory.NoteRepository());
        
        GetUeByIdUseCase getUeByIdUseCase = new GetUeByIdUseCase(factory);
        Ue? ue = await getUeByIdUseCase.ExecuteAsync(ueId);
        if (ue == null) throw new UeNotFoundException($"Aucune UE avec l'id {ueId}");

        return ue;
    }
    
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}