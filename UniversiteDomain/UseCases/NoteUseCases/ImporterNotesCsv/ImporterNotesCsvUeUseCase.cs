// ImporterNotesCsvUeUseCase.cs
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;
using UniversiteDomain.UseCases.NoteUseCases.Create;
using UniversiteDomain.UseCases.NoteUseCases.Update;
using UniversiteDomain.UseCases.UeUseCases.Get;

namespace UniversiteDomain.UseCases.NoteUseCases.ImporterNotesCsv;

public class ImporterNotesCsvUeUseCase(IRepositoryFactory factory)
{
    public async Task ExecuteAsync(long ueId, Stream csvStream)
    {
        GetUeByIdUseCase getUeByIdUseCase = new GetUeByIdUseCase(factory);
        Ue? ue = await getUeByIdUseCase.ExecuteAsync(ueId);        
        ArgumentNullException.ThrowIfNull(csvStream);

        // Lecture CSV -> List<NoteCsvDto>
        List<NoteCsvDto> records = await ReadCsvAsync(csvStream);

        // Création des notes
        CreateNoteUseCase createNoteUseCase = new CreateNoteUseCase(factory);
        UpdateNoteUseCase updateNoteUseCase = new UpdateNoteUseCase(factory);
        List<Note> notesACreer = new List<Note>();
        List<Note> notesAModifier = new List<Note>();

        foreach (NoteCsvDto record in records)
        {
            if (record.Note == null) continue; // Ignore les lignes sans note
            Etudiant etudiant = await CheckBusinessRules(record.NumEtud);
            Note note = record.ToEntity();
            note.EtudiantId = etudiant.Id;
            // Vérifier si la note existe déjà pour cet étudiant et cette UE
            Note? noteExistante = (await factory.NoteRepository().FindByConditionAsync(n => n.EtudiantId == note.EtudiantId && n.UeId == note.UeId)).FirstOrDefault();
            if (noteExistante != null)
            {
                note.Id = noteExistante.Id;
                notesAModifier.Add(note);
            }
            else
            {
                notesACreer.Add(note);
            }
        }
        
        foreach (Note note in notesACreer)
        {
            await createNoteUseCase.ExecuteAsync(note.EtudiantId, note.UeId, note.Valeur);
        }
        foreach (Note note in notesAModifier)
        {
            await updateNoteUseCase.ExecuteAsync(note);
        }
    }
    
    private async Task<Etudiant> CheckBusinessRules(string numEtud)
    {
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentNullException.ThrowIfNull(factory.EtudiantRepository());
        ArgumentNullException.ThrowIfNull(factory.NoteRepository());
        
        List<Etudiant> etudiant = await factory.EtudiantRepository().FindByConditionAsync(e => e.NumEtud == numEtud);
        if (etudiant.Count == 0) {
            throw new EtudiantNotFoundException(numEtud);
        }
        return etudiant[0];
    }

    private async Task<List<NoteCsvDto>> ReadCsvAsync(Stream csvStream)
    {
        csvStream.Position = 0;

        using var reader = new StreamReader(csvStream, leaveOpen: true);

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            MissingFieldFound = null,   // ignore champs manquants
            BadDataFound = null,        // ignore data "bizarre"
            HeaderValidated = null      // ignore headers inattendus
        };

        using var csv = new CsvReader(reader, config);

        // Lis en DTO
        return csv.GetRecords<NoteCsvDto>().ToList();
    }
    
    public bool IsAuthorized(string role)
    {
        return role.Equals(Roles.Responsable) || role.Equals(Roles.Scolarite);
    }
}