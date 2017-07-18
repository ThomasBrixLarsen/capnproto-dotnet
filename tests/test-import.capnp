@0xd693321951fee8f3;

using DotNet = import "/capnp/dotnet.capnp";
$DotNet.package("Capnproto.Tests.Test");
$DotNet.outerClassname("TestImport");

using import "test.capnp".TestAllTypes;

struct Foo {
	importedStruct @0 :TestAllTypes;
}
