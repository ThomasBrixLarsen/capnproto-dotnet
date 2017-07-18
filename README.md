# capnproto-dotnet: Cap'n Proto for C#/.NET

[Cap'n Proto](http://capnproto.org) is an extremely efficient protocol for sharing data
and capabilities, and capnproto-dotnet is a pure C# implementation.

# State

* Passes Cap'n Proto testsuite.
* 5-15x slower than the official C++ implementation (see [benchmarks](#benchmarks)).
* .NET 3.5 - Can be used from Unity3D.
* Port of generics not done yet.
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

# <a name="benchmarks"></a>Benchmarks

Benchmarked on Skylake i7. Best of three runs.

```bash
csc /optimize+ /r:runtime/bin/Release/CapnProto-dotnet.dll benchmark/CarSales.cs benchmark/CarSalesSchema.cs benchmark/Common.cs benchmark/TestCase.cs benchmark/Compression.cs benchmark/Packed.cs benchmark/Uncompressed.cs

[capnproto-dotnet]$ time ./CarSales.exe object 0 none 20000
real    0m5,772s
user    0m5,569s
sys     0m0,190s

[capnproto-c++]$ time ./capnproto-carsales object no-reuse none 20000
real    0m0,410s
user    0m0,406s
sys     0m0,001s

[capnproto-c++]$ time ./capnproto-carsales object reuse none 20000
real    0m0,350s
user    0m0,346s
sys     0m0,002s

csc /optimize+ /r:runtime/bin/Release/CapnProto-dotnet.dll benchmark/CatRank.cs benchmark/CatRankSchema.cs benchmark/Common.cs benchmark/TestCase.cs benchmark/Compression.cs benchmark/Packed.cs benchmark/Uncompressed.cs

[capnproto-dotnet]$ time ./CatRank.exe object 0 none 20000
real    0m54,494s
user    0m52,739s
sys     0m1,647s

[capnproto-c++]$ time ./capnproto-catrank object no-reuse none 20000
real    0m11,259s
user    0m10,789s
sys     0m0,422s

[capnproto-c++]$ time ./capnproto-catrank object reuse none 20000
real    0m10,287s
user    0m10,251s
sys     0m0,003s

csc /optimize+ /r:runtime/bin/Release/CapnProto-dotnet.dll benchmark/Eval.cs benchmark/EvalSchema.cs benchmark/Common.cs benchmark/TestCase.cs benchmark/Compression.cs benchmark/Packed.cs benchmark/Uncompressed.cs

[capnproto-dotnet]$ time ./Eval.exe object 0 none 20000
real    0m0,493s
user    0m0,425s
sys     0m0,071s

[capnproto-c++]$ time ./capnproto-eval object no-reuse none 20000
real    0m0,191s
user    0m0,189s
sys     0m0,002s

[capnproto-c++]$ time ./capnproto-eval object reuse none 20000
real    0m0,185s
user    0m0,183s
sys     0m0,001s

```
