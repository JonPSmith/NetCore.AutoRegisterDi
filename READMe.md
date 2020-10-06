# NetCore.AutoRegisterDi

This [NuGet library](https://www.nuget.org/packages/NetCore.AutoRegisterDi/) contains an extension method to scan an assemby and register all the non-generic public classes (see [excluded class types](https://github.com/JonPSmith/NetCore.AutoRegisterDi#1-the-registerassemblypublicnongenericclasses-method)) against their implemented interfaces(s) into the `Microsoft.Extensions.DependencyInjection` dependency injection provider. 

**Version 2.0.0 update**: New attributes for defining the `ServiceLifetime` of your classes, e.g. adding the `[RegisterAsScoped]` attribute to a class will mean its `ServiceLifetime` in the DI will be set to `Scoped`. *Added by Fedor Zhekov (GitHub @ZFi88)*.

**Version 2.1.0 update**: Added ability ignore an interface, plus added `ISerializable` interface to list of ignored interfaces and outputs results so that you can check it is registering the correct things.

I have written a simple version of AutoFac's `RegisterAssemblyTypes` method that works directly with Microsoft's DI provider. Here are an example of me using this with ASP.NET Core.

## Example 1 - scan the calling assembly

```c#
public void ConfigureServices(IServiceCollection services)
{
   //... other configure code removed

   service.RegisterAssemblyPublicNonGenericClasses()
     .Where(c => c.Name.EndsWith("Service"))  //optional
     .IgnoreThisInterface<IMyInterface>()     //optional
     .AsPublicImplementedInterfaces();
```

## Example 2 - scanning multiple assemblies

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
     .Where(c => c.Name.EndsWith("Service"))  //optional
     .IgnoreThisInterface<IMyInterface>()     //optional
     .AsPublicImplementedInterfaces(); 
```

Licence: MIT.

**See [this article](https://www.thereformedprogrammer.net/asp-net-core-fast-and-automatic-dependency-injection-setup/)
for a bigger coverage of Microsoft DI and the use of this library in real applications.**

## Why have I written this extension

There are two reasons:

1. I really hate having to hand-code each registering of the services - this
extension method scans assembles and finds/registers classes with interfaces for you.
2. I used to use [AutoFac's](https://autofac.org/) [assembly scanning](http://autofac.readthedocs.io/en/latest/register/scanning.html#assembly-scanning)
feature, but I then saw a [tweet by @davidfowl](https://twitter.com/davidfowl/status/987866910946615296) about 
[Dependency Injection container benchmarks](https://ipjohnson.github.io/DotNet.DependencyInjectionBenchmarks/)
which showed the Microsoft's DI provider was much faster than AutoFac.
I therefore implemented a similar (but not exactly the same) feature for the
Microsoft.Extensions.DependencyInjection library.

## Detailed information

There are five parts:
1. `RegisterAssemblyPublicNonGenericClasses`, which finds all the public classes that : 
    - Aren't abstract
    - Aren't a generic type, e.g. MyClass\<AnotherClass\>
    - Isn't nested. e.g. It won't look at classes defined inside other classes
2. An `Where` extension method, which allows you to filter the classes to be considered.
3. An `IgnoreThisInterface<IMyInterface>` extension method that allows you to add an interface to be ignored. This allows you to stop a class being registered 
4. The `AsPublicImplementedInterfaces` method which finds ant interfaces on a class and registers those interfaces as pointing to the class. It also returns a list of results that allows you check what classes have been registered with the DI provider.
5. Various attributes that you can add to your classes to tell `NetCore.AutoRegisterDi` what to do:
   i) Set the `ServiceLifetime` of your class, e.g. `[RegisterAsSingleton]` to apply a `Singleton` lifetime to your class.
   ii) A `[DoNotAutoRegister]` attribute to stop library your class from being registered with the DI.

The following sections describes each part in detail.

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

Pretty straightforward - you are provided with the `Type` of each class and you can filter by any of the `Type` properties etc. This allows you to do things like only registering certain classes, e.g `Where(c => c.Name.EndsWith("Service"))`

*NOTES:*

- Useful also if you want to register some classes with a different lifetime scope: See section 4.
- You can use multiple `Where` methods. They have an `&&` effect, e.g. the following code would register all the classes other than `Class1` and `Class2`:  

```c#
service.RegisterAssemblyPublicNonGenericClasses()
    .Where(c => c.Name != "Class1"))
    .Where(c => c.Name != "Class2"))
    .AsPublicImplementedInterfaces();
```

### 3. The `IgnoreThisInterface<IMyInterface>` method

There are some interfaces that you don't want your classes to be registered, for instance `IDisposable` and `ISerializable` and these are already registered in NetCore.AutoRegisterDi. This means that no class will be registered against these two interfaces.

You can add extra interfaces to the interface ignore list using the `IgnoreThisInterface<IMyInterface>`, e.g. the following code would add the two interfaces  `IMyInterface1` and `IMyInterface2` to the  interface ignore list.

```c#
service.RegisterAssemblyPublicNonGenericClasses()
    .IgnoreThisInterface<IMyInterface1>()
    .IgnoreThisInterface<IMyInterface2>()
    .AsPublicImplementedInterfaces();
```

### 4. The `AsPublicImplementedInterfaces` method

The `AsPublicImplementedInterfaces` method finds any public, non-nested interfaces 
(apart from the interfaces in the interface ignore list) that each class implements and registers each interface, known as *service type*, against the class, known as the *implementation type*. This means if you use an interface in a constructor (or other DI-enabled places) then the Microsoft DI resolver will provide an instance of the class that interface
was linked to.

*See [Microsoft DI Docs](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-2.1) for more on this*.*

#### 4a. Debugging what classes/interfaces have been registered

The `AsPublicImplementedInterfaces` method returns a list of `AutoRegisteredResult` classes, which shows what classes have been registered with the DI provider. By catching this result you can quickly see what the NetCore.AutoRegisterDi library has registered. 

The following code would capture the list of results

```c#
var results = service.RegisterAssemblyPublicNonGenericClasses()
     .IgnoreThisInterface<IMyInterface>()
     .AsPublicImplementedInterfaces();
```

If you hover of the `results` list it will show a list similar to the one below, but with you own classes/interfaces.

```text
The interface IDisposable is ignored
The interface ISerializable is ignored
The interface IMyInterface is ignored
LocalScopeService : ILocalService (Scoped)
LocalService : ILocalService (Transient)
LocalService : IAnotherInterface (Transient)
```

##### 4b. Changing the default ServiceLifetime

The default lifetime of  registered class is `ServiceLifetime.Transient`, there are two ways to change this.

- Add a NetCore.AutoRegisterDi lifetime attribute to your class (see section 5).
- You can provide a `ServiceLifetime` to the `AsPublicImplementedInterfaces` method, e.g. `AsPublicImplementedInterfaces(ServiceLifetime.Scoped)`, which will change the default lifetime to the parameter value you provide.

*See this [useful article](https://joonasw.net/view/aspnet-core-di-deep-dive)
on what lifetime (and other terms) means.*

### 5. The attributes

Fedor Zhekov, (GitHub @ZFi88) added attributes to allow you to define the `ServiceLifetime` of your class, and also exclude your class from being registered with the DI.

Here are the attributes that sets the `ServiceLifetime` to be used when `NetCore.AutoRegisterDi` registers your class with the DI.

1. `[RegisterAsSingleton]` - Singleton lifetime.
2. `[RegisterAsTransient]` - Transient lifetime.
3. `[RegisterAsScoped]` - Scoped lifetime.

The last attribute is `[DoNotAutoRegister]`, which stops `NetCore.AutoRegisterDi` registered that class with the DI.