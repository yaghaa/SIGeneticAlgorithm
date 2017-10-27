using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIGeneticAlgorithm
{
  class Program
  {
   static void Main(string[] args)
    {
      var ga = new GeneticAlgorithm(50,0.7f);
      var result = ga.Evaluate();

      Console.WriteLine("Iteracja:");
      Console.WriteLine(result.Item1);
      Console.WriteLine("Osobnik:");
      Console.WriteLine(result.Item2);
      Console.WriteLine("Funkcja Przystosowania:");
      Console.WriteLine(result.Item3);
      Console.WriteLine("Funkcja Przystosowania MIN:");
      Console.WriteLine(result.Item4);
      Console.WriteLine("Funkcja Przystosowania AVG:");
      Console.WriteLine(result.Item5);
      Console.ReadKey();
    }
  }
}
