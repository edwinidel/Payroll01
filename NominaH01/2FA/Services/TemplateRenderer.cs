using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using _2FA.Data.Entities;

namespace _2FA.Services
{
    public class TemplateRenderer : ITemplateRenderer
    {
        public Task<string> RenderAsync(DocumentTemplateEntity template, EmployeeEntity employee)
        {
            if (template == null) throw new ArgumentNullException(nameof(template));
            if (employee == null) throw new ArgumentNullException(nameof(employee));

            var tokens = BuildTokens(template, employee);
            var output = template.Content ?? string.Empty;
            foreach (var kvp in tokens)
            {
                output = output.Replace(kvp.Key, kvp.Value ?? string.Empty, StringComparison.Ordinal);
            }
            return Task.FromResult(output);
        }

        private static Dictionary<string, string?> BuildTokens(DocumentTemplateEntity template, EmployeeEntity employee)
        {
            var dict = new Dictionary<string, string?>(StringComparer.Ordinal);

            AddSimpleProperties(dict, employee, "Employee");
            AddNavigationProperties(dict, employee);

            dict["{{Today}}"] = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

            // Signature prefers template signature, then document type
            var signaturePath = template.SignaturePath ?? template.DocumentType?.SignaturePath;
            dict["{{Signature}}"] = BuildImageTag(signaturePath);

            // Company logo placeholder falls back to template logo
            dict["{{Company.Logo}}"] = BuildImageTag(template.LogoPath);

            return dict;
        }

        private static void AddSimpleProperties(Dictionary<string, string?> dict, object source, string prefix)
        {
            var props = source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in props)
            {
                var pt = prop.PropertyType;
                var underlying = Nullable.GetUnderlyingType(pt) ?? pt;
                if (!IsSimpleType(underlying)) continue;

                var val = prop.GetValue(source);
                dict[$"{{{{{prefix}.{prop.Name}}}}}"] = FormatValue(val);
            }
        }

        private static void AddNavigationProperties(Dictionary<string, string?> dict, EmployeeEntity employee)
        {
            var props = employee.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in props)
            {
                var pt = prop.PropertyType;
                if (pt == typeof(string)) continue;
                if (typeof(System.Collections.IEnumerable).IsAssignableFrom(pt) && pt != typeof(string)) continue;
                if (pt.Namespace != "_2FA.Data.Entities") continue;

                var navValue = prop.GetValue(employee);
                if (navValue == null) continue;

                AddSimpleProperties(dict, navValue, prop.Name);
            }
        }

        private static bool IsSimpleType(Type t)
        {
            return t.IsPrimitive || t.IsEnum || t == typeof(string) || t == typeof(decimal) || t == typeof(DateTime) || t == typeof(DateTimeOffset) || t == typeof(Guid) || t == typeof(bool);
        }

        private static string? FormatValue(object? value)
        {
            if (value == null) return null;

            return value switch
            {
                DateTime dt => dt.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                DateTimeOffset dto => dto.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
                _ => value.ToString()
            };
        }

        private static string? BuildImageTag(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) return null;
            var sanitized = relativePath.Replace("\\", "/");
            if (!sanitized.StartsWith("/", StringComparison.Ordinal))
            {
                sanitized = "/" + sanitized;
            }
            return $"<img src=\"{sanitized}\" style=\"max-height:150px;\" alt=\"\" />";
        }
    }
}
