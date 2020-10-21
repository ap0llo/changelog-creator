﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Grynwald.ChangeLog.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Scriban;
using Scriban.Runtime;

namespace generate_docs
{
    internal static class DocsRenderer
    {
        /// <summary>
        /// Configuration functions made available to scriban templates
        /// </summary>
        private class ConfigurationFunctions : ScriptObject
        {
            private static readonly JsonSerializerSettings s_JsonSerializerSettings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            };

            public static string? GetScalar(string settingsKey)
            {
                var value = GetConfigurationValue(settingsKey);

                return value switch
                {
                    null => null,
                    string stringValue when String.IsNullOrEmpty(stringValue) => null,
                    string stringValue => stringValue,
                    Enum enumValue => enumValue.ToString(),
                    _ => JsonConvert.SerializeObject(value, s_JsonSerializerSettings)
                };
            }

            public static IEnumerable<object> GetEnumerable(string settingsKey)
            {
                var value = GetConfigurationValue(settingsKey);

                return value switch
                {
                    IEnumerable enumerable => enumerable.Cast<object>(),
                    _ => Enumerable.Empty<object>()
                };
            }

            public static string GetEnvironmentVariableName(string settingsKey) => settingsKey.Replace(":", "__").ToUpper();


            private static object? GetConfigurationValue(string settingsKey)
            {
                var rootObject = new
                {
                    ChangeLog = ChangeLogConfigurationLoader.GetDefaultConfiguration()
                };

                return GetPropertyValue(rootObject, settingsKey, ':');
            }
        }

        /// <summary>
        /// IEnumerable functions made available to scriban templates
        /// </summary>
        private class EnumerableFunctions : ScriptObject
        {
            public static object? OrderByDescending(IEnumerable toSort, string sortBy)
            {
                return toSort?.Cast<object>()?.OrderByDescending(obj => GetPropertyValue(obj, sortBy, '.'));
            }
        }


        public static string RenderTemplate(string inputPath)
        {
            var input = File.ReadAllText(inputPath);

            var context = new TemplateContext();
            var rootScriptObject = new ScriptObject()
            {
                { "configuration", new ConfigurationFunctions() },
                { "enumerable", new EnumerableFunctions() }
            };

            context.PushGlobal(rootScriptObject);

            var output = new StringBuilder();

            output.AppendLine("<!--");
            output.AppendLine("  <auto-generated>");
            output.AppendLine("    The contents of this file were generated by a tool.");
            output.AppendLine("    Any changes to this file will be overwritten.");
            output.AppendLine($"    To change the content of this file, edit '{Path.GetFileName(inputPath)}'");
            output.AppendLine("  </auto-generated>");
            output.AppendLine("-->");

            var template = Template.Parse(input);
            output.Append(template.Render(context));
            return output.ToString();
        }


        private static object? GetPropertyValue(object configuration, string propertyPath, char separator)
        {
            var propertyNames = propertyPath.Split(separator);

            var currentObject = configuration;
            foreach (var propertyName in propertyNames)
            {
                currentObject = currentObject
                    ?.GetType()
                    ?.GetProperties()
                    ?.SingleOrDefault(p => StringComparer.OrdinalIgnoreCase.Equals(p.Name, propertyName))
                    ?.GetValue(currentObject);
            }

            return currentObject;
        }
    }
}
