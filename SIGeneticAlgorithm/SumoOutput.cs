namespace SIGeneticAlgorithm
{
  public class SumoOutput
  {
    public int Loaded { get; set; }
    public int Inserted { get; set; }
    public int Running { get; set; }
    public int Waiting { get; set; }

    public float Evaluation => (1 - (float.Parse(Loaded.ToString()) - Inserted + Running + Waiting)/ Loaded);
  }
}