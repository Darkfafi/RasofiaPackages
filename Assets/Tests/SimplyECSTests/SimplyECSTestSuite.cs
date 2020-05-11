using NUnit.Framework;
using RasofiaGames.SimplyECS;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Tests
{
	public class TestSuite
	{
		private const int TestAmount = 30000;

		// A Test behaves as an ordinary method
		[Test]
		public void TestNormal()
		{
			Stopwatch stopWatch = new Stopwatch();
			stopWatch.Start();
			List<NormalUnit> normalUnits = new List<NormalUnit>();
			for(int i = 0; i < TestAmount; i++)
			{
				normalUnits.Add(new NormalUnit(100));
			}

			NormalUnit[] units = GetAll(normalUnits, x => x.HealthNormal != null);
			NormalUnit[] units2 = GetAll(normalUnits, x => x.HealthNormal != null);
			NormalUnit[] units3 = GetAll(normalUnits, x => x.HealthNormal != null);

			string a = "";
			for(int i = 0; i < units.Length; i++)
			{
				a += i;
			}

			for(int i = 0; i < units2.Length; i++)
			{
				a += i;
			}

			for(int i = 0; i < units3.Length; i++)
			{
				a += i;
			}

			stopWatch.Stop();
			UnityEngine.Debug.Log(stopWatch.ElapsedMilliseconds.ToString("#,##0.00") + " < Normal");
		}

		// A Test behaves as an ordinary method
		[Test]
        public void TestECS()
		{
			Stopwatch stopWatch = new Stopwatch();
			stopWatch.Start();

			List<EntityUnit> entityUnits = new List<EntityUnit>();
			for(int i = 0; i < TestAmount; i++)
			{
				entityUnits.Add(new EntityUnit(100));
			}

			IEntityFilter<TestFilterData> filter = EntityFilter<TestFilterData>.Create(null, null);

			TestFilterData[] a = filter.GetAllData();
			TestFilterData[] b = filter.GetAllData();
			TestFilterData[] c = filter.GetAllData();

			string cc = "";
			for(int i = 0; i < a.Length; i++)
			{
				cc += i;
			}

			for(int i = 0; i < b.Length; i++)
			{
				cc += i;
			}

			for(int i = 0; i < c.Length; i++)
			{
				cc += i;
			}

			stopWatch.Stop();
			UnityEngine.Debug.Log(stopWatch.ElapsedMilliseconds.ToString("#,##0.00") + " < ECS");

		}

		private struct TestFilterData : IFilterData
		{
			public HealthComp HealthComp;

			public void InitData(Entity entity)
			{
				HealthComp = entity.GetEntityComponent<HealthComp>();
			}

			public bool IsValidEntity(Entity entity)
			{
				return entity.HasEntityComponent<HealthComp>();
			}
		}

		private T[] GetAll<T>(List<T> l, Predicate<T> predicate)
		{
			List<T> rv = new List<T>();
			for(int i = 0, c = l.Count; i < c; i++)
			{
				if(predicate(l[i]))
				{
					rv.Add(l[i]);
				}
			}
			return rv.ToArray();
		}

		public class NormalUnit
		{
			public HealthNormal HealthNormal
			{
				get; private set;
			}

			public NormalUnit(int hp)
			{
				HealthNormal = new HealthNormal(hp);
			}
		}

		public class HealthNormal
		{
			public int HP = 100;

			public HealthNormal(int hp)
			{
				HP = hp;
			}
		}

		public class EntityUnit : Entity
		{
			public EntityUnit(int hp)
			{
				AddEntityComponent<HealthComp>().HP = hp;
			}
		}

		public class HealthComp : EntityComponent
		{
			public int HP = 100;
		}
    }
}
