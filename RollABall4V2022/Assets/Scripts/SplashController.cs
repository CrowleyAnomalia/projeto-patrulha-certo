using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SplashController : MonoBehaviour
{
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(2f);
        GameManager.Instance.LoadMenu();
    }

}
