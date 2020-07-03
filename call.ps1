$baseUrl = "http://agi920a.sc/api/sitecore"
$response = Invoke-RestMethod -Uri "$baseUrl/Cortex/RegisterContactsExportToMsSql"

$taskId = $response.TaskId

Write-Host "Task has been created: TaskId: $taskId"

# Get Task Status

$Form = @{
    TaskId  = $taskId
}

$responseTaskStatus = Invoke-RestMethod -Uri "$baseUrl/Cortex/GetTaskStatus" -Method Post -Form $Form

Write-Host $responseTaskStatus | Format-Table -AutoSize