using System.Collections;
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

        private bool canCreateNewVisualEffect = true;

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
                Color data = i < posListLen - 1 ? new Color(pos[i].x - vfxPos.x, pos[i].y - vfxPos.y, pos[i].z - vfxPos.z, 1) : new Color(0, 0, 0, 0);
                positions[i] = data;
            }

            texture.SetPixels(positions);
            texture.Apply();
            currentVFX.SetTexture(TEXTURE_NAME, texture);
            currentVFX.Reinit();
        }

        private VisualEffect NewVisualEffect(VisualEffect visualEffect, out Texture2D texture, out Color[] positions)
        {
            VisualEffect vfx = Instantiate(visualEffect, transform.position, Quaternion.identity, vfxContainer.transform);
            vfx.SetUInt(RESOLUTION_PARAMETER_NAME, (uint)resolution);
            texture = new Texture2D(resolution, resolution, TextureFormat.RGBAFloat, false);
            positions = new Color[resolution * resolution];
            return vfx;
        }

        private void Scan()
        {
            if (_fire.triggered)
            {


                // Allow CreateNewVisualEffect() after 4 seconds since last call
                if (canCreateNewVisualEffect)
                {
                    StartCoroutine(DelayedCreateVisualEffect());
                }


                for (int i = 0; i < pointsPerScan; i++)
                {
                    Vector3 randomPoint = Random.insideUnitSphere * radius;
                    randomPoint += castPoint.position;
                    Vector3 dir = (randomPoint - transform.position).normalized;

                    if (Physics.Raycast(transform.position, dir, out RaycastHit hit, range, layerMask))
                    {
                        if (hit.collider.CompareTag(REJECT_LAYER_NAME)) continue;
                        // On Hit
                        // check which color was hit
                        int resolution2 = resolution * resolution;

                        foreach (PointsData data in pointsData)
                        {
                            foreach (string tag in data.includedTags)
                            {
                                if (hit.collider.CompareTag(tag))
                                {
                                    if (data.positionsList.Count < resolution * resolution)
                                    {
                                        data.positionsList.Add(hit.point);
                                    }
                                    else if (reuseOldParticles)
                                    {
                                        data.positionsList.RemoveAt(0);
                                        data.positionsList.Add(hit.point);
                                    }
                                    else
                                    {
                                        data.currentVisualEffect = NewVisualEffect(data.prefab, out data.texture, out data.positionsAsColors);
                                        data.positionsList.Clear();
                                    }
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

                pointsData.ForEach(data => ApplyPositions(data.positionsList, data.currentVisualEffect, data.texture, data.positionsAsColors));
            }
            else
            {
                _lineRenderer.enabled = false;
            }
        }


        private IEnumerator DelayedCreateVisualEffect()
        {
            // Prevent further calls to CreateNewVisualEffect() for 4 seconds
            canCreateNewVisualEffect = false;

            // Wait for 4 seconds
            yield return new WaitForSeconds(4f);

            // Allow CreateNewVisualEffect() again
            canCreateNewVisualEffect = true;

            // Call CreateNewVisualEffect()
            CreateNewVisualEffect();
        }


        private void CreateNewVisualEffect()
        {
            // Create a new container for the children
            GameObject newContainer = new GameObject("New_VFX_Container");

            // Rename existing VisualEffect children of the vfxContainer
            foreach (Transform child in vfxContainer.transform)
            {
                if (child.TryGetComponent(out VisualEffect vfx))
                {
                    // You can rename the children as per your requirement
                    vfx.name = "Old_VisualEffect_" + vfx.GetInstanceID();
                    child.SetParent(newContainer.transform);
                }
            }

            // Create new VisualEffect instxances
            pointsData.ForEach(data =>
            {
                if (data.currentVisualEffect != null)
                {
                    Destroy(data.currentVisualEffect.gameObject, 10f);
                }
                data.currentVisualEffect = NewVisualEffect(data.prefab, out data.texture, out data.positionsAsColors);
            });
        }

    }
}