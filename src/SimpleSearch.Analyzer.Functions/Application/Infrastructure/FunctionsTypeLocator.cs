using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Azure.WebJobs;

namespace SimpleSearch.Analyzer.Functions.Application.Infrastructure
{
    public class FunctionsTypeLocator : ITypeLocator
    {
        private readonly Type[] _types;

        public FunctionsTypeLocator()
        {
            _types = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(x => x.IsClass && x.Namespace == "SimpleSearch.Analyzer.Functions.Functions")
                .ToArray();
        }

        public IReadOnlyList<Type> GetTypes() => _types;
    }
}