using System.Reflection.Emit;
using Sandbox;
using Sandbox.Citizen;

public sealed class NPC : Component, Component.IPressable
{
	[Property] GameObject Target { get; set; }
	[Property] DialoguePanel Dialogue { get; set; }
	[Property] float StopInteractDistance { get; set; }
	[RequireComponent] ScreenPanel SPanel { get; set; }
	[RequireComponent] NavMeshAgent Agent { get; set; }
	[RequireComponent] CitizenAnimationHelper Anim { get; set; }
	Component TargetPlayer { get; set; }
	PlayerController NearestPlayer { get; set; }
	protected override void OnStart()
	{
		Agent.MoveTo( Target.WorldPosition );
		Dialogue.Enabled = false;
		SPanel.Enabled = false;
	}

	protected override void OnFixedUpdate()
	{
		NearestPlayer = Scene.FindInPhysics( new Sphere( Anim.EyeSource.WorldPosition, 128 ) )
			.FirstOrDefault( x => x.GetComponent<PlayerController>().IsValid() )
			?.GetComponent<PlayerController>();
		
		if ( (Target.WorldPosition - WorldPosition).Length < 16 ) Agent.Stop();
		if ( Agent.Velocity.Length > 0.1f ) WorldRotation = Rotation.LookAt( Agent.Velocity );
		Anim.WithVelocity( Agent.Velocity );

		if ( TargetPlayer.IsValid() )
		{
			var dist = (TargetPlayer.WorldPosition - WorldPosition).Length;
			
			if ( dist > StopInteractDistance )
			{
				Dialogue.Enabled = false;
				SPanel.Enabled = false;
			}
		}

		if ( NearestPlayer.IsValid() && (NearestPlayer.WorldPosition.WithZ( 0 ) - WorldPosition.WithZ( 0 )).Length < 128 )
		{ 
			Anim.WithLook( Scene.Camera.WorldPosition - Anim.EyeSource.WorldPosition );
		} else Anim.WithLook( Anim.EyeSource.WorldRotation.Forward );
	}

	public bool Press( IPressable.Event press )
	{
		Agent.Velocity = 0;
		Agent.Stop();
		
		Dialogue.Enabled = true;
		SPanel.Enabled = true;
		
		TargetPlayer = press.Source;
		Dialogue.Player = TargetPlayer;
		var rot = Rotation.LookAt( TargetPlayer.WorldPosition - WorldPosition ).Angles().WithPitch( 0 );
		if ( Rotation.Difference( WorldRotation, rot ).Angle() > 90 ) WorldRotation = rot;
		
		return true;
	}
}
