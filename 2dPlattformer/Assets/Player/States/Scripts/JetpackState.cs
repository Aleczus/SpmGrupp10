﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "States/JetpakState")]
public class JetpackState : State
{
    private float _gravityTmp;

    ParticleSystem jetpackParticles;

    [Header("Movement")]
    public float Acceleration = 100;
    public MinMaxFloat jetPackAcceleration;

    public float waitBeforeTransitionToGround = 1.0f;
    private float jetPackSpeed; 

    private bool initialHeight;
    

    private List<Collider2D> _ignoredPlatforms = new List<Collider2D>();

    private PlayerController _controller;
    private bool _wallhit;
    private bool _roofhit;
    private Vector2 getCurrentVelocity;
    [Range(0f,2f)]
    public float toAirSpeed = 0;

    private Transform _transform { get { return _controller.transform; } }
    private Vector2 _velocity
    {
        get { return _controller.Velocity; }
        set { _controller.Velocity = value; }
    }

    public override void Initialize(Controller owner)
    {
        _controller = (PlayerController)owner;
        
    }

    public override void Enter()
    { 
        _transform.Translate(Vector3.up * 0.1f);
        _gravityTmp = _controller.Gravity;
        _controller.Gravity = 0;
        _controller.JetPackTrail.gameObject.SetActive(true);
        jetpackParticles = _controller.JetPackTrail.GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    public override void Update()
    {
        jetPackSpeed = Input.GetAxisRaw("LeftTrigger");
        if (jetPackSpeed != 0)
        {
            _controller.Velocity = Vector2.zero;
            jetpackParticles.Play();

            if (_controller.playerManager.currentFuel <= 0)
            {
                jetPackSpeed = 0;
                TransitionToAirState();
            }
            //Debug.Log(jetPackSpeed);

            _controller.playerManager.Fuel();

            RaycastHit2D[] hits = _controller.DetectHits();

            UpdateMovement();

            OverLappCollidersUpdate();

            UpdateNormalForce(hits);

            _transform.Translate(_velocity * Time.deltaTime);
        }
        else
        {
            TransitionToAirState();
        }



    }

    private void OverLappCollidersUpdate()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(_transform.position + (Vector3)_controller.Collider.offset, _controller.Collider.size, 0.0f, _controller.CollisionLayers);
        for (int i = _ignoredPlatforms.Count - 1; i >= 0; i--)
        {
            if (!colliders.Contains(_ignoredPlatforms[i]))
                _ignoredPlatforms.Remove(_ignoredPlatforms[i]);
        }
    }

    private void TransitionToAirState()
    {
        RaycastHit2D[] hits = _controller.DetectHits();

        Vector2 delta = TransitionAirTime();

        OverLappCollidersUpdate();

        UpdateNormalForce(hits);

        _transform.Translate((getCurrentVelocity + delta) * Time.deltaTime);

        if (getCurrentVelocity.y < 0.05f)
        {
            _controller.Velocity = getCurrentVelocity;
            _controller.TransitionTo<AirState>();
        }
    }

    private Vector2 TransitionAirTime()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        Vector2 delta = Vector2.zero;
        if (!_wallhit)
        { 
            if (Mathf.Abs(horizontalInput) > _controller.InputMagnitudeToMove)
                delta = Vector2.right * horizontalInput * Acceleration * Time.deltaTime;
                getCurrentVelocity.y = getCurrentVelocity.y - toAirSpeed;
        }
        else
        {
            getCurrentVelocity = delta;
        }

        

        return delta;
    }

    private void UpdateNormalForce(RaycastHit2D[] hits)
    {

        if (hits.Length == 0) return; //Kollar om vi träffar nått

        _controller.SnapToHit(hits[0]);  //kollar om vi ska snappa till marken

        //kollar om marken är rätt inom till låten vinkel
        foreach (RaycastHit2D hit in hits)
        {

            if (hit.collider.CompareTag("OneWay") && _velocity.y > 0.0f && !_ignoredPlatforms.Contains(hit.collider))
            {
                _ignoredPlatforms.Add(hit.collider);
            }

            if (_ignoredPlatforms.Contains(hit.collider))
                continue;

            if (!MathHelper.CheckAllowedSlope(_controller.SlopeAngles, hit.normal))
                _wallhit = true;
            else
                _wallhit = false;

            if (hit.collider.CompareTag("EnemyMove"))
            {
                _velocity += hit.collider.gameObject.GetComponent<MiniBossController>().Velocity;
            }

            _velocity += MathHelper.GetNormalForce(_velocity, hit.normal);

            if (MathHelper.CheckAllowedSlope(_controller.SlopeAngles, hit.normal))
            {                
                _controller.TransitionTo<GroundState>();
            }
        }
    }

    private void UpdateMovement()
    {
        float verticalInput = Input.GetAxisRaw("Vertical");
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        Vector2 delta;
        if (_wallhit && Mathf.Abs(verticalInput) < _controller.InputMagnitudeToMove && Mathf.Abs(horizontalInput) < _controller.InputMagnitudeToMove)
        {
            delta = Vector2.up * jetPackAcceleration.Min * jetPackSpeed;
            _velocity += delta;
            
            return;
        }
        if (_wallhit && Mathf.Abs(verticalInput) < _controller.InputMagnitudeToMove)
        {
            delta = Vector2.up * jetPackAcceleration.Min * jetPackSpeed;
            _velocity += delta;
           
        }

        if (Mathf.Abs(verticalInput) < _controller.InputMagnitudeToMove && Mathf.Abs(horizontalInput) < _controller.InputMagnitudeToMove)
        {
            delta = Vector2.up * jetPackAcceleration.Max * jetPackSpeed;
            _velocity += delta;
        }

        if (Mathf.Abs(verticalInput) > _controller.InputMagnitudeToMove)
        {
            delta = Vector2.up * verticalInput * jetPackAcceleration.Max * jetPackSpeed;
            _velocity += delta;
        }

        if (Mathf.Abs(horizontalInput) > _controller.InputMagnitudeToMove)
        {        
            delta = Vector2.right * horizontalInput * jetPackAcceleration.Max * jetPackSpeed;
            _velocity += delta;
        }

        _wallhit = false;

        getCurrentVelocity = _velocity;

    }

    public override void Exit()
    {
        //jetpackParticles.Stop();
        _controller.JetPackTrail.gameObject.SetActive(false);
        _controller.Gravity = _gravityTmp;
        
    }
}