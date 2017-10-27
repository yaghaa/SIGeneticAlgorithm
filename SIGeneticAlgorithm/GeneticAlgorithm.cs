using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SIGeneticAlgorithm
{
  public class GeneticAlgorithm
  {
    private List<Add> _generation = new List<Add>();
    private List<SumoOutput> _sumoOutputs = new List<SumoOutput>();
    private XmlHelper _hlpr = new XmlHelper();
    private Sumo _sumo = new Sumo();
    private const int FFE = 1000;
    private int _ffe = 0;
    private readonly int _n;
    private Tuple<int, int, float,float,float> result = new Tuple<int, int, float,float, float>(0, 0, 0,1,0); // iteracja, nr osobnika, ffe
    private int iteration = 1;
    private readonly float _crossProbability;
    private float _mutationProbability = 0;

    private const string Path =
      "C:\\aga\\PWR\\semestr 7\\Sztuczna inteligencja -l-\\dane_symulacji\\acosta\\acosta_tls.add.xml";

    public GeneticAlgorithm(int n,float crossProbability, float mutationProbability=0)
    {
      _crossProbability = crossProbability;
      _n = n;
      _mutationProbability = mutationProbability;
    }

    public Tuple<int, int, float, float, float> Evaluate()
    {
      // Inicjacja / dolosować osobników
      Add baseIndividual = _hlpr.XMLToAddObject(_hlpr.ReadFile(Path).InnerXml, typeof(Add));

      CalculateMutationProbability(baseIndividual);

      _generation.Add(baseIndividual);

      for (int i = 0; i < _n - 1; i++)
      {
        var individual = (Add)baseIndividual.Clone();
        individual.Initialize();
        _generation.Add(individual);
      }

      do
      {
        // Sumo
        Evaluate(_generation);
        
        // Warunek stopu
        if (_ffe >= FFE)
        {
          break;
        }
        iteration++;

        // Selekcja
        Selection();

        // Krzyżowanie
        CrossingGenes();

        // Mutacja
        Mutation();
      } while (true);

      _hlpr.SaveXmlFile(baseIndividual,typeof(Add),Path);
      result = new Tuple<int, int, float, float, float>(result.Item1,result.Item2,result.Item3,result.Item4,_sumoOutputs.Average(x=> x.Evaluation));
      return result;
    }

    private void Evaluate(List<Add> generation)
    {
      var index = 0;
      foreach (var individual in generation)
      {
        _hlpr.SaveXmlFile(individual, typeof(Add), Path);
        var sumoOutput = _sumo.RunSumo(@"cd C:\aga\PWR\semestr 7\Sztuczna inteligencja -l-\dane_symulacji\acosta");
        _sumoOutputs.Add(sumoOutput);

        Console.WriteLine($"iteration {iteration} nr osobnika {index}");

        if (sumoOutput.Evaluation > result.Item3)
        {
          result = new Tuple<int, int, float, float, float>(iteration, index, sumoOutput.Evaluation, result.Item4, result.Item5);
        }

        if (sumoOutput.Evaluation < result.Item4)
        {
          result = new Tuple<int, int, float, float, float>(result.Item1, result.Item2, result.Item3, sumoOutput.Evaluation, result.Item5);
        }
        index++;
      }

      _ffe += _n;
    }

    private void Selection()
    {
      _sumoOutputs.Reverse();
      var lastGenerationSumo = _sumoOutputs.Take(_n).ToList();
      lastGenerationSumo.Reverse();
      _sumoOutputs.Reverse();

      var newGeneration = new List<Add>();
      var random = new Random();
      for (var i = 0; i < _generation.Count(); i++)
      {
        var first = random.Next(0, _generation.Count()-1);
        var second = random.Next(0, _generation.Count()-1);

        newGeneration.Add(lastGenerationSumo[first].Evaluation >= lastGenerationSumo[second].Evaluation
          ? _generation[first]
          : _generation[second]);
      }

      _generation = newGeneration;
    }

    private void CrossingGenes()
    {
      var newGeneration = new List<Add>();
      var random = new Random();
      for (var i = 0; i < _n/2; i++)
      {
        var probability = random.NextDouble();

        var first = random.Next(0, _generation.Count-1);
        var second = random.Next(0, _generation.Count-1);

        if ((float)probability < _crossProbability)
        {
          OnePointCrossing(first, second, ref newGeneration);
          //TwoPointCrossing(first, second, ref newGeneration);
        }
        else
        {
          newGeneration.Add(_generation[first]);
          newGeneration.Add(_generation[second]);
        }

        _generation.Remove(_generation[first]);
        _generation.Remove(_generation[second]);
      }

      _generation = newGeneration;
    }

    private void Mutation()
    {
      var random = new Random();

      foreach (var add in _generation)
      {
        foreach (var ttl in add.TlLogic)
        {
          foreach (var phase in ttl.Phases)
          {
            if (phase.MinDur == phase.MaxDur) continue;
            var probability = random.NextDouble();

            if ((float) probability <= _mutationProbability)
            {
              phase.Duration = random.Next(phase.MinDur, phase.MaxDur);
            }
          }
        }
      }
    }

    private void OnePointCrossing(int first, int second, ref List<Add> newGeneration)
    {
      var firstItem = (Add)_generation[first].Clone();
      var secondItem = (Add)_generation[second].Clone();
      var random = new Random();

      var crossTtl = random.Next(0, firstItem.TlLogic.Count-1);
      var crossPhase = random.Next(0, firstItem.TlLogic[crossTtl].Phases.Count-1);

      Cross(crossTtl, crossPhase, 0, 0, ref firstItem, ref secondItem);

      newGeneration.AddRange(new[] { firstItem, secondItem });
    }

    private void TwoPointCrossing(int first, int second, ref List<Add> newGeneration)
    {
      var firstItem = (Add)_generation[first].Clone();
      var secondItem = (Add)_generation[second].Clone();
      var random = new Random();

      var crossTtl = random.Next(0, firstItem.TlLogic.Count-1);
      var crossPhase = random.Next(0, firstItem.TlLogic[crossTtl].Phases.Count-1);

      Cross(crossTtl, crossPhase, 0, 0, ref firstItem, ref secondItem);

      crossTtl = random.Next(crossTtl, firstItem.TlLogic.Count-1);
      crossPhase = random.Next(crossPhase, firstItem.TlLogic[crossTtl].Phases.Count-1);

      Cross(crossTtl, crossPhase, crossTtl, crossPhase, ref firstItem, ref secondItem);

      newGeneration.AddRange(new[] { firstItem, secondItem });
    }

    private void EqualCrossing(int first, int second, ref List<Add> newGeneration)
    {
      var firstItem = (Add)_generation[first].Clone();
      var secondItem = (Add)_generation[second].Clone();
      var random = new Random();

      var firstChild = (Add)_generation[first].Clone();
      var secondChild = (Add)_generation[second].Clone();

      for (var j = 0; j< firstItem.TlLogic.Count(); j++)
      {
        for (var i = 0; i < firstItem.TlLogic[j].Phases.Count; i++)
        {
          var selectParent = random.Next(0, 2);
          if (selectParent == 0)
          {
            firstChild.TlLogic[j].Phases[i] = firstItem.TlLogic[j].Phases[i];
            secondChild.TlLogic[j].Phases[i] = secondItem.TlLogic[j].Phases[i];
          }
          else
          {
            firstChild.TlLogic[j].Phases[i] = secondItem.TlLogic[j].Phases[i];
            secondChild.TlLogic[j].Phases[i] = firstItem.TlLogic[j].Phases[i];
          }
        }
      }

      newGeneration.AddRange(new[] { firstChild, secondChild });
    }

    private void Cross(int crossTtl, int crossPhase, int crossFromTtl, int crossFromPhase, ref Add firstItem, ref Add secondItem)
    {
      for (int i = crossFromTtl; i < crossTtl; i++)
      {
        secondItem.TlLogic[i] = firstItem.TlLogic[i];
      }

      for (int i = crossFromPhase; i < crossPhase; i++)
      {
        secondItem.TlLogic[crossTtl].Phases[i] = firstItem.TlLogic[crossTtl].Phases[i];
      }

      for (int i = crossPhase; i < firstItem.TlLogic[crossTtl].Phases.Count; i++)
      {
        firstItem.TlLogic[crossTtl].Phases[i] = secondItem.TlLogic[crossTtl].Phases[i];
      }

      for (int i = crossTtl; i < firstItem.TlLogic.Count; i++)
      {
        firstItem.TlLogic[i] = secondItem.TlLogic[i];
      }
    }

    private void CalculateMutationProbability(Add baseIndividual)
    {
      if (_mutationProbability == 0)
      {
        var vectorPhaseCount = 0;
        foreach (var ttl in baseIndividual.TlLogic)
        {
          vectorPhaseCount += ttl.Phases.Count(x => x.MaxDur != x.MinDur);
        }

        _mutationProbability = 1f / vectorPhaseCount;
      }
    }
  }
}
