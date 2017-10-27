using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace SIGeneticAlgorithm
{
  public class Sumo
  {
    public SumoOutput RunSumo(string path)
    {

      var output = Run(path);
      return TranslateSumoOutput(output);
    }

    private SumoOutput TranslateSumoOutput(string output)
    {
      var sumo = new SumoOutput();
      var splitedValues = output.Split('|');

      foreach (var value in splitedValues)
      {
        var splitedRow = value.Trim().Split(' ');

        if (splitedRow.Any(x => x.Contains("Inserted")))
        {
          sumo.Inserted = int.Parse(splitedRow[1]);
          if (splitedRow.Any(x => x.Contains("Loaded")))
            sumo.Loaded = int.Parse(splitedRow[3].Replace(")",""));
        }

        if(splitedRow.Any(x=> x.Contains("Running")))
          sumo.Running = int.Parse(splitedRow[1]);

        if (splitedRow.Any(x => x.Contains("Waiting")))
          sumo.Waiting = int.Parse(splitedRow[1]);
      }

      return sumo;
    }

    private string Run(string path)
    {
      var output = "";
      var doWork = true;
      var compiler = new Process
      {
        StartInfo =
        {
          FileName = "C:\\Users\\Agata\\Downloads\\sumo-win64-0.31.0\\sumo-0.31.0\\bin\\start-command-line.bat",
          Arguments = "",
          UseShellExecute = false,
          RedirectStandardOutput = true,
          RedirectStandardInput = true
        },
        EnableRaisingEvents = true
      };
      compiler.OutputDataReceived += (sender, arg) =>
      {
        if (arg.Data.Contains("Inserted: ") || arg.Data.Contains("Running:") || arg.Data.Contains("(Loaded:") ||
            arg.Data.Contains("Waiting:"))
        {
          output += arg.Data;
          output += "|";
        }
          

        if (output.Contains("Waiting:"))
          doWork = false;
      };
      compiler.Start();

      compiler.StandardInput.WriteLine(path);
      
      compiler.StandardInput.WriteLine(@"sumo -c run.sumo.cfg -v");

      compiler.BeginOutputReadLine();
      while (doWork)
      {
      }
      compiler.CancelOutputRead();

      return output;
    }
  }
}