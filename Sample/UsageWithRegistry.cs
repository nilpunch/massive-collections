namespace Massive.Collections.Samples
{
	public class FootballService : IMassive
	{
		public MassiveVariable<int> Score { get; } = new MassiveVariable<int>();
		public MassiveVariable<bool> IsMatchOver { get; } = new MassiveVariable<bool>();
		public MassiveList<Entity> WinningPlayers { get; } = new MassiveList<Entity>();

		public int CanRollbackFrames => Score.CanRollbackFrames;

		public void SaveFrame()
		{
			Score.SaveFrame();
			IsMatchOver.SaveFrame();
			WinningPlayers.SaveFrame();
		}

		public void Rollback(int frames)
		{
			Score.Rollback(frames);
			IsMatchOver.Rollback(frames);
			WinningPlayers.Rollback(frames);
		}
	}

	public class UsageWithRegistry
	{
		public UsageWithRegistry()
		{
			var footballService = new FootballService();
			footballService.SaveFrame();

			var massiveRegistry = new MassiveRegistry();
			massiveRegistry.FrameSaved += footballService.SaveFrame;
			massiveRegistry.Rollbacked += footballService.Rollback;

			// You can use service as you wish, it will be rolled back with registry.
			footballService.Score.Value += 1;
			footballService.IsMatchOver.Value = true;
			footballService.WinningPlayers.Add(Entity.Dead);

			massiveRegistry.Rollback(0); // Restore to last SaveFrame call.

			if (footballService.IsMatchOver.Value == false)
			{
				// We rolled back.
			}
		}
	}
}
