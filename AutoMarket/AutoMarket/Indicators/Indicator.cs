using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoMarket.Indicators
{
    class Indicator
    {

        public List<String> FindDerivedClasses()
        {
            List<String> listofDerivedClasses = new List<string>();

            var listOfBs = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                            from assemblyType in domainAssembly.GetTypes()
                            where typeof(Indicator).IsAssignableFrom(assemblyType)
                            select assemblyType).ToArray();

            for (int i = 0; i < listOfBs.Count(); i++)
            {
                if (listOfBs[i].Name != "Indicator")
                    listofDerivedClasses.Add(listOfBs[i].Name);
            }

            return listofDerivedClasses;
        }

        public Type DerivedClass(String clsDerived)
        {
            var listOfBs = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                            from assemblyType in domainAssembly.GetTypes()
                            where typeof(Indicator).IsAssignableFrom(assemblyType)
                            select assemblyType).ToArray();

            Type derivedClass = null;
            for (int i = 0; i < listOfBs.Count(); i++)
            {
                if (clsDerived == listOfBs[i].Name)
                    derivedClass = listOfBs[i];
            }

            return derivedClass;

        }

    }
}
