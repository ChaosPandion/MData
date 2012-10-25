get-item -Path:@("../*/Properties/AssemblyInfo.cs") | foreach {
    #echo $_
    #$text = select-string $_ -pattern "(\d+)\.(\d+)\.(\d+)\.(\d+)"
    $text = [IO.File]::ReadAllText($_)
    $evaluator = { param($m)  
                    $m.Groups[1].Value + "." + 
                    $m.Groups[2].Value + "." + 
                    $m.Groups[3].Value + "." +  
                    (([int]$m.Groups[4].Value) + 1)}
    $text = [regex]::Replace($text, '(\d+)\.(\d+)\.(\d+)\.(\d+)', $evaluator); 
    [IO.File]::WriteAllText($_, $text)
}