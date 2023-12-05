# Release notes

NOTE TO SELF: The Test project didn't compile when in Release mode (but works in Debug). I unloaded the Test project to build the NuGet

## 2.2.0

- New Feature: Method to ignore generic interfaces. Useful if you use record types and don't want to add its IEquatable<> to the DI

## 2.1.0

- New feature: IgnoreThisInterface method adds a interface to its interface ignore list.
- Improvement: Added ISerializable interface to the default interface ignore list (IDisposable is already in base ignore list)
- Improvement: You can use muliple have Where methods to filter out classes
- Improvement: Returns a list of the classes/interfaces registered with the DI provider (useful for debugging)

## 2.0.0

- New Feature: You can now set what ServiceLifetime your class has via Attributes. *Added by Fedor Zhekov (GitHub @ZFi88)*.

## 1.1.0

- New Feature: if no assembly is provided it scans the assembly that called the method.

## 1.0.0

- First release