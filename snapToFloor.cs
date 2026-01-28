using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Linq;

[ExecuteInEditMode]
public class SnapFloor : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("GameObject/Snap object to floor _END")]
    private static void Menu()
    {
        Mesh colliderMesh;
        Vector3 minBlueVector = Vector3.zero;
        Vector3 maxBlueVector = Vector3.zero;
        float maxZ = float.MinValue;
        float minZ = float.MaxValue;
        Vector3 minRedVector = Vector3.zero;
        Vector3 maxRedVector = Vector3.zero;
        float maxX = float.MinValue;
        float minX = float.MaxValue;
        Vector3 minYVector = Vector3.zero;
        Vector3 maxYVector = Vector3.zero;
        float maxY = float.MinValue;
        float minY = float.MaxValue;  
        GameObject[] selectedGameObjects = Selection.gameObjects; 
        Vector3 newPos = Vector3.zero;
        RaycastHit hit;
        Vector3[] vertices;
        SkinnedMeshRenderer skinnedMeshRenderer;
        MeshCollider meshCollider = null;
        Collider[] colliders = new Collider[1];
        bool[] colliderInitialState = new bool[1];

        foreach (GameObject selectedObject in selectedGameObjects)
        { 
            if (selectedObject.GetComponentsInChildren<Collider>() != null)
            {
                colliders = selectedObject.GetComponentsInChildren<Collider>();
                colliderInitialState = new bool[colliders.Length];

                for (int i = 0; i < colliders.Length; i++)
                {
                    colliderInitialState[i] = colliders[i].enabled;
                    colliders[i].enabled = false;
                }
            }

            Renderer rnd = selectedObject.GetComponent<Renderer>();

            foreach (Renderer renderer in selectedObject.GetComponentsInChildren<Renderer>(true))
            {
                if (renderer.GetComponentInChildren<ParticleSystem>() != null)
                    continue;

                Mesh mesh;
                
                if (renderer.transform.GetComponent<MeshFilter>() != null)
                {
                    mesh = renderer.transform.GetComponent<MeshFilter>().sharedMesh;
                    if (mesh != null)
                    {
                        vertices = mesh.vertices;

                        for (int i = 0; i < vertices.Length; i++)
                        {
                            newPos = renderer.gameObject.transform.TransformPoint(vertices[i]);

                            if (newPos.x < minX)
                            {
                                minX = newPos.x;
                                minRedVector = newPos;
                            }

                            if (newPos.x > maxX)
                            {
                                maxX = newPos.x;
                                maxRedVector = newPos;
                            }

                            if (newPos.z < minZ)
                            {
                                minZ = newPos.z;
                                minBlueVector = newPos;
                            }

                            if (newPos.z > maxZ)
                            {
                                maxZ = newPos.z;
                                maxBlueVector = newPos;
                            }

                            if (newPos.y < minY)
                            {
                                minY = newPos.y;
                                minYVector = newPos;
                            }

                            if (newPos.y > maxY)
                            {
                                maxY = newPos.y;
                                maxYVector = newPos;
                            }
                        }
                    }
                }

                if (renderer.GetComponent<SkinnedMeshRenderer>() != null)
                {
                    renderer.transform.localScale = new Vector3(1/renderer.transform.root.localScale.x, 1 / renderer.transform.root.localScale.y, 1 / renderer.transform.root.localScale.z);
                    skinnedMeshRenderer = renderer.GetComponent<SkinnedMeshRenderer>();
                    colliderMesh = new Mesh();
                    skinnedMeshRenderer.BakeMesh(colliderMesh);
                    meshCollider = renderer.gameObject.AddComponent<MeshCollider>();
                    meshCollider.sharedMesh = colliderMesh;
                    meshCollider.convex = true;
                    vertices = colliderMesh.vertices;

                    for (int i = 0; i < vertices.Length; i++)
                    {
                        newPos = renderer.gameObject.transform.TransformPoint(vertices[i]);

                        if (newPos.x < minX)
                        {
                            minX = newPos.x;
                            minRedVector = newPos;
                        }

                        if (newPos.x > maxX)
                        {
                            maxX = newPos.x;
                            maxRedVector = newPos;
                        }

                        if (newPos.z < minZ)
                        {
                            minZ = newPos.z;
                            minBlueVector = newPos;
                        }

                        if (newPos.z > maxZ)
                        {
                            maxZ = newPos.z;
                            maxBlueVector = newPos;
                        }

                        if (newPos.y < minY)
                        {
                            minY = newPos.y;
                            minYVector = newPos;
                        }

                        if (newPos.y > maxY)
                        {
                            maxY = newPos.y;
                            maxYVector = newPos;
                        }
                    }

                    if (meshCollider != null)
                        DestroyImmediate(meshCollider);
                }

                Undo.RecordObject(selectedObject.transform, "Change Transform Position");
                if (renderer.transform == selectedObject) continue; 
            }

            Vector3 bottomCentreVertex = new Vector3((maxRedVector.x + minRedVector.x) / 2, minY, (minBlueVector.z + maxBlueVector.z) / 2);
            float smallestDistance = float.MaxValue;
            Vector3 selectedPoint = Vector3.zero;
            Vector3 scaledUp = Vector3.up * 0.01f;
            Vector3 aux1 = new Vector3(minRedVector.x, minY, maxBlueVector.z);
            Vector3 aux2 = new Vector3(maxRedVector.x, minY, minBlueVector.z);
            Vector3 aux3 = new Vector3(maxRedVector.x, minY, maxBlueVector.z);
            Vector3 aux4 = new Vector3(minRedVector.x, minY, minBlueVector.z);
            Vector3 aux5 = Vector3.zero;

            for (int i = 0; i < 200; i++)
            {
                aux5 = new Vector3(Random.Range(minRedVector.x, maxRedVector.x), minY, Random.Range(minBlueVector.z, maxBlueVector.z));
                Physics.Raycast(aux5, -Vector3.up, out hit);
                if (smallestDistance > hit.distance) { smallestDistance = hit.distance; selectedPoint = hit.point; }
            }

            Physics.Raycast(aux1, -Vector3.up, out hit);
            if (smallestDistance > hit.distance) { smallestDistance = hit.distance; selectedPoint = hit.point; }

            Physics.Raycast(aux2, -Vector3.up, out hit);
            if (smallestDistance > hit.distance) { smallestDistance = hit.distance; selectedPoint = hit.point; }

            Physics.Raycast(aux3, -Vector3.up, out hit);
            if (smallestDistance > hit.distance) { smallestDistance = hit.distance; selectedPoint = hit.point; }

            Physics.Raycast(aux4, -Vector3.up, out hit);
            if (smallestDistance > hit.distance) { smallestDistance = hit.distance; selectedPoint = hit.point; }

            Physics.Raycast(bottomCentreVertex, -Vector3.up, out hit);
            if (smallestDistance > hit.distance) { smallestDistance = hit.distance; selectedPoint = hit.point; }

            Physics.Raycast(selectedObject.transform.position, -Vector3.up, out hit);
            if (smallestDistance > hit.distance) { smallestDistance = hit.distance; selectedPoint = hit.point; }

            selectedObject.transform.position = new Vector3(selectedObject.transform.position.x, selectedObject.transform.position.y - smallestDistance, selectedObject.transform.position.z);
            selectedObject.transform.position += new Vector3(0f, 0.001f, 0f);

            if (selectedObject.GetComponentsInChildren<Collider>() != null)
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    colliders[i].enabled = colliderInitialState[i];
                }
            }
        }
    }
#endif
}

