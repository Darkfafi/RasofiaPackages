using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// THIS CLASS IS GENERATED!
/// </summary>
namespace RasofiaGames.SaveLoadSystem.Internal
{
	public class StorageObjectFactory : IStorageObjectFactory
	{
		public const ulong ID_MILESTONE = 0;

		private readonly Dictionary<Type, ulong> _typeToIdMap = new Dictionary<Type, ulong>()
		{
		};

		public ISaveable LoadSaveableObject(ulong id, IStorageLoader loader)
		{
			switch(id)
			{
				default:
					throw new Exception($"No ISaveable found with id {id}. It can not be constructed!");
			}
		}

		public Type GetTypeForId(ulong id)
		{
			return _typeToIdMap.FirstOrDefault(x => x.Value == id).Key;
		}

		public ulong GetIdForSaveable<T>() where T : ISaveable
		{
			return GetIdForSaveable(typeof(T));
		}

		public ulong GetIdForSaveable(Type type)
		{
			if(!_typeToIdMap.TryGetValue(type, out ulong id))
			{
				throw new Exception($"No ID found with for ISaveable of type {type.FullName}.");
			}

			return id;
		}
	}
}
