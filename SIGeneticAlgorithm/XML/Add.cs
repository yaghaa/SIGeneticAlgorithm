using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Xml.Serialization;
using Newtonsoft.Json;
using SIGeneticAlgorithm;

namespace SIGeneticAlgorithm
{
  [Serializable, XmlRoot("add")]
  public class Add : ICloneable
  {
    [XmlElement("tlLogic")]
    public List<TlLogic> TlLogic { get; set; }

    public object Clone()
    {
      var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };

      return JsonConvert.DeserializeObject<Add>(JsonConvert.SerializeObject(this), deserializeSettings);
    }

    public void Initialize()
    {
      var random = new Random();
      foreach (var logic in TlLogic)
      {
        foreach (var phase in logic.Phases)
        {
          if (phase.MaxDur != phase.MinDur)
          {
            phase.Duration = random.Next(phase.MinDur, phase.MaxDur);
          }
        }
      }
    }
  }
}