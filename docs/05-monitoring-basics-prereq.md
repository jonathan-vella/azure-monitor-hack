﻿# Monitoring pre-requisites

If you don't have VM and container deployed or you deleted everything from before, you can use the following scripts to set them up
from scratch:

1. deploy VM with solution and SQL server on VM - [Deploy VM with solution](#deploy-vm-with-solution)
2. deploy container app with SQL, images and registry - [Container app deployment](#container-app-deployment)

You will need to clone the repo (**git clone https://github.com/vrhovnik/azure-monitor-automation-wth**) to access all of the artifacts. The easiest way is to use [Azure Shell](https://shell.azure.com).

When cloned, navigate to folder (**ROOT/scripts/IaC**). VM folder contains bicep files and scripts to deploy VM with
solution. Modernization folder contains bicep files and scripts to deploy container app with SQL, images and registry, container
app.

To install all of the tools on the machine (doing it from local machine), you can use the following script below step by step:

1. install az cli and login - [here](../scripts/PWSH/PreReqs/00-install.ps1)
2. install all of the tools and configure IIS via chocolatey - [here](../scripts/PWSH/PreReqs/00-install-tools.ps1)
3. install bicep, configure subscription (add providers, extensions,...), create access and store them to env variables - [here](../scripts/PWSH/PreReqs/01-az-and-bicep-configuration.ps1)
4. [OPTIONAL] fork and clone repo and store secrets to GitHub to have it available for the DevOps cycle - [here](../scripts/PWSH/PreReqs/02-set-gh-secrets.ps1)
5. repeat the procedure with cloning and navigating to folder IaC

## Deploy VM with solution

To deploy the VM, you can leverage the following [script](../scripts/IaC/VM/azure-creation.ps1):

1. Create or update the resource group
2. Create or update VM
3. Opens Remote Connection to connect into VM

It takes 6 parameters (or you can use default values where applicable):

1. **regionToDeploy** -- resource group location (most common is westeurope)
2. **rgName** -- name of the resource group (it will be created or updated)
3. **adminName** -- username name for VM to RDP into
4. **adminPass** -- password for user to RDP into
5. **workDir** - directory where you cloned your solution f.e. "C:/Work/Projects/azure-monitor-automation-wth",

```powershell

.\azure-creation.ps1 -regionToDeploy "westeurope" -rgName "rg-automation-wth" -workDir "C:/Work/Projects/azure-monitor-automation-wth" -adminName "admin" -adminPass "P@ssw0rd"

``` 

After running the solution, you should see the RDP dialog with (newly) created (or updated) IP connecting to the VM.

## Container app deployment

Container app can be deployed in the same resource group as the VM. For clean install, define new resource group as a
parameter.

To deploy container app this is the [script](../scripts/IaC/Modernization/azure-creation.ps1) which will do the
following:

1. Create or update the resource group
2. Create or update registry
3. Builds and deploy container images to registry
4. Create SQL, adds firewall rules and creates database
5. Imports existing data to SQL, prepares connection string to be used later
6. Create or update container app with latest image from registry

It takes 6 parameters (or you can use default values where applicable):

1. **regionToDeploy** -- resource group location (most common is westeurope)
2. **rgName** -- name of the resource group (it will be created or updated)
3. **workDir** - directory where you cloned your solution f.e. "C:/Work/Projects/azure-monitor-automation-wth",
4. **acrName** -- name of the container registry (it will be created or updated)
5. **containerappenv** -- name of the container environment
6. **containerapp** -- name of the container environment

_Usage_:

```powershell
.\azure-creation.ps1 -regionToDeploy "westeurope" -rgName "rg-automation-wth" -workDir "C:/Work/Projects/azure-monitor-automation-wth" -acrName "acrautomationwth" -containerappenv "containerappenv" -containerapp "containerapp"
```

# Additional information and links

1. [PowerShell](https://learn.microsoft.com/en-us/PowerShell/)
2. [Azure Container Registry](https://learn.microsoft.com/en-us/azure/container-registry/)
3. [Create VM with IIS](https://learn.microsoft.com/en-us/azure/virtual-machines/windows/quick-create-cli)
4. [Azure Container Apps](https://learn.microsoft.com/en-us/azure/container-apps/overview)