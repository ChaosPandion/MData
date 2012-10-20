open System
open System.IO
open System.Text.RegularExpressions

for file in Directory.EnumerateFiles("../", "AssemblyInfo.cs", SearchOption.AllDirectories) do
  File.WriteAllText(file, Regex.Replace(File.ReadAllText(file), "", fun m -> ""))