namespace ApiGateway.Application.Extensions;

public static class QueryStringExtensions {

  public static string ToQueryString (this object obj) {
    if (obj == null) return string.Empty;

    var properties = obj.GetType ().GetProperties ()
     .Where (p => p.GetValue (obj) != null) // Исключаем свойства, где значение == null
     .SelectMany (p =>
      {
        var value = p.GetValue (obj);

        // Проверяем, если вложенный объект, преобразуем его свойства
        if (value != null && !value.GetType ().IsPrimitive && !(value is string)) {
          return value.GetType ().GetProperties ()
           .Where (subProp => subProp.GetValue (value) != null) // Исключаем null из вложенных
           .Select (subProp => $"{subProp.Name}={Uri.EscapeDataString (subProp.GetValue (value)?.ToString ())}");
        }

        // Если простое свойство (не коллекция и не вложенный объект)
        return new[] { $"{p.Name}={Uri.EscapeDataString (value.ToString ())}" };
      });

    return string.Join ("&", properties);
  }

}