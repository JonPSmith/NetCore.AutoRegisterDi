# NetCore.AutoRegisterDi

I have written a simple version of AutoFac's `RegisterAssemblyTypes` method that works directly with Microsoft's DI provider, i.e this library to scan an assemby (or assemblies) on your application and register all the public normal classes (i.e not [generic classes](https://www.tutorialspoint.com/Generics-vs-non-generics-in-Chash)) that have an interface into the Microsoft NET's [Dependency injection provider](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection) (DI for short).

The NetCore.AutoRegisterDi is available on [NuGet as EfCore.SchemaCompare](https://www.nuget.org/packages/NetCore.AutoRegisterDi) and is an open-source library under the MIT license. The documenation is found in the [README file](https://github.com/JonPSmith/NetCore.AutoRegisterDi/blob/master/README.md) and the [ReleaseNotes](https://github.com/JonPSmith/NetCore.AutoRegisterDi/blob/masterReleaseNotes.md) contains the details of each release.

## Why have I written this extension?

There are two reasons:

1. I really hate having to hand-code each registering of my services - this extension method scans assembles and finds/registers classes with interfaces for you.
2. I used to use [AutoFac's](https://autofac.org/) [assembly scanning](http://autofac.readthedocs.io/en/latest/register/scanning.html#assembly-scanning)
feature, but I then saw a [tweet by @davidfowl](https://twitter.com/davidfowl/status/987866910946615296) about [Dependency Injection container benchmark](https://ipjohnson.github.io/DotNet.DependencyInjectionBenchmarks/) which showed the Microsoft's DI provider was much faster than AutoFac. I therefore implemented a similar (but not exactly the same) feature for the `Microsoft.Extensions.DependencyInjection` library.

# Documentation

_NOTE: There is an [article about this library](https://www.thereformedprogrammer.net/asp-net-core-fast-and-automatic-dependency-injection-setup/) which gives you an overview of this library. Useful if you haven't used this library before._

## Two, simple examples

### Example 1 - scan the calling assembly

This example scans the assembly where you call the AutoRegisterDi's `RegisterAssemblyPublicNonGenericClasses` method for all the classes which has one or more public interfaces and the Class's name ends with "Service" are registered with .NET's 

```c#
public void ConfigureServices(IServiceCollection services)
{
   //... other configure code removed

   service.RegisterAssemblyPublicNonGenericClasses()
     .Where(c => c.Name.EndsWith("Service"))
     .AsPublicImplementedInterfaces();
```

### Example 2 - scanning multiple assemblies

This example scans the three assemblies and registers *all* the classes that have one or more public interfaces. That's because I have commented out the `.Where(c => c.Name.EndsWith("Service"))` method.

```c#
public void ConfigureServices(IServiceCollection services)
{
   //... other configure code removed

   var assembliesToScan = new [] 
   {
        Assembly.GetExecutingAssembly(),
        Assembly.GetAssembly(typeof(MyServiceInAssembly1)),
        Assembly.GetAssembly(typeof(MyServiceInAssembly2))
   };   

   service.RegisterAssemblyPublicNonGenericClasses(assembliesToScan)
     //commenting the line below means it will scan all public classes
     //.Where(c => c.Name.EndsWith("Service"))  
     .AsPublicImplementedInterfaces(); 
```

## Detailed information

There are four parts:

1. `RegisterAssemblyPublicNonGenericClasses`, which finds all the public classes that:
    - Aren't abstract
    - Aren't a generic type, e.g. MyClass\<AnotherClass\>
    - Isn't nested. e.g. It won't look at classes defined inside other classes
2. An optional `Where` method, which allows you to filter the classes to be considered.
3. The `AsPublicImplementedInterfaces` method which finds ant interfaces on a class and registers those interfaces as pointing to the class.
4. There are two methods to allow you to say that a specific interface should be ignored:
    - The `IgnoreThisInterface<TInterface>()` method to add an interface to the the ignored list.
    - The `IgnoreThisGenericInterface(Type interfaceType)` method to add an generic interface to the the ignored list.
5. Various attributes that you can add to your classes to tell `NetCore.AutoRegisterDi` what to do:
    - Set the `ServiceLifetime` of your class, e.g. `[RegisterAsSingleton]` to apply a `Singleton` lifetime to your class.
    - A `[DoNotAutoRegister]` attribute to stop library your class from being registered with the DI.

### 1. The `RegisterAssemblyPublicNonGenericClasses` method

The `RegisterAssemblyPublicNonGenericClasses` method will find all the classes in

1. If no assemblies are provided then it scans the assembly that called this method.
2. You can provide one or more assemblies to be scanned. The easiest way to reference an assembly is to use something like this `Assembly.GetAssembly(typeof(MyService))`, which gets the assembly that `MyService` was defined in.

I only consider classes which match ALL of the criteria below:

- Public access
- Not nested, e.g. It won't look at classes defined inside other classes
- Not Generic, e.g. MyClass\<T\>
- Not Abstract

### 2. The `Where` method

Pretty straightforward - you are provided with the `Type` of each class and you can filter by any of the `Type` properties etc. This allows you to do things like only registering certain classes, e.g `Where(c => c.Name.EndsWith("Service"))`.

*NOTE: Useful also if you want to register some classes with a different time scope - See next section.*

### 3. The `AsPublicImplementedInterfaces` method

The `AsPublicImplementedInterfaces` method finds any public, non-nested interfaces (apart from `IDisposable` and `ISerializable`) that each class implements and registers each interface, known as _service type_, against the class, known as the _implementation type_. This means if you use an interface in a constructor (or other DI-enabled places) then the Microsoft DI resolver will provide an instance of the class that interface was linked to. _See [Microsoft DI Docs](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-2.1) for more on this_.

By default it will register the classes as having a lifetime of `ServiceLifetime.Transient`, but the `AsPublicImplementedInterfaces` method has an optional parameter called `lifetime` which you can change to another lifetime. Note that all the classes will have the same lifetime, but you can use AutoRegisterDi's attributes to set different lifetime to a class (see section 5).  

*See this [useful article](https://joonasw.net/view/aspnet-core-di-deep-dive)
on what lifetime (and other terms) means.*

### 4. Ignore Interfaces

Some classes have interfaces that we don't really want to be registered to the DI provider - for instance `IDisposable` and `ISerializable`. Therefore AutoRegisterDi has two methods which allow you to define a interface that shouldn't be registered on any classes. They are:

#### The `IgnoreThisInterface<TInterface>` method

This method allows you to add a interface to to a list of interfaces that you don't register to the DI provider. The example below adds the `IMyInterface` in the list of interfaces to not register.

```c#
service.RegisterAssemblyPublicNonGenericClasses()
   .IgnoreThisInterface<IMyInterface>()
   .AsPublicImplementedInterfaces();
```

NOTES

- The list of interfaces to ignore has already got the `IDisposable` and `ISerializable` interfaces.
- You can ignore many interfaces by including have many `IgnoreThisInterface<TInterface>` calls in the setup.

#### The `IgnoreThisGenericInterface(Type interfaceType)` method

This method was asked for Lajos Marton (GitHub @martonx). He was using [`record`s](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/record) and found that each `record` has a `IEquatable<RecordType>` and he didn't wanted the DI provider be up with interfaces that aren't used.

You could use the `IgnoreThisInterface<IEquatable<RecordType>>`, but you would need to do that for every record. The other solution is to say that ALL `IEquatable<T>` are ignored. The code below will do that.

```c#
service.RegisterAssemblyPublicNonGenericClasses()
   .IgnoreThisGenericInterface(typeof(IEquatable<>))
   .AsPublicImplementedInterfaces();
```

NOTES:

- I haven't `IEquatable<>` to the ignore interface list as there may be a valid use to register a `IEquatable<SomeClass>` sometimes. In that case you would need to use `IgnoreThisGenericInterface` to define all your `record`s separably.
- The method works for any generic interface type as long that the generic interface has no arguments are filled in, e.g `IDictionary<,>` is fine, but `IDictionary<,string>` won't work (you get an exception).

### 5. The attributes

Fedor Zhekov, (GitHub @ZFi88) added attributes to allow you to define the `ServiceLifetime` of your class, and also exclude your class from being registered with the DI.

Here are the attributes that sets the `ServiceLifetime` to be used when `NetCore.AutoRegisterDi` registers your class with the DI.

1. `[RegisterAsSingleton]` - Singleton lifetime.
2. `[RegisterAsTransient]` - Transient lifetime.
3. `[RegisterAsScoped]` - Scoped lifetime.

The last attribute is `[DoNotAutoRegister]`, which stops `NetCore.AutoRegisterDi` registered that class with the DI. 
