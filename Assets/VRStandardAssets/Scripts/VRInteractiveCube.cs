using System;
using UnityEngine;

namespace VRStandardAssets.Utils
{
    // Tą klasę należy dodać do dowolnego GameObject w scenie.
    // która powinna reagować na podstawie wprowadzonego wzroku użytkownika. |that should react to input based on the user's gaze.
    // Zawiera zdarzenia, które mogą być subskrybowane przez klasy
    // trzeba wiedzieć o wprowadzeniu konkretnego do tego objektu. |need to know about input specifics to this gameobject.
    public class VRInteractiveCube : MonoBehaviour
    {
        public event Action OnOver;             // Wywoływana, gdy wzrok przesuwa się nad tym obiekcie.
        public event Action OnOut;              // Wywoływana, gdy wzrok opuszcza ten obiekt.
		public event Action OnClick;            // Wywoływana, gdy użyte (input) kliknięcie zostanie wykryte podczas gdy wzrok jest na tym obiekcie.
        public event Action OnDoubleClick;      // Wywoływana, gdy użyto podwójnego kliknięcia, podczas gdy wzrok jest na tym obiekcie.
        public event Action OnSwipe;            // Wywoływana, gdy zostanie naciśnięty Fire1 podczas gdy wzrok jest na tym obiekcie.


        protected bool m_IsOver;


        public bool IsOver
        {
            get { return m_IsOver; }              // Czy wzrok jest obecnie na tym obiekcie?
        }


        // Poniższe funkcje są wywoływane przez VREyeRaycaster gdy wykryte jest odpowiednie wprowadzenie (input). | The below functions are called by the VREyeRaycaster when the appropriate input is detected.
        // Oni z kolei wywołują odpowiednie zdarzenia powinny mieć subscriberów. | They in turn call the appropriate events should they have subscribers.
        public void Over()
        {
            m_IsOver = true;

            if (OnOver != null)
                OnOver();
        }


        public void Out()
        {
            m_IsOver = false;

            if (OnOut != null)
                OnOut();
        }


        /*public void Click()
        {
            if (OnClick != null)
                OnClick();
        }*/


        /*public void DoubleClick()
        {
            if (OnDoubleClick != null)
                OnDoubleClick();
        }*/


        public void Swipe()
        {
            if (OnSwipe != null)
                OnSwipe();
        }
    }
}


/*
 
using System.Collections;

public class VRDisableShooting : MonoBehaviour {
    void OnClickDisable() {
    	GameObject[] gos = GameObject.FindGameObjectsWithTag ("EditorTouch");
        //print("script was removed");
    }
} 




*/