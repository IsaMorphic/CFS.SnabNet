# CFS.SnabNet

*SnabNet*, also known as *SNAB.NET* is a barebones, no nonsense implementation of the [SNAB generalized bitstream format](https://github.com/AlubJ/SNAB) for efficient, low-overhead, high-performance serialization with support for multiple programming languages and cross-platform scenarios. Importantly, this implementation supports all official versions of the original specification and includes goodies for parsing and serializing arbitrary data in a flexible fashion. 

# Features

Key features of SNAB that are fully functional in SnabNet are listed as follows:

- Parsing / serializing little AND big endian bitstreams
- SNAB compression (via zlib deflate)
- Bitstream validation based on crc32 checksum and length fields
- Registering custom datatypes for extensible parsing / serialization

Other key library features that are implementation-specific include the following:

* Source generation for serialization of custom C# types
* Unit tests for specification coverage and overall correctness
* Support for `ExpandoObject` for both parsing and serialization

# How to Use

Example code for both parsing and serializing custom data types is written in the sections below.

## `SnabInstance` (Type Registration)

```csharp
using CFS.SnabNet;

/* Somewhere in your application code */
public class CustomType : ISnabType 
{
    public const byte TypeId = 0x80;
    
    public HashSet<byte> TypeIds { get; }
    
    public CustomType() 
    {
        TypeIds = new() { TypeId }
    }
    
    public object? ReadFromInstance(SnabReader instance, byte typeId) 
    {
        /* Your implementation goes here */
    }
    
    public void WriteToInstance(SnabWriter instance, byte typeId, object? obj) 
    {
        /* Your implementation goes here */
    }
}

/* Later, where you want to use it */
SnabInstance instance = new();
instance.RegisterType<CustomType>();

/* Continue with next examples for reading and writing */
```

## `SnabReader` (Deserialization)

```csharp
using CFS.SnabNet;

SnabInstance instance = new();
/* Register custom types here */

dynamic parsedData;
using (SnabReader reader = instance.CreateReader(/* Stream with your data */)) 
{
    /* Grab data as ExpandoObject */
    parsedData = reader.Deserialize();
}

/* Grab data from arbitrary fields based on what you expect */
int intData = parsedData.int_field;
string strData = parsedData.string_field;
```

## `SnabWriter` (Serialization)

```csharp
using System.Dynamic; /* for ExpandoObject */
using CFS.SnabNet;

/* Input data can be ExpandoObject or specific class instance (see next example) */
dynamic dataObject = new ExpandoObject();
dataObject.int_field = 42;
dataObject.real_field = 3.1415;

SnabInstance instance = new();
/* Register custom types here */

using (SnabWriter writer = instance.CreateWriter(/* Stream to write data to */, SnabFlags.None)) 
{
    writer.Serialize(dataObject);
}
```

## `ISnabStruct` (Source Generation)

```csharp
using CFS.SnabNet;
using CFS.SnabNet.SourceGeneration;

/* Mark this type for source generation */
[SnabStruct]
public class TestStruct 
{
    /* Declare property for serialization 
    (parameters optional, but recommended) */
    [SnabField("int_field", SnabType.Integer)]
    public int IntField { get; set; }
    
    /* ... */
}

/* Later, to use... */
SnabInstance instance = new();

/* For serialization */
using(SnabWriter writer = instance.CreateWriter(/* ... */))
{
    writer.Serialize(new TestStruct { /* ... */ });
}

/* For deserialization */
TestStruct parsedData;
using (SnabReader reader = instance.CreateReader(/* ... */))
{
    parsedData = reader.Deserialize<TestStruct>();
}
```

# Development Guide

Ensure that you have Git and .NET SDK 8.0 or higher installed and run the following commands to get started:

```powershell
# Grab source from GitHub repository
git clone "https://github.com/IsaMorphic/CFS.SnabNet.git"
cd ".\CFS.SnabNet"

# Full rebuild of solution
dotnet build --no-incremental

# Run all unit tests
dotnet test --no-build
```

# Special Thanks

Special thanks to [AlubJ](https://alub.dev/) for coordinating the SNAB standard, and to my partner Hazel for putting up with me <3