using Sandbox;

public sealed class Room : Component
{
	[Property] public NPC Occupant { get; set; }
	protected override void OnUpdate()
	{

	}
}
