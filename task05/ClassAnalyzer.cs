using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace task05
{
    public class ClassAnalyzer
    {
        private Type _type;

        public ClassAnalyzer(Type type)
        {
            _type = type ?? throw new ArgumentNullException(nameof(type));
        }
        public IEnumerable<string> GetPublicMethods()
        {
            if (_type == null)
                return Enumerable.Empty<string>();

            return _type.GetMethods().Select(a => a.Name);
        }

        public IEnumerable<string> GetMethodParams(string methodname)
        {
            var method = _type.GetMethod(methodname);
            if (method == null)
            {
                return Enumerable.Empty<string>();
            }
            var paramList = method.GetParameters().Select(p => p.ParameterType + " " + p.Name);
            string allParams = string.Join(", ", paramList);
            string result = method.ReturnType.Name + " " + methodname + "(" + allParams + ")";
            return new List<string> { result };
        }

        public IEnumerable<string> GetAllFields()
        {
            return _type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).Select(b => b.Name);
        }

        public IEnumerable<string> GetProperties()
        {
            return _type.GetProperties().Select(p => p.Name);
        }

        public bool HasAttribute<T>() where T : Attribute
        {
            if (_type == null)
                return false;

            return _type.IsDefined(typeof(T), true);
        }
    }
}
