using BomComparer.Enums;
using BomComparer.Models;

namespace BomComparer
{
    public class BomComparer
    {
        public BomComparisonResult Compare(BomFile sourceFile, BomFile targetFile)
        {
            var result = new BomComparisonResult();

            foreach (var sourceEntry in sourceFile.Data)
            {
                var targetEntry = targetFile.Data.Find(d => d.PartNumber == sourceEntry.PartNumber);

                result.ResultEntries.Add(CompareRows(sourceEntry, targetEntry));
            }

            // To find all Added
            foreach (var targetEntry in targetFile.Data)
            {
                var sourceEntry = sourceFile.Data.Find(d => d.PartNumber == targetEntry.PartNumber);

                if (sourceEntry == null)
                {
                    result.ResultEntries.Add(CompareRows(sourceEntry, targetEntry));
                }
            }

            return result;
        }

        private BomComparisonResultEntry CompareRows(BomDataRow? source, BomDataRow? target)
        {
            var result = new BomComparisonResultEntry();
            var isModified = false;

            var properties = typeof(BomDataRow).GetProperties();

            foreach (var property in properties)
            {
                if (property.Name is "PartNumber" or "Designators") continue;

                var sourceValue = source != null ? property.GetValue(source) : null;
                var targetValue = target != null ? property.GetValue(target) : null;

                if (!Equals(sourceValue, targetValue)) isModified = true;

                var comparedValuesType = typeof(ComparedValues<>).MakeGenericType(property.PropertyType);
                var comparedValuesInstance = Activator.CreateInstance(comparedValuesType, sourceValue, targetValue);

                var resultEntryProperty = typeof(BomComparisonResultEntry).GetProperty(property.Name);
                resultEntryProperty?.SetValue(result, comparedValuesInstance);
            }

            if (source != null && target == null)
            {
                result.Status = ComparisonResultStatus.Removed;
                result.PartNumber = source.PartNumber;
            }
            else if (source == null && target != null)
            {
                result.Status = ComparisonResultStatus.Added;
                result.PartNumber = target.PartNumber;
            }
            else if (isModified)
            {
                result.Status = ComparisonResultStatus.Modified;
                result.PartNumber = source!.PartNumber;
            }
            else
            {
                result.Status = ComparisonResultStatus.Unchanged;
                result.PartNumber = source!.PartNumber;
            }

            return result;
        }
    }
}
