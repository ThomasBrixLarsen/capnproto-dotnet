pushd runtime && xbuild /p:Configuration=Release CapnProto.net.csproj && popd &&
make &&
pushd examples && capnpc -o../capnpc-dotnet addressbook.capnp && popd &&
csc /r:runtime/bin/Release/CapnProto-dotnet.dll examples/AddressbookMain.cs examples/Addressbook.cs &&
chmod +x AddressbookMain.exe &&
cp runtime/bin/Release/CapnProto-dotnet.dll . &&
./AddressbookMain.exe write | ./AddressbookMain.exe read
