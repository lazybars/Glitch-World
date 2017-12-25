using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CycleObjects : MonoBehaviour {
    public float spawnDistance;
    public float showInterval = 1;
    public GameObject[] cyclicObjects;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        cycleThrough();
	}

    void spawnObjectInFront(float spawnDistance, GameObject objectToSpawn) {
        Vector3 playerPos = transform.position;
        Vector3 playerDirection = transform.forward;
        Quaternion playerRotation = transform.rotation;

        Vector3 spawnPos = playerPos + playerDirection * spawnDistance;

        Instantiate(objectToSpawn, spawnPos, playerRotation);
    }


    void cycleThrough() {
        bool isHoldingLeftMouse = false;
        if (Input.GetMouseButton(0)) {
            isHoldingLeftMouse = true;
            StartCoroutine(showPotentialGameObjects());
        } else if (Input.GetMouseButtonUp(0)) {
            isHoldingLeftMouse = false;
            StopCoroutine(showPotentialGameObjects());
        };

        print(isHoldingLeftMouse);
    }

    IEnumerator showPotentialGameObjects() {
        for (int i = 0; i < cyclicObjects.Length; i++) {
            //GameObject instantiatedObj = Instantiate(cyclicObjects[i], new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
            spawnObjectInFront(spawnDistance, cyclicObjects[i]);
            yield return new WaitForSeconds(showInterval);
        }
    }
}
