// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;

namespace Benchmarks.Configuration
{
    public class Scenarios
    {
        public Scenarios(IScenariosConfiguration scenariosConfiguration)
        {
            scenariosConfiguration.ConfigureScenarios(this);
        }

        [ScenarioPath("/plaintext")]
        public bool Plaintext { get; set; }

        [ScenarioPath("/responsecaching/plaintext/cached")]
        public bool ResponseCachingPlaintextCached { get; set; }

        [ScenarioPath("/responsecaching/plaintext/responsenocache")]
        public bool ResponseCachingPlaintextResponseNoCache { get; set; }

        [ScenarioPath("/responsecaching/plaintext/requestnocache")]
        public bool ResponseCachingPlaintextRequestNoCache { get; set; }

        [ScenarioPath("/responsecaching/plaintext/varybycached")]
        public bool ResponseCachingPlaintextVaryByCached { get; set; }

        [ScenarioPath("/memorycache/plaintext")]
        public bool MemoryCachePlaintext { get; set; }

        [ScenarioPath("/memorycache/plaintext/setremove")]
        public bool MemoryCachePlaintextSetRemove { get; set; }

        [ScenarioPath("/json")]
        public bool Json { get; set; }

        [ScenarioPath("/ep-plaintext")]
        public bool EndpointPlaintext { get; set; }

        [ScenarioPath("/mvc/plaintext")]
        public bool MvcPlaintext { get; set; }

        [ScenarioPath("/mvc/json")] // Hello World using System.Text.Json
        public bool MvcJson { get; set; }

        [ScenarioPath("/mvc/json2k")] // Uses System.Text.Json
        public bool MvcJson2k { get; set; }

        [ScenarioPath("/mvc/jsoninput")]
        public bool MvcJsonInput2k { get; set; }

        [ScenarioPath("/mvc/jsoninput")]
        public bool MvcJsonInput2M { get; set; }

        [ScenarioPath("/mvc/json2M")] // Uses System.Text.Json
        public bool MvcJsonOutput2M { get; set; }

        [ScenarioPath("/plaintext", "/128B.txt", "/512B.txt", "/1KB.txt", "/4KB.txt", "/16KB.txt", "/512KB.txt", "/1MB.txt", "/5MB.txt")]
        public bool StaticFiles { get; set; }

        //**
        // Scenarios below this point are not currently automated. They are available for use when starting up the
        // Benchmarks application manually.
        //**

        [ScenarioPath("/copytoasync")]
        public bool CopyToAsync { get; set; }

        [ScenarioPath("/db/raw")]
        public bool DbSingleQueryRaw { get; set; }

        [ScenarioPath("/db/ef")]
        public bool DbSingleQueryEf { get; set; }

        [ScenarioPath("/db/dapper")]
        public bool DbSingleQueryDapper { get; set; }

        [ScenarioPath("/db/mongodb")]
        public bool DbSingleQueryMongoDb { get; set; }

        [ScenarioPath("/queries/raw")]
        public bool DbMultiQueryRaw { get; set; }

        [ScenarioPath("/queries/ef")]
        public bool DbMultiQueryEf { get; set; }

        [ScenarioPath("/queries/dapper")]
        public bool DbMultiQueryDapper { get; set; }

        [ScenarioPath("/queries/mongodb")]
        public bool DbMultiQueryMongoDb { get; set; }

        [ScenarioPath("/updates/raw")]
        public bool DbMultiUpdateRaw { get; set; }

        [ScenarioPath("/updates/ef")]
        public bool DbMultiUpdateEf { get; set; }

        [ScenarioPath("/updates/dapper")]
        public bool DbMultiUpdateDapper { get; set; }

        [ScenarioPath("/fortunes/raw")]
        public bool DbFortunesRaw { get; set; }

        [ScenarioPath("/fortunes/raw-sync")]
        public bool DbFortunesRawSync { get; set; }

        [ScenarioPath("/fortunes/ef")]
        public bool DbFortunesEf { get; set; }

        [ScenarioPath("/fortunes/dapper")]
        public bool DbFortunesDapper { get; set; }

        [ScenarioPath("/fortunes/mongodb")]
        public bool DbFortunesMongoDb { get; set; }

        [ScenarioPath("/mvc/view")]
        public bool MvcViews { get; set; }

        [ScenarioPath("/mvc/db/raw")]
        public bool MvcDbSingleQueryRaw { get; set; }

        [ScenarioPath("/mvc/db/dapper")]
        public bool MvcDbSingleQueryDapper { get; set; }

        [ScenarioPath("/mvc/db/ef")]
        public bool MvcDbSingleQueryEf { get; set; }

        [ScenarioPath("/mvc/queries/raw")]
        public bool MvcDbMultiQueryRaw { get; set; }

        [ScenarioPath("/mvc/queries/dapper")]
        public bool MvcDbMultiQueryDapper { get; set; }

        [ScenarioPath("/mvc/queries/ef")]
        public bool MvcDbMultiQueryEf { get; set; }

        [ScenarioPath("/mvc/updates/raw")]
        public bool MvcDbMultiUpdateRaw { get; set; }

        [ScenarioPath("/mvc/updates/dapper")]
        public bool MvcDbMultiUpdateDapper { get; set; }

        [ScenarioPath("/mvc/updates/ef")]
        public bool MvcDbMultiUpdateEf { get; set; }

        [ScenarioPath("/mvc/fortunes/raw")]
        public bool MvcDbFortunesRaw { get; set; }

        [ScenarioPath("/mvc/fortunes/ef")]
        public bool MvcDbFortunesEf { get; set; }

        [ScenarioPath("/mvc/fortunes/dapper")]
        public bool MvcDbFortunesDapper { get; set; }

        public bool Any(string partialName) =>
            typeof(Scenarios).GetTypeInfo().DeclaredProperties
                .Where(p => p.Name.IndexOf(partialName, StringComparison.Ordinal) >= 0 && (bool)p.GetValue(this))
                .Any();

        public IEnumerable<EnabledScenario> GetEnabled() =>
            typeof(Scenarios).GetTypeInfo().DeclaredProperties
                .Where(p => p.GetValue(this) is bool && (bool)p.GetValue(this))
                .Select(p => new EnabledScenario(p.Name, p.GetCustomAttribute<ScenarioPathAttribute>()?.Paths));

        public static IEnumerable<string> GetNames() =>
            typeof(Scenarios).GetTypeInfo().DeclaredProperties
                .Select(p => p.Name);

        public static string[] GetPaths(Expression<Func<Scenarios, bool>> scenarioExpression) =>
            scenarioExpression.GetPropertyAccess().GetCustomAttribute<ScenarioPathAttribute>().Paths;

        public static string GetPath(Expression<Func<Scenarios, bool>> scenarioExpression) =>
            GetPaths(scenarioExpression)[0];

        public int Enable(string name)
        {
            if (string.Equals(name, "[default]", StringComparison.OrdinalIgnoreCase))
            {
                EnableDefault();
                return 2;
            }

             var props = typeof(Scenarios).GetTypeInfo().DeclaredProperties
                .Where(p => string.Equals(name, "[all]", StringComparison.OrdinalIgnoreCase) || p.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var p in props)
            {
                p.SetValue(this, true);
            }

            return props.Count;
        }

        public void EnableDefault()
        {
            Plaintext = true;
            Json = true;
        }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class ScenarioPathAttribute : Attribute
    {
        public ScenarioPathAttribute(params string[] paths)
        {
            Paths = paths;
        }

        public string[] Paths { get; }
    }
}
