﻿using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private PlayerGUI playerGui;
    [SerializeField] private MeshRenderer meshRenderer;


    public LayerMask LayerMask;

    public float LastTimeAddMana = 0f;
    public AbilityBase aBase;


    /*-------------------------------------------
     * ----This Area defines The Players Stat----
     * -----------------------------------------*/

    public bool PlayedDeadSequence = false;
    // Use this for initialization
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        playerGui = FindObjectOfType<PlayerGUI>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (aBase.Health > 0)
        {
            if (Input.GetMouseButton(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    navMeshAgent.destination = hit.point;
                }

            }

            if (Input.GetMouseButton(1) && aBase.Mana > 5)
            { 
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                aBase.Fireball(ray, this.transform);
            }

            const float ManaRefillRate = 0.1f;
            if (Time.time - LastTimeAddMana > ManaRefillRate)
            {
                aBase.Mana += Mathf.RoundToInt((Time.time - LastTimeAddMana) / ManaRefillRate);
                aBase.Mana = Mathf.Clamp(aBase.Mana, 0, 100);
                LastTimeAddMana = Time.time;
            }

        }
        else// he ded
        {

            if (!PlayedDeadSequence)
            {
                PlayedDeadSequence = true;
                navMeshAgent.enabled = false;
                GetComponent<Collider>().enabled = false;
                Sequence deadSequence = DOTween.Sequence();
                deadSequence.Append(transform.DORotate(new Vector3(90f, 0f, 0f), 0.2f, RotateMode.LocalAxisAdd));
                deadSequence.Append(transform.DOMoveY(0f, 0.2f));
            }
        }


        playerGui.SetMana(aBase.Mana);
        playerGui.SetHealth(aBase.Health);
    }

    public void Damage(int damage)
    {
        // Grab a free Sequence to use
        Sequence mySequence = DOTween.Sequence();
        // Add a rotation tween as soon as the previous one is finished
        mySequence.Append(meshRenderer.material.DOColor(Color.red, 0.02f));
        // Delay the whole Sequence by 1 second
        mySequence.PrependInterval(0.05f);

        mySequence.Append(meshRenderer.material.DOColor(Color.white, 0.02f));

        aBase.Health -= damage;
    }





}
