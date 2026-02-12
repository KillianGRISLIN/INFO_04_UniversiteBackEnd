using System.Linq.Expressions;
using Moq;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.EtudiantUseCases;
using UniversiteDomain.UseCases.EtudiantUseCases.Create;
using UniversiteDomain.UseCases.ParcoursUseCases.UeDansParcours;
using UniversiteDomain.UseCases.UeUseCases.Create;

namespace UniversiteDomainUnitTests;

public class UeUnitTest
{
    [SetUp]
    public void Setup()
    {
    }
    [Test]
    public async Task CreateUeUseCase()
    {
        long id = 1;
        String numeroUe = "UE1"; 
        String intitule = "Mathématiques";
        
        // On crée l'UE qui doit être ajoutée
        Ue ueSansId = new Ue{NumeroUe=numeroUe, Intitule=intitule};
        //  Créons le mock du repository
        // On initialise une fausse datasource qui va simuler un UeRepository
        var mock = new Mock<IUeRepository>();
        // Il faut ensuite aller dans le use case pour voir quelles fonctions simuler
        // Nous devons simuler FindByCondition et Create
        
        // Simulation de la fonction FindByCondition
        // On dit à ce mock que l'étudiant n'existe pas déjà
        // La réponse à l'appel FindByCondition est donc une liste vide
        var reponseFindByCondition = new List<Ue>();
        // On crée un bouchon dans le mock pour la fonction FindByCondition
        // Quelque soit le paramètre de la fonction FindByCondition, on renvoie la liste vide
        mock.Setup(repo=>repo.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>())).ReturnsAsync(reponseFindByCondition);
        
        // Simulation de la fonction Create
        // On lui dit que l'ajout d'une Ue renvoie une Ue avec l'Id 1
        Ue ueCree =new Ue{Id=id, NumeroUe=numeroUe, Intitule=intitule};
        mock.Setup(repoEtudiant=>repoEtudiant.CreateAsync(ueSansId)).ReturnsAsync(ueCree);
        
        // Création d'une fausse factory qui contient le repository simulé
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(facto=>facto.UeRepository()).Returns(mock.Object);
        
        // Création du use case en injectant notre faux repository
        CreateUeUseCase useCase=new CreateUeUseCase(mockFactory.Object);
        // Appel du use case
        var ueTeste = await useCase.ExecuteAsync(ueSansId);
        
        // Vérification du résultat
        Assert.That(ueTeste.Id, Is.EqualTo(ueCree.Id));
        Assert.That(ueTeste.NumeroUe, Is.EqualTo(ueCree.NumeroUe));
        Assert.That(ueTeste.Intitule, Is.EqualTo(ueCree.Intitule));
    }

    [Test]
    public async Task AddUeDansParcoursUseCase()
    {
        long idUe = 1;
        long idParcours = 3;
        
        // On crée le parcours et l'UE que l'on veut rajouter dans le parcours 
        Ue ue = new Ue { Id = 1, Intitule = "ASIt", NumeroUe = "INFO_04" };
        Parcours parcoursInitial = new Parcours{Id=3, NomParcours = "MIAGE", AnneeFormation = 1};
        
        // On crée l'bjet qui sera modifié
        Parcours parcoursAModifier = new Parcours{Id=3, NomParcours = "MIAGE", AnneeFormation = 1};
        
        // On initialise des faux repositories
        var mockUeRepository = new Mock<IUeRepository>();
        var mockParcoursRepository = new Mock<IParcoursRepository>();
        
        // 
        List<Ue> ues = new List<Ue>();
        ues.Add(ue);
        mockUeRepository.Setup(repo=>repo.FindByConditionAsync(u=>u.Id.Equals(idUe))).ReturnsAsync(ues);

        List<Parcours> lesParcoursInitiaux = new List<Parcours>();
        lesParcoursInitiaux.Add(parcoursInitial);
        
        List<Parcours> lesParcoursAModifier = new List<Parcours>();
        parcoursAModifier.UesEnseignees.Add(ue);
        lesParcoursAModifier.Add(parcoursAModifier);
        
        mockParcoursRepository
            .Setup(repo=>repo.FindByConditionAsync(e=>e.Id.Equals(idParcours)))
            .ReturnsAsync(lesParcoursInitiaux);
        mockParcoursRepository
            .Setup(repo => repo.AddUeAsync(idParcours, idUe))
            .ReturnsAsync(parcoursAModifier);
        
        // Création d'une fausse factory qui contient les faux repositories
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(facto=>facto.UeRepository()).Returns(mockUeRepository.Object);
        mockFactory.Setup(facto=>facto.ParcoursRepository()).Returns(mockParcoursRepository.Object);
        
        // Création du use case en utilisant le mock comme datasource
        AddUeDansParcoursUseCase useCase=new AddUeDansParcoursUseCase(mockFactory.Object);
        
        // Appel du use case
        var parcoursTest=await useCase.ExecuteAsync(idParcours, idUe);
        // Vérification du résultat
        Assert.That(parcoursTest.Id, Is.EqualTo(parcoursAModifier.Id));
        Assert.That(parcoursTest.UesEnseignees, Is.Not.Null);
        Assert.That(parcoursTest.UesEnseignees.Count, Is.EqualTo(1));
        Assert.That(parcoursTest.UesEnseignees[0].Id, Is.EqualTo(idUe));
    }
}