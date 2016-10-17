using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using VRStandardAssets.Common;
using VRStandardAssets.Utils;

namespace VRStandardAssets.ShootingGallery
{
    // Klasa ta kontroluje przepływ gier strzelanych | This class controls the flow of the shooter games.  It
    // Zawiera wstęp, spawn celów oraz| includes the introduction, spawning of targets and
    // outro. | Cele pojawiają się w systemie który sprawia | Targets are spawned with a system that makes
    // , że spawnowanie jest bardziej prawdopodobne gdy jest ich mniej. | spawnning more likely when there are fewer.
    public class ShootingGalleryController : MonoBehaviour
    {
        [SerializeField] private SessionData.GameType m_GameType;       // Czy to jest arena 180 czy 360
		[SerializeField] private int m_IdealTargetNumber = 7;           // Ile celi ma być wyświetlanych jednocześnie.(5?)
        [SerializeField] private float m_BaseSpawnProbability = 0.7f;   // Gdy jest idealna ilość celi, to jest prawdopodobieństwo, że kolejny się odnowi.
		[SerializeField] private float m_GameLength = 75f;              // Czas trwania gry w sekundach.(60?)
        [SerializeField] private float m_SpawnInterval = 1f;            // Z jaką czestotliwością cel może się odnawiać.
        [SerializeField] private float m_EndDelay = 1.5f;               // Czas który użytkownik potrzebuje odczekać pomiędzy zakończeniem UI, a byciem w stanie kontynuować.
        [SerializeField] private float m_SphereSpawnInnerRadius = 5f;   // Dla arena 360, najbliższe cele mogą się odnowić.
        [SerializeField] private float m_SphereSpawnOuterRadius = 10f;  // Dla arena 360, najdalsze cele mogą się odnowić.
        [SerializeField] private float m_SphereSpawnMaxHeight = 15f;    // Dla arena 360, najwyżej cele mogą się odnowić.
        [SerializeField] private SelectionSlider m_SelectionSlider;     // Służy do potwiedzenia, że użytkownik rozumie wprowadznie UI.
        [SerializeField] private Transform m_Camera;                    // Służy do określenia, gdzie cele mogą się pojawiać.
        [SerializeField] private SelectionRadial m_SelectionRadial;     // Służy do kontynuowania po zakończeniu.
        [SerializeField] private Reticle m_Reticle;                     // Jest włączany i wyłączany, gdy jest to wymagane i nie. <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
        [SerializeField] private Image m_TimerBar;                      // Pozostały czas jest wyświetlany w interfejsie użytkownika przy pistolecie. Jest to odniesienie do obrazu, pokazującego pozostały czas.
		[SerializeField] private ObjectPool m_TargetObjectPool;         // (the object pool) Pole objektu które przechowuje wszystkie cele.
        [SerializeField] private BoxCollider m_SpawnCollider;           // Dla arena 180, wielkość, że cele mogą pojawiać się wewnątrz.
        [SerializeField] private UIController m_UIController;           // Służy do hermetyzacji UI.
        [SerializeField] private InputWarnings m_InputWarnings;         // Ostrzeżenia przytrzymania muszą znajdować się po rozpoczęciu i zakończeniu, ale wyłączone dla samej gry.


        private float m_SpawnProbability;                               // Obecny prawdopodobieństwo, że cel będzie odnowiony w następnym odstępie czasu.
        private float m_ProbabilityDelta;                               // Różnica w stosunku do prawdopodobieństwa spowodowanego przez cel odnawiany lub nieodnawiany.


        public bool IsPlaying { get; private set; }                     // To, czy gra jest aktualnie grana.


        private IEnumerator Start()
        {
            // Ustaw typ gry dla wyniku żeby ten był poprawnie zapisany.
            SessionData.SetGameType(m_GameType);

			// Różnica prawdopodobieństwa dla każdego odnowienia jest różnica między 100% a podstawowe prawdopodobieństwo na liczbę oczekiwanych celi.
			// To znaczy, że jeśli idealna liczba celi była 5, a podstawowe prawdopodobieństwo to 0.8 wtedy delta jest 0.06.
			// Więc jeśli tam nie ma celi to prawdopodobieństwo, że jeden się odnowi będzie 1 wtedy 0.94 a wtedy 0.88 itd.
            m_ProbabilityDelta = (1f - m_BaseSpawnProbability) / m_IdealTargetNumber;

            // Kontynuowanie pętli przez wszystkie fazy.
            while (true)
            {
                yield return StartCoroutine (StartPhase ());
                yield return StartCoroutine (PlayPhase ());
                yield return StartCoroutine (EndPhase ());
            }
        }


        private IEnumerator StartPhase ()
        {
            // Wait for the intro UI to fade in.
            yield return StartCoroutine (m_UIController.ShowIntroUI ());

            // Show the reticle (since there is now a selection slider) and hide the radial.
            m_Reticle.Show ();
            m_SelectionRadial.Hide ();

            // Turn on the tap warnings for the selection slider.
            m_InputWarnings.TurnOnDoubleTapWarnings ();
            m_InputWarnings.TurnOnSingleTapWarnings ();

            // Wait for the selection slider to finish filling.
            yield return StartCoroutine (m_SelectionSlider.WaitForBarToFill ());

            // Turn off the tap warnings since it will now be tap to fire.
            m_InputWarnings.TurnOffDoubleTapWarnings ();
            m_InputWarnings.TurnOffSingleTapWarnings ();

            // Wait for the intro UI to fade out.
            yield return StartCoroutine (m_UIController.HideIntroUI ());
        }


        private IEnumerator PlayPhase ()
        {
            // Wait for the UI on the player's gun to fade in.
            yield return StartCoroutine(m_UIController.ShowPlayerUI());

            // The game is now playing.
            IsPlaying = true;

            // Make sure the reticle is being shown.
            m_Reticle.Show ();

            // Reset the score.
            SessionData.Restart ();

            // Wait for the play updates to finish.
            yield return StartCoroutine (PlayUpdate ());

            // Wait for the gun's UI to fade.
            yield return StartCoroutine(m_UIController.HidePlayerUI());

            // The game is no longer playing.
            IsPlaying = false;
        }


        private IEnumerator EndPhase ()
        {
            // Hide the reticle since the radial is about to be used.
            m_Reticle.Hide ();
            
            // In order, wait for the outro UI to fade in then wait for an additional delay.
            yield return StartCoroutine (m_UIController.ShowOutroUI ());
            yield return new WaitForSeconds(m_EndDelay);
            
            // Turn on the tap warnings.
            m_InputWarnings.TurnOnDoubleTapWarnings();
            m_InputWarnings.TurnOnSingleTapWarnings();

            // Wait for the radial to fill (this will show and hide the radial automatically).
            yield return StartCoroutine(m_SelectionRadial.WaitForSelectionRadialToFill());

            // The radial is now filled so stop the warnings.
            m_InputWarnings.TurnOffDoubleTapWarnings();
            m_InputWarnings.TurnOffSingleTapWarnings();

            // Wait for the outro UI to fade out.
            yield return StartCoroutine(m_UIController.HideOutroUI());
        }


        private IEnumerator PlayUpdate ()
        {
            // When the updates start, the probability of a target spawning is 100%.
            m_SpawnProbability = 1f;

            // The time remaining is the full length of the game length.
            float gameTimer = m_GameLength;

            // The amount of time before the next spawn is the full interval.
            float spawnTimer = m_SpawnInterval;

            // While there is still time remaining...
            while (gameTimer > 0f)
            {
                // ... check if the timer for spawning has reached zero.
                if (spawnTimer <= 0f)
                {
                    // If it's time to spawn, check if a spawn should happen based on the probability.
                    if (Random.value < m_SpawnProbability)
                    {
                        // If a spawn should happen, restart the timer for spawning.
                        spawnTimer = m_SpawnInterval;

                        // Decrease the probability of a spawn next time because there are now more targets.
                        m_SpawnProbability -= m_ProbabilityDelta;

                        // Spawn a target.
                        Spawn (gameTimer);
                    }
                }

                // Wait for the next frame.
                yield return null;

                // Decrease the timers by the time that was waited.
                gameTimer -= Time.deltaTime;
                spawnTimer -= Time.deltaTime;

                // Set the timer bar to be filled by the amount 
                m_TimerBar.fillAmount = gameTimer / m_GameLength;
            }
        }


        private void Spawn (float timeRemaining)
        {
            // Get a reference to a target instance from the object pool.
            GameObject target = m_TargetObjectPool.GetGameObjectFromPool ();

            // Set the target's position to a random position. 
            target.transform.position = SpawnPosition();

            // Znajdź odwołanie do skryptu ShootingTarget na 'cel' typu gameobject i ?nazywaj? to funkcją Restart. | Find a reference to the ShootingTarget script on the target gameobject and call it's Restart function.
            ShootingTarget shootingTarget = target.GetComponent<ShootingTarget>();
            shootingTarget.Restart(timeRemaining);

            /*ShootingTarget2 shootingTarget2 = target.GetComponent<ShootingTarget2>();
            shootingTarget.Restart(timeRemaining);*/

            // Subscribe to the OnRemove event.
            shootingTarget.OnRemove += HandleTargetRemoved;

        }


        private Vector3 SpawnPosition ()
        {
            // If this game is a 180 game then the random spawn position should be within the given collider.
            if (m_GameType == SessionData.GameType.SHOOTER180)
            {
                // Find the centre and extents of the spawn collider.
                Vector3 center = m_SpawnCollider.bounds.center;
                Vector3 extents = m_SpawnCollider.bounds.extents;

                // Get a random value between the extents on each axis.
                float x = Random.Range(center.x - extents.x, center.x + extents.x);
                float y = Random.Range(center.y - extents.y, center.y + extents.y);
                float z = Random.Range(center.z - extents.z, center.z + extents.z);
                
                // Return the point these random values make.
                return new Vector3(x, y, z);
            }

            // Otherwise the game is 360 and the spawn should be within a cylinder shape.
            // Find a random point on a unit circle and give it a radius between the inner and outer radii.
            Vector2 randomCirclePoint = Random.insideUnitCircle.normalized *
                                  Random.Range (m_SphereSpawnInnerRadius, m_SphereSpawnOuterRadius);

            // Find a random height between the camera's height and the maximum.
            float randomHeight = Random.Range (m_Camera.position.y, m_SphereSpawnMaxHeight);

            // The the random point on the circle is on the XZ plane and the random height is the Y axis.
            return new Vector3(randomCirclePoint.x, randomHeight, randomCirclePoint.y);
        }


        private void HandleTargetRemoved(ShootingTarget target)
        {
            // Now that the event has been hit, unsubscribe from it.
            target.OnRemove -= HandleTargetRemoved;

            // Return the target to it's object pool.
            m_TargetObjectPool.ReturnGameObjectToPool (target.gameObject);

            // Increase the likelihood of a spawn next time because there are fewer targets now.
            m_SpawnProbability += m_ProbabilityDelta;
        }
    }
}