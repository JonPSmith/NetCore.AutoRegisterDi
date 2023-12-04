# NetCore.AutoRegisterDi

I have written a simple version of AutoFac's `RegisterAssemblyTypes` method that works directly with Microsoft's DI provider, i.e this library to scan an assemby (or assemblies) on your application and register all the public normal classes (i.e not [generic classes](https://www.tutorialspoint.com/Generics-vs-non-generics-in-Chash)) that have an interface into the Microsoft NET's [Dependency injection provider]https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection (DI for short).

The NetCore.AutoRegisterDi is available on [NuGet as EfCore.SchemaCompare](https://www.nuget.org/packages/NetCore.AutoRegisterDi) and is an open-source library under the MIT license. See [ReleaseNotes](https://github.com/JonPSmith/NetCore.AutoRegisterDi/blob/masterReleaseNotes.md) for details of changes and information for each release.

## Why have I written this extension?

There are two reasons:

1. I really hate having to hand-code each registering of my services - this extension method scans assembles and finds/registers classes with interfaces for you.
2. I used to use [AutoFac's](https://autofac.org/) [assembly scanning](http://autofac.readthedocs.io/en/latest/register/scanning.html#assembly-scanning)
feature, but I then saw a [tweet by @davidfowl](https://twitter.com/davidfowl/status/987866910946615296) about [Dependency Injection container benchmark (https://ipjohnson.github.io/DotNet.DependencyInjectionBenchmarks/) which showed the Microsoft's DI provider was much faster than AutoFac. I therefore implemented a similar (but not exactly the same) feature for the Microsoft.Extensions.DependencyInjection library.

## Documentation

_NOTE: There is an [article about this library](https://www.thereformedprogrammer.net/asp-net-core-fast-and-automatic-dependency-injection-setup/) which gives you an overview of this library. Useful if you haven't used this library before._

### Two, simple examples

#### Example 1 - scan the calling assembly

This example scans the assembly where you call the AutoRegisterDi's `RegisterAssemblyPublicNonGenericClasses` method for all the classes which has one or more public interfaces and the Class's name ends with "Service" are registered with .NET's 

```c#
public void ConfigureServices(IServiceCollection services)
{
   //... other configure code removed

   service.RegisterAssemblyPublicNonGenericClasses()
     .Where(c => c.Name.EndsWith("Service"))
     .AsPublicImplementedInterfaces();
```

#### Example 2 - scanning multiple assemblies

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

### Detailed information

There are four parts:

1. `RegisterAssemblyPublicNonGenericClasses`, which finds all the public classes that: 
    - Aren't abstract
    - Aren't a generic type, e.g. MyClass\<AnotherClass\>
    - Isn't nested. e.g. It won't look at classes defined inside other classes
2. An optional `Where` method, which allows you to filter the classes to be considered.
3. The `AsPublicImplementedInterfaces` method which finds ant interfaces on a class and registers those interfaces as pointing to the class.
4. Various attributes that you can add to your classes to tell `NetCore.AutoRegisterDi` what to do:
   i) Set the `ServiceLifetime` of your class, e.g. `[RegisterAsSingleton]` to apply a `Singleton` lifetime to your class.
   ii) A `[DoNotAutoRegister]` attribute to stop library your class from being registered with the DI.


#### 1. The `RegisterAssemblyPublicNonGenericClasses` method

The `RegisterAssemblyPublicNonGenericClasses` method will find all the classes in

1. If no assemblies are provided then it scans the assembly that called this method.
2. You can provide one or more assemblies to be scanned. The easiest way to reference an assembly is to use something like this `Assembly.GetAssembly(typeof(MyService))`, which gets the assembly that `MyService` was defined in.

I only consider classes which match ALL of the criteria below:

- Public access
- Not nested, e.g. It won't look at classes defined inside other classes
- Not Generic, e.g. MyClass\<T\>
- Not Abstract

#### 2. The `Where` method

Pretty straightforward - you are provided with the `Type` of each class and you can filter by any of the `Type` properties etc. This allows you to do things like only registering certain classes, e.g `Where(c => c.Name.EndsWith("Service"))`

*NOTE: Useful also if you want to register some classes with a different time scope - See next section.*

#### 3. The `AsPublicImplementedInterfaces` method

The `AsPublicImplementedInterfaces` method finds any public, non-nested interfaces 
(apart from `IDisposable`) that each class implements and registers each
interface, known as *service type*, against the class, known as the *implementation type*.
This means if you use an interface in a constructor (or other DI-enabled places)
then the Microsoft DI resolver will provide an instance of the class that interface
was linked to.
*See [Microsoft DI Docs](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-2.1) for more on this*.*

By default it will register the classes as having a lifetime of `ServiceLifetime.Transient`,
but there is a parameter that allows you to override that.

*See this [useful article](https://joonasw.net/view/aspnet-core-di-deep-dive)
on what lifetime (and other terms) means.*

#### 4. The attributes

Fedor Zhekov, (GitHub @ZFi88) added attributes to allow you to define the `ServiceLifetime` of your class, and also exclude your class from being registered with the DI. 

Here are the attributes that sets the `ServiceLifetime` to be used when `NetCore.AutoRegisterDi` registers your class with the DI.

1. `[RegisterAsSingleton]` - Singleton lifetime.
2. `[RegisterAsTransient]` - Transient lifetime.
3. `[RegisterAsScoped]` - Scoped lifetime.

The last attribute is `[DoNotAutoRegister]`, which stops `NetCore.AutoRegisterDi` registered that class with the DI.
