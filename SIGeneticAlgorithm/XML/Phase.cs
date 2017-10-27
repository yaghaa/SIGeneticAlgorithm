using System;
using System.Xml.Serialization;

namespace SIGeneticAlgorithm
{
  [Serializable]
  public class Phase
  {
    [XmlAttribute("duration")]
    public int Duration { get; set; }
    [XmlAttribute("minDur")]
    public int MinDur { get; set; }
    [XmlAttribute("maxDur")]
    public int MaxDur { get; set; }
    [XmlAttribute("state")]
    public string State { get; set; }
  }
}