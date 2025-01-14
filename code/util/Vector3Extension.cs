﻿public static class Vector3Extension
{
	public static float DistanceToLine( this Vector3 self, Vector3 start, Vector3 end, out Vector3 intersection )
	{
		var v = end - start;
		var w = self - start;

		var c1 = Vector3.Dot( w, v );
		if ( c1 <= 0 )
		{
			intersection = start;
			return self.Distance( start );
		}

		var c2 = Vector3.Dot( v, v );
		if ( c2 <= c1 )
		{
			intersection = end;
			return self.Distance( end );
		}

		var b = c1 / c2;
		var pb = start + b * v;

		intersection = pb;
		return self.Distance( pb );
	}
	public static float SqrDistanceToLine( this Vector3 self, Vector3 start, Vector3 end, out Vector3 intersection )
	{
		var v = end - start;
		var w = self - start;

		var c1 = Vector3.Dot( w, v );
		if ( c1 <= 0 )
		{
			intersection = start;
			return self.DistanceSquared( start );
		}

		var c2 = Vector3.Dot( v, v );
		if ( c2 <= c1 )
		{
			intersection = end;
			return self.DistanceSquared( end );
		}

		var b = c1 / c2;
		var pb = start + b * v;

		intersection = pb;
		return self.DistanceSquared( pb );
	}

	public static float DistanceToRay( this Vector3 self, Ray ray )
	{
		return Vector3.Cross( ray.Forward, self - ray.Position ).Length;
	}
}
