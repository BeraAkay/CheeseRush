using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Booster : MonoBehaviour
{
    public enum BoosterType { JumpBoost, Continuous, Point };
    public BoosterType boosterType;

    public float boosterMult;
    PlayerController playerController;

    public bool pulsating;
    public float pulseRate;
    [Range(0f, 1f)]
    public float pulseIntensity;
    SpriteRenderer boosterVisual;
    Vector3 baseScale;

    private void Start()
    {
        playerController = GameManager.instance.GetPlayerController();
        boosterVisual = GetComponentInChildren<SpriteRenderer>();

        baseScale = boosterVisual.transform.localScale;

        pulsating = boosterVisual;

    }

    private void FixedUpdate()
    {
        if (pulsating)
        {
            boosterVisual.transform.localScale = baseScale * ((Mathf.Sin(Time.time * pulseRate) * pulseIntensity) + 1);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(boosterType == BoosterType.JumpBoost)
            {
                playerController.Jump(boosterMult);

                gameObject.SetActive(false);
            }
            else if(boosterType == BoosterType.Continuous)
            {
                playerController.Jump(1);
                playerController.GetComponent<Rigidbody2D>().isKinematic = true;
                StartCoroutine(BoostTimer());
                boosterVisual.enabled = false;
            }
            else if(boosterType == BoosterType.Point)
            {
                GameManager.instance.ScoreBoost(boosterMult);
                gameObject.SetActive(false);
            }
        }
    }

    IEnumerator BoostTimer()
    {
        yield return new WaitForSeconds(boosterMult);
        playerController.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

        gameObject.SetActive(false);
    }



}
