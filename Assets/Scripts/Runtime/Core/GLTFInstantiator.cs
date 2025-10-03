#region Namespaces

using System.IO;
using System.Threading.Tasks;
using GLTFast;
using Unity.Collections;
using UnityEngine;

#endregion

namespace Phygtl.ARAssessment.Core
{
	/// <summary>
	/// A struct containing the information for instantiating GLTF assets.
	/// </summary>
	public struct GLTFInstantiator : IInstantiator
	{
		public void AddAnimation(AnimationClip[] animationClips)
		{
			throw new System.NotImplementedException();
		}

		public void AddCamera(uint nodeIndex, uint cameraIndex)
		{
			throw new System.NotImplementedException();
		}

		public void AddLightPunctual(uint nodeIndex, uint lightIndex)
		{
			throw new System.NotImplementedException();
		}

		public void AddPrimitive(uint nodeIndex, string meshName, MeshResult meshResult, uint[] joints = null, uint? rootJoint = null, float[] morphTargetWeights = null, int meshNumeration = 0)
		{
			throw new System.NotImplementedException();
		}

		public void AddPrimitiveInstanced(uint nodeIndex, string meshName, MeshResult meshResult, uint instanceCount, NativeArray<Vector3>? positions, NativeArray<Quaternion>? rotations, NativeArray<Vector3>? scales, int meshNumeration = 0)
		{
			throw new System.NotImplementedException();
		}

		public void BeginScene(string name, uint[] rootNodeIndices)
		{
			throw new System.NotImplementedException();
		}

		public void CreateNode(uint nodeIndex, uint? parentIndex, Vector3 position, Quaternion rotation, Vector3 scale)
		{
			throw new System.NotImplementedException();
		}

		public void EndScene(uint[] rootNodeIndices)
		{
			throw new System.NotImplementedException();
		}

		public void SetNodeName(uint nodeIndex, string name)
		{
			throw new System.NotImplementedException();
		}
	}
}
