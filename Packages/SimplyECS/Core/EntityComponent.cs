namespace RasofiaGames.SimplyECS
{
	public abstract class EntityComponent
	{
		public Entity Parent
		{
			get; private set;
		}

		internal void Initialize(Entity parent)
		{
			Parent = parent;
			Init();
		}

		internal void Deinitialize()
		{
			DeInit();
			Parent = null;
		}

		protected virtual void Init()
		{

		}

		protected virtual void DeInit()
		{

		}
	}
}