# WhyDeploy

[![Dev Pipeline](https://github.com/yorschor/WhyDeploy/actions/workflows/dev-pipeline.yml/badge.svg?branch=trunk&event=push)](https://github.com/yorschor/WhyDeploy/actions/workflows/dev-pipeline.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

## 📚About 
WhyDeploy is a simple deployment library meant for small repeatable local tasks. 
The core lib exposes an API for executing "operations" in order to eg. delete/copy files or folders, replacing strings within files, executing git commands etc.
The accompanying CLI tool allows for a simple creation and usage of the core API. A desktop client will follow in the future. 

## ❗ Important
> **WhyDeploy is still in development!!**
> * While the CLI is usable today future updates might brake existing configs
> * The Desktop app, while planned, is still in its conception phase and no estimation can be made when the first working prototype might arrive :) 

## 🔮 Planned features
- [ ] Usage Tutorial 
- [ ] No exception in the CLI -> Only nice feedback
- [ ] Desktop app 🙃
- [ ] Whatever comes along

## 🚀 Getting started 
> TODO

## ✍️ Examples 
> TODO

## 📐 Architecture
Why Deploy is split into 4 (5 if you´re exact) different layers. 
- The **Abstraction** Layer contains common data models as well as the base classes and interfaces that are needed by multiple other layers.
- The **Utility** Layer exposes Methods that do not concern the deploy logic and can safely be implemented into other C# apps via a nuget package.
itself. Example for this is the FileSystemHelper.cs.
- The **Core** layer contains the main business logic for deploying operations (As well as containing the operations themselves), Creating new Operations handling of common erros and so on. 
- The **Application** layer is the actual app the user is interacting with. The CLI and DesktopClient in this case. These apps only consume the CoreLibs and don´t implemnt 
any additional logic that isn't part of its specific domain. 

The *fifth* layer consists of the "OperationModules". Those are classes that contain additional modules that can be restricted to a specific operation system 
or extending the basic functionality that is provide out of the box. Those modules live in the core layer but can be added further down the line. All modules located in a "/module"
directory directly outside the app directory are loaded dynamically in application start.

> On a less conceptual level the project can be sliced into applications, core libs and modules (see below). I created the layer model as a guidelines of how to handle different things within the project.   
![A high level architecture overview of WhyDeploy showing the basic relations between the internal projects ](/docs/images/WDArchitectureOverview.png "WhyDeploy architecture overview")


###  ⚠️ Error Handling
Errors are Handled differently depending on the layer. 
- Within the **Utility** layer exception should be thrown if something has gone wrong. No Console output/logging should be happening at this stage.


- Within the **Core** layer Exceptions are handled and "converted" into Result<T> types.
  - No exception should be thrown at this stage. Every publicly facing method should return a result instead of an direct value. 
  - Progress should be logged using "Nlog". Everything that is communicated further up to the apps should also be handled by a accompanying structured log message.
  

- Within the **Application** layer errors are presented to the user in a app specific way. 
  - The **WhyDeployCLI** consumes results and prints them using the Spectre console lib. 
  - The **WhyDeploy Desktop Client** presents results in a visual way.