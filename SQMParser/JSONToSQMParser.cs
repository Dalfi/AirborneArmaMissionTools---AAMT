using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace AirborneArmaMissionTools.SQM
{
  public class JSONToSQMParser
  {
    private static int indentLevel = 0;

    public static void Parse(string sourcefilepath, string destinationfilepath)
    {
      try
      {
        indentLevel = 0;

        using (StringWriter sw = new StringWriter())
        using (StreamReader reader = File.OpenText(sourcefilepath))
        {
          JObject o = (JObject)JToken.ReadFrom(new JsonTextReader(reader));

          ParseJObject(sw, o);

          if (!File.Exists(destinationfilepath))
            File.Create(destinationfilepath);

          File.WriteAllText(destinationfilepath, sw.ToString());
        }
      }
      catch (IOException ex)
      {
        Console.WriteLine("The file could not be read:");
        Console.WriteLine(ex.Message);
      }


      return;

    }

    private static void Write(StringWriter sw, string text, bool doIndentation = true)
    {
      if (doIndentation)
        text = new string('\t', indentLevel) + text;

      sw.Write(text);
    }

    private static void WriteLine(StringWriter sw, string text, bool doIndentation = true)
    {
      if (doIndentation)
        text = new string('\t', indentLevel) + text;

      sw.WriteLine(text);
    }

    private static void ParseJObject(StringWriter sw, JObject Jobj)
    {
      if (Jobj.HasValues)
      {
        JEnumerable<JToken> children = Jobj.Children();

        for (int i = 0; i < Jobj.Count; i++)
        {
          var child = children.ElementAt(i);
          TokenType type = child.GetTokenType();

          if (child.Type == JTokenType.Property)
          {
            JProperty prop = (JProperty)child;

            if (type == TokenType.Property)
            {
              if (prop.Value.Type == JTokenType.String)
                WriteLine(sw, $"{prop.Name}=\"{prop.Value}\";");
              else
                WriteLine(sw, $"{prop.Name}={prop.Value};");
              continue;
            }

            if (type == TokenType.Object)
            {
              WriteLine(sw, $"class {prop.Name}");
              WriteLine(sw, "{");
              indentLevel++;
              ParseJObject(sw, (JObject)prop.First);
              indentLevel--;
              WriteLine(sw, "};");
              continue;
            }

            if (type == TokenType.Array)
            {
              JArray array = (JArray)prop.First;

              //Sample First Value to get type
              JValue value = (JValue)array.First;

              if (value.Type == JTokenType.String)
              {
                WriteLine(sw, $"{prop.Name}[]=");
                WriteLine(sw, "{");
                indentLevel++;
                ParseJArray(sw, array, true);
                indentLevel--;
                WriteLine(sw, "};");
              }
              else
              {
                Write(sw, $"{prop.Name}[]={{");
                ParseJArray(sw, array);
                WriteLine(sw, "};", false);
              }

              continue;
            }
          }
        }

        Console.WriteLine($"ENDOBJECT{Jobj}");
      }
    }

    private static void ParseJArray(StringWriter sw, JArray jArray, bool multiline = false)
    {
      if (jArray.HasValues)
      {
        JEnumerable<JToken> children = jArray.Children();

        for (int i = 0; i < children.Count(); i++)
        {
          JValue child = (JValue)children.ElementAt(i);

          if (multiline)
          {
            if (i == children.Count() - 1)
            {
              if (child.Type == JTokenType.String)
                WriteLine(sw, $"\"{child}\"");
              else
                WriteLine(sw, $"{child}");
            }
            else
            {
              if (child.Type == JTokenType.String)
                WriteLine(sw, $"\"{child}\",");
              else
                WriteLine(sw, $"{child},");
            }
          }
          else
          {
            if (i == children.Count() - 1)
            {
              if (child.Type == JTokenType.String)
                Write(sw, $"\"{child}\"", false);
              else
                Write(sw, $"{child}", false);
            }
            else
            {
              if (child.Type == JTokenType.String)
                Write(sw, $"\"{child}\",", false);
              else
                Write(sw, $"{child},", false);
            }
          }
        }
      }
    }
  }
}
