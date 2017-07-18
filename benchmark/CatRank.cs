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

using Capnproto;

namespace Capnproto.Benchmark
{
	public class CatRank : TestCase<CatRankSchema.SearchResultList.Factory, CatRankSchema.SearchResultList.Builder, CatRankSchema.SearchResultList.Reader, CatRankSchema.SearchResultList.Factory, CatRankSchema.SearchResultList.Builder, CatRankSchema.SearchResultList.Reader, int>
	{
		internal class ScoredResult : System.IComparable<CatRank.ScoredResult>
		{
			public double score;
			
			public CatRankSchema.SearchResult.Reader result;
			
			public ScoredResult(double score, CatRankSchema.SearchResult.Reader result)
			{
				this.score = score;
				this.result = result;
			}
			
			//Decreasing order.
			public virtual int CompareTo(CatRank.ScoredResult other)
			{
				if(this.score < other.score)
					return 1;
				if(this.score > other.score)
					return -1;
				return 0;
			}
		}
		
		internal static readonly Text.Reader URL_PREFIX = new Text.Reader("http://example.com");
		
		public override int setupRequest(Common.FastRand rng, CatRankSchema.SearchResultList.Builder request)
		{
			int count = rng.nextLessThan(1000);
			int goodCount = 0;
			StructList.Builder<CatRankSchema.SearchResult.Builder> list = request.InitResults(count);
			for(int i = 0; i < list.Length; ++i)
			{
				CatRankSchema.SearchResult.Builder result = list.Get(i);
				result.SetScore(1000.0 - (double)i);
				int urlSize = rng.nextLessThan(100);
				int urlPrefixLength = URL_PREFIX.Length;
				Text.Builder url = result.InitUrl(urlSize + urlPrefixLength);
				java.nio.ByteBuffer bytes = url.asByteBuffer();
				bytes.put(URL_PREFIX.asByteBuffer());
				for(int j = 0; j < urlSize; j++)
					bytes.put(unchecked((byte)(97 + rng.nextLessThan(26))));
				bool isCat = rng.nextLessThan(8) == 0;
				bool isDog = rng.nextLessThan(8) == 0;
				if(isCat && !isDog)
					goodCount += 1;
				System.Text.StringBuilder snippet = new System.Text.StringBuilder(" ");
				int prefix = rng.nextLessThan(20);
				for(int j = 0; j < prefix; ++j)
					snippet.Append(Common.WORDS[rng.nextLessThan(Common.WORDS.Length)]);
				if(isCat)
					snippet.Append("cat ");
				if(isDog)
					snippet.Append("dog ");
				int suffix = rng.nextLessThan(20);
				for(int j = 0; j < suffix; ++j)
					snippet.Append(Common.WORDS[rng.nextLessThan(Common.WORDS.Length)]);
				result.SetSnippet(snippet.ToString());
			}
			return goodCount;
		}
		
		public override void handleRequest(CatRankSchema.SearchResultList.Reader request, CatRankSchema.SearchResultList.Builder response)
		{
			System.Collections.Generic.List<CatRank.ScoredResult> scoredResults = new System.Collections.Generic.List<CatRank.ScoredResult>();
			foreach(CatRankSchema.SearchResult.Reader result in request.GetResults())
			{
				double score = result.GetScore();
				if(result.GetSnippet().ToString().Contains(" cat "))
					score *= 10000.0;
				if(result.GetSnippet().ToString().Contains(" dog "))
					score /= 10000.0;
				scoredResults.Add(new CatRank.ScoredResult(score, result));
			}
			scoredResults.Sort();
			StructList.Builder<CatRankSchema.SearchResult.Builder> list = response.InitResults(scoredResults.Count);
			for(int i = 0; i < list.Length; ++i)
			{
				CatRankSchema.SearchResult.Builder item = list.Get(i);
				CatRank.ScoredResult result = scoredResults[i];
				item.SetScore(result.score);
				item.SetUrl(result.result.GetUrl());
				item.SetSnippet(result.result.GetSnippet());
			}
		}
		
		public override bool checkResponse(CatRankSchema.SearchResultList.Reader response, int expectedGoodCount)
		{
			int goodCount = 0;
			foreach(CatRankSchema.SearchResult.Reader result in response.GetResults())
			{
				if(result.GetScore() > 1001.0)
					goodCount += 1;
				else
					break;
			}
			return goodCount == expectedGoodCount;
		}
		
		public static void Main(string[] args)
		{
			CatRank testCase = new CatRank();
			testCase.execute(args, CatRankSchema.SearchResultList.SingleFactory, CatRankSchema.SearchResultList.SingleFactory);
		}
	}
}
