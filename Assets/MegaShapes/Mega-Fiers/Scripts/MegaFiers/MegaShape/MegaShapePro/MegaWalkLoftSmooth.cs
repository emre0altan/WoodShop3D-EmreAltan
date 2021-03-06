
#if true
using UnityEngine;

[AddComponentMenu("MegaShapes/Walk Loft Smooth")]
[ExecuteInEditMode]
public class MegaWalkLoftSmooth : MonoBehaviour
{
	public MegaShapeLoft	surfaceLoft;
	public int				surfaceLayer	= -1;
	public float			alpha = 0.0f;
	public float			crossalpha = 0.0f;
	//public float			delay = 0.0f;
	public float			offset = 0.0f;
	public float			tangent = 0.01f;
	public Vector3			rotate = Vector3.zero;
	public MegaWalkMode		mode = MegaWalkMode.Alpha;
	public float			distance = 0.0f;
	public bool				lateupdate = true;
	public bool				animate = false;
	public float			speed = 0.0f;
	public float			upright = 0.0f;
	public Vector3			uprot = Vector3.zero;
	//public bool				initrot = true;
	public bool				useRaycast = false;

	[ContextMenu("Help")]
	public void Help()
	{
		Application.OpenURL("http://www.west-racing.com/mf/?page_id=2785");
	}

	void Update()
	{
		if ( !lateupdate )
			DoUpdate();
	}

	void LateUpdate()
	{
		if ( lateupdate )
			DoUpdate();
	}

	Ray ray = new Ray();

	void RaycastPosition(Vector3 p, Vector3 pf, Vector3 fwd, Vector3 up)
	{
		RaycastHit info,info1;
		ray.origin = p;
		ray.direction = Vector3.down;

		bool hit = Physics.Raycast(ray, out info);
		if ( hit )
		{
			p = info.point;

			ray.origin = pf;
			Physics.Raycast(ray, out info1);
			pf = info1.point;
			fwd = (pf - p).normalized;

			p += offset * info.normal;

			Quaternion rot = Quaternion.LookRotation(fwd, info.normal);

			//Quaternion rot1 = Quaternion.Euler(uprot);
			//rot = Quaternion.Lerp(rot, rot1, upright);

			//Vector3 rt = rotate;
			//rt.x -= tw;
			//Quaternion locrot = Quaternion.Euler(rt);	//otate);

			//rot = rot * locrot;

			transform.rotation = rot;	// * locrot;

			transform.position = p;
		}
		else
		{
			Quaternion rot = Quaternion.LookRotation(fwd, up);

			Quaternion rot1 = Quaternion.Euler(uprot);
			rot = Quaternion.Lerp(rot, rot1, upright);

			Vector3 rt = rotate;
			//rt.x -= tw;
			Quaternion locrot = Quaternion.Euler(rt);	//otate);

			rot = rot * locrot;

			transform.rotation = rot;	// * locrot;
			transform.position = p;	//layer.transform.TransformPoint(p);
		}
	}

	void DoUpdate()
	{
		if ( surfaceLoft && surfaceLayer >= 0 && surfaceLayer < surfaceLoft.Layers.Length )
		{
			MegaLoftLayerSimple layer = (MegaLoftLayerSimple)surfaceLoft.Layers[surfaceLayer];

			if ( animate )
			{
				distance += speed * Time.deltaTime;
				distance = Mathf.Repeat(distance, layer.layerPath.splines[layer.curve].length);
				alpha = distance / layer.layerPath.splines[layer.curve].length;
			}

			if ( mode == MegaWalkMode.Distance )
				alpha = distance / layer.layerPath.splines[layer.curve].length;
			else
				distance = alpha * layer.layerPath.splines[layer.curve].length;

			//float tw = surfaceLoft.Layers[surfaceLayer].layerPath.splines[0].GetTwist(alpha);

			Vector3 p = layer.transform.TransformPoint(layer.SampleSplines(surfaceLoft, crossalpha, alpha));
			Vector3 pr = layer.transform.TransformPoint(layer.SampleSplines(surfaceLoft, crossalpha + tangent, alpha));
			Vector3 pf = layer.transform.TransformPoint(layer.SampleSplines(surfaceLoft, crossalpha, alpha + tangent));

			Vector3 fwd = (pf - p).normalized;
			Vector3 right = (pr - p).normalized;
			Vector3 up = Vector3.Cross(fwd, right);	//.normalized;

			if ( useRaycast )
			{
				RaycastPosition(p, pf, fwd, up);
			}
			else
			{
				p += up * offset;

				Quaternion rot = Quaternion.LookRotation(fwd, up);

				Quaternion rot1 = Quaternion.Euler(uprot);
				rot = Quaternion.Lerp(rot, rot1, upright);

				Vector3 rt = rotate;
				//rt.x -= tw;
				Quaternion locrot = Quaternion.Euler(rt);	//otate);

				rot = rot * locrot;

				transform.rotation = rot;	// * locrot;
				transform.position = p;	//layer.transform.TransformPoint(p);
			}
		}
	}

	public Vector3 GetPos(float a, float ca)
	{
		if ( surfaceLoft && surfaceLayer >= 0 && surfaceLayer < surfaceLoft.Layers.Length )
		{
			MegaLoftLayerSimple layer = (MegaLoftLayerSimple)surfaceLoft.Layers[surfaceLayer];

			return layer.SampleSplines(surfaceLoft, ca, a);
		}

		return Vector3.zero;
	}

	public Vector3 GetPosDist(float dist, float ca)
	{
		if ( surfaceLoft && surfaceLayer >= 0 && surfaceLayer < surfaceLoft.Layers.Length )
		{
			MegaLoftLayerSimple layer = (MegaLoftLayerSimple)surfaceLoft.Layers[surfaceLayer];

			float a = dist / layer.layerPath.splines[layer.curve].length;

			return layer.SampleSplines(surfaceLoft, ca, a);
		}

		return Vector3.zero;
	}

	public Vector3 GetLocalPoint(float dist, float crossa)
	{
		Vector3 retval = Vector3.zero;

		if ( surfaceLoft && surfaceLayer >= 0 )
		{
			MegaLoftLayerSimple layer = (MegaLoftLayerSimple)surfaceLoft.Layers[surfaceLayer];

			float a = (distance + dist) / layer.layerPath.splines[layer.curve].length;

			Vector3 p = layer.transform.TransformPoint(layer.SampleSplines(surfaceLoft, crossalpha, a));
			Vector3 pr = layer.transform.TransformPoint(layer.SampleSplines(surfaceLoft, crossalpha + tangent, a));
			Vector3 pf = layer.transform.TransformPoint(layer.SampleSplines(surfaceLoft, crossalpha, a + tangent));

			Vector3 fwd = (pf - p).normalized;
			Vector3 right = (pr - p).normalized;
			Vector3 up = Vector3.Cross(fwd, right);	//.normalized;

			p += up * offset;

			p = layer.transform.TransformPoint(p);
			retval = transform.worldToLocalMatrix.MultiplyPoint3x4(p);
		}

		return retval;
	}

	public Vector3 GetPoint(float dist, float crossa)
	{
		Vector3 retval = Vector3.zero;

		if ( surfaceLoft && surfaceLayer >= 0 )
		{
			MegaLoftLayerSimple layer = (MegaLoftLayerSimple)surfaceLoft.Layers[surfaceLayer];

			float a = dist / layer.layerPath.splines[layer.curve].length;

			Vector3 p = layer.transform.TransformPoint(layer.SampleSplines(surfaceLoft, crossalpha, a));
			Vector3 pr = layer.transform.TransformPoint(layer.SampleSplines(surfaceLoft, crossalpha + tangent, a));
			Vector3 pf = layer.transform.TransformPoint(layer.SampleSplines(surfaceLoft, crossalpha, a + tangent));

			Vector3 fwd = (pf - p).normalized;
			Vector3 right = (pr - p).normalized;
			Vector3 up = Vector3.Cross(fwd, right);	//.normalized;

			p += up * offset;

			p = layer.transform.TransformPoint(p);
			retval = transform.worldToLocalMatrix.MultiplyPoint3x4(p);
		}

		return retval;
	}
}
#endif