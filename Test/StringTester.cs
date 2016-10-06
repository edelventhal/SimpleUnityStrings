using UnityEngine;

namespace SimpleUnityStrings
{
    public class StringTester : MonoBehaviour
    {
        public void Start()
        {
            //* - To access a string in the root:
            Debug.Log( Strings.Get( "monkey" ) ); //if the language is Spanish, returns "melón", otherwise returns "melon"
            //* - To access a string underneath an object:
            Debug.Log( Strings.Get( "intro/hello" ) ); //Hello, World!
            //* - To access a string underneath multiple objects:
            Debug.Log( Strings.Get( "intro/options/copy" ) ); //Copy
            //* - To substitute values in a returned string:
            Debug.Log( Strings.Get( "intro/bye", "Eli", "Megan" ) ); //Goodbye, Eli! I hope you enjoy seeing Megan!
            //* - To return an array element:
            Debug.Log( Strings.Get( "choices/0" ) ); //Choice A
            Debug.Log( Strings.Get( "choices/1" ) ); //Choice B
            //* - To return an array element that uses a bounded index:
            Debug.Log( Strings.Get( "choices/b-1" ) ); //Choice A
            Debug.Log( Strings.Get( "choices/b0"  ) ); //Choice A
            Debug.Log( Strings.Get( "choices/b1"  ) ); //Choice B
            Debug.Log( Strings.Get( "choices/b2"  ) ); //Choice B
            //* - To return a random array element:
            Debug.Log( Strings.Get( "choices/?" ) ); //Either "Choice A" or "Choice B"
            Debug.Log( Strings.Get( "choices/?" ) ); //Either "Choice A" or "Choice B"
            Debug.Log( Strings.Get( "choices/?" ) ); //Either "Choice A" or "Choice B"
            //* - To return a random array element, enforcing even distribution:
            Debug.Log( Strings.Get( "choices/!" ) ); //Either "Choice A" or "Choice B"
            Debug.Log( Strings.Get( "choices/!" ) ); //Either "Choice A" or "Choice B", whatever was not returned last time
            Debug.Log( Strings.Get( "choices/!" ) ); //Either "Choice A" or "Choice B"
            //* - When you mess up:
            Debug.Log( Strings.Get( "missing/not/there" ) ); //ERROR: "missing/not/there"
        }
    }
}