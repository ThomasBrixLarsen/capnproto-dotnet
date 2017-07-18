// Copyright (c) 2013-2014 Sandstorm Development Group, Inc. and contributors
// Licensed under the MIT License:
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

namespace Capnproto
{
	internal sealed class WireHelpers
	{
		internal static int roundBytesUpToWords(int bytes)
		{
			return (bytes + 7) / 8;
		}
		
		internal static int roundBitsUpToBytes(int bits)
		{
			return (bits + 7) / Capnproto.Constants.BITS_PER_BYTE;
		}
		
		internal static int roundBitsUpToWords(long bits)
		{
			//# This code assumes 64-bit words.
			return (int)((bits + 63) / ((long)Capnproto.Constants.BITS_PER_WORD));
		}
		
		internal class AllocateResult
		{
			public readonly int ptr;
			
			public readonly int refOffset;
			
			public readonly Capnproto.SegmentBuilder segment;
			
			internal AllocateResult(int ptr, int refOffset, Capnproto.SegmentBuilder segment)
			{
				this.ptr = ptr;
				this.refOffset = refOffset;
				this.segment = segment;
			}
		}
		
		internal static Capnproto.WireHelpers.AllocateResult allocate(int refOffset, Capnproto.SegmentBuilder segment, int amount, byte kind)
		{
			// in words
			long @ref = segment.get(refOffset);
			if(!Capnproto.WirePointer.isNull(@ref))
				zeroObject(segment, refOffset);
			if(amount == 0 && kind == Capnproto.WirePointer.STRUCT)
			{
				Capnproto.WirePointer.setKindAndTargetForEmptyStruct(segment.buffer, refOffset);
				return new Capnproto.WireHelpers.AllocateResult(refOffset, refOffset, segment);
			}
			int ptr = segment.allocate(amount);
			if(ptr == Capnproto.SegmentBuilder.FAILED_ALLOCATION)
			{
				//# Need to allocate in a new segment. We'll need to
				//# allocate an extra pointer worth of space to act as
				//# the landing pad for a far pointer.
				int amountPlusRef = amount + Capnproto.Constants.POINTER_SIZE_IN_WORDS;
				Capnproto.BuilderArena.AllocateResult allocation = segment.getArena().allocate(amountPlusRef);
				//# Set up the original pointer to be a far pointer to
				//# the new segment.
				Capnproto.FarPointer.set(segment.buffer, refOffset, false, allocation.offset);
				Capnproto.FarPointer.setSegmentId(segment.buffer, refOffset, allocation.segment.id);
				//# Initialize the landing pad to indicate that the
				//# data immediately follows the pad.
				int resultRefOffset = allocation.offset;
				int ptr1 = allocation.offset + Capnproto.Constants.POINTER_SIZE_IN_WORDS;
				Capnproto.WirePointer.setKindAndTarget(allocation.segment.buffer, resultRefOffset, kind, ptr1);
				return new Capnproto.WireHelpers.AllocateResult(ptr1, resultRefOffset, allocation.segment);
			}
			Capnproto.WirePointer.setKindAndTarget(segment.buffer, refOffset, kind, ptr);
			return new Capnproto.WireHelpers.AllocateResult(ptr, refOffset, segment);
		}
		
		internal class FollowBuilderFarsResult
		{
			public readonly int ptr;
			
			public readonly long @ref;
			
			public readonly Capnproto.SegmentBuilder segment;
			
			internal FollowBuilderFarsResult(int ptr, long @ref, Capnproto.SegmentBuilder segment)
			{
				this.ptr = ptr;
				this.@ref = @ref;
				this.segment = segment;
			}
		}
		
		internal static Capnproto.WireHelpers.FollowBuilderFarsResult followBuilderFars(long @ref, int refTarget, Capnproto.SegmentBuilder segment)
		{
			//# If `ref` is a far pointer, follow it. On return, `ref` will
			//# have been updated to point at a WirePointer that contains
			//# the type information about the target object, and a pointer
			//# to the object contents is returned. The caller must NOT use
			//# `ref->target()` as this may or may not actually return a
			//# valid pointer. `segment` is also updated to point at the
			//# segment which actually contains the object.
			//#
			//# If `ref` is not a far pointer, this simply returns
			//# `refTarget`. Usually, `refTarget` should be the same as
			//# `ref->target()`, but may not be in cases where `ref` is
			//# only a tag.
			if(Capnproto.WirePointer.kind(@ref) == Capnproto.WirePointer.FAR)
			{
				Capnproto.SegmentBuilder resultSegment = segment.getArena().getSegment(Capnproto.FarPointer.getSegmentId(@ref));
				int padOffset = Capnproto.FarPointer.positionInSegment(@ref);
				long pad = resultSegment.get(padOffset);
				if(!Capnproto.FarPointer.isDoubleFar(@ref))
					return new Capnproto.WireHelpers.FollowBuilderFarsResult(Capnproto.WirePointer.target(padOffset, pad), pad, resultSegment);
				//# Landing pad is another far pointer. It is followed by a
				//# tag describing the pointed-to object.
				int refOffset = padOffset + 1;
				@ref = resultSegment.get(refOffset);
				resultSegment = resultSegment.getArena().getSegment(Capnproto.FarPointer.getSegmentId(pad));
				return new Capnproto.WireHelpers.FollowBuilderFarsResult(Capnproto.FarPointer.positionInSegment(pad), @ref, resultSegment);
			}
			return new Capnproto.WireHelpers.FollowBuilderFarsResult(refTarget, @ref, segment);
		}
		
		internal class FollowFarsResult
		{
			public readonly int ptr;
			
			public readonly long @ref;
			
			public readonly Capnproto.SegmentReader segment;
			
			internal FollowFarsResult(int ptr, long @ref, Capnproto.SegmentReader segment)
			{
				this.ptr = ptr;
				this.@ref = @ref;
				this.segment = segment;
			}
		}
		
		internal static Capnproto.WireHelpers.FollowFarsResult followFars(long @ref, int refTarget, Capnproto.SegmentReader segment)
		{
			//# If the segment is null, this is an unchecked message,
			//# so there are no FAR pointers.
			if(segment != null && Capnproto.WirePointer.kind(@ref) == Capnproto.WirePointer.FAR)
			{
				Capnproto.SegmentReader resultSegment = segment.arena.tryGetSegment(Capnproto.FarPointer.getSegmentId(@ref));
				int padOffset = Capnproto.FarPointer.positionInSegment(@ref);
				long pad = resultSegment.get(padOffset);
				int padWords = Capnproto.FarPointer.isDoubleFar(@ref)? 2 : 1;
				//TODO: Read limiting.
				if(!Capnproto.FarPointer.isDoubleFar(@ref))
					return new Capnproto.WireHelpers.FollowFarsResult(Capnproto.WirePointer.target(padOffset, pad), pad, resultSegment);
				
				//# Landing pad is another far pointer. It is
				//# followed by a tag describing the pointed-to
				//# object.
				long tag = resultSegment.get(padOffset + 1);
				resultSegment = resultSegment.arena.tryGetSegment(Capnproto.FarPointer.getSegmentId(pad));
				return new Capnproto.WireHelpers.FollowFarsResult(Capnproto.FarPointer.positionInSegment(pad), tag, resultSegment);
			}
			return new Capnproto.WireHelpers.FollowFarsResult(refTarget, @ref, segment);
		}
		
		internal static void zeroObject(Capnproto.SegmentBuilder segment, int refOffset)
		{
			//# Zero out the pointed-to object. Use when the pointer is
			//# about to be overwritten making the target object no longer
			//# reachable.
			//# We shouldn't zero out external data linked into the message.
			if(!segment.isWritable())
				return;
			long @ref = segment.get(refOffset);
			switch(Capnproto.WirePointer.kind(@ref))
			{
				case Capnproto.WirePointer.STRUCT:
				case Capnproto.WirePointer.LIST:
				{
					zeroObject(segment, @ref, Capnproto.WirePointer.target(refOffset, @ref));
					break;
				}
				case Capnproto.WirePointer.FAR:
				{
					segment = segment.getArena().getSegment(Capnproto.FarPointer.getSegmentId(@ref));
					if(segment.isWritable())
					{
						//# Don't zero external data.
						int padOffset = Capnproto.FarPointer.positionInSegment(@ref);
						long pad = segment.get(padOffset);
						if(Capnproto.FarPointer.isDoubleFar(@ref))
						{
							Capnproto.SegmentBuilder otherSegment = segment.getArena().getSegment(Capnproto.FarPointer.getSegmentId(@ref));
							if(otherSegment.isWritable())
							{
								zeroObject(otherSegment, padOffset + 1, Capnproto.FarPointer.positionInSegment(pad));
							}
							segment.buffer.putLong(padOffset * 8, 0L);
							segment.buffer.putLong((padOffset + 1) * 8, 0L);
						}
						else
						{
							zeroObject(segment, padOffset);
							segment.buffer.putLong(padOffset * 8, 0L);
						}
					}
					break;
				}
				case Capnproto.WirePointer.OTHER:
				{
					break;
				}
			}
		}
		
		//TODO
		internal static void zeroObject(Capnproto.SegmentBuilder segment, long tag, int ptr)
		{
			//# We shouldn't zero out external data linked into the message.
			if(!segment.isWritable())
				return;
			switch(Capnproto.WirePointer.kind(tag))
			{
				case Capnproto.WirePointer.STRUCT:
				{
					int pointerSection = ptr + Capnproto.StructPointer.dataSize(tag);
					int count = Capnproto.StructPointer.ptrCount(tag);
					for(int ii = 0; ii < count; ++ii)
						zeroObject(segment, pointerSection + ii);
					memset(segment.buffer, ptr * Capnproto.Constants.BYTES_PER_WORD, unchecked((byte)0), Capnproto.StructPointer.wordSize(tag) * Capnproto.Constants.BYTES_PER_WORD);
					break;
				}
				case Capnproto.WirePointer.LIST:
				{
					switch(Capnproto.ListPointer.elementSize(tag))
					{
						case Capnproto.ElementSize.VOID:
							break;
						case Capnproto.ElementSize.BIT:
						case Capnproto.ElementSize.BYTE:
						case Capnproto.ElementSize.TWO_BYTES:
						case Capnproto.ElementSize.FOUR_BYTES:
						case Capnproto.ElementSize.EIGHT_BYTES:
						{
							memset(segment.buffer, ptr * Capnproto.Constants.BYTES_PER_WORD, unchecked((byte)0), roundBitsUpToWords(Capnproto.ListPointer.elementCount(tag) * Capnproto.ElementSize.dataBitsPerElement(Capnproto.ListPointer.elementSize(tag))) * Capnproto.Constants.BYTES_PER_WORD);
							break;
						}
						case Capnproto.ElementSize.POINTER:
						{
							int count = Capnproto.ListPointer.elementCount(tag);
							for(int ii = 0; ii < count; ++ii)
								zeroObject(segment, ptr + ii);
							memset(segment.buffer, ptr * Capnproto.Constants.BYTES_PER_WORD, unchecked((byte)0), count * Capnproto.Constants.BYTES_PER_WORD);
							break;
						}
						case Capnproto.ElementSize.INLINE_COMPOSITE:
						{
							long elementTag = segment.get(ptr);
							if(Capnproto.WirePointer.kind(elementTag) != Capnproto.WirePointer.STRUCT)
								throw new System.Exception("Don't know how to handle non-STRUCT inline composite.");
							int dataSize = Capnproto.StructPointer.dataSize(elementTag);
							int pointerCount = Capnproto.StructPointer.ptrCount(elementTag);
							int pos = ptr + Capnproto.Constants.POINTER_SIZE_IN_WORDS;
							int count = Capnproto.WirePointer.inlineCompositeListElementCount(elementTag);
							for(int ii = 0; ii < count; ++ii)
							{
								pos += dataSize;
								for(int jj = 0; jj < pointerCount; ++jj)
								{
									zeroObject(segment, pos);
									pos += Capnproto.Constants.POINTER_SIZE_IN_WORDS;
								}
							}
							memset(segment.buffer, ptr * Capnproto.Constants.BYTES_PER_WORD, unchecked((byte)0), (Capnproto.StructPointer.wordSize(elementTag) * count + Capnproto.Constants.POINTER_SIZE_IN_WORDS) * Capnproto.Constants.BYTES_PER_WORD);
							break;
						}
					}
					break;
				}
				case Capnproto.WirePointer.FAR:
				{
					throw new System.Exception("Unexpected FAR pointer.");
				}
				case Capnproto.WirePointer.OTHER:
				{
					throw new System.Exception("Unexpected OTHER pointer.");
				}
			}
		}
		
		internal static void zeroPointerAndFars(Capnproto.SegmentBuilder segment, int refOffset)
		{
			//# Zero out the pointer itself and, if it is a far pointer, zero the landing pad as well,
			//# but do not zero the object body. Used when upgrading.
			long @ref = segment.get(refOffset);
			if(Capnproto.WirePointer.kind(@ref) == Capnproto.WirePointer.FAR)
			{
				Capnproto.SegmentBuilder padSegment = segment.getArena().getSegment(Capnproto.FarPointer.getSegmentId(@ref));
				if(padSegment.isWritable())
				{
					//# Don't zero external data.
					int padOffset = Capnproto.FarPointer.positionInSegment(@ref);
					padSegment.buffer.putLong(padOffset * Capnproto.Constants.BYTES_PER_WORD, 0L);
					if(Capnproto.FarPointer.isDoubleFar(@ref))
						padSegment.buffer.putLong(padOffset * Capnproto.Constants.BYTES_PER_WORD + 1,0L);
				}
			}
			segment.put(refOffset, 0L);
		}
		
		internal static void transferPointer(Capnproto.SegmentBuilder dstSegment, int dstOffset, Capnproto.SegmentBuilder srcSegment, int srcOffset)
		{
			//# Make *dst point to the same object as *src. Both must reside in the same message, but can
			//# be in different segments.
			//#
			//# Caller MUST zero out the source pointer after calling this, to make sure no later code
			//# mistakenly thinks the source location still owns the object.  transferPointer() doesn't do
			//# this zeroing itself because many callers transfer several pointers in a loop then zero out
			//# the whole section.
			long src = srcSegment.get(srcOffset);
			if(Capnproto.WirePointer.isNull(src))
				dstSegment.put(dstOffset, 0L);
			else if(Capnproto.WirePointer.kind(src) == Capnproto.WirePointer.FAR)
			{
				//# Far pointers are position-independent, so we can just copy.
				dstSegment.put(dstOffset, srcSegment.get(srcOffset));
			}
			else
				transferPointer(dstSegment, dstOffset, srcSegment, srcOffset, Capnproto.WirePointer.target(srcOffset, src));
		}
		
		internal static void transferPointer(Capnproto.SegmentBuilder dstSegment, int dstOffset, Capnproto.SegmentBuilder srcSegment, int srcOffset, int srcTargetOffset)
		{
			//# Like the other overload, but splits src into a tag and a target. Particularly useful for
			//# OrphanBuilder.
			long src = srcSegment.get(srcOffset);
			long srcTarget = srcSegment.get(srcTargetOffset);
			if(dstSegment == srcSegment)
			{
				//# Same segment, so create a direct pointer.
				if(Capnproto.WirePointer.kind(src) == Capnproto.WirePointer.STRUCT && Capnproto.StructPointer.wordSize(src) == 0)
					Capnproto.WirePointer.setKindAndTargetForEmptyStruct(dstSegment.buffer, dstOffset);
				else
					Capnproto.WirePointer.setKindAndTarget(dstSegment.buffer, dstOffset, Capnproto.WirePointer.kind(src), srcTargetOffset);
				//We can just copy the upper 32 bits.
				dstSegment.buffer.putInt(dstOffset * Capnproto.Constants.BYTES_PER_WORD + 4, srcSegment.buffer.getInt(srcOffset * Capnproto.Constants.BYTES_PER_WORD + 4));
			}
			else
			{
				//# Need to create a far pointer. Try to allocate it in the same segment as the source,
				//# so that it doesn't need to be a double-far.
				int landingPadOffset = srcSegment.allocate(1);
				if(landingPadOffset == Capnproto.SegmentBuilder.FAILED_ALLOCATION)
				{
					//# Darn, need a double-far.
					Capnproto.BuilderArena.AllocateResult allocation = srcSegment.getArena().allocate(2);
					Capnproto.SegmentBuilder farSegment = allocation.segment;
					landingPadOffset = allocation.offset;
					Capnproto.FarPointer.set(farSegment.buffer, landingPadOffset, false, srcTargetOffset);
					Capnproto.FarPointer.setSegmentId(farSegment.buffer, landingPadOffset, srcSegment.id);
					Capnproto.WirePointer.setKindWithZeroOffset(farSegment.buffer, landingPadOffset + 1, Capnproto.WirePointer.kind(src));
					farSegment.buffer.putInt((landingPadOffset + 1) * Capnproto.Constants.BYTES_PER_WORD + 4, srcSegment.buffer.getInt(srcOffset * Capnproto.Constants.BYTES_PER_WORD + 4));
					Capnproto.FarPointer.set(dstSegment.buffer, dstOffset, true, landingPadOffset);
					Capnproto.FarPointer.setSegmentId(dstSegment.buffer, dstOffset, farSegment.id);
				}
				else
				{
					//# Simple landing pad is just a pointer.
					Capnproto.WirePointer.setKindAndTarget(srcSegment.buffer, landingPadOffset, Capnproto.WirePointer.kind(srcTarget), srcTargetOffset);
					srcSegment.buffer.putInt(landingPadOffset * Capnproto.Constants.BYTES_PER_WORD + 4, srcSegment.buffer.getInt(srcOffset * Capnproto.Constants.BYTES_PER_WORD + 4));
					Capnproto.FarPointer.set(dstSegment.buffer, dstOffset, false, landingPadOffset);
					Capnproto.FarPointer.setSegmentId(dstSegment.buffer, dstOffset, srcSegment.id);
				}
			}
		}
		
		internal static T initStructPointer<T>(Capnproto.StructBuilder.Factory<T> factory, int refOffset, Capnproto.SegmentBuilder segment, Capnproto.StructSize size)
		{
			Capnproto.WireHelpers.AllocateResult allocation = allocate(refOffset, segment, size.total(), Capnproto.WirePointer.STRUCT);
			Capnproto.StructPointer.setFromStructSize(allocation.segment.buffer, allocation.refOffset, size);
			return factory.constructBuilder(allocation.segment, allocation.ptr * Capnproto.Constants.BYTES_PER_WORD, allocation.ptr + size.data, size.data * 64, size.pointers);
		}
		
		internal static T getWritableStructPointer<T>(Capnproto.StructBuilder.Factory<T> factory, int refOffset, Capnproto.SegmentBuilder segment, Capnproto.StructSize size, Capnproto.SegmentReader defaultSegment, int defaultOffset)
		{
			long @ref = segment.get(refOffset);
			int target = Capnproto.WirePointer.target(refOffset, @ref);
			if(Capnproto.WirePointer.isNull(@ref))
			{
				if(defaultSegment == null)
					return initStructPointer(factory, refOffset, segment, size);
				throw new System.Exception("unimplemented");
			}
			Capnproto.WireHelpers.FollowBuilderFarsResult resolved = followBuilderFars(@ref, target, segment);
			short oldDataSize = Capnproto.StructPointer.dataSize(resolved.@ref);
			short oldPointerCount = Capnproto.StructPointer.ptrCount(resolved.@ref);
			int oldPointerSection = resolved.ptr + oldDataSize;
			if(oldDataSize < size.data || oldPointerCount < size.pointers)
			{
				//# The space allocated for this struct is too small. Unlike with readers, we can't just
				//# run with it and do bounds checks at access time, because how would we handle writes?
				//# Instead, we have to copy the struct to a new space now.
				short newDataSize = (short)System.Math.Max(oldDataSize, size.data);
				short newPointerCount = (short)System.Math.Max(oldPointerCount, size.pointers);
				int totalSize = newDataSize + newPointerCount * Capnproto.Constants.WORDS_PER_POINTER;
				//# Don't let allocate() zero out the object just yet.
				zeroPointerAndFars(segment, refOffset);
				Capnproto.WireHelpers.AllocateResult allocation = allocate(refOffset, segment, totalSize, Capnproto.WirePointer.STRUCT);
				Capnproto.StructPointer.set(allocation.segment.buffer, allocation.refOffset, newDataSize, newPointerCount);
				//# Copy data section.
				memcpy(allocation.segment.buffer, allocation.ptr * Capnproto.Constants.BYTES_PER_WORD, resolved.segment.buffer, resolved.ptr * Capnproto.Constants.BYTES_PER_WORD, oldDataSize * Capnproto.Constants.BYTES_PER_WORD);
				//# Copy pointer section.
				int newPointerSection = allocation.ptr + newDataSize;
				for(int ii = 0; ii < oldPointerCount; ++ii)
					transferPointer(allocation.segment, newPointerSection + ii, resolved.segment, oldPointerSection + ii);
				//# Zero out old location.  This has two purposes:
				//# 1) We don't want to leak the original contents of the struct when the message is written
				//#    out as it may contain secrets that the caller intends to remove from the new copy.
				//# 2) Zeros will be deflated by packing, making this dead memory almost-free if it ever
				//#    hits the wire.
				memset(resolved.segment.buffer, resolved.ptr * Capnproto.Constants.BYTES_PER_WORD, unchecked((byte)0), (oldDataSize + oldPointerCount * Capnproto.Constants.WORDS_PER_POINTER) * Capnproto.Constants.BYTES_PER_WORD);
				return factory.constructBuilder(allocation.segment, allocation.ptr * Capnproto.Constants.BYTES_PER_WORD, newPointerSection, newDataSize * Capnproto.Constants.BITS_PER_WORD, newPointerCount);
			}
			return factory.constructBuilder(resolved.segment, resolved.ptr * Capnproto.Constants.BYTES_PER_WORD, oldPointerSection, oldDataSize * Capnproto.Constants.BITS_PER_WORD, oldPointerCount);
		}
		
		internal static T initListPointer<T>(Capnproto.ListBuilder.Factory<T> factory, int refOffset, Capnproto.SegmentBuilder segment, int elementCount, byte elementSize)
		{
			System.Diagnostics.Debug.Assert(elementSize != Capnproto.ElementSize.INLINE_COMPOSITE, "Should have called initStructListPointer instead");
			int dataSize = Capnproto.ElementSize.dataBitsPerElement(elementSize);
			int pointerCount = Capnproto.ElementSize.pointersPerElement(elementSize);
			int step = dataSize + pointerCount * Capnproto.Constants.BITS_PER_POINTER;
			int wordCount = roundBitsUpToWords((long)elementCount * (long)step);
			Capnproto.WireHelpers.AllocateResult allocation = allocate(refOffset, segment, wordCount, Capnproto.WirePointer.LIST);
			Capnproto.ListPointer.set(allocation.segment.buffer, allocation.refOffset, elementSize, elementCount);
			return factory.constructBuilder(allocation.segment, allocation.ptr * Capnproto.Constants.BYTES_PER_WORD, elementCount, step, dataSize, (short)pointerCount);
		}
		
		internal static T initStructListPointer<T>(Capnproto.ListBuilder.Factory<T> factory, int refOffset, Capnproto.SegmentBuilder segment, int elementCount, Capnproto.StructSize elementSize)
		{
			int wordsPerElement = elementSize.total();
			//# Allocate the list, prefixed by a single WirePointer.
			int wordCount = elementCount * wordsPerElement;
			Capnproto.WireHelpers.AllocateResult allocation = allocate(refOffset, segment, Capnproto.Constants.POINTER_SIZE_IN_WORDS + wordCount, Capnproto.WirePointer.LIST);
			//# Initialize the pointer.
			Capnproto.ListPointer.setInlineComposite(allocation.segment.buffer, allocation.refOffset, wordCount);
			Capnproto.WirePointer.setKindAndInlineCompositeListElementCount(allocation.segment.buffer, allocation.ptr, Capnproto.WirePointer.STRUCT, elementCount);
			Capnproto.StructPointer.setFromStructSize(allocation.segment.buffer, allocation.ptr, elementSize);
			return factory.constructBuilder(allocation.segment, (allocation.ptr + 1) * Capnproto.Constants.BYTES_PER_WORD, elementCount, wordsPerElement * Capnproto.Constants.BITS_PER_WORD, elementSize.data * Capnproto.Constants.BITS_PER_WORD, elementSize.pointers);
		}
		
		internal static T getWritableListPointer<T>(Capnproto.ListBuilder.Factory<T> factory, int origRefOffset, Capnproto.SegmentBuilder origSegment, byte elementSize, Capnproto.SegmentReader defaultSegment, int defaultOffset)
		{
			System.Diagnostics.Debug.Assert(elementSize != Capnproto.ElementSize.INLINE_COMPOSITE, "Use getWritableStructListPointer() for struct lists");
			long origRef = origSegment.get(origRefOffset);
			int origRefTarget = Capnproto.WirePointer.target(origRefOffset, origRef);
			if(Capnproto.WirePointer.isNull(origRef))
				throw new System.Exception("unimplemented");
			//# We must verify that the pointer has the right size. Unlike
			//# in getWritableStructListPointer(), we never need to
			//# "upgrade" the data, because this method is called only for
			//# non-struct lists, and there is no allowed upgrade path *to*
			//# a non-struct list, only *from* them.
			Capnproto.WireHelpers.FollowBuilderFarsResult resolved = followBuilderFars(origRef, origRefTarget, origSegment);
			if(Capnproto.WirePointer.kind(resolved.@ref) != Capnproto.WirePointer.LIST)
				throw new Capnproto.DecodeException("Called getList{Field,Element}() but existing pointer is not a list");
			byte oldSize = Capnproto.ListPointer.elementSize(resolved.@ref);
			if(oldSize == Capnproto.ElementSize.INLINE_COMPOSITE)
			{
				//# The existing element size is InlineComposite, which
				//# means that it is at least two words, which makes it
				//# bigger than the expected element size. Since fields can
				//# only grow when upgraded, the existing data must have
				//# been written with a newer version of the protocol. We
				//# therefore never need to upgrade the data in this case,
				//# but we do need to validate that it is a valid upgrade
				//# from what we expected.
				throw new System.Exception("unimplemented");
			}
			else
			{
				int dataSize = Capnproto.ElementSize.dataBitsPerElement(oldSize);
				int pointerCount = Capnproto.ElementSize.pointersPerElement(oldSize);
				if(dataSize < Capnproto.ElementSize.dataBitsPerElement(elementSize))
					throw new Capnproto.DecodeException("Existing list value is incompatible with expected type.");
				if(pointerCount < Capnproto.ElementSize.pointersPerElement(elementSize))
					throw new Capnproto.DecodeException("Existing list value is incompatible with expected type.");
				int step = dataSize + pointerCount * Capnproto.Constants.BITS_PER_POINTER;
				return factory.constructBuilder(resolved.segment, resolved.ptr * Capnproto.Constants.BYTES_PER_WORD, Capnproto.ListPointer.elementCount(resolved.@ref), step, dataSize, (short)pointerCount);
			}
		}
		
		internal static T getWritableStructListPointer<T>(Capnproto.ListBuilder.Factory<T> factory, int origRefOffset, Capnproto.SegmentBuilder origSegment, Capnproto.StructSize elementSize, Capnproto.SegmentReader defaultSegment, int defaultOffset)
		{
			long origRef = origSegment.get(origRefOffset);
			int origRefTarget = Capnproto.WirePointer.target(origRefOffset, origRef);
			if(Capnproto.WirePointer.isNull(origRef))
				throw new System.Exception("unimplemented");
			//# We must verify that the pointer has the right size and potentially upgrade it if not.
			Capnproto.WireHelpers.FollowBuilderFarsResult resolved = followBuilderFars(origRef, origRefTarget, origSegment);
			if(Capnproto.WirePointer.kind(resolved.@ref) != Capnproto.WirePointer.LIST)
				throw new Capnproto.DecodeException("Called getList{Field,Element}() but existing pointer is not a list");
			byte oldSize = Capnproto.ListPointer.elementSize(resolved.@ref);
			if(oldSize == Capnproto.ElementSize.INLINE_COMPOSITE)
			{
				//# Existing list is INLINE_COMPOSITE, but we need to verify that the sizes match.
				long oldTag = resolved.segment.get(resolved.ptr);
				int oldPtr = resolved.ptr + Capnproto.Constants.POINTER_SIZE_IN_WORDS;
				if(Capnproto.WirePointer.kind(oldTag) != Capnproto.WirePointer.STRUCT)
					throw new Capnproto.DecodeException("INLINE_COMPOSITE list with non-STRUCT elements not supported.");
				int oldDataSize = Capnproto.StructPointer.dataSize(oldTag);
				short oldPointerCount = Capnproto.StructPointer.ptrCount(oldTag);
				int oldStep = oldDataSize + oldPointerCount * Capnproto.Constants.POINTER_SIZE_IN_WORDS;
				int elementCount = Capnproto.WirePointer.inlineCompositeListElementCount(oldTag);
				if(oldDataSize >= elementSize.data && oldPointerCount >= elementSize.pointers)
				{
					//# Old size is at least as large as we need. Ship it.
					return factory.constructBuilder(resolved.segment, oldPtr * Capnproto.Constants.BYTES_PER_WORD, elementCount, oldStep * Capnproto.Constants.BITS_PER_WORD, oldDataSize * Capnproto.Constants.BITS_PER_WORD, oldPointerCount);
				}
				//# The structs in this list are smaller than expected, probably written using an older
				//# version of the protocol. We need to make a copy and expand them.
				short newDataSize = (short)System.Math.Max(oldDataSize, elementSize.data);
				short newPointerCount = (short)System.Math.Max(oldPointerCount, elementSize.pointers);
				int newStep = newDataSize + newPointerCount * Capnproto.Constants.WORDS_PER_POINTER;
				int totalSize = newStep * elementCount;
				//# Don't let allocate() zero out the object just yet.
				zeroPointerAndFars(origSegment, origRefOffset);
				Capnproto.WireHelpers.AllocateResult allocation = allocate(origRefOffset, origSegment, totalSize + Capnproto.Constants.POINTER_SIZE_IN_WORDS, Capnproto.WirePointer.LIST);
				Capnproto.ListPointer.setInlineComposite(allocation.segment.buffer, allocation.refOffset, totalSize);
				long tag = allocation.segment.get(allocation.ptr);
				Capnproto.WirePointer.setKindAndInlineCompositeListElementCount(allocation.segment.buffer, allocation.ptr, Capnproto.WirePointer.STRUCT, elementCount);
				Capnproto.StructPointer.set(allocation.segment.buffer, allocation.ptr, newDataSize, newPointerCount);
				int newPtr = allocation.ptr + Capnproto.Constants.POINTER_SIZE_IN_WORDS;
				int src = oldPtr;
				int dst = newPtr;
				for(int ii = 0; ii < elementCount; ++ii)
				{
					//# Copy data section.
					memcpy(allocation.segment.buffer, dst * Capnproto.Constants.BYTES_PER_WORD, resolved.segment.buffer, src * Capnproto.Constants.BYTES_PER_WORD, oldDataSize * Capnproto.Constants.BYTES_PER_WORD);
					//# Copy pointer section.
					int newPointerSection = dst + newDataSize;
					int oldPointerSection = src + oldDataSize;
					for(int jj = 0; jj < oldPointerCount; ++jj)
						transferPointer(allocation.segment, newPointerSection + jj, resolved.segment, oldPointerSection + jj);
					dst += newStep;
					src += oldStep;
				}
				//# Zero out old location. See explanation in getWritableStructPointer().
				//# Make sure to include the tag word.
				memset(resolved.segment.buffer, resolved.ptr * Capnproto.Constants.BYTES_PER_WORD, unchecked((byte)0), (1 + oldStep * elementCount) * Capnproto.Constants.BYTES_PER_WORD);
				return factory.constructBuilder(allocation.segment, newPtr * Capnproto.Constants.BYTES_PER_WORD, elementCount, newStep * Capnproto.Constants.BITS_PER_WORD, newDataSize * Capnproto.Constants.BITS_PER_WORD, newPointerCount);
			}
			{
				//# We're upgrading from a non-struct list.
				int oldDataSize = Capnproto.ElementSize.dataBitsPerElement(oldSize);
				int oldPointerCount = Capnproto.ElementSize.pointersPerElement(oldSize);
				int oldStep = oldDataSize + oldPointerCount * Capnproto.Constants.BITS_PER_POINTER;
				int elementCount = Capnproto.ListPointer.elementCount(origRef);
				if(oldSize == Capnproto.ElementSize.VOID)
				{
					//# Nothing to copy, just allocate a new list.
					return initStructListPointer(factory, origRefOffset, origSegment, elementCount, elementSize);
				}
				
				//# Upgrading to an inline composite list.
				if(oldSize == Capnproto.ElementSize.BIT)
					throw new System.Exception("Found bit list where struct list was expected; upgrading boolean lists to struct is no longer supported.");
				short newDataSize = elementSize.data;
				short newPointerCount = elementSize.pointers;
				if(oldSize == Capnproto.ElementSize.POINTER)
					newPointerCount = (short)System.Math.Max(newPointerCount, (short)1);
				else
				{
					//# Old list contains data elements, so we need at least 1 word of data.
					newDataSize = (short)System.Math.Max(newDataSize, (short)1);
				}
				int newStep = newDataSize + newPointerCount * Capnproto.Constants.WORDS_PER_POINTER;
				int totalWords = elementCount * newStep;
				//# Don't let allocate() zero out the object just yet.
				zeroPointerAndFars(origSegment, origRefOffset);
				Capnproto.WireHelpers.AllocateResult allocation = allocate(origRefOffset, origSegment, totalWords + Capnproto.Constants.POINTER_SIZE_IN_WORDS, Capnproto.WirePointer.LIST);
				Capnproto.ListPointer.setInlineComposite(allocation.segment.buffer, allocation.refOffset, totalWords);
				long tag = allocation.segment.get(allocation.ptr);
				Capnproto.WirePointer.setKindAndInlineCompositeListElementCount(allocation.segment.buffer, allocation.ptr, Capnproto.WirePointer.STRUCT, elementCount);
				Capnproto.StructPointer.set(allocation.segment.buffer, allocation.ptr, newDataSize, newPointerCount);
				int newPtr = allocation.ptr + Capnproto.Constants.POINTER_SIZE_IN_WORDS;
				if(oldSize == Capnproto.ElementSize.POINTER)
				{
					int dst = newPtr + newDataSize;
					int src = resolved.ptr;
					for(int ii = 0; ii < elementCount; ++ii)
					{
						transferPointer(origSegment, dst, resolved.segment, src);
						dst += newStep / Capnproto.Constants.WORDS_PER_POINTER;
						src += 1;
					}
				}
				else
				{
					int dst = newPtr;
					int srcByteOffset = resolved.ptr * Capnproto.Constants.BYTES_PER_WORD;
					int oldByteStep = oldDataSize / Capnproto.Constants.BITS_PER_BYTE;
					for(int ii = 0; ii < elementCount; ++ii)
					{
						memcpy(allocation.segment.buffer, dst * Capnproto.Constants.BYTES_PER_WORD, resolved.segment.buffer, srcByteOffset, oldByteStep);
						srcByteOffset += oldByteStep;
						dst += newStep;
					}
				}
				//# Zero out old location. See explanation in getWritableStructPointer().
				memset(resolved.segment.buffer, resolved.ptr * Capnproto.Constants.BYTES_PER_WORD, unchecked((byte)0), roundBitsUpToBytes(oldStep * elementCount));
				return factory.constructBuilder(allocation.segment, newPtr * Capnproto.Constants.BYTES_PER_WORD, elementCount, newStep * Capnproto.Constants.BITS_PER_WORD, newDataSize * Capnproto.Constants.BITS_PER_WORD, newPointerCount);
			}
		}
		
		//size is in bytes
		internal static Capnproto.Text.Builder initTextPointer(int refOffset, Capnproto.SegmentBuilder segment, int size)
		{
			//# The byte list must include a NUL terminator.
			int byteSize = size + 1;
			//# Allocate the space.
			Capnproto.WireHelpers.AllocateResult allocation = allocate(refOffset, segment, roundBytesUpToWords(byteSize), Capnproto.WirePointer.LIST);
			//# Initialize the pointer.
			Capnproto.ListPointer.set(allocation.segment.buffer, allocation.refOffset, Capnproto.ElementSize.BYTE, byteSize);
			return new Capnproto.Text.Builder(allocation.segment.buffer, allocation.ptr * Capnproto.Constants.BYTES_PER_WORD, size);
		}
		
		internal static Capnproto.Text.Builder setTextPointer(int refOffset, Capnproto.SegmentBuilder segment, Capnproto.Text.Reader value)
		{
			Capnproto.Text.Builder builder = initTextPointer(refOffset, segment, value.size);
			java.nio.ByteBuffer slice = value.buffer.duplicate();
			slice.position(value.offset);
			slice.limit(value.offset + value.size);
			builder.buffer.position(builder.offset);
			builder.buffer.put(slice);
			return builder;
		}
		
		internal static Capnproto.Text.Builder getWritableTextPointer(int refOffset, Capnproto.SegmentBuilder segment, java.nio.ByteBuffer defaultBuffer, int defaultOffset, int defaultSize)
		{
			long @ref = segment.get(refOffset);
			if(Capnproto.WirePointer.isNull(@ref))
			{
				if(defaultBuffer == null)
					return new Capnproto.Text.Builder();
				Capnproto.Text.Builder builder = initTextPointer(refOffset, segment, defaultSize);
				//TODO: Is there a way to do this with bulk methods?
				for(int i = 0; i < builder.size; ++i)
					builder.buffer.put(builder.offset + i, defaultBuffer.get(defaultOffset * 8 + i));
				return builder;
			}
			int refTarget = Capnproto.WirePointer.target(refOffset, @ref);
			Capnproto.WireHelpers.FollowBuilderFarsResult resolved = followBuilderFars(@ref, refTarget, segment);
			if(Capnproto.WirePointer.kind(resolved.@ref) != Capnproto.WirePointer.LIST)
				throw new Capnproto.DecodeException("Called getText{Field,Element} but existing pointer is not a list.");
			if(Capnproto.ListPointer.elementSize(resolved.@ref) != Capnproto.ElementSize.BYTE)
				throw new Capnproto.DecodeException("Called getText{Field,Element} but existing list pointer is not byte-sized.");
			int size = Capnproto.ListPointer.elementCount(resolved.@ref);
			if(size == 0 || resolved.segment.buffer.get(resolved.ptr * Capnproto.Constants.BYTES_PER_WORD + size - 1) != 0)
				throw new Capnproto.DecodeException("Text blob missing NUL terminator.");
			return new Capnproto.Text.Builder(resolved.segment.buffer, resolved.ptr * Capnproto.Constants.BYTES_PER_WORD, size - 1);
		}
		
		//size is in bytes
		internal static Capnproto.Data.Builder initDataPointer(int refOffset, Capnproto.SegmentBuilder segment, int size)
		{
			//# Allocate the space.
			Capnproto.WireHelpers.AllocateResult allocation = allocate(refOffset, segment, roundBytesUpToWords(size), Capnproto.WirePointer.LIST);
			//# Initialize the pointer.
			Capnproto.ListPointer.set(allocation.segment.buffer, allocation.refOffset, Capnproto.ElementSize.BYTE, size);
			return new Capnproto.Data.Builder(allocation.segment.buffer, allocation.ptr * Capnproto.Constants.BYTES_PER_WORD, size);
		}
		
		internal static Capnproto.Data.Builder setDataPointer(int refOffset, Capnproto.SegmentBuilder segment, Capnproto.Data.Reader value)
		{
			Capnproto.Data.Builder builder = initDataPointer(refOffset, segment, value.size);
			//TODO: Is there a way to do this with bulk methods?
			for(int i = 0; i < builder.size; ++i)
				builder.buffer.put(builder.offset + i, value.buffer.get(value.offset + i));
			return builder;
		}
		
		internal static Capnproto.Data.Builder getWritableDataPointer(int refOffset, Capnproto.SegmentBuilder segment, java.nio.ByteBuffer defaultBuffer, int defaultOffset, int defaultSize)
		{
			long @ref = segment.get(refOffset);
			if(Capnproto.WirePointer.isNull(@ref))
			{
				if(defaultBuffer == null)
					return new Capnproto.Data.Builder();
				Capnproto.Data.Builder builder = initDataPointer(refOffset, segment, defaultSize);
				//TODO: Is there a way to do this with bulk methods?
				for(int i = 0; i < builder.size; ++i)
					builder.buffer.put(builder.offset + i, defaultBuffer.get(defaultOffset * 8 + i));
				return builder;
			}
			int refTarget = Capnproto.WirePointer.target(refOffset, @ref);
			Capnproto.WireHelpers.FollowBuilderFarsResult resolved = followBuilderFars(@ref, refTarget, segment);
			if(Capnproto.WirePointer.kind(resolved.@ref) != Capnproto.WirePointer.LIST)
				throw new Capnproto.DecodeException("Called getData{Field,Element} but existing pointer is not a list.");
			if(Capnproto.ListPointer.elementSize(resolved.@ref) != Capnproto.ElementSize.BYTE)
				throw new Capnproto.DecodeException("Called getData{Field,Element} but existing list pointer is not byte-sized.");
			return new Capnproto.Data.Builder(resolved.segment.buffer, resolved.ptr * Capnproto.Constants.BYTES_PER_WORD, Capnproto.ListPointer.elementCount(resolved.@ref));
		}
		
		internal static T readStructPointer<T>(Capnproto.StructReader.Factory<T> factory, Capnproto.SegmentReader segment, int refOffset, Capnproto.SegmentReader defaultSegment, int defaultOffset, int nestingLimit)
		{
			long @ref = segment.get(refOffset);
			if(Capnproto.WirePointer.isNull(@ref))
			{
				if(defaultSegment == null)
					return factory.ConstructReader(Capnproto.SegmentReader.EMPTY, 0, 0, 0, (short)0, (int)(0x7fffffff));
				segment = defaultSegment;
				refOffset = defaultOffset;
				@ref = segment.get(refOffset);
			}
			if(nestingLimit <= 0)
				throw new Capnproto.DecodeException("Message is too deeply nested or contains cycles.");
			int refTarget = Capnproto.WirePointer.target(refOffset, @ref);
			Capnproto.WireHelpers.FollowFarsResult resolved = followFars(@ref, refTarget, segment);
			int dataSizeWords = Capnproto.StructPointer.dataSize(resolved.@ref);
			if(Capnproto.WirePointer.kind(resolved.@ref) != Capnproto.WirePointer.STRUCT)
				throw new Capnproto.DecodeException("Message contains non-struct pointer where struct pointer was expected.");
			resolved.segment.arena.checkReadLimit(Capnproto.StructPointer.wordSize(resolved.@ref));
			return factory.ConstructReader(resolved.segment, resolved.ptr * Capnproto.Constants.BYTES_PER_WORD, (resolved.ptr + dataSizeWords), dataSizeWords * Capnproto.Constants.BITS_PER_WORD, Capnproto.StructPointer.ptrCount(resolved.@ref), nestingLimit - 1);
		}
		
		internal static Capnproto.SegmentBuilder setStructPointer(Capnproto.SegmentBuilder segment, int refOffset, Capnproto.StructReader value)
		{
			short dataSize = (short)roundBitsUpToWords(value.dataSize);
			int totalSize = dataSize + value.pointerCount * Capnproto.Constants.POINTER_SIZE_IN_WORDS;
			Capnproto.WireHelpers.AllocateResult allocation = allocate(refOffset, segment, totalSize, Capnproto.WirePointer.STRUCT);
			Capnproto.StructPointer.set(allocation.segment.buffer, allocation.refOffset, dataSize, value.pointerCount);
			if(value.dataSize == 1)
				throw new System.Exception("single bit case not handled");
			memcpy(allocation.segment.buffer, allocation.ptr * Capnproto.Constants.BYTES_PER_WORD, value.segment.buffer, value.data, value.dataSize / Capnproto.Constants.BITS_PER_BYTE);
			int pointerSection = allocation.ptr + dataSize;
			for(int i = 0; i < value.pointerCount; ++i)
				copyPointer(allocation.segment, pointerSection + i, value.segment, value.pointers + i, value.nestingLimit);
			return allocation.segment;
		}
		
		internal static Capnproto.SegmentBuilder setListPointer(Capnproto.SegmentBuilder segment, int refOffset, Capnproto.ListReader value)
		{
			int totalSize = roundBitsUpToWords(value.elementCount * value.step);
			if(value.step <= Capnproto.Constants.BITS_PER_WORD)
			{
				//# List of non-structs.
				Capnproto.WireHelpers.AllocateResult allocation = allocate(refOffset, segment, totalSize, Capnproto.WirePointer.LIST);
				if(value.structPointerCount == 1)
				{
					//# List of pointers.
					Capnproto.ListPointer.set(allocation.segment.buffer, allocation.refOffset, Capnproto.ElementSize.POINTER, value.elementCount);
					for(int i = 0; i < value.elementCount; ++i)
						copyPointer(allocation.segment, allocation.ptr + i, value.segment, value.ptr / Capnproto.Constants.BYTES_PER_WORD + i, value.nestingLimit);
				}
				else
				{
					//# List of data.
					byte elementSize = Capnproto.ElementSize.VOID;
					switch(value.step)
					{
						case 0:
						{
							elementSize = Capnproto.ElementSize.VOID;
							break;
						}
						case 1:
						{
							elementSize = Capnproto.ElementSize.BIT;
							break;
						}
						case 8:
						{
							elementSize = Capnproto.ElementSize.BYTE;
							break;
						}
						case 16:
						{
							elementSize = Capnproto.ElementSize.TWO_BYTES;
							break;
						}
						case 32:
						{
							elementSize = Capnproto.ElementSize.FOUR_BYTES;
							break;
						}
						case 64:
						{
							elementSize = Capnproto.ElementSize.EIGHT_BYTES;
							break;
						}
						default:
						{
							throw new System.Exception("invalid list step size: " + value.step);
						}
					}
					Capnproto.ListPointer.set(allocation.segment.buffer, allocation.refOffset, elementSize, value.elementCount);
					memcpy(allocation.segment.buffer, allocation.ptr * Capnproto.Constants.BYTES_PER_WORD, value.segment.buffer, value.ptr, totalSize * Capnproto.Constants.BYTES_PER_WORD);
				}
				return allocation.segment;
			}
			{
				//# List of structs.
				Capnproto.WireHelpers.AllocateResult allocation = allocate(refOffset, segment, totalSize + Capnproto.Constants.POINTER_SIZE_IN_WORDS, Capnproto.WirePointer.LIST);
				Capnproto.ListPointer.setInlineComposite(allocation.segment.buffer, allocation.refOffset, totalSize);
				short dataSize = (short)roundBitsUpToWords(value.structDataSize);
				short pointerCount = value.structPointerCount;
				Capnproto.WirePointer.setKindAndInlineCompositeListElementCount(allocation.segment.buffer, allocation.ptr, Capnproto.WirePointer.STRUCT, value.elementCount);
				Capnproto.StructPointer.set(allocation.segment.buffer, allocation.ptr, dataSize, pointerCount);
				int dstOffset = allocation.ptr + Capnproto.Constants.POINTER_SIZE_IN_WORDS;
				int srcOffset = value.ptr / Capnproto.Constants.BYTES_PER_WORD;
				for(int i = 0; i < value.elementCount; ++i)
				{
					memcpy(allocation.segment.buffer, dstOffset * Capnproto.Constants.BYTES_PER_WORD, value.segment.buffer, srcOffset * Capnproto.Constants.BYTES_PER_WORD, value.structDataSize / Capnproto.Constants.BITS_PER_BYTE);
					dstOffset += dataSize;
					srcOffset += dataSize;
					for(int j = 0; j < pointerCount; ++j)
					{
						copyPointer(allocation.segment, dstOffset, value.segment, srcOffset, value.nestingLimit);
						dstOffset += Capnproto.Constants.POINTER_SIZE_IN_WORDS;
						srcOffset += Capnproto.Constants.POINTER_SIZE_IN_WORDS;
					}
				}
				return allocation.segment;
			}
		}
		
		internal static void memset(java.nio.ByteBuffer dstBuffer, int dstByteOffset, byte value, int length)
		{
			//TODO: We can probably do this faster.
			for(int ii = dstByteOffset; ii < dstByteOffset + length; ++ii)
				dstBuffer.put(ii, value);
		}
		
		internal static void memcpy(java.nio.ByteBuffer dstBuffer, int dstByteOffset, java.nio.ByteBuffer srcBuffer, int srcByteOffset, int length)
		{
			java.nio.ByteBuffer dstDup = dstBuffer.duplicate();
			dstDup.position(dstByteOffset);
			dstDup.limit(dstByteOffset + length);
			java.nio.ByteBuffer srcDup = srcBuffer.duplicate();
			srcDup.position(srcByteOffset);
			srcDup.limit(srcByteOffset + length);
			dstDup.put(srcDup);
		}
		
		internal static Capnproto.SegmentBuilder copyPointer(Capnproto.SegmentBuilder dstSegment, int dstOffset, Capnproto.SegmentReader srcSegment, int srcOffset, int nestingLimit)
		{
			//Deep-copy the object pointed to by src into dst. It turns out we can't reuse
			//readStructPointer(), etc. because they do type checking whereas here we want to accept any
			//valid pointer.
			long srcRef = srcSegment.get(srcOffset);
			if(Capnproto.WirePointer.isNull(srcRef))
			{
				dstSegment.buffer.putLong(dstOffset * 8, 0L);
				return dstSegment;
			}
			int srcTarget = Capnproto.WirePointer.target(srcOffset, srcRef);
			Capnproto.WireHelpers.FollowFarsResult resolved = followFars(srcRef, srcTarget, srcSegment);
			switch(Capnproto.WirePointer.kind(resolved.@ref))
			{
				case Capnproto.WirePointer.STRUCT:
				{
					if(nestingLimit <= 0)
						throw new Capnproto.DecodeException("Message is too deeply nested or contains cycles. See Capnproto.ReaderOptions.");
					resolved.segment.arena.checkReadLimit(Capnproto.StructPointer.wordSize(resolved.@ref));
					return setStructPointer(dstSegment, dstOffset, new Capnproto.StructReader(resolved.segment, resolved.ptr * Capnproto.Constants.BYTES_PER_WORD, resolved.ptr + Capnproto.StructPointer.dataSize(resolved.@ref), Capnproto.StructPointer.dataSize(resolved.@ref) * Capnproto.Constants.BITS_PER_WORD, Capnproto.StructPointer.ptrCount(resolved.@ref), nestingLimit - 1));
				}
				case Capnproto.WirePointer.LIST:
				{
					byte elementSize = Capnproto.ListPointer.elementSize(resolved.@ref);
					if(nestingLimit <= 0)
						throw new Capnproto.DecodeException("Message is too deeply nested or contains cycles. See Capnproto.ReaderOptions.");
					if(elementSize == Capnproto.ElementSize.INLINE_COMPOSITE)
					{
						int wordCount = Capnproto.ListPointer.inlineCompositeWordCount(resolved.@ref);
						long tag = resolved.segment.get(resolved.ptr);
						int ptr = resolved.ptr + 1;
						resolved.segment.arena.checkReadLimit(wordCount + 1);
						if(Capnproto.WirePointer.kind(tag) != Capnproto.WirePointer.STRUCT)
							throw new Capnproto.DecodeException("INLINE_COMPOSITE lists of non-STRUCT type are not supported.");
						int elementCount = Capnproto.WirePointer.inlineCompositeListElementCount(tag);
						int wordsPerElement = Capnproto.StructPointer.wordSize(tag);
						if((long)wordsPerElement * elementCount > wordCount)
							throw new Capnproto.DecodeException("INLINE_COMPOSITE list's elements overrun its word count.");
						if(wordsPerElement == 0)
						{
							//Watch out for lists of zero-sized structs, which can claim to be arbitrarily
							//large without having sent actual data.
							resolved.segment.arena.checkReadLimit(elementCount);
						}
						return setListPointer(dstSegment, dstOffset, new Capnproto.ListReader(resolved.segment, ptr * Capnproto.Constants.BYTES_PER_WORD, elementCount, wordsPerElement * Capnproto.Constants.BITS_PER_WORD, Capnproto.StructPointer.dataSize(tag) * Capnproto.Constants.BITS_PER_WORD, Capnproto.StructPointer.ptrCount(tag), nestingLimit - 1));
					}
					{
						int dataSize = Capnproto.ElementSize.dataBitsPerElement(elementSize);
						short pointerCount = Capnproto.ElementSize.pointersPerElement(elementSize);
						int step = dataSize + pointerCount * Capnproto.Constants.BITS_PER_POINTER;
						int elementCount = Capnproto.ListPointer.elementCount(resolved.@ref);
						int wordCount = roundBitsUpToWords((long)elementCount * step);
						resolved.segment.arena.checkReadLimit(wordCount);
						if(elementSize == Capnproto.ElementSize.VOID)
						{
							//Watch out for lists of void, which can claim to be arbitrarily large without
							//having sent actual data.
							resolved.segment.arena.checkReadLimit(elementCount);
						}
						return setListPointer(dstSegment, dstOffset, new Capnproto.ListReader(resolved.segment, resolved.ptr * Capnproto.Constants.BYTES_PER_WORD, elementCount, step, dataSize, pointerCount, nestingLimit - 1));
					}
				}
				case Capnproto.WirePointer.FAR:
				{
					throw new Capnproto.DecodeException("Unexpected FAR pointer.");
				}
				case Capnproto.WirePointer.OTHER:
				{
					throw new System.Exception("copyPointer is unimplemented for OTHER pointers");
				}
			}
			throw new System.Exception("unreachable");
		}
		
		internal static T readListPointer<T>(Capnproto.ListReader.Factory<T> factory, Capnproto.SegmentReader segment, int refOffset, Capnproto.SegmentReader defaultSegment, int defaultOffset, byte expectedElementSize, int nestingLimit)
		{
			long @ref = segment.get(refOffset);
			if(Capnproto.WirePointer.isNull(@ref))
			{
				if(defaultSegment == null)
					return factory.ConstructReader(Capnproto.SegmentReader.EMPTY, 0, 0, 0, 0, (short)0, unchecked((int)(0x7fffffff)));
				segment = defaultSegment;
				refOffset = defaultOffset;
				@ref = segment.get(refOffset);
			}
			if(nestingLimit <= 0)
				throw new System.Exception("nesting limit exceeded");
			int refTarget = Capnproto.WirePointer.target(refOffset, @ref);
			Capnproto.WireHelpers.FollowFarsResult resolved = followFars(@ref, refTarget, segment);
			byte elementSize = Capnproto.ListPointer.elementSize(resolved.@ref);
			switch(elementSize)
			{
				case Capnproto.ElementSize.INLINE_COMPOSITE:
				{
					int wordCount = Capnproto.ListPointer.inlineCompositeWordCount(resolved.@ref);
					long tag = resolved.segment.get(resolved.ptr);
					int ptr = resolved.ptr + 1;
					resolved.segment.arena.checkReadLimit(wordCount + 1);
					int size = Capnproto.WirePointer.inlineCompositeListElementCount(tag);
					int wordsPerElement = Capnproto.StructPointer.wordSize(tag);
					if((long)size * wordsPerElement > wordCount)
						throw new Capnproto.DecodeException("INLINE_COMPOSITE list's elements overrun its word count.");
					if(wordsPerElement == 0)
					{
						//Watch out for lists of zero-sized structs, which can claim to be arbitrarily
						//large without having sent actual data.
						resolved.segment.arena.checkReadLimit(size);
					}
					//TODO: Check whether the size is compatible.
					return factory.ConstructReader(resolved.segment, ptr * Capnproto.Constants.BYTES_PER_WORD, size, wordsPerElement * Capnproto.Constants.BITS_PER_WORD, Capnproto.StructPointer.dataSize(tag) * Capnproto.Constants.BITS_PER_WORD, Capnproto.StructPointer.ptrCount(tag), nestingLimit - 1);
				}
				default:
				{
					//# This is a primitive or pointer list, but all such
					//# lists can also be interpreted as struct lists. We
					//# need to compute the data size and pointer count for
					//# such structs.
					int dataSize = Capnproto.ElementSize.dataBitsPerElement(Capnproto.ListPointer.elementSize(resolved.@ref));
					int pointerCount = Capnproto.ElementSize.pointersPerElement(Capnproto.ListPointer.elementSize(resolved.@ref));
					int elementCount = Capnproto.ListPointer.elementCount(resolved.@ref);
					int step = dataSize + pointerCount * Capnproto.Constants.BITS_PER_POINTER;
					resolved.segment.arena.checkReadLimit(roundBitsUpToWords(elementCount * step));
					if(elementSize == Capnproto.ElementSize.VOID)
					{
						//Watch out for lists of void, which can claim to be arbitrarily large without
						//having sent actual data.
						resolved.segment.arena.checkReadLimit(elementCount);
					}
					//# Verify that the elements are at least as large as
					//# the expected type. Note that if we expected
					//# InlineComposite, the expected sizes here will be
					//# zero, because bounds checking will be performed at
					//# field access time. So this check here is for the
					//# case where we expected a list of some primitive or
					//# pointer type.
					int expectedDataBitsPerElement = Capnproto.ElementSize.dataBitsPerElement(expectedElementSize);
					int expectedPointersPerElement = Capnproto.ElementSize.pointersPerElement(expectedElementSize);
					if(expectedDataBitsPerElement > dataSize)
						throw new Capnproto.DecodeException("Message contains list with incompatible element type.");
					if(expectedPointersPerElement > pointerCount)
						throw new Capnproto.DecodeException("Message contains list with incompatible element type.");
					return factory.ConstructReader(resolved.segment, resolved.ptr * Capnproto.Constants.BYTES_PER_WORD, Capnproto.ListPointer.elementCount(resolved.@ref), step, dataSize, (short)pointerCount, nestingLimit - 1);
				}
			}
		}
		
		internal static Capnproto.Text.Reader readTextPointer(Capnproto.SegmentReader segment, int refOffset, java.nio.ByteBuffer defaultBuffer, int defaultOffset, int defaultSize)
		{
			long @ref = segment.get(refOffset);
			if(Capnproto.WirePointer.isNull(@ref))
			{
				if(defaultBuffer == null)
					return new Capnproto.Text.Reader();
				return new Capnproto.Text.Reader(defaultBuffer, defaultOffset, defaultSize);
			}
			int refTarget = Capnproto.WirePointer.target(refOffset, @ref);
			Capnproto.WireHelpers.FollowFarsResult resolved = followFars(@ref, refTarget, segment);
			int size = Capnproto.ListPointer.elementCount(resolved.@ref);
			if(Capnproto.WirePointer.kind(resolved.@ref) != Capnproto.WirePointer.LIST)
				throw new Capnproto.DecodeException("Message contains non-list pointer where text was expected.");
			if(Capnproto.ListPointer.elementSize(resolved.@ref) != Capnproto.ElementSize.BYTE)
				throw new Capnproto.DecodeException("Message contains list pointer of non-bytes where text was expected.");
			resolved.segment.arena.checkReadLimit(roundBytesUpToWords(size));
			if(size == 0 || resolved.segment.buffer.get(8 * resolved.ptr + size - 1) != 0)
				throw new Capnproto.DecodeException("Message contains text that is not NUL-terminated.");
			return new Capnproto.Text.Reader(resolved.segment.buffer, resolved.ptr, size - 1);
		}
		
		internal static Capnproto.Data.Reader readDataPointer(Capnproto.SegmentReader segment, int refOffset, java.nio.ByteBuffer defaultBuffer, int defaultOffset, int defaultSize)
		{
			long @ref = segment.get(refOffset);
			if(Capnproto.WirePointer.isNull(@ref))
			{
				if(defaultBuffer == null)
					return new Capnproto.Data.Reader();
				return new Capnproto.Data.Reader(defaultBuffer, defaultOffset, defaultSize);
			}
			int refTarget = Capnproto.WirePointer.target(refOffset, @ref);
			Capnproto.WireHelpers.FollowFarsResult resolved = followFars(@ref, refTarget, segment);
			int size = Capnproto.ListPointer.elementCount(resolved.@ref);
			if(Capnproto.WirePointer.kind(resolved.@ref) != Capnproto.WirePointer.LIST)
				throw new Capnproto.DecodeException("Message contains non-list pointer where data was expected.");
			if(Capnproto.ListPointer.elementSize(resolved.@ref) != Capnproto.ElementSize.BYTE)
				throw new Capnproto.DecodeException("Message contains list pointer of non-bytes where data was expected.");
			resolved.segment.arena.checkReadLimit(roundBytesUpToWords(size));
			return new Capnproto.Data.Reader(resolved.segment.buffer, resolved.ptr, size);
		}
	}
}
