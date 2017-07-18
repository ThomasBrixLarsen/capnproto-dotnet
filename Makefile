CAPNP_CXX_FLAGS=$(shell pkg-config capnp --cflags --libs)

ifeq ($(CAPNP_CXX_FLAGS),)
$(warning "Warning: pkg-config failed to find compilation configuration for capnp.")
$(warning "Falling back to a guess based on the location of the capnp executable.")
CAPNP_PREFIX=$(shell dirname $(shell which capnp))/..
CAPNP_CXX_FLAGS=-I $(CAPNP_PREFIX)/include -L $(CAPNP_PREFIX)/lib -lkj -lcapnp
endif

CXX=g++
CXX_FLAGS=-std=c++11 $(CAPNP_CXX_FLAGS)

CAPNPC_DOTNET_SOURCES=compiler/src/main/cpp/capnpc-dotnet.c++

.PHONY: all clean

all: capnpc-dotnet

clean:
	rm -f capnpc-dotnet capnpc-dotnet.exe

capnpc-dotnet: $(CAPNPC_DOTNET_SOURCES)
	$(CXX) $(CAPNPC_DOTNET_SOURCES) $(CXX_FLAGS) -g -o capnpc-dotnet


MINGW_LIBS=~/src/capnproto/c++/build-mingw/.libs/libcapnp.a ~/src/capnproto/c++/build-mingw/.libs/libkj.a
MINGW_CXX=i686-w64-mingw32-g++
MINGW_FLAGS=-O2 -DNDEBUG -I/usr/local/include -std=c++11 -static -static-libgcc -static-libstdc++
capnpc-dotnet.exe: $(CAPNPC_DOTNET_SOURCES)
	$(MINGW_CXX) $(MINGW_FLAGS) $(CAPNPC_DOTNET_SOURCES) $(MINGW_LIBS) -o capnpc-dotnet.exe
