﻿[Library]
public partial class BouncingProjectile : BulletDropProjectile
{
	public float Bounciness { get; set; } = 1f;

	protected override void PostSimulate( TraceResult trace )
	{
		if ( LifeTime.HasValue )
		{
			if ( trace.Hit )
			{
				var reflect = Vector3.Reflect( Velocity.Normal, trace.Normal );
				GravityModifier = 0f;
				Velocity = reflect * Velocity.Length * Bounciness;
			}
		}

		base.PostSimulate( trace );
	}

	protected override bool HasHitTarget( TraceResult trace )
	{
		if ( LifeTime.HasValue )
		{
			return false;
		}

		return base.HasHitTarget( trace );
	}
}