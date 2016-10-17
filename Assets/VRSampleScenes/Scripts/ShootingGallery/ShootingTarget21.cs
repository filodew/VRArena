using System;
using System.Collections;
using UnityEngine;
using VRStandardAssets.Common;
using VRStandardAssets.Utils;

namespace VRStandardAssets.ShootingGallery
{
    // Ten skrypt obsługuje cel w scenach strzelania.
    // Obejmuje on co powinno się zdarzyć, gdy jest trafiony i
    // jak długo zanim znikną.
    public class ShootingTarget2 : MonoBehaviour
    {
        public event Action<ShootingTarget2> OnRemove;                   // To zdarzenie jest uruchamiane wtedy, gdy cel musi być usunięty.


        [SerializeField] private int m_Score = 0;                       // This is the amount added to the users score when the target is hit.
        [SerializeField] private float m_TimeOutDuration = 6f;          // Jak długo trwa cel zanim zniknie.
        [SerializeField] private float m_DestroyTimeOutDuration = 3f;   // When the target is hit, it shatters. This is how long before the shattered pieces disappear.
        [SerializeField] private GameObject m_DestroyPrefab;            // The prefab for the shattered target.
        [SerializeField] private AudioClip m_DestroyClip;               // The audio clip to play when the target shatters.
        [SerializeField] private AudioClip m_SpawnClip;                 // The audio clip that plays when the target appears.
        [SerializeField] private AudioClip m_MissedClip;                // The audio clip that plays when the target disappears without being hit.


        private Transform m_CameraTransform;                            // Służy do upewnienia się, że cel stoi przed kamerą.
        private VRInteractiveItem m_InteractiveItem;                    // Służy do obsługi kliknięcia użytkownika, podczas gdy patrzy na cel.
		private VRInteractiveCube m_InteractiveCube;
        private AudioSource m_Audio;                                    // Used to play the various audio clips.
        private Renderer m_Renderer;                                    // Wykorzystywane zniknięcia celu, zanim zostanie usunięte. | Used to make the target disappear before it is removed.
        private Collider m_Collider;                                    // Używany do upewnienia się, że docelowo nie przerywają inne strzały. | Used to make sure the target doesn't interupt other shots happening.
        private bool m_IsEnding;                                        // Czy cel jest obecnie usuwa się z innego źródła. | Whether the target is currently being removed by another source.
        
        
        private void Awake()
        {
            // Setup the references. | Konfiguracja odniesienia.
            m_CameraTransform = Camera.main.transform;
            m_Audio = GetComponent<AudioSource> ();
            m_InteractiveItem = GetComponent<VRInteractiveItem>();
			m_InteractiveCube = GetComponent<VRInteractiveCube>();
            m_Renderer = GetComponent<Renderer>();
            m_Collider = GetComponent<Collider>();
        }


        private void OnEnable ()
        {
            m_InteractiveItem.OnDown += HandleDown;
		//	m_InteractiveCube.OnDown -= HandleDown;
        }


        private void OnDisable ()
        {
            m_InteractiveItem.OnDown -= HandleDown;
		//	m_InteractiveCube.OnDown -= HandleDown;
        }


        private void OnDestroy()
        {
            // Ensure the event is completely unsubscribed when the target is destroyed.
            OnRemove = null;
        }
        

        public void Restart (float gameTimeRemaining)
        {
            // When the target is spawned turn the visual and physical aspects on.
            m_Renderer.enabled = true;
            m_Collider.enabled = true;

            // Since the target has just spawned, it's not ending yet.
            m_IsEnding = false;
            
            // Play the spawn clip.
            m_Audio.clip = m_SpawnClip;
            m_Audio.Play();

            // Make sure the target is facing the camera.
            transform.LookAt(m_CameraTransform);

            // Start the time out for when the target would naturally despawn.
            StartCoroutine (MissTarget());

            // Start the time out for when the game ends.
            // Note this will only come into effect if the game time remaining is less than the time out duration.
            StartCoroutine (GameOver (gameTimeRemaining));
        }
        

        private IEnumerator MissTarget()
        {
            // Wait for the target to disappear naturally.
            yield return new WaitForSeconds (m_TimeOutDuration);

            // If by this point it's already ending, do nothing else.
            if(m_IsEnding)
                yield break;

            // Otherwise this is ending the target's lifetime.
            m_IsEnding = true;

            // Turn off the visual and physical aspects.
            m_Renderer.enabled = false;
            m_Collider.enabled = false;
            
            // Play the clip of the target being missed.
            m_Audio.clip = m_MissedClip;
            m_Audio.Play();

            // Wait for the clip to finish.
            yield return new WaitForSeconds(m_MissedClip.length);

            // Tell subscribers that this target is ready to be removed.
            if (OnRemove != null)
                OnRemove(this);
        }


        private IEnumerator GameOver (float gameTimeRemaining)
        {
            // Wait for the game to end.
            yield return new WaitForSeconds (gameTimeRemaining);

            // If by this point it's already ending, do nothing else.
            if(m_IsEnding)
                yield break;

            // Otherwise this is ending the target's lifetime.
            m_IsEnding = true;

            // Turn off the visual and physical aspects.
            m_Renderer.enabled = false;
            m_Collider.enabled = false;

            // Tell subscribers that this target is ready to be removed.
            if (OnRemove != null)
                OnRemove (this);
        }


        private void HandleDown()
        {
            // If it's already ending, do nothing else.
            if (m_IsEnding)
                return;

            // Otherwise this is ending the target's lifetime.
            m_IsEnding = true;

            // Turn off the visual and physical aspects.
            m_Renderer.enabled = false;
            m_Collider.enabled = false;

            // Play the clip of the target being hit.
            m_Audio.clip = m_DestroyClip;
            m_Audio.Play();

            // Add to the player's score.
            SessionData.AddScore(m_Score);

            // Instantiate the shattered target prefab in place of this target.
            GameObject destroyedTarget = Instantiate(m_DestroyPrefab, transform.position, transform.rotation) as GameObject;

            // Destroy the shattered target after it's time out duration.
            Destroy(destroyedTarget, m_DestroyTimeOutDuration);

            // Tell subscribers that this target is ready to be removed.
            if (OnRemove != null)
                OnRemove(this);
        }
    }
}