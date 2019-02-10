using JsonMigrator.Exceptions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JsonMigrator
{
    public static class JsonMigrator
    {
        private static readonly Type _migrationAttirbuteType = typeof(MigrationAttribute);
        private static readonly Type _jtokenType = typeof(JToken);

        public static void Migrate<T>(JToken token)
        {
            var type = typeof(T);
            var version = token.Value<string>("JsonVersion");

            if (string.IsNullOrEmpty(version))
            {
                throw new JsonMigrationVersionException("No version found in the root of the given JToken");
            }

            var migrationMethods = type.GetMethods().Where(m => m.GetCustomAttributes(_migrationAttirbuteType, false).Length > 0).ToList();
            var methodsWithMigrations = new Dictionary<MethodInfo, MigrationAttribute>();

            foreach (var migrationMethod in migrationMethods)
            {
                var parameters = migrationMethod.GetParameters();

                if (!migrationMethod.IsStatic)
                {
                    throw new JsonMigrationMethodException($"Method {migrationMethod.Name} is not static");
                }
                else if (migrationMethod.IsPublic)
                {
                    throw new JsonMigrationMethodException($"Method {migrationMethod.Name} should not be public");
                }
                else if (migrationMethod.ReturnType != _jtokenType)
                {
                    throw new JsonMigrationMethodException($"Method {migrationMethod.Name} has a wrong return type. Return type should be {_jtokenType.Name}");
                }
                else if (parameters.Count() != 1)
                {
                    throw new JsonMigrationMethodException($"Method {migrationMethod.Name} has not enough or too much parameters, there should be one parameter of type {_jtokenType.Name}");
                }
                else if (parameters[0].ParameterType != _jtokenType)
                {
                    throw new JsonMigrationMethodException($"Method {migrationMethod.Name} has a wrong parameter type. Parameter type should be {_jtokenType.Name}");
                }

                var attribute = migrationMethod.GetCustomAttribute<MigrationAttribute>();
                methodsWithMigrations.Add(migrationMethod, attribute);
            }

            while (true)
            {
                if (!methodsWithMigrations.Values.Any(m => m.FromVersion.Equals(version)))
                {
                    break; //break when there are no more migrations to execute
                }

                var methodWithMigration = methodsWithMigrations.FirstOrDefault(m => m.Value.FromVersion.Equals(version));

                try
                {
                    token = (JToken)type.InvokeMember(methodWithMigration.Key.Name, BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.NonPublic, null, null, new object[] { token });

                    version = methodWithMigration.Value.ToVersion;
                    token["JsonVersion"] = version;
                }
                catch (Exception e)
                {
                    throw new JsonMigrationException($"Error during execution of migration method {methodWithMigration.Key.Name}. See the inner exception for details", e);
                }
            }
        }
    }
}
