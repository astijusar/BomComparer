using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BomComparer.Enums;
using BomComparer.ExcelReaders;
using BomComparer.Models;
using FluentAssertions;
using Moq;

namespace BomComparer.Tests
{
    public class BomComparerTests
    {
        private readonly BomComparer _comparer = new();
        private readonly NpoiReader _reader = new();

        [Fact]
        public void Compare_EmptyData_ShouldReturnEmptyResult()
        {
            var data = new BomFile();

            var result = _comparer.Compare(data, data);

            result.Should().BeOfType<BomComparisonResult>();
            result.ResultEntries.Should().HaveCount(0);
        }

        [Fact]
        public void Compare_ValidData_ShouldReturnValidResults()
        {
            var sourcePath = "TestData/source.xlsx";
            var targetPath = "TestData/target.xlsx";

            var sourceData = _reader.ReadData(sourcePath);
            var targetData = _reader.ReadData(targetPath);

            var result = _comparer.Compare(sourceData, targetData);

            result.Should().BeOfType<BomComparisonResult>();

            result.ResultEntries.ElementAt(0).Status.Should().Be(ComparisonResult.Unchanged);
            result.ResultEntries.ElementAt(0).Smd!.Status.Should().Be(ComparisonResult.Unchanged);
            result.ResultEntries.ElementAt(0).Designators!.First().Status.Should()
                .Be(DesignatorComparisonResult.Unchanged);

            result.ResultEntries.ElementAt(1).Status.Should().Be(ComparisonResult.Removed);
            result.ResultEntries.ElementAt(1).Smd!.Status.Should().Be(ComparisonResult.Removed);
            result.ResultEntries.ElementAt(1).Designators!.First().Status.Should()
                .Be(DesignatorComparisonResult.Removed);

            result.ResultEntries.ElementAt(2).Status.Should().Be(ComparisonResult.Modified);
            result.ResultEntries.ElementAt(2).Smd!.SourceValue.Should().Be("No");
            result.ResultEntries.ElementAt(2).Smd!.TargetValue.Should().Be("Yes");
            result.ResultEntries.ElementAt(2).Smd!.Status.Should().Be(ComparisonResult.Modified);
            result.ResultEntries.ElementAt(2).Designators!.ElementAt(1).Status.Should()
                .Be(DesignatorComparisonResult.Added);

            result.ResultEntries.ElementAt(3).Status.Should().Be(ComparisonResult.Added);
            result.ResultEntries.ElementAt(3).Smd!.Status.Should().Be(ComparisonResult.Added);
            result.ResultEntries.ElementAt(3).Designators!.First().Status.Should()
                .Be(DesignatorComparisonResult.Added);
        }
    }
}
