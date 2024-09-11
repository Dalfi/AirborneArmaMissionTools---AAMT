using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirborneArmaMissionTools.SQM
{
  public class SQMToJSONParser
  {
    public static void Parse(string sourcefilepath, string destinationfilepath)
    {
      bool InArray = false;

      try
      {
        using (StringWriter sw = new StringWriter())
        using (JsonTextWriter writer = new JsonTextWriter(sw))
        {
          writer.Formatting = Formatting.Indented;

          writer.WriteStartObject();

          // read from file
          using (StreamReader file = File.OpenText(sourcefilepath))
          {
            while (!file.EndOfStream)
            {
              string line = file.ReadLine().Trim();

              if (!string.IsNullOrEmpty(line) && line != "{")
              {
                // Aktuell in einem mehrzeiligen Array
                if (InArray)
                {
                  // Ende des Arrays erreicht?
                  if (line == "};")
                  {
                    writer.WriteEndArray();
                    InArray = false;
                  }
                  else
                  {
                    WriteJSONValue(writer, line);
                  }

                  continue;
                }

                //Neues Element
                if (line.StartsWith("class "))
                {
                  writer.WritePropertyName(line.Substring(6));
                  writer.WriteStartObject();
                  continue;
                }

                // Ist es ein Array?
                if (line.Contains("[]"))
                {
                  writer.WritePropertyName(line.Substring(0, line.IndexOf("[]")));
                  writer.WriteStartArray();

                  // Ist es ein mehrzeiliges Array?
                  if (!line.Contains("};"))
                    InArray = true;
                  else
                  {
                    string values = line.Substring(line.IndexOf("{") + 1);
                    values = values.TrimEnd('}', ';');

                    foreach (string value in values.Split(','))
                    {
                      WriteJSONValue(writer, value);
                    }

                    writer.WriteEndArray();
                  }

                  continue;
                }

                // Ende des aktuellen Elements
                if (line == "};")
                {
                  writer.WriteEndObject();
                  continue;
                }

                // Standardfall es handelt sich um ein Property
                WriteJSONProperty(writer, line);
              }
            }
          }

          writer.WriteEndObject();

          if(!File.Exists(destinationfilepath))
            File.Create(destinationfilepath);
            
          File.WriteAllText(destinationfilepath, sw.ToString());
        }
      }
      catch (IOException ex)
      {
        Console.WriteLine("The file could not be read:");
        Console.WriteLine(ex.Message);
      }
    }

    private static void WriteJSONProperty(JsonTextWriter writer, string line)
    {
      int i = line.IndexOf('=');
      writer.WritePropertyName(line.Substring(0, i));
      WriteJSONValue(writer, line.Substring(i + 1));
    }

    private static void WriteJSONValue(JsonTextWriter writer, string value)
    {
      value = value.TrimEnd(';');

      // Check ob String 
      if (value.StartsWith("\""))
        writer.WriteValue(value.Replace("\"", string.Empty));
      else
      {
        if (value.Contains('.'))
        {
          double v;

          if (double.TryParse(value, out v))
            writer.WriteValue(v);
        }
        else
        {
          int v;

          if (int.TryParse(value, out v))
            writer.WriteValue(v);
        }
      }
    }
  }
}
