﻿using System.IO;
using System.Linq;
using System.Reflection;
using Cake.Core.Scripting.CodeGen;
using Cake.Core.Tests.Data;

namespace Cake.Core.Tests.Fixtures
{
    public sealed class PropertyAliasGeneratorFixture
    {
        private readonly Assembly _assembly;
        private readonly MethodInfo[] _methods;

        public PropertyAliasGeneratorFixture()
        {
            _assembly = typeof (PropertyAliasGeneratorFixture).Assembly;
            _methods = typeof (PropertyAliasGeneratorData).GetMethods();
        }

        public string GetExpectedData(string name)
        {
            var resource = string.Concat("Cake.Core.Tests.Unit.Scripting.CodeGen.Expected.Properties.", name);
            using (var stream = _assembly.GetManifestResourceStream(resource))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd().NormalizeGeneratedCode();
            }
        }

        public string Generate(string name)
        {
            var method = _methods.SingleOrDefault(x => x.Name == name);
            return PropertyAliasGenerator.Generate(method).NormalizeGeneratedCode();
        }
    }
}
