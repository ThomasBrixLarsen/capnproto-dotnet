make &&
pushd runtime &&
xbuild /p:Configuration=Release CapnProto.net.csproj &&
popd &&
pushd tests &&
capnpc -o../capnpc-dotnet test.capnp &&
capnpc -o../capnpc-dotnet test-import.capnp &&
popd &&
csc /r:NUnit/nunit.framework.dll /r:runtime/bin/Release/CapnProto-dotnet.dll tests/EncodingSuite.cs tests/LayoutSuite.cs tests/SerializeSuite.cs tests/SerializePackedSuite.cs tests/TestUtil.cs tests/Test.cs tests/TestImport.cs /target:library &&
cp runtime/bin/Release/CapnProto-dotnet.dll NUnit/ &&
cp EncodingSuite.dll NUnit/ &&
pushd NUnit/ &&
nunit3-console EncodingSuite.dll &&
popd
