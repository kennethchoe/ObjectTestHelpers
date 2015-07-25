using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ObjectTestHelpers
{
    public class ObjectComparer
    {
        public void AssertEquality(object obj1, object obj2, string[] propertiesToIgnore)
        {
            Compare(obj1, obj2, propertiesToIgnore, (x, y) => x != y, "Changed");
        }

        public void AssertDifference(object obj1, object obj2, string[] propertiesToIgnore)
        {
            Compare(obj1, obj2, propertiesToIgnore, (x, y) => x == y, "Unchanged");
        }

        private void Compare(object obj1, object obj2, string[] propertiesToIgnore, Func<string, string, bool> hasProblem, string problemDescription)
        {
            propertiesToIgnore = propertiesToIgnore ?? new string[] { };
            var propertiesIgnored = new List<string>();

            var problemCount = 0;

            var serializer = new ObjectSerializer();

            var obj1Content = serializer.SerializeForComparison(obj1);
            var obj2Content = serializer.SerializeForComparison(obj2);

            var linesToCompare = obj1Content.Count();
            if (linesToCompare > obj2Content.Count())
            {
                linesToCompare = obj2Content.Count();
                Debug.WriteLine("PROBLEM: Total lines different. Object 1: {0}, Object 2: {1}", obj1Content.Count(), obj2Content.Count());
                problemCount++;
            }

            for (int i = 0; i < linesToCompare; i++)
            {
                if (hasProblem(obj1Content[i], obj2Content[i]))
                {
                    var matches = propertiesToIgnore.Where(x => obj1Content[i].Contains(x)).ToList();
                    if (matches.Any())
                        propertiesIgnored.AddRange(matches);
                    else
                    {
                        Debug.WriteLine("PROBLEM: " + problemDescription + " property detected: " + obj1Content[i]);
                        problemCount++;
                    }
                }
            }

            if (problemCount > 0)
            {
                Debug.WriteLine("\nObject 1: ");
                obj1Content.ForEach(x => Debug.WriteLine(x));
                Debug.WriteLine("\nObject 2: ");
                obj2Content.ForEach(x => Debug.WriteLine(x));

                throw new AssertDifferenceException(problemCount + " issue(s) found. See output.");
            }

            var propertiesToIgnoreUnused = propertiesToIgnore.Where(x => !propertiesIgnored.Contains(x)).ToList();
            if (propertiesToIgnoreUnused.Any())
            {
                Debug.WriteLine("PropertiesToIgnore contains unused items.");
                propertiesToIgnoreUnused.ForEach(x => Debug.WriteLine(x));
                throw new AssertDifferenceException("PropertiesToIgnore contains unused items. See output.");
            }
        }
    }
}
