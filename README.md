# Build2020WvdMigrationDemo
Code for the demos shown in the Microsoft Build 2020 Skilling Session 127.

There are two solutions in this repo under the `src` directory:

1. The `WvdMigration` directory holds the `WvdMigration` WPF project used in the demo, along with the `MsixPackage` project. Neither of them has anything particularly novel.
1. The `StageAppxPackage` directory holds the `StageAppxPackage` tool which is an incredibly simply wrapper around [`PackageManager.StagePackageAsync`](https://docs.microsoft.com/en-us/uwp/api/windows.management.deployment.packagemanager.stagepackageasync?view=winrt-18362#Windows_Management_Deployment_PackageManager_StagePackageAsync_Windows_Foundation_Uri_Windows_Foundation_Collections_IIterable_Windows_Foundation_Uri__Windows_Management_Deployment_DeploymentOptions_) -- ideally there will be a built-in PowerShell cmdlet soon that pairs with `Add-AppxPackage`.

The most interesting part of the repo is [`azure-pipelines.yml`](./azure-pipelines.yml) which was developed primarily by [@Huios](https://github.com/Huios). It contains the commands to build the WPF app, package it as an MSIX, sign it correctly, create a VHD file, and expand the MSIX package into the VHD ready for MSIX AppAttach. 
