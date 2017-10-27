using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace SIGeneticAlgorithm
{
  public class XmlHelper
  {
    public XmlDocument ReadFile(string path)
    {
      XmlDocument doc = new XmlDocument();
      doc.Load(path);

      return doc;
    }

    public Add XMLToAddObject(string xml, Type objectType)
    {
      StringReader strReader = null;
      XmlSerializer serializer = null;
      XmlTextReader xmlReader = null;
      Add obj = null;
      try
      {
        strReader = new StringReader(xml);
        serializer = new XmlSerializer(objectType);
        xmlReader = new XmlTextReader(strReader);
         var test = serializer.Deserialize(xmlReader);
        obj = (Add) test;
      }
      catch (Exception exp)
      {
        //Handle Exception Code
      }
      finally
      {
        if (xmlReader != null)
        {
          xmlReader.Close();
        }
        if (strReader != null)
        {
          strReader.Close();
        }
      }
      return obj;
    }

    public void SaveXmlFile(Add source, Type objectType, string path)
    {
      XmlSerializer xsSubmit = new XmlSerializer(objectType);

      using (var sww = new StringWriter())
      {
        using (XmlWriter writer = XmlWriter.Create(sww))
        {
          xsSubmit.Serialize(writer, source);
          var xml = sww.ToString();
          XmlDocument doc = new XmlDocument()
          {
            InnerXml = xml
          };
          doc.Save(path);
        }
      }
    }
  }
}