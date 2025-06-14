using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

public class MovimentoJogador : MonoBehaviour
{

    private CharacterController controller;
    private Transform myCamera;
    private Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
        myCamera = Camera.main.transform;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");



        UnityEngine.Vector3 movimento = new UnityEngine.Vector3(horizontal, 0, vertical);

        movimento = myCamera.TransformDirection(movimento);
        movimento.y = 0;

        controller.Move(movimento * Time.deltaTime * 5);
        controller.Move(new UnityEngine.Vector3(0, -9.81f, 0) * Time.deltaTime);

        if (movimento != UnityEngine.Vector3.zero)
        {
            transform.rotation = UnityEngine.Quaternion.Slerp(transform.rotation, UnityEngine.Quaternion.LookRotation(movimento), Time.deltaTime * 10);
        }

        animator.SetBool("MovFrente", movimento != UnityEngine.Vector3.zero);

        
    }
}
