using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

namespace LRS
{
    [RequireComponent(typeof(LineRenderer))]
    public class test : MonoBehaviour
    {
        private InputAction _fire;
        private InputAction _changeRadius;
        private LineRenderer _lineRenderer;

        [SerializeField] private List<PointsData> pointsData = new List<PointsData>();
        private List<CloneData> cloneDataList = new List<CloneData>();


        private const string REJECT_LAYER_NAME = "PointReject";
        private const string TEXTURE_NAME = "PositionsTexture";
        private const string RESOLUTION_PARAMETER_NAME = "Resolution";

        [SerializeField] private bool reuseOldParticles = false;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private GameObject vfxContainer;
        [SerializeField] private Transform castPoint;
        [SerializeField] private float radius = 10f;
        [SerializeField] private float maxRadius = 10f;
        [SerializeField] private float minRadius = 1f;
        [SerializeField] private int pointsPerScan = 100;
        [SerializeField] private float range = 10f;
        [SerializeField] private int resolution = 100;
        [SerializeField] private float battery = 100f;

        private int nextCloneNumber = 1; // Counter to assign unique numbers to clones

        private void Start()
        {
            _fire = playerInput.actions["Fire"];
            _changeRadius = playerInput.actions["Scroll"];
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.enabled = false;

            pointsData.ForEach(data =>
            {
                data.ClearData();
                data.currentVisualEffect = NewVisualEffect(data.prefab, out data.texture, out data.positionsAsColors);
                ApplyPositions(data.positionsList, data.currentVisualEffect, data.texture, data.positionsAsColors);
            });
        }

        private void FixedUpdate()
        {
            Scan();
            ChangeRadius();
        }

        private void ChangeRadius()
        {
            if (_changeRadius.triggered)
            {
                radius = Mathf.Clamp(radius + _changeRadius.ReadValue<float>() * Time.deltaTime, minRadius, maxRadius);
            }
        }


        private void ApplyPositions(List<Vector3> positionsList, VisualEffect currentVFX, Texture2D texture, Color[] positions)
        {
            Vector3[] pos = positionsList.ToArray();
            Vector3 vfxPos = currentVFX.transform.position;

            int loopLength = texture.width * texture.height;
            int posListLen = pos.Length;

            for (int i = 0; i < loopLength; i++)
            {
                Color data;

                if (i < posListLen - 1)
                {
                    data = new Color(pos[i].x - vfxPos.x, pos[i].y - vfxPos.y, pos[i].z - vfxPos.z, 1);
                }
                else
                {
                    data = new Color(0, 0, 0, 0);
                }
                positions[i] = data;
            }

            texture.SetPixels(positions);
            texture.Apply();

            currentVFX.SetTexture(TEXTURE_NAME, texture);
            currentVFX.Reinit();
        }


        private VisualEffect NewVisualEffect(VisualEffect visualEffect, out Texture2D texture, out Color[] positions)
        {
            texture = new Texture2D(resolution, resolution, TextureFormat.RGBAFloat, false);
            positions = new Color[resolution * resolution];

            VisualEffect vfx = Instantiate(visualEffect, transform.position, Quaternion.identity, vfxContainer.transform);
            vfx.SetUInt(RESOLUTION_PARAMETER_NAME, (uint)resolution);

            return vfx;
        }


        private void Scan()
        {
            if (_fire.IsPressed())
            {

                if (battery > 0)
                {
                    foreach (PointsData data in pointsData)
                    {
                        foreach (string tag in data.includedTags)
                        {
                            // Clone PointsData based on tag
                            if (_fire.IsPressed() && tag != REJECT_LAYER_NAME)
                            {
                                PointsData originalData = data;
                                PointsData clone = Instantiate(originalData);
                                clone.ClearData();

                                // Instantiate a new GameObject and set its position and rotation
                                GameObject cloneGameObject = new GameObject();
                                cloneGameObject.transform.position = originalData.prefab.transform.position;
                                cloneGameObject.transform.rotation = originalData.prefab.transform.rotation;

                                // Clone the VisualEffect component
                                VisualEffect originalVFX = originalData.prefab.GetComponent<VisualEffect>();
                                VisualEffect cloneVFX = cloneGameObject.AddComponent<VisualEffect>();
                                cloneVFX.visualEffectAsset = originalVFX.visualEffectAsset; // Set the same visual effect asset

                                // Make the cloneGameObject a child of vfxContainer
                                cloneGameObject.transform.SetParent(vfxContainer.transform);


                                // Assign a unique number to the clone
                                int cloneNumber = GetNextCloneNumber();

                                // Add the clone data to the list
                                cloneDataList.Add(new CloneData(originalData, cloneGameObject, cloneNumber));
                                for (int i = 0; i < pointsPerScan; i++)
                                {
                                    Vector3 randomPoint = Random.insideUnitSphere * radius;
                                    randomPoint += castPoint.position;
                                    Vector3 dir = (randomPoint - transform.position).normalized;

                                    if (Physics.Raycast(transform.position, dir, out RaycastHit hit, range, layerMask))
                                    {
                                        if (hit.collider.CompareTag(REJECT_LAYER_NAME)) continue;

                                        foreach (PointsData data2 in pointsData)
                                        {
                                            if (data2.includedTags.Contains(hit.collider.tag))
                                            {
                                                if (data2.positionsList.Count < resolution * resolution)
                                                {
                                                    data2.positionsList.Add(hit.point);
                                                }
                                                else if (reuseOldParticles)
                                                {
                                                    data2.positionsList.RemoveAt(0);
                                                    data2.positionsList.Add(hit.point);
                                                }
                                                else
                                                {
                                                    data2.ClearData();
                                                    data2.currentVisualEffect = NewVisualEffect(data2.prefab, out data2.texture, out data2.positionsAsColors);
                                                    data2.positionsList.Clear();
                                                }
                                            }
                                        }

                                        _lineRenderer.enabled = true;
                                        _lineRenderer.SetPositions(new[] { transform.position, hit.point });
                                    }
                                    else
                                    {
                                        Debug.DrawRay(transform.position, dir * range, Color.red);

                                    }
                                }
                        }
                    }
                }
            }
            else
            {
                if (battery < 100)
                {
                    battery = battery + 0.1f;

                }
            }
             

            
            }

            pointsData.ForEach(data => ApplyPositions(data.positionsList, data.currentVisualEffect, data.texture, data.positionsAsColors));

            _lineRenderer.enabled = !_fire.triggered;
        }


        private int GetNextCloneNumber()
        {
            return nextCloneNumber++;
        }


        private void DestroyClones()
        {
            // Find the clone with the lowest numbered suffix
            CloneData cloneToRemove = null;
            int lowestCloneNumber = int.MaxValue;
            foreach (CloneData cloneData in cloneDataList)
            {
                if (cloneData.CloneNumber < lowestCloneNumber)
                {
                    lowestCloneNumber = cloneData.CloneNumber;
                    cloneToRemove = cloneData;
                }
            }

            // Remove the clone with the lowest numbered suffix
            if (cloneToRemove != null)
            {
                cloneDataList.Remove(cloneToRemove);
                Destroy(cloneToRemove.ClonedGameObject);
            }
        }

        // Define CloneData class
        public class CloneData
        {
            public PointsData OriginalPointsData { get; }
            public GameObject ClonedGameObject { get; }
            public int CloneNumber { get; }

            public CloneData(PointsData originalPointsData, GameObject clonedGameObject, int cloneNumber)
            {
                OriginalPointsData = originalPointsData;
                ClonedGameObject = clonedGameObject;
                CloneNumber = cloneNumber;
            }
        }

        // Other methods...
    }

}


