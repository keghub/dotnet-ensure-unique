# dotnet-ensure-unique
A dotnet global tool that ensures an application is executed only once at the same by leveraging AWS S3 as sync medium.

## Objectives
The main objective is to use this tool in a Docker container so that the same application can't be executed more than once at the same time.

The tool should be able to execute both .NET Core executables and normal executables.

### .NET Core executables

A .NET Core executable is generally executed using the `dotnet` utility.

```bash
$ dotnet ./my-program.dll
```

The same program should be able to be executed with this tool as follows
```bash
$ dotnet ensure-unique dotnet ./my-program.dll [options]
```

### Normal executables

The tool is also able to execute normal executables

```bash
$ dotnet ensure-unique exe ./my-classic-program.exe [options]
```

### Options

The following options will be supported

|Option|Short version|Required|Description|
|-|-|-|-|
|`--bucket`|||The name of the bucket to place the lock file|
|`--prefix`|||The prefix to be prepended to the key of the lock file|
|`--token`|||The token to be used to ensure uniqueness.<br />If not specified, an hash of the executable is used|
|`--verbosity`|`-v`||The verbosity of the logging for the tool|
|`--args`|||Arguments to be forwarded to the program to execute|

## Environment variables

Certain options can be specified also via environment variables.

|Name|Description|
|-|-|
|`DotNetEnsureUnique__S3__BucketName`|The name of the bucket to place the lock file|
|`DotNetEnsureUnique__S3__FilePrefix`|The prefix to be prepended to the key of the lock file|

Command line arguments will override environment variables values.

Please notice that either one between the environment variable or the command line option for the AWS S3 bucket name must be specified.
