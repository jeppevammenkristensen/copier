$filePath = Join-Path -Path $PSScriptRoot -ChildPath "Copier\appsettings.custom.json"

if (-not (Test-Path $filePath)) {
    $content = @'
{
    "Combos": {
        "FileCopy": []
    }
}
'@
    $content | Out-File -FilePath $filePath -Encoding UTF8
}