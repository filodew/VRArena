using System;
using UnityEngine;

namespace VRStandardAssets.Utils
{
    // Klasa ta oddaje cały wkład wymagany do większości gier VR.
    // Zawiera zdarzenia, które mogą być subskrybowane przez klasy, które wymagają określonego wprowadzenia(input).
    // Klasa ta musi istnieć w każdej scenie i może być również załączona do głównego
    // aparatu w celu ułatwienia.
    public class VRInput : MonoBehaviour
    {
        //Swipe directions
        public enum SwipeDirection
        {
            NONE,
            UP,
            DOWN,
            LEFT,
            RIGHT
        };
            
        public bool MouseControl = false;

        public event Action<SwipeDirection> OnSwipe;                // Wywoływana każda klatka przekazując swipe, w tym jeśli nie ma machnięcia(swipe).
        public event Action OnClick;                                // Wywoływana, gdy Fire1 zostaje zwolniony i nie jest to podwójne kliknięcie.
        public event Action OnDown;                                 // Called when Fire1 is pressed.
		public event Action OnUp;                                   // Called when Fire1 is released (zwolniony).
        public event Action OnDoubleClick;                          // Called when a double click is detected.
		public event Action OnLongDown;
        //public event Action OnCancel;                               // Called when Cancel is pressed.

        [SerializeField] private float m_DoubleClickTime = 0.3f;    //The max time allowed between double clicks
		[SerializeField] private float m_SwipeWidth = 0.3f;         //Szerokość machnięcia (swipe)
		[SerializeField] private float m_OnLongDown = 1.5f;			//długość  przytrzymania Fire1

        
        private Vector2 m_MouseDownPosition;                        // The screen position of the mouse when Fire1 is pressed.
        private Vector2 m_MouseUpPosition;                          // The screen position of the mouse when Fire1 is released.
        private float m_LastMouseUpTime;                            // Czas, kiedy Fire1 ostatnio zwolniony.
        private float m_LastHorizontalValue;                        // Poprzednia wartość osi poziomej wykorzystywane do wykrywania swipes klawiszowych.
        private float m_LastVerticalValue;                          // Poprzednia wartość osi pionowej wykorzystywane do wykrywania swipes klawiszowych.

        // MOUSE MOVEMENT
        private Transform CameraTransform = null;
        // ~MOUSE MOVEMENT

        public float DoubleClickTime{ get { return m_DoubleClickTime; } }

        private void Start()
        {
            if (CameraTransform == null)
            {
                CameraTransform = transform;
            }
            if (MouseControl == true)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        private void Update()
        {
            CheckInput();
        }


        private void CheckInput()
        {
            
            // Ustaw domyślnie swipe być 'none'.
            SwipeDirection swipe = SwipeDirection.NONE;

            if (Input.GetButtonDown("Fire1"))
            {
				// Gdy Fire1 jest wciśnięty nagrywaj(wykrywaj(record)) pozycję myszy.
                m_MouseDownPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            
                // Jeśli coś zostało objęte OnDown wezwać (odwołać do tego) to.
                if (OnDown != null)
                    OnDown();
			/*	else if (Physics.Raycast(ray, out hit, m_RayLength, ~m_ExclusionLayers))

			
			 {
                VRInteractiveCube interactible = hit.collider.GetComponent<VRInteractiveCube>(); //Próba uzyskania dostęp do VRInteractiveCube na objekcie udeżonym.
                m_CurrentInteractible = interactible; 
             */
            }

            // Ten if jest zebranie informacji o myszy, gdy przycisk jest w górze.
            if (Input.GetButtonUp ("Fire1"))
            {
                // Gdy Fire1 jest wyzwolony nagrywaj(wykrywaj(record)) pozycję myszy.
                m_MouseUpPosition = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);

                // Wykrywa kierunek między położeniami myszy, gdy Fire1 zostanie naciśnięty i zwolniony.
                swipe = DetectSwipe ();
            }

            // Jeśli nie było machnięcia ta ramka z myszą, sprawdź machnięcia klawiatury.
            if (swipe == SwipeDirection.NONE)
                swipe = DetectKeyboardEmulatedSwipe();

            // Jeśli są jacyś subskrybenci OnSwipe wezwać je przekazując wykryte machnięce.
            if (OnSwipe != null)
                OnSwipe(swipe);

            // Ten if ma wyzwalać zdarzenia w oparciu o informacje zebrane wcześniej. | This if statement is to trigger events based on the information gathered before.
            if(Input.GetButtonUp ("Fire1"))
            {
                // Jeśli coś zostało objęte OnUp to wzywa to. | If anything has subscribed to OnUp call it.
                if (OnUp != null)
                    OnUp();

                // Jeżeli czas pomiędzy ostatnim zwolnieniem Fire 1, a teraz jest mniejszy
                // niż dozwolony czas podwójnego kliknięcia to jest to podwójne kliknięcie.
                if (Time.time - m_LastMouseUpTime < m_DoubleClickTime)
                {
                    // Jeśli są jacyś subskrybenci OnDoubleClick wezwać (odwołać do tego) to.
                    if (OnDoubleClick != null)
                        OnDoubleClick();
                    //to nie jest użyte w tym projekci, ale jest gotowy inny sampel. zaraz go otworze
                }
                else
                {
                    // Jeśli to nie jest podwójne kliknięcie, to jest to jednym kliknięciem.
                    // If anything has subscribed to OnClick wezwać (odwołać do tego) to
                    if (OnClick != null)
                        OnClick();
                }

                // Record the time when Fire1 is released.
                m_LastMouseUpTime = Time.time;
            }

			// UWAGA TEST
			if (Time.time - m_OnLongDown  >= m_OnLongDown)
			{
				
				if (OnLongDown != null)
                        OnLongDown();
			}


            // Jeśli przycisk Cancel zostanie wciśnięty i jeśli są jacyś subskrybenci OnCancel call it.
          // if (Input.GetButtonDown("Cancel"))
           // {
           //     if (OnCancel != null)
           //         OnCancel();
           // }

            if (MouseControl == true)
            {
                float tiltX = Input.GetAxis("Mouse X");
                float tiltY = Input.GetAxis("Mouse Y");

                transform.Rotate(-tiltY, tiltX, 0);
            }

        }
            
        private SwipeDirection DetectSwipe ()
        {
            // Get the direction from the mouse position when Fire1 is pressed to when it is released.
            Vector2 swipeData = (m_MouseUpPosition - m_MouseDownPosition).normalized;

            // If the direction of the swipe has a small width it is vertical.
            bool swipeIsVertical = Mathf.Abs (swipeData.x) < m_SwipeWidth;

            // If the direction of the swipe has a small height it is horizontal.
            bool swipeIsHorizontal = Mathf.Abs(swipeData.y) < m_SwipeWidth;

            // If the swipe has a positive y component and is vertical the swipe is up.
            if (swipeData.y > 0f && swipeIsVertical)
                return SwipeDirection.UP;

            // If the swipe has a negative y component and is vertical the swipe is down.
            if (swipeData.y < 0f && swipeIsVertical)
                return SwipeDirection.DOWN;

            // If the swipe has a positive x component and is horizontal the swipe is right.
            if (swipeData.x > 0f && swipeIsHorizontal)
                return SwipeDirection.RIGHT;

            // If the swipe has a negative x component and is vertical the swipe is left.
            if (swipeData.x < 0f && swipeIsHorizontal)
                return SwipeDirection.LEFT;

            // If the swipe meets none of these requirements there is no swipe.
            return SwipeDirection.NONE;
        }


        private SwipeDirection DetectKeyboardEmulatedSwipe ()
        {
            // Store the values for Horizontal and Vertical axes.
            float horizontal = Input.GetAxis ("Horizontal");
            float vertical = Input.GetAxis ("Vertical");

            // Store whether there was horizontal or vertical input before.
            bool noHorizontalInputPreviously = Mathf.Abs (m_LastHorizontalValue) < float.Epsilon;
            bool noVerticalInputPreviously = Mathf.Abs(m_LastVerticalValue) < float.Epsilon;

            // The last horizontal values are now the current ones.
            m_LastHorizontalValue = horizontal;
            m_LastVerticalValue = vertical;

            // If there is positive vertical input now and previously there wasn't the swipe is up. Jeśli jest dodatnia wejście pionowe teraz i wcześniej nie był machnięcie jest do góry.
            if (vertical > 0f && noVerticalInputPreviously)
                return SwipeDirection.UP;

            // If there is negative vertical input now and previously there wasn't the swipe is down.
            if (vertical < 0f && noVerticalInputPreviously)
                return SwipeDirection.DOWN;

            // If there is positive horizontal input now and previously there wasn't the swipe is right.
            if (horizontal > 0f && noHorizontalInputPreviously)
                return SwipeDirection.RIGHT;

            // If there is negative horizontal input now and previously there wasn't the swipe is left.
            if (horizontal < 0f && noHorizontalInputPreviously)
                return SwipeDirection.LEFT;

            // Jeśli swape nie spełnia żadnego z tych wymogów nie ma machnięcia.
            return SwipeDirection.NONE;
        }
        

        private void OnDestroy()
        {
            // Ensure that all events are unsubscribed when this is destroyed. | Upewnić się, że wszystkie wydarzenia są wypisany kiedy jest zniszczona.
            OnSwipe = null;
            OnClick = null;
            OnDoubleClick = null;
            OnDown = null;
            OnUp = null;
        }
    }
}