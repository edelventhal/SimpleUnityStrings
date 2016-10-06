using UnityEngine;
using UnityEngine.UI;

namespace SimpleUnityStrings
{
    public class TextLocalizer : MonoBehaviour
    {
        public string key;
        public string[] substitutions;
        
        
        public void Start()
        {
            Text text = GetComponent<Text>();
            if ( text != null )
            {
                text.text = Strings.Get( key, substitutions );
            }
            else
            {
                TextMesh textMesh = GetComponent<TextMesh>();
                if ( textMesh != null )
                {
                    textMesh.text = Strings.Get( key, substitutions );
                }
                else
                {
                    Debug.LogWarning( "TextLocalizer expects to be attached to either a UI Text component or a TextMesh component." );
                }
            }
            
        }
    }
}