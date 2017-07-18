# capnproto-dotnet: Cap'n Proto for C#/.Net

[Cap'n Proto](http://capnproto.org) is an extremely efficient protocol for sharing data
and capabilities, and capnproto-dotnet is a pure C# implementation.

# State

* Passes Cap'n Proto testsuite.
* .NET 3.5 - Can be used from Unity3D.
* Missing porting of generics.
* Missing RPC part of Cap'n Proto.
* Missing JSON codec (workaround: capnp tool can convert to and from JSON).
* Missing Cap'n Proto toString format (workaround: capnp tool can convert to and from text format).

# Schema compilation
Build the dotnet plugin for the Cap'n Proto compiler.

```bash
make
```

Run the Cap'n Proto compiler to generate the C# interface code for your schema.

```bash
capnpc -odotnet example.capnp
```

Or

```bash
capnpc -o/path/to/capnpc-dotnet example.capnp
```

Depending on whether the dotnet plugin is installed to path.

# Use in code

```C#
public class ExampleApp
{
    public static void Main()
    {
        var message = new Capnproto.MessageBuilder();
        Example.AnyObject.Builder rootObject = message.InitRoot(Example.AnyObject.SingleFactory); //AnyObject from Example namespace.
        //Do stuff with rootObject.
        //Use Serialize or SerializePacked to get the serialized message.
    }
}
```

## Sample

A full example including pregenerated C# code from schema is available [here](https://github.com/ThomasBrixLarsen/capnproto-dotnet/tree/master/examples).

```bash
[capnproto-dotnet]$ pushd runtime && xbuild /p:Configuration=Release CapnProto.net.csproj && popd
[capnproto-dotnet]$ make
[capnproto-dotnet]$ pushd examples && capnpc -o../capnpc-dotnet addressbook.capnp && popd
[capnproto-dotnet]$ csc /r:runtime/bin/Release/CapnProto-dotnet.dll examples/AddressbookMain.cs examples/Addressbook.cs
[capnproto-dotnet]$ chmod +x AddressbookMain.exe
[capnproto-dotnet]$ cp runtime/bin/Release/CapnProto-dotnet.dll .
[capnproto-dotnet]$ ./AddressbookMain.exe write | ./AddressbookMain.exe read
Alice: alice@example.com
  mobile phone: 555-1212
  student at: MIT
Bob: bob@example.com
  home phone: 555-4567
  work phone: 555-7654
  unemployed
```
