﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLI
{
    public static class Utilities
    {
        public static string ConstructFilePath(string outputPath, string sourcePath, string targetPath)
        {
            var sourceName = Path.GetFileName(sourcePath);
            var targetName = Path.GetFileName(targetPath);

            var basePath = string.IsNullOrWhiteSpace(outputPath) ? Directory.GetCurrentDirectory() : outputPath;
            var fileName = $"{sourceName}_vs_{targetName}.xlsx";

            return Path.Combine(basePath, fileName);
        }
    }
}
