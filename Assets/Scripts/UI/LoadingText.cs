using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class LoadingText : MonoBehaviour
{
    public TMP_Text text;
    public bool isLoading;
    public float animationDuration = 5f;
    public float animationSpeed = 5f;
    private float timer = 0;

    private float textMinSpacing = 4.61f;
    private float textMaxSpacing = 13.6f;

    public void EnableLoadText() {
        text.enabled = true;
        isLoading = true;
    }
    public void DisableLoadText() {
        text.enabled = false;
        isLoading = false;
    }
    private void Update() {
       

        if (isLoading) {
           
            timer += Time.deltaTime;
            float txtSize = Mathf.Lerp(textMinSpacing, textMaxSpacing, Mathf.PingPong(timer, animationDuration) / animationDuration);
            text.characterSpacing = txtSize;
        }
    }
}
