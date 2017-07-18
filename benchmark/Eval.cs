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

using System;

using Capnproto;

namespace Capnproto.Benchmark
{
	public class Eval : TestCase<EvalSchema.Expression.Factory, EvalSchema.Expression.Builder, EvalSchema.Expression.Reader, EvalSchema.EvaluationResult.Factory, EvalSchema.EvaluationResult.Builder, EvalSchema.EvaluationResult.Reader, int>
	{
		internal static readonly EvalSchema.Operation[] operations = (EvalSchema.Operation[])Enum.GetValues(typeof(EvalSchema.Operation));
		
		public static int makeExpression(Common.FastRand rng, EvalSchema.Expression.Builder exp, int depth)
		{
			exp.SetOp(operations[rng.nextLessThan((short)EvalSchema.Operation.Modulus + 1)]);
			int left = 0;
			if(rng.nextLessThan(8) < depth)
			{
				int tmp = rng.nextLessThan(128) + 1;
				exp.GetLeft().SetValue(tmp);
				left = tmp;
			}
			else
				left = makeExpression(rng, exp.GetLeft().InitExpression(), depth + 1);
			int right = 0;
			if(rng.nextLessThan(8) < depth)
			{
				int tmp = rng.nextLessThan(128) + 1;
				exp.GetRight().SetValue(tmp);
				right = tmp;
			}
			else
				right = makeExpression(rng, exp.GetRight().InitExpression(), depth + 1);
			
			switch((short)exp.GetOp())
			{
				case 0:
					return left + right;
				case 1:
					return left - right;
				case 2:
					return left * right;
				case 3:
					return Common.div(left, right);
				case 4:
					return Common.modulus(left, right);
				default:
					throw new System.Exception("impossible");
			}
		}
		
		public static int evaluateExpression(EvalSchema.Expression.Reader exp)
		{
			int left = 0;
			int right = 0;
			switch((short)exp.GetLeft().Which)
			{
				case 0:
				{
					left = exp.GetLeft().GetValue();
					break;
				}
				case 1:
				{
					left = evaluateExpression(exp.GetLeft().GetExpression());
					break;
				}
			}
			switch((short)exp.GetRight().Which)
			{
				case 0:
				{
					right = exp.GetRight().GetValue();
					break;
				}
				case 1:
				{
					right = evaluateExpression(exp.GetRight().GetExpression());
					break;
				}
			}
			switch((short)exp.GetOp())
			{
				case 0:
					return left + right;
				case 1:
					return left - right;
				case 2:
					return left * right;
				case 3:
					return Common.div(left, right);
				case 4:
					return Common.modulus(left, right);
				default:
					throw new System.Exception("impossible");
			}
		}
		
		public sealed override int setupRequest(Common.FastRand rng, EvalSchema.Expression.Builder request)
		{
			return makeExpression(rng, request, 0);
		}
		
		public sealed override void handleRequest(EvalSchema.Expression.Reader request, EvalSchema.EvaluationResult.Builder response)
		{
			response.SetValue(evaluateExpression(request));
		}
		
		public sealed override bool checkResponse(EvalSchema.EvaluationResult.Reader response, int expected)
		{
			return response.GetValue() == expected;
		}
		
		public static void Main(string[] args)
		{
			Eval testCase = new Eval();
			testCase.execute(args, EvalSchema.Expression.SingleFactory, EvalSchema.EvaluationResult.SingleFactory);
		}
	}
}
