using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirborneArmaMissionTools
{
  public enum TokenType : int
  {
    None = 0,
    Object = 1,
    Array = 2,
    Constructor = 3,
    Property = 4,
    Comment = 5,
    Value = 6
  }

  /// <summary>
  /// Stellt Erweiterungsmethoden zur Verfügung.
  /// </summary>
  public static class Extensions
  {
    public static void ThrowIfArgumentIsNull<T>(this T obj, string parameterName) where T : class
    {
      if (obj == null)
        throw new ArgumentNullException(parameterName + " not allowed to be null");
    }

    public static bool IsNullOrEmpty(this string s)
    {
      return String.IsNullOrEmpty(s);
    }

    public static bool IsAnyOf<T>(this T source, params T[] values)
    {
      return values.Contains(source);
    }

    public static TokenType GetTokenType(this JToken token)
    {
      int i = (int)token.Type;

      if (i >= 6)
        i = 6;

      if (token.Count() == 1)
      {
        if (token.First().Type == JTokenType.Object)
          return TokenType.Object;
        if (token.First().Type == JTokenType.Array)
          return TokenType.Array;
      }

      return (TokenType)i;
    }
  }
}
