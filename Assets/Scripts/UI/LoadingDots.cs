using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cryptigo.Math;
using DG.Tweening;

public class LoadingDots : MonoBehaviour
{
    // Total time of the animation
    public float repeatTime = 1;

    // The time for a dot to bounce up and come back down
    public float bounceTime = 0.25f;

    // How far each dot moves.
    public float bounceHeight = 10;

    public bool isLoading = false;
    public Transform[] dots;

    private void Update() {



        if (isLoading) {
            for(int i = 0; i < this.dots.Length; i++) {
                var p = this.dots[i].localPosition;
                var t = Time.time * this.repeatTime * Constants.Pi + p.x;
                var y = (Mathf.Cos(t) - this.bounceTime) / (1f - this.bounceTime);
                p.y = Mathf.Max(0, y * this.bounceHeight);
                this.dots[i].localPosition = p;

            }
        }
    }



}
