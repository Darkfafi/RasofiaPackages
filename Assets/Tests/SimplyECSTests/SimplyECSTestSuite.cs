using NUnit.Framework;
using RasofiaGames.SimplyECS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Tests
{
	public class TestSuite
	{
		private const int TestAmount = 1000;
		private const int ListsAmount = 20;

		// A Test behaves as an ordinary method
		[Test]
		public void TestNormal()
		{
			Stopwatch stopWatch = new Stopwatch();
			stopWatch.Start();

			Canvas gameObject = new GameObject("<Track Equalizer>").AddComponent<Canvas>();

			List<NormalUnit> normalUnits = new List<NormalUnit>();
			for(int i = 0; i < TestAmount; i++)
			{
				normalUnits.Add(new NormalUnit(100, i > TestAmount / 2));
			}

			string a = "";

			for(int i = 0; i < ListsAmount; i++)
			{
				NormalUnit[] units = GetAll(normalUnits, x => i % 2 == 0 ? x.HealthNormal != null : x.DamageNormal != null);

				for(int j = 0; j < units.Length; j++)
				{
					a += i;
				}
			}

			stopWatch.Stop();
			UnityEngine.Debug.Log(stopWatch.ElapsedMilliseconds.ToString("#,##0.00") + " < Normal");
			UnityEngine.Debug.Log(a.Length);
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
				entityUnits.Add(new EntityUnit(100, i > TestAmount / 2));
			}

			// Call Flush when you need the entities within the same frame, else the entities will be added to the system the next frame
			EntityTracker.Instance.Flush();

			string cc = "";

			for(int i = 0; i < ListsAmount; i++)
			{
				if(i % 2 == 0)
				{
					HealthFilterData[] units = EntityFilter<HealthFilterData>.Create(null, null).GetAllData();

					for(int j = 0; j < units.Length; j++)
					{
						cc += i;
					}
				}
				else
				{
					DamageFilterData[] units = EntityFilter<DamageFilterData>.Create(null, null).GetAllData();

					for(int j = 0; j < units.Length; j++)
					{
						cc += i;
					}
				}
			}

			stopWatch.Stop();
			UnityEngine.Debug.Log(stopWatch.ElapsedMilliseconds.ToString("#,##0.00") + " < ECS");
			UnityEngine.Debug.Log(cc.Length);

		}

		private struct HealthFilterData : IFilterData
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

		private struct DamageFilterData : IFilterData
		{
			public DamageComp DamageComp;

			public void InitData(Entity entity)
			{
				DamageComp = entity.GetEntityComponent<DamageComp>();
			}

			public bool IsValidEntity(Entity entity)
			{
				return entity.HasEntityComponent<DamageComp>();
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

			public DamageNormal DamageNormal
			{
				get; private set;
			}

			public NormalUnit(int hp, bool damage)
			{
				HealthNormal = new HealthNormal(hp);
				DamageNormal = damage ? new DamageNormal(50) : null;
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

		public class DamageNormal
		{
			public int Damage = 100;

			public DamageNormal(int damage)
			{
				Damage = damage;
			}
		}

		public class EntityUnit : Entity
		{
			public EntityUnit(int hp, bool damage)
			{
				AddEntityComponent<HealthComp>().HP = hp;
				if(damage)
				{
					AddEntityComponent<DamageComp>().Damage = 50;
				}
			}
		}

		public class HealthComp : EntityComponent
		{
			public int HP = 100;
		}

		public class DamageComp : EntityComponent
		{
			public int Damage = 100;
		}
	}
}
