using System.Reflection;
using BomComparer.Attributes;
using BomComparer.Enums;
using BomComparer.Models;

namespace BomComparer.Comparer
{
    public class BomCompare
    {
        public BomComparisonResult Compare(BomFile sourceFile, BomFile targetFile)
        {
            var result = new BomComparisonResult
            {
                SourceFileName = sourceFile.Name,
                TargetFileName = targetFile.Name
            };

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

            result.Designators = CompareDesignators(source?.Designators, target?.Designators);
            isRowModified = result.Designators
                .Any(d => d.Status is DesignatorComparisonResult.Removed or DesignatorComparisonResult.Added);

            (result.Status, result.PartNumber) = (source, target) switch
            {
                (not null, null) => (ComparisonResult.Removed, source!.PartNumber),
                (null, not null) => (ComparisonResult.Added, target.PartNumber),
                _ => (isRowModified ? ComparisonResult.Modified : ComparisonResult.Unchanged, source!.PartNumber)
            };

            return result;
        }

        private List<DesignatorComparisonResultEntry> CompareDesignators(List<string>? source, List<string>? target)
        {
            if (source == null && target == null)
                return new List<DesignatorComparisonResultEntry>();

            if (source == null)
                return target!.Select(designator =>
                    new DesignatorComparisonResultEntry(DesignatorComparisonResult.Added, designator))
                    .ToList();

            if (target == null)
                return source!.Select(designator =>
                    new DesignatorComparisonResultEntry(DesignatorComparisonResult.Removed, designator))
                    .ToList();

            var sourceSet = new HashSet<string>(source);
            var sourceIndex = source.Select((value, index) => new { value, index })
                .ToDictionary(x => x.value, x => x.index);

            var result = new List<DesignatorComparisonResultEntry>();

            var targetIndex = 0;
            var sourceIndexForAddition = 0;

            while (targetIndex < target.Count)
            {
                var targetItem = target[targetIndex];

                if (sourceSet.Contains(targetItem))
                {
                    var sourceItemIndex = sourceIndex[targetItem];

                    for (var i = sourceIndexForAddition; i < sourceItemIndex; i++)
                        result.Add(new DesignatorComparisonResultEntry(DesignatorComparisonResult.Removed, source[i]));

                    result.Add(new DesignatorComparisonResultEntry(DesignatorComparisonResult.Unchanged, targetItem));
                    sourceIndexForAddition = sourceItemIndex + 1;
                    targetIndex++;
                }
                else
                {
                    result.Add(new DesignatorComparisonResultEntry(DesignatorComparisonResult.Added, targetItem));
                    targetIndex++;
                }
            }

            for (var i = sourceIndexForAddition; i < source.Count; i++)
                result.Add(new DesignatorComparisonResultEntry(DesignatorComparisonResult.Removed, source[i]));

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
