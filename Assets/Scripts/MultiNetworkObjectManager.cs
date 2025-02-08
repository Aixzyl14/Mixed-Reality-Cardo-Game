using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MultiNetworkObjectManager : NetworkBehaviour
{
    [SerializeField]
    private List<GameObject> prefabList; // List of Objects to Sync (each has a NetworkObject + NetworkTransform)
    private List<GameObject> networkObjects; 


    public void SpawnAndSyncObjects(Vector3[] spawnPositions, Quaternion[] spawnRotation)
    {
        if (!IsServer) return; // Only the server can spawn objects
        
        for (int i = 0; i < prefabList.Count; ++i)
        {
            if (i < spawnPositions.Length && i < spawnRotation.Length)
            {
                // Spawn the Object
                NetworkObject networkObject = prefabList[i].GetComponent<NetworkObject>();

                if (networkObject != null)
                {
                    networkObject.Spawn(); // Spawn the object on the network
                    networkObjects.Add(prefabList[i]); // Add to the managed list
                }
                else
                {
                    Debug.LogError("Prefab does not have a NetworkObject component!");
                }
            }
        }

    }


    public void UpdateObjectTransform(Vector3[] newPositions, Quaternion[] newRotations)
    {
        if (!IsServer) return; // Only the server can update transforms

        for (int i = 0; i < networkObjects.Count; ++i)
        {
            if (i < newPositions.Length && i < newRotations.Length)
            {
                // Update the transforms Locally on the server
                networkObjects[i].transform.position = newPositions[i];
                networkObjects[i].transform.rotation = newRotations[i];

                // Sync the changes across clients
                UpdateTransformClientRpc(networkObjects[i].GetComponent<NetworkObject>().NetworkObjectId, newPositions[i], newRotations[i]);
            }
        }

    }

    [ClientRpc]
    private void UpdateTransformClientRpc(ulong networkObjectId, Vector3 newPosition, Quaternion newRotation)
    {
        // Find the object using its NetworkObjectId
        NetworkObject networkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId];

        if (networkObject != null)
        {
            // Update the transform on the client
            networkObject.transform.position = newPosition;
            networkObject.transform.rotation = newRotation;

        }

    }

}
