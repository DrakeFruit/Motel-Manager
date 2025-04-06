using Sandbox;

public sealed class Plot : Component
{
	[Property, Sync] public static NetList<GameObject> Rooms { get; set; }
	public PlayerController Owner { get; set; }
	protected override void OnFixedUpdate()
	{
		// foreach ( var i in Rooms )
		// {
		// 	Log.Info( i.GetComponent<Room>() );
		// }
	}

	public void SetupFromConnection( Connection connection )
	{
		GameObject.Name = $"Plot - {connection.SteamId} - {connection.DisplayName}";
	}
}
