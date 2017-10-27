using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SIGeneticAlgorithm
{
  [Serializable]
  public class TlLogic
  {
    [XmlAttribute("id")]
    public int Id { get; set; }
    [XmlAttribute("type")]
    public string Type { get; set; }
    [XmlAttribute("programID")]
    public string ProgramID { get; set; }
    [XmlAttribute("offset")]
    public string Offset { get; set; }
    [XmlElement("phase")]
    public List<Phase> Phases { get; set; }
  }
}