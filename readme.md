# Project Title

Sitecore.SampleCortexJob

## Description

Sample project that shows an example of Sitecore Cortex Worker Job.
The job exports some interactions data to Azure Table Storage

## Prerequisites

Sitecore 9.2.0 XP


## Installation

1. Build Solution
1. From ```ExportXConnectDataToTableStorage.Cortex``` project Copy DLLs and config file ```sc.ExportXConnectDataToTableStorage.xml``` to the folder ```\App_Data\jobs\continuous\ProcessingEngine\App_Data\Config\Sitecore\ExportXConnectDataToTableStorage``` in your Cortex Processing Engine root
1. From ```ExportXConnectDataToTableStorage.Sitecore``` project copy dlls, config file to the Sitecore CM instance

## How to run
1. Make sure you have some data in xConnect (use for example [xGenerator](https://github.com/Sitecore/xGenerator))
1. Perform Web API request (for example using [Postman](https://www.getpostman.com/))

```
http://<hostname>/api/sitecore/Cortex/RegisterExportToAzTableStorage
```
As a response you will get TaskId:

```
{
    "TaskId": "28eea604-99e9-4274-a389-47b0aa13f868"
}
```

You can use this TaskId in order to get status of that Cortex task. Use following call:

```
http://<hostname>/api/sitecore/Cortex/GetTaskStatus?taskId=28eea604-99e9-4274-a389-47b0aa13f868
```
