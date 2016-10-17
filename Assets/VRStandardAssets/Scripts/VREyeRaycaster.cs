using System;
using UnityEngine;

namespace VRStandardAssets.Utils
{
    // Aby wejść w interakcję z obiektami w scenie.
    // klasa ta rzuca promień w scene, a jeżeli znajdzie
    // VRInteractiveItem to wystawia ją na inne klasy do użycia (for other classes to use.).
    // Skrypt ten powinien być ogólnie umieszczone w Camera.
    public class VREyeRaycaster : MonoBehaviour
    {
        public event Action<RaycastHit> OnRaycasthit;                   // To zdarzenie jest odwoływane w każdej klatce, że wzrok użytkownika jest na zderzaczu.


        [SerializeField] private Transform m_Camera;
        [SerializeField] private LayerMask m_ExclusionLayers;           // Warstwa wyłączająca z raycast. Layers to exclude from the raycast.
		[SerializeField] private Reticle m_Reticle;                     // Siatka(Reticle), jeżeli dotyczy.
        [SerializeField] private VRInput m_VrInput;                     // Uzywane do wywołąnia podstawowego zdarzenia w aktualnym VRInteractiveItem.
//		[SerializeField] private VRInput2 m_VrInput2;
        [SerializeField] private bool m_ShowDebugRay;                   // Opcjonalnie pokaż debugowany promień.
        [SerializeField] private float m_DebugRayLength = 5f;           // Debug ray długość.
        [SerializeField] private float m_DebugRayDuration = 1f;         // Jak długo Debug ray pozostanie widoczny.
        [SerializeField] private float m_RayLength = 500f;              // Jak daleko w głąb sceny jest rzucany promień.

        
        private VRInteractiveItem m_CurrentInteractible;                // Aktualny interaktywny element.
        private VRInteractiveItem m_LastInteractible;                   // Ostani interaktywny element.
                                                                        //		private ExampleTouchpad m_NewInteractible;			// Nowy interaktywny element(cube).

        public bool bPointsAtFriend;

		/*
		 public ExampleTouchpad NewInteractible
		  {
            get { return m_NewInteractible; }
        }
		/*
		private void IsDisable ()
        {
            m_VrInput.OnClick -= HandleClick;
            m_VrInput.OnDoubleClick -= HandleDoubleClick;
            m_VrInput.OnUp -= HandleUp;
            m_VrInput.OnDown -= HandleDown;
        }

		private void IUpdate()
        {
            EyeRaycast();
        }


		*/
		
        // Użyteczności dla innych klas dla odwołania się do obecnego interaktywnego elementu.
        public VRInteractiveItem CurrentInteractible
        {
            get { return m_CurrentInteractible; }
        }

        
        private void OnEnable()
        {
            m_VrInput.OnClick += HandleClick;
            m_VrInput.OnDoubleClick += HandleDoubleClick;
            m_VrInput.OnUp += HandleUp;
            m_VrInput.OnDown += HandleDown;
        }


        private void Disable ()
        {
            m_VrInput.OnClick -= HandleClick;
            m_VrInput.OnDoubleClick -= HandleDoubleClick;
            m_VrInput.OnUp -= HandleUp;
            m_VrInput.OnDown -= HandleDown;
        }


        private void Update()
        {
            EyeRaycast();
        }

      
        private void EyeRaycast()
        {
            // Pokaż debug ray jeśli wymagane.
            if (m_ShowDebugRay)
            {
                Debug.DrawRay(m_Camera.position, m_Camera.forward * m_DebugRayLength, Color.blue, m_DebugRayDuration);
            }

            // Stwórz promień do punktu wskazywanego przez kamere(przed sobą, tam gdzie patrzysz)
            Ray ray = new Ray(m_Camera.position, m_Camera.forward);
            RaycastHit hit;
            
            // Wykonaj raycast do przodu, aby zobaczyć czy trafiliśmy interaktywny element.
            if (Physics.Raycast(ray, out hit, m_RayLength, ~m_ExclusionLayers))
            {
                VRInteractiveItem interactible = hit.collider.GetComponent<VRInteractiveItem>(); //Próba uzyskania dostęp do VRInteractiveItem na objekcie udeżonym.
                m_CurrentInteractible = interactible;

                if ((hit.collider.name == "SwipeCube") || (hit.collider.name == "SwipeCube1") || (hit.collider.name == "SwipeCube2"))
                {
                    bPointsAtFriend = true;
                }
                else
                {
                    bPointsAtFriend = false;
                }


                //if
                //VRInteractiveCube interactible = hit.collider.GetComponent<VRInteractiveCube>(); //Próba uzyskania dostęp do ExampleTouchpad (wcześniej doVRInteractiveCube) na objekcie udeżonym.
                //m_NewInteractible = interactible; }



                // Jeśli trafiliśmy interaktywny element, a to nie jest ten sam co poprzedni interaktywny element wtedy wywołuje Over | If we hit an interactive item and it's not the same as the last interactive item, then call Over
                if (interactible && interactible != m_LastInteractible)
                    interactible.Over();


                // Dezaktywuje ostatni element interaktywny.
                if (interactible != m_LastInteractible)
                    DeactiveLastInteractible();

                m_LastInteractible = interactible;

                // Coś zostało uderzone, ustaw pozycje uderzenia | Something was hit, set at the hit position.
                if (m_Reticle)
                    m_Reticle.SetPosition(hit);

                if (OnRaycasthit != null)
                    OnRaycasthit(hit);
            }
            else
            {
				//_____________________________________________________________________________________________________

				/*

				// Wykonaj raycast do przodu, aby zobaczyć czy trafiliśmy interaktywny element.
				if (Physics.Raycast(ray, out hit, m_RayLength, ~m_ExclusionLayers))
				{
					ExampleTouchpad interactible = hit.collider.GetComponent<ExampleTouchpad>(); //Próba uzyskania dostęp do ExampleTouchpad (wcześniej doVRInteractiveCube) na objekcie udeżonym.
                	m_ExampleTouchpad = interactible;

					// Jeśli trafiliśmy interaktywny element, a to nie jest ten sam co poprzedni interaktywny element wtedy wywołuje Over | If we hit an interactive item and it's not the same as the last interactive item, then call Over
                	if (interactible && interactible != m_LastInteractible)
                    interactible.Over();
					
				*/

				//_____________________________________________________________________________________________________

                // Nic nie zostało trafione, dezaktywuje ostatni element interaktywny.
                DeactiveLastInteractible();
                m_CurrentInteractible = null;

                // Ustawić siatkę/pozycja siatki (reticle) w domyślnej odległości.
                if (m_Reticle)
                    m_Reticle.SetPosition();
            }
        }


        private void DeactiveLastInteractible()
        {
            if (m_LastInteractible == null)
                return;

            m_LastInteractible.Out();
            m_LastInteractible = null;
        }


        private void HandleUp()
        {
            if (m_CurrentInteractible != null)
                m_CurrentInteractible.Up();
        }


        private void HandleDown()
        {
            if (m_CurrentInteractible != null)
                m_CurrentInteractible.Down();
        }


        private void HandleClick()
        {
            if (m_CurrentInteractible != null)
                m_CurrentInteractible.Click();
        }


        private void HandleDoubleClick()
        {
            if (m_CurrentInteractible != null)
                m_CurrentInteractible.DoubleClick();

        }
    }
}