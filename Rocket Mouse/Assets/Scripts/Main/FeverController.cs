using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeverController : MonoBehaviour
{
    public MouseController mc;

    public GeneratorScript gs;

    public bool fever;

    private void Start()
    {
        StartCoroutine(FeverLoop());
    }

    private void Update()
    {
        //Debug.Log(fever);
    }

    IEnumerator FeverLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(60f);

            fever = true;
            mc.fowardMovementSpeed = 10f;
            gs.RemoveLasers();

            yield return new WaitForSeconds(10f);

            fever = false;
            mc.LevelApply();
        }
    }
}
