using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] private InputAction thrust;
    [SerializeField] private InputAction rotation;
    [SerializeField] private InputAction shoot;
    
    [Header("Movement Settings")]
    [SerializeField] private float moveForce = 200f;
    [SerializeField] private float rotationForce = 220f;
    [SerializeField] private float linearDamping = 2.1f;
    [SerializeField] private float maxSpeed = 6.5f;

    [Header("Particle Systems")]
    [SerializeField] private ParticleSystem thrustParticleSystem1;
    [SerializeField] private ParticleSystem thrustParticleSystem2;
    
    [Header("Audio Settings")]
    [SerializeField] private AudioClip thrustAudioClip;
  
    Rigidbody rb;
    AudioSource audioSource;
    PlayerShooting playerShooting;
    PlayerFuel fuel;

    void Awake()
    {
        if (GetComponent<PlayerHealth>() == null)
        {
            gameObject.AddComponent<PlayerHealth>();
        }
    }

    void OnEnable()
    {
        thrust.Enable();
        rotation.Enable();
        shoot.Enable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearDamping = linearDamping;
        rb.freezeRotation = true;
        audioSource = GetComponent<AudioSource>();
        playerShooting = GetComponent<PlayerShooting>();
        fuel = GetComponent<PlayerFuel>();
    }

    void Update()
    {
        ProcessShooting();
    }

    void FixedUpdate()
    {
        ProcessThrust();
        ProcessRotation();

        // Limita la velocidad máxima acumulada
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }

    private void ProcessShooting()
    {
        if (shoot.IsPressed()) playerShooting.Shoot();        
    }

    private void ProcessRotation()
    {
        float rotationValue = rotation.ReadValue<float>();
        if (rotationValue == 0) return;

        // >0 = izquierda (A), <0 = derecha (D)
        float angle = Mathf.Sign(rotationValue) * rotationForce * Time.fixedDeltaTime;
        transform.Rotate(Vector3.forward, angle, Space.World);
    }
    private void ProcessThrust()
    {
        // Without fuel the ship cannot thrust (if there is no PlayerFuel component, fuel is unlimited).
        bool hasFuel = fuel == null || fuel.HasFuel;

        if (thrust.IsPressed() && hasFuel)
        {
            StartThrusting();

            if (fuel != null)
            {
                fuel.Consume(Time.fixedDeltaTime);
            }
        }
        else
        {
            StopThrusting();
        }
    }

    private void StopThrusting()
    {   
        if (thrustParticleSystem1 != null) thrustParticleSystem1.Stop();
        if (thrustParticleSystem2 != null) thrustParticleSystem2.Stop();
        audioSource.Stop();
    }

    private void StartThrusting()
    {
        // Empuja hacia donde apunta la nave (su Z local, que visualmente es "arriba")
        rb.AddForce(transform.forward * moveForce);

        if (thrustParticleSystem1 != null && !thrustParticleSystem1.isPlaying)
            thrustParticleSystem1.Play();

        if (thrustParticleSystem2 != null && !thrustParticleSystem2.isPlaying)
            thrustParticleSystem2.Play();

        if (!audioSource.isPlaying)
            audioSource.PlayOneShot(thrustAudioClip);
    }
}