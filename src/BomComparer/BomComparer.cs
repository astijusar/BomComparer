using System.Reflection;
using BomComparer.Attributes;
using BomComparer.Enums;
using BomComparer.Models;

namespace BomComparer
{
    public class BomComparer
    {
        public BomComparisonResult Compare(BomFile sourceFile, BomFile targetFile)
        {
            var result = new BomComparisonResult();

            var allPartNumbers = sourceFile.Data
                .Union(targetFile.Data)
                .Select(x => x.PartNumber)
                .ToList();

            if (!allPartNumbers.Any()) return result;

            foreach (var partNumber in allPartNumbers)
            {
                var sourceEntry = sourceFile.Data.Find(d => d.PartNumber == partNumber);
                var targetEntry = targetFile.Data.Find(d => d.PartNumber == partNumber);

                var comparisonResult = CompareRows(sourceEntry, targetEntry);

                // TO-DO: Make it work if source or entry are null
                if (sourceEntry != null && targetEntry != null)
                {
                    comparisonResult.Designators = CompareDesignators(sourceEntry.Designators!, targetEntry.Designators!);
                }

                result.ResultEntries.Add(comparisonResult);
            }

            return result;
        }

        private BomComparisonResultEntry CompareRows(BomDataRow? source, BomDataRow? target)
        {
            var result = new BomComparisonResultEntry();
            var isRowModified = false;

            var properties = typeof(BomDataRow).GetProperties();

            foreach (var property in properties)
            {
                if (property.GetCustomAttribute<PrimaryKeyAttribute>() != null) continue;
                if (property.Name == "Designators") continue;

                var sourceValue = source != null ? property.GetValue(source) : null;
                var targetValue = target != null ? property.GetValue(target) : null;

                var comparisonResult = CompareObjects(sourceValue, targetValue);

                if (comparisonResult != ComparisonResult.Unchanged) 
                    isRowModified = true;

                var comparedValuesType = typeof(ComparedValues<>).MakeGenericType(property.PropertyType);
                var comparedValuesInstance = Activator.CreateInstance(comparedValuesType, sourceValue, targetValue, comparisonResult);

                var resultEntryProperty = typeof(BomComparisonResultEntry).GetProperty(property.Name);
                resultEntryProperty?.SetValue(result, comparedValuesInstance);
            }

            if (isRowModified)
            {
                result.Status = ComparisonResult.Modified;
                result.PartNumber = source!.PartNumber;
                return result;
            }

            if (source != null && target == null)
            {
                result.Status = ComparisonResult.Removed;
                result.PartNumber = source.PartNumber;
                return result;
            }

            if (source == null && target != null)
            {
                result.Status = ComparisonResult.Added;
                result.PartNumber = target.PartNumber;
                return result;
            }

            result.Status = ComparisonResult.Unchanged;
            result.PartNumber = source!.PartNumber;

            return result;
        }

        private List<DesignatorComparisonResultEntry> CompareDesignators(List<string> source, List<string> target)
        {
            var result = new List<DesignatorComparisonResultEntry>();

            foreach (var designator in target)
            {
                result.Add(source.Contains(designator)
                    ? new DesignatorComparisonResultEntry(DesignatorComparisonResult.Unchanged, designator)
                    : new DesignatorComparisonResultEntry(DesignatorComparisonResult.Added, designator));
            }

            foreach (var designator in source)
            {
                if (!target.Contains(designator))
                    result.Add(new DesignatorComparisonResultEntry(DesignatorComparisonResult.Removed, designator));
            }

            return result;
        }

        private ComparisonResult CompareObjects(object? source, object? target)
        {
            if (source == null && target != null) return ComparisonResult.Added;
            if (source != null && target == null) return ComparisonResult.Removed;
            if (source == null && target == null) return ComparisonResult.Unchanged;
            if (source!.Equals(target)) return ComparisonResult.Unchanged;
            return ComparisonResult.Modified;
        }
    }
}
