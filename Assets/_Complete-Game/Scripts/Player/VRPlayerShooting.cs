using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace CompleteProject
{
    public class VRPlayerShooting : MonoBehaviour
    {

        public SteamVR_Input_Sources HandType;
        public SteamVR_Action_Boolean ShootAction;

        public int damagePerShot = 20;
        public float timeBetweenBullets = 0.15f;
        public float range = 100f;

        float timer;
        Ray shootRay = new Ray();
        RaycastHit shootHit;
        int shootableMask;
        ParticleSystem gunParticles;
        LineRenderer gunLine;
        AudioSource gunAudio;
        Light gunLight;
        float effectDisplayTime = 0.2f;

        // Use this for initialization
        void Awake()
        {
            shootableMask = LayerMask.GetMask("Shootable");
            gunParticles = GetComponent<ParticleSystem>();
            gunLine = GetComponent<LineRenderer>();
            gunAudio = GetComponent<AudioSource>();
            gunLight = GetComponent<Light>();
        }

        // Update is called once per frame
        void Update()
        {
            timer += Time.deltaTime;

            if (ShootAction.GetStateDown(HandType))
            {
                Shoot();
            }

            if (timer >= timeBetweenBullets * effectDisplayTime)
            {
                DisableEffects();
            }
        }

        public void DisableEffects()
        {
            gunLine.enabled = false;
            gunLight.enabled = false;
        }

        void Shoot()
        {
            timer = 0f;
            gunAudio.Play();
            gunLight.enabled = true;

            gunParticles.Stop();
            gunParticles.Play();

            gunLight.enabled = true;
            gunLine.SetPosition(0, transform.position);

            shootRay.origin = transform.position;
            shootRay.direction = transform.forward;

            if (Physics.Raycast(shootRay, out shootHit, range, shootableMask))
            {
                EnemyHealth enemyHealth = shootHit.collider.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damagePerShot, shootHit.point);
                }
                gunLine.SetPosition(1, shootHit.point);
            }
            else
            {
                gunLine.SetPosition(1, shootRay.origin + shootRay.direction * range);
            }
        }
    }
}