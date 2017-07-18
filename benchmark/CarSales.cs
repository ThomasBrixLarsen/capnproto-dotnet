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
	public class CarSales : TestCase<CarSalesSchema.ParkingLot.Factory, CarSalesSchema.ParkingLot.Builder, CarSalesSchema.ParkingLot.Reader, CarSalesSchema.TotalValue.Factory, CarSalesSchema.TotalValue.Builder, CarSalesSchema.TotalValue.Reader, ulong>
	{
		internal static ulong carValue(CarSalesSchema.Car.Reader car)
		{
			ulong result = 0;
			result += car.GetSeats() * 200UL;
			result += car.GetDoors() * 350UL;
			foreach(CarSalesSchema.Wheel.Reader wheel in car.GetWheels())
			{
				result += (ulong)wheel.GetDiameter() * (ulong)wheel.GetDiameter();
				result += wheel.GetSnowTires()? 100UL : 0UL;
			}
			result += (ulong)car.GetLength() * (ulong)car.GetWidth() * (ulong)car.GetHeight() / 50;
			CarSalesSchema.Engine.Reader engine = car.GetEngine();
			result += (ulong)engine.GetHorsepower() * 40UL;
			if(engine.GetUsesElectric())
				result += engine.GetUsesGas()? 5000UL : 3000UL;
			result += car.GetHasPowerWindows()? 100UL : 0UL;
			result += car.GetHasPowerSteering()? 200UL : 0UL;
			result += car.GetHasCruiseControl()? 400UL : 0UL;
			result += car.GetHasNavSystem()? 2000UL : 0UL;
			result += (ulong)car.GetCupHolders() * 25UL;
			return result;
		}
		
		internal static Text.Reader[] MAKES = new Text.Reader[] { new Text.Reader("Toyota"), new Text.Reader("GM"), new Text.Reader("Ford"), new Text.Reader("Honda"), new Text.Reader("Tesla") };
		
		internal static Text.Reader[] MODELS = new Text.Reader[] { new Text.Reader("Camry"), new Text.Reader("Prius"), new Text.Reader("Volt"), new Text.Reader("Accord"), new Text.Reader("Leaf"), new Text.Reader("Model S") };
		
		internal static readonly CarSalesSchema.Color[] colors = (CarSalesSchema.Color[])Enum.GetValues(typeof(CarSalesSchema.Color));
		
		internal static void randomCar(Common.FastRand rng, CarSalesSchema.Car.Builder car)
		{
			car.SetMake(MAKES[rng.nextLessThan(MAKES.Length)]);
			car.SetModel(MODELS[rng.nextLessThan(MODELS.Length)]);
			car.SetColor(colors[rng.nextLessThan((short)CarSalesSchema.Color.Silver + 1)]);
			car.SetSeats(unchecked((byte)(2 + rng.nextLessThan(6))));
			car.SetDoors(unchecked((byte)(2 + rng.nextLessThan(3))));
			foreach(CarSalesSchema.Wheel.Builder wheel in car.InitWheels(4))
			{
				wheel.SetDiameter((ushort)(25 + rng.nextLessThan(15)));
				wheel.SetAirPressure((float)(30.0 + rng.nextDouble(20.0)));
				wheel.SetSnowTires(rng.nextLessThan(16) == 0);
			}
			car.SetLength((ushort)(170 + rng.nextLessThan(150)));
			car.SetWidth((ushort)(48 + rng.nextLessThan(36)));
			car.SetHeight((ushort)(54 + rng.nextLessThan(48)));
			car.SetWeight((uint)car.GetLength() * (uint)car.GetWidth() * (uint)car.GetHeight() / 200);
			CarSalesSchema.Engine.Builder engine = car.InitEngine();
			engine.SetHorsepower((ushort)(100 * rng.nextLessThan(400)));
			engine.SetCylinders(unchecked((byte)(4 + 2 * rng.nextLessThan(3))));
			engine.SetCc((uint)(800 + rng.nextLessThan(10000)));
			engine.SetUsesGas(true);
			engine.SetUsesElectric(rng.nextLessThan(2) == 1);
			car.SetFuelCapacity((float)(10.0 + rng.nextDouble(30.0)));
			car.SetFuelLevel((float)(rng.nextDouble(car.GetFuelCapacity())));
			car.SetHasPowerWindows(rng.nextLessThan(2) == 1);
			car.SetHasPowerSteering(rng.nextLessThan(2) == 1);
			car.SetHasCruiseControl(rng.nextLessThan(2) == 1);
			car.SetCupHolders(unchecked((byte)rng.nextLessThan(12)));
			car.SetHasNavSystem(rng.nextLessThan(2) == 1);
		}
		
		public sealed override ulong setupRequest(Common.FastRand rng, CarSalesSchema.ParkingLot.Builder request)
		{
			ulong result = 0;
			StructList.Builder<CarSalesSchema.Car.Builder> cars = request.InitCars(rng.nextLessThan(200));
			for(int i = 0; i < cars.Length; ++i)
			{
				CarSalesSchema.Car.Builder car = cars.Get(i);
				randomCar(rng, car);
				result += carValue(car.AsReader());
			}
			return result;
		}
		
		public sealed override void handleRequest(CarSalesSchema.ParkingLot.Reader request, CarSalesSchema.TotalValue.Builder response)
		{
			ulong result = 0;
			foreach(CarSalesSchema.Car.Reader car in request.GetCars())
				result += carValue(car);
			response.SetAmount(result);
		}
		
		public sealed override bool checkResponse(CarSalesSchema.TotalValue.Reader response, ulong expected)
		{
			return response.GetAmount() == expected;
		}
		
		public static void Main(string[] args)
		{
			CarSales testCase = new CarSales();
			testCase.execute(args, CarSalesSchema.ParkingLot.SingleFactory, CarSalesSchema.TotalValue.SingleFactory);
		}
	}
}
