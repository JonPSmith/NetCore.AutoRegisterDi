# NetCore.AutoRegisterDi

This [NuGet library](https://www.nuget.org/packages/NetCore.AutoRegisterDi/)
contains an extension method to scan an assemby and register all 
the public classes against their implemented interfaces(s) into the
Microsoft.Extensions.DependencyInjection dependency injection provider. 

I have written a simple version of AutoFac's `RegisterAssemblyTypes`
method that works directly with Microsoft's DI provider.
Here is an example of me using this with ASP.NET Core

```c#
public void ConfigureServices(IServiceCollection services)
{
   //... other configure code removed

   var assemblyToScan = Assembly.GetExecutingAssembly(); //..or whatever assembly you need

   service.RegisterAssemblyPublicNonGenericClasses(assemblyToScan)
     .Where(c => c.Name.EndsWith("Service"))
     .AsPublicImplementedInterfaces();
```

Licence: MIT.

**See [this article](https://www.thereformedprogrammer.net/asp-net-core-fast-and-automatic-dependency-injection-setup/)
for a bigger coverage of Microsoft DI and the use of this library in real applications.**

## Why have I written this extension?

There are two reasons:

1. I really hate having to hand-code each registering of the services - this
extension method scans assembles and finds/registers classes with interfaces for you.
2. I used to use [AutoFac's](https://autofac.org/) [assembly scanning](http://autofac.readthedocs.io/en/latest/register/scanning.html#assembly-scanning)
feature, but I then saw a [tweet by @davidfowl](https://twitter.com/davidfowl/status/987866910946615296) about 
[Dependency Injection container benchmarks](https://ipjohnson.github.io/DotNet.DependencyInjectionBenchmarks/)
which showed the Microsoft's DI provider was much faster than AutoFac.
I therefore implemented a similar (but not exactly the same) feature for the
Microsoft.Extensions.DependencyInjection library.

**Thanks to Inventory Innovations, Inc. who sponsored the creation of this library.**

### Detailed information

There are three parts:
1. `RegisterAssemblyPublicNonGenericClasses`, which finds all the classes.
2. An options `Where` method, which allows you to filter the classes to be considered.
3. The `AsPublicImplementedInterfaces` method which finds ant interfaces on a class and
registers those interfaces as pointing to the class.


#### 1. The `RegisterAssemblyPublicNonGenericClasses` method

The `RegisterAssemblyPublicNonGenericClasses` method will find all the classes
in the assembly that I referenced that are considered useful for registering.
The exact criteria I use are:

- Public access
- Not nested, e.g. It won't look at classes defined inside other classes
- Not Generic, e.g. MyClass\<T\>
- Not Abstract

The method takes a list/array of assemblies to scan. Two typical ways of providing an assembly are:

- `Assembly.GetExecutingAssembly()`, which does what is says
- `Assembly.GetAssembly(typeof(YourClass))`, which gets the assembly that `YourClass` was defined in.

#### 2. The `Where` method

Pretty straightforward - you are provided with the `Type` of each class and
you can filter by any of the `Type` properties etc. This allows you to
do things like only registering certain classes,
e.g `Where(c => c.Name.EndsWith("Service"))`

*NOTE: Useful also if you want to register some classes with a different timetime scope -
See next section.*

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