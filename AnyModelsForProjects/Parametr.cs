using System;

namespace Respiratory_Analysis_CPET.AnyModelsForProjects
{
    public record Parameter
    {
        public Parameter(string name, ParameterValue parameterValue)
        {
            Name = name;
            ParameterValue = parameterValue;
        }

        public string Name { get; }
        public ParameterValue ParameterValue { get; }
    }
}
