<!--
GENERATED FILE - DO NOT EDIT
This file was generated by [MarkdownSnippets](https://github.com/SimonCropp/MarkdownSnippets).
Source File: /readme.source.md
To change this file edit the source file and then run MarkdownSnippets.
-->

# <img src="/src/icon.png" height="30px"> Verify.EntityFramework

[![Build status](https://ci.appveyor.com/api/projects/status/g6njwv0aox62atu0?svg=true)](https://ci.appveyor.com/project/SimonCropp/verify-entityframework)
[![NuGet Status](https://img.shields.io/nuget/v/Verify.EntityFramework.svg)](https://www.nuget.org/packages/Verify.EntityFramework/)
[![NuGet Status](https://img.shields.io/nuget/v/Verify.EntityFrameworkClassic.svg)](https://www.nuget.org/packages/Verify.EntityFrameworkClassic/)

Extends [Verify](https://github.com/VerifyTests/Verify) to allow verification of EntityFramework bits.

Support is available via a [Tidelift Subscription](https://tidelift.com/subscription/pkg/nuget-verify?utm_source=nuget-verify&utm_medium=referral&utm_campaign=enterprise).

<a href='https://dotnetfoundation.org' alt='Part of the .NET Foundation'><img src='https://raw.githubusercontent.com/VerifyTests/Verify/master/docs/dotNetFoundation.svg' height='30px'></a><br>
Part of the <a href='https://dotnetfoundation.org' alt=''>.NET Foundation</a>

<!-- toc -->
## Contents

  * [Usage](#usage)
    * [EF Core](#ef-core)
    * [EF Classic](#ef-classic)
    * [ChangeTracking](#changetracking)
    * [Queryable](#queryable)
    * [EF Core](#ef-core-1)
    * [EF Classic](#ef-classic-1)
  * [Security contact information](#security-contact-information)<!-- endtoc -->


## NuGet package

 * https://nuget.org/packages/Verify.EntityFramework/
 * https://nuget.org/packages/Verify.EntityFrameworkClassic/


## Usage

Enable VerifyEntityFramewok once at assembly load time:


### EF Core

<!-- snippet: EnableCore -->
<a id='snippet-enablecore'></a>
```cs
VerifyEntityFramework.Enable();
```
<sup><a href='/src/Verify.EntityFramework.Tests/CoreTests.cs#L144-L146' title='File snippet `enablecore` was extracted from'>snippet source</a> | <a href='#snippet-enablecore' title='Navigate to start of snippet `enablecore`'>anchor</a></sup>
<!-- endsnippet -->


### EF Classic

<!-- snippet: EnableClassic -->
<a id='snippet-enableclassic'></a>
```cs
VerifyEntityFrameworkClassic.Enable();
```
<sup><a href='/src/Verify.EntityFrameworkClassic.Tests/ClassicTests.cs#L129-L131' title='File snippet `enableclassic` was extracted from'>snippet source</a> | <a href='#snippet-enableclassic' title='Navigate to start of snippet `enableclassic`'>anchor</a></sup>
<!-- endsnippet -->


### ChangeTracking

Added, deleted, and Modified entities can be verified by performing changes on a DbContext and then verifying that context. This approach leverages the [EntityFramework ChangeTracker](https://docs.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.changetracking.changetracker).


#### Added entity

This test:

<!-- snippet: Added -->
<a id='snippet-added'></a>
```cs
[Test]
public async Task Added()
{
    var options = DbContextOptions();

    await using var data = new SampleDbContext(options);
    var company = new Company
    {
        Content = "before"
    };
    data.Add(company);
    await Verifier.Verify(data);
}
```
<sup><a href='/src/Verify.EntityFramework.Tests/CoreTests.cs#L12-L26' title='File snippet `added` was extracted from'>snippet source</a> | <a href='#snippet-added' title='Navigate to start of snippet `added`'>anchor</a></sup>
<!-- endsnippet -->

Will result in the following verified file:

<!-- snippet: CoreTests.Added.verified.txt -->
<a id='snippet-CoreTests.Added.verified.txt'></a>
```txt
{
  Added: {
    Company: {
      Id: 0,
      Content: 'before'
    }
  }
}
```
<sup><a href='/src/Verify.EntityFramework.Tests/CoreTests.Added.verified.txt#L1-L8' title='File snippet `CoreTests.Added.verified.txt` was extracted from'>snippet source</a> | <a href='#snippet-CoreTests.Added.verified.txt' title='Navigate to start of snippet `CoreTests.Added.verified.txt`'>anchor</a></sup>
<!-- endsnippet -->


#### Deleted entity

This test:

<!-- snippet: Deleted -->
<a id='snippet-deleted'></a>
```cs
[Test]
public async Task Deleted()
{
    var options = DbContextOptions();

    await using var data = new SampleDbContext(options);
    data.Add(new Company {Content = "before"});
    await data.SaveChangesAsync();

    var company = data.Companies.Single();
    data.Companies.Remove(company);
    await Verifier.Verify(data);
}
```
<sup><a href='/src/Verify.EntityFramework.Tests/CoreTests.cs#L28-L42' title='File snippet `deleted` was extracted from'>snippet source</a> | <a href='#snippet-deleted' title='Navigate to start of snippet `deleted`'>anchor</a></sup>
<!-- endsnippet -->

Will result in the following verified file:

<!-- snippet: CoreTests.Deleted.verified.txt -->
<a id='snippet-CoreTests.Deleted.verified.txt'></a>
```txt
{
  Deleted: {
    Company: {
      Id: 0
    }
  }
}
```
<sup><a href='/src/Verify.EntityFramework.Tests/CoreTests.Deleted.verified.txt#L1-L7' title='File snippet `CoreTests.Deleted.verified.txt` was extracted from'>snippet source</a> | <a href='#snippet-CoreTests.Deleted.verified.txt' title='Navigate to start of snippet `CoreTests.Deleted.verified.txt`'>anchor</a></sup>
<!-- endsnippet -->


#### Modified entity

This test:

<!-- snippet: Modified -->
<a id='snippet-modified'></a>
```cs
[Test]
public async Task Modified()
{
    var options = DbContextOptions();

    await using var data = new SampleDbContext(options);
    var company = new Company
    {
        Content = "before"
    };
    data.Add(company);
    await data.SaveChangesAsync();

    data.Companies.Single().Content = "after";
    await Verifier.Verify(data);
}
```
<sup><a href='/src/Verify.EntityFramework.Tests/CoreTests.cs#L44-L61' title='File snippet `modified` was extracted from'>snippet source</a> | <a href='#snippet-modified' title='Navigate to start of snippet `modified`'>anchor</a></sup>
<!-- endsnippet -->

Will result in the following verified file:

<!-- snippet: CoreTests.Modified.verified.txt -->
<a id='snippet-CoreTests.Modified.verified.txt'></a>
```txt
{
  Modified: {
    Company: {
      Id: 0,
      Content: {
        Original: 'before',
        Current: 'after'
      }
    }
  }
}
```
<sup><a href='/src/Verify.EntityFramework.Tests/CoreTests.Modified.verified.txt#L1-L11' title='File snippet `CoreTests.Modified.verified.txt` was extracted from'>snippet source</a> | <a href='#snippet-CoreTests.Modified.verified.txt' title='Navigate to start of snippet `CoreTests.Modified.verified.txt`'>anchor</a></sup>
<!-- endsnippet -->


### Queryable

This test:

<!-- snippet: Queryable -->
<a id='snippet-queryable'></a>
```cs
[Test]
public async Task Queryable()
{
    var database = await DbContextBuilder.GetDatabase("Queryable");
    var data = database.Context;
    var queryable = data.Companies
        .Where(x => x.Content == "value");
    await Verifier.Verify(queryable);
}
```
<sup><a href='/src/Verify.EntityFramework.Tests/CoreTests.cs#L122-L132' title='File snippet `queryable` was extracted from'>snippet source</a> | <a href='#snippet-queryable' title='Navigate to start of snippet `queryable`'>anchor</a></sup>
<!-- endsnippet -->

Will result in the following verified file:


### EF Core

<!-- snippet: CoreTests.Queryable.verified.txt -->
<a id='snippet-CoreTests.Queryable.verified.txt'></a>
```txt
SELECT [c].[Id], [c].[Content]
FROM [Companies] AS [c]
WHERE [c].[Content] = N'value'
```
<sup><a href='/src/Verify.EntityFramework.Tests/CoreTests.Queryable.verified.txt#L1-L3' title='File snippet `CoreTests.Queryable.verified.txt` was extracted from'>snippet source</a> | <a href='#snippet-CoreTests.Queryable.verified.txt' title='Navigate to start of snippet `CoreTests.Queryable.verified.txt`'>anchor</a></sup>
<!-- endsnippet -->


### EF Classic

<!-- snippet: ClassicTests.Queryable.verified.txt -->
<a id='snippet-ClassicTests.Queryable.verified.txt'></a>
```txt
SELECT 
    [Extent1].[Id] AS [Id], 
    [Extent1].[Content] AS [Content]
    FROM [dbo].[Companies] AS [Extent1]
    WHERE N'value' = [Extent1].[Content]
```
<sup><a href='/src/Verify.EntityFrameworkClassic.Tests/ClassicTests.Queryable.verified.txt#L1-L5' title='File snippet `ClassicTests.Queryable.verified.txt` was extracted from'>snippet source</a> | <a href='#snippet-ClassicTests.Queryable.verified.txt' title='Navigate to start of snippet `ClassicTests.Queryable.verified.txt`'>anchor</a></sup>
<!-- endsnippet -->


## Security contact information

To report a security vulnerability, use the [Tidelift security contact](https://tidelift.com/security). Tidelift will coordinate the fix and disclosure.


## Icon

[Database](https://thenounproject.com/term/database/310841/) designed by [Creative Stall](https://thenounproject.com/creativestall/) from [The Noun Project](https://thenounproject.com/creativepriyanka).
