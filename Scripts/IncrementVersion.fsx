open System
open System.IO
open System.Text.RegularExpressions

module Version =
    let fw = new System.IO.FileSystemWatcher("")
    let increment (index:int) =
            let x = List.ofSeq (Directory.EnumerateFiles(fsi.CommandLineArgs.[1], "AssemblyInfo.cs", SearchOption.AllDirectories))
            if x.Length = 0 then printfn "no files: %s" fsi.CommandLineArgs.[1] else
            for filePath in x  do
                let text = File.ReadAllText(filePath)
                let evaluator (m:Match) = 
                    printfn "match found: %s" m.Value
                    let bn = ((m.Groups.[index].Value |> int) + 1) |> string
                    m.Groups.[1].Value + "." + m.Groups.[2].Value + "." + m.Groups.[3].Value + "." + bn
                let text = Regex.Replace(text, @"(\d+)\.(\d+)\.(\d+).(\d+)", evaluator)
                File.WriteAllText(filePath, text)
    let incrementRevision () = increment 4