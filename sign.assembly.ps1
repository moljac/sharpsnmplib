if ($env:CI -eq "true") {
    exit 0
}

$cert = Get-ChildItem -Path Cert:\CurrentUser\My -CodeSigningCert | Select-Object -First 1
if ($null -eq $cert) {
    Write-Host "No code signing certificate found in MY store. Exit."
    exit 0
}

$signtool = Get-ChildItem -Path "${env:ProgramFiles(x86)}\Windows Kits", "${env:ProgramFiles(x86)}\Microsoft SDKs" -Recurse -Filter "signtool.exe" | Select-Object -First 1 -ExpandProperty FullName
Write-host "Signtool path: $signtool"
if (Test-Path $signtool) {
    Write-Output "sign the assembly"
    & $signtool sign /tr http://timestamp.digicert.com /td sha256 /fd sha256 /a $args[0]
    if ($LASTEXITCODE -ne 0)
    {
        Write-Host 'Assembly $args[0] is not signed. Exit.'
    }
}

exit 0
