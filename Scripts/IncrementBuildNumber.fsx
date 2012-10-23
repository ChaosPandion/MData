open System
open System.IO
open System.Text.RegularExpressions

let path = if fsi.CommandLineArgs.Length > 0 then @"C:\Users\Matthew\Projects\MData" else fsi.CommandLineArgs.[1]
let x = List.ofSeq (Directory.EnumerateFiles(path, "AssemblyInfo.cs", SearchOption.AllDirectories))
if x.Length = 0 then printfn "no files: %s" path else
for filePath in x  do
    let text = File.ReadAllText(filePath)
    let evaluator (m:Match) = 
        printfn "match found: %s" m.Value
        let bn = ((m.Groups.[3].Value |> int) + 1) |> string
        m.Groups.[1].Value + "." + m.Groups.[2].Value + "." + bn + ".0"
    let text = Regex.Replace(text, @"(\d+)\.(\d+)\.(\d+).(\d+)", evaluator)
    File.WriteAllText(filePath, text)