# Script to update namespaces from HelloApi to MessageApi

# Update model files
Get-ChildItem -Path ".\Models\*.cs" -Recurse | ForEach-Object {
    $content = Get-Content $_.FullName -Raw
    $content = $content -replace 'namespace HelloApi\.Models', 'namespace MessageApi.Models'
    Set-Content -Path $_.FullName -Value $content
}

# Update DTOs
Get-ChildItem -Path ".\Models\DTOs\*.cs" -Recurse | ForEach-Object {
    $content = Get-Content $_.FullName -Raw
    $content = $content -replace 'namespace HelloApi\.Models\.DTOs', 'namespace MessageApi.Models.DTOs'
    $content = $content -replace 'using HelloApi\.Models', 'using MessageApi.Models'
    Set-Content -Path $_.FullName -Value $content
}

# Update repositories
Get-ChildItem -Path ".\Repositories\*.cs" -Recurse | ForEach-Object {
    $content = Get-Content $_.FullName -Raw
    $content = $content -replace 'namespace HelloApi\.Repositories', 'namespace MessageApi.Repositories'
    $content = $content -replace 'using HelloApi\.Models', 'using MessageApi.Models'
    $content = $content -replace 'using HelloApi\.Data', 'using MessageApi.Data'
    Set-Content -Path $_.FullName -Value $content
}

# Update services
Get-ChildItem -Path ".\Services\*.cs" -Recurse | ForEach-Object {
    $content = Get-Content $_.FullName -Raw
    $content = $content -replace 'namespace HelloApi\.Services', 'namespace MessageApi.Services'
    $content = $content -replace 'using HelloApi\.Models', 'using MessageApi.Models'
    $content = $content -replace 'using HelloApi\.Repositories', 'using MessageApi.Repositories'
    $content = $content -replace 'using HelloApi\.Data', 'using MessageApi.Data'
    Set-Content -Path $_.FullName -Value $content
}

# Update controllers
Get-ChildItem -Path ".\Controllers\*.cs" -Recurse | ForEach-Object {
    $content = Get-Content $_.FullName -Raw
    $content = $content -replace 'using HelloApi\.Models', 'using MessageApi.Models'
    $content = $content -replace 'using HelloApi\.Services', 'using MessageApi.Services'
    Set-Content -Path $_.FullName -Value $content
}

Write-Host "Namespaces have been updated successfully!"
