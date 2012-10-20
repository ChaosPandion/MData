open System
open System.IO
open System.Text.RegularExpressions

for filePath in Directory.EnumerateFiles("../", "AssemblyInfo.cs", SearchOption.AllDirectories) do
    let text = File.ReadAllText(filePath)
    let evaluator (m:Match) = 
        let bn = m.Groups.[3].Value |> int |> string
        m.Groups.[1].Value + "." + m.Groups.[2].Value + "." + bn
    let text = Regex.Replace(text, @"(\d+)\.(\d+)\.(\d+)", evaluator)
    File.WriteAllText(filePath, text)