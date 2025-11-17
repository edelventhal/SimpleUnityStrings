using UnityEngine;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;

/**
 * Simple but powerful interface for localizing strings in Unity. Also
 * useful for general strings management, even if not localizing.
 * - To use, you must create a GameObject and add a Strings component to it.
 *      This should be placed in your very first scene.
 *      Add a JSON file with your strings in it to your project. Then, drag that
 *      JSON file to the "strings" TextAsset value in your GameObject.
 * - Then, simply call Strings.Get( "path/to/string" ) to get any string.
 *
 * Your JSON file should have a root object for each language that matches a value
 * in the SystemLanguage enum. The default language is English and will be used as
 * a fallback, however, you can change the default at any time by changing the 
 * DefaultLanguage property to whatever you want. From there, you can have whatever
 * structure to your strings you want, where you use a "/" to go to sub-objects or
 * array elements.
 *
 * ex:
{
    "English":
    {
        "intro":
        {
            "hello": "Hello, World!",
            "bye": "Goodbye, {0}! I hope you enjoy seeing {1}!",
            "options":
            {
                "copy": "Copy",
                "paste": "Paste"
            }
        },
        "choices":
        [
            "Choice A",
            "Choice B"
        ],
        "monkey": "melon"
    },
    "Spanish":
    {
        "monkey": "melón"
    }
}
 *
 * Strings has a lot of simple but powerful functionality on top of basic string
 * lookups, as you can guess from the example above.
 * - To access a string in the root:
 *      Strings.Get( "monkey" ) //if the language is Spanish, returns "melón", otherwise returns "melon"
 * - To access a string underneath an object:
 *      Strings.Get( "intro/hello" ) //Hello, World!
 * - To access a string underneath multiple objects:
 *      Strings.Get( "intro/options/copy" ) //Copy
 * - To substitute values in a returned string:
 *      Strings.Get( "intro/bye", "Eli", "Megan" ) //Goodbye, Eli! I hope you enjoy seeing Megan!
 * - To return an array element:
 *      Strings.Get( "choices/0" ) //Choice A
 *      Strings.Get( "choices/1" ) //Choice B
 * - To return an array element that uses a bounded index:
 *      Strings.Get( "choices/b-1" ) //Choice A
 *      Strings.Get( "choices/b0"  ) //Choice A
 *      Strings.Get( "choices/b1"  ) //Choice B
 *      Strings.Get( "choices/b2"  ) //Choice B
 * - To return a random array element:
 *      Strings.Get( "choices/?" ) //Either "Choice A" or "Choice B"
 *      Strings.Get( "choices/?" ) //Either "Choice A" or "Choice B"
 *      Strings.Get( "choices/?" ) //Either "Choice A" or "Choice B"
 * - To return a random array element, enforcing even distribution:
 *      Strings.Get( "choices/!" ) //Either "Choice A" or "Choice B"
 *      Strings.Get( "choices/!" ) //Either "Choice A" or "Choice B", whatever was not returned last time
 *      Strings.Get( "choices/!" ) //Either "Choice A" or "Choice B"
 * - When you mess up:
 *      Strings.Get( "missing/not/there" ) //ERROR: "missing/not/there"
 */

namespace SimpleUnityStrings
{
    public class Strings : MonoBehaviour
    {
        public TextAsset strings;
    
        private static JSONNode stringTable;
        private static Dictionary<string,List<int>> remainingArrayIndices;
        protected static Strings instance;
    
        public void Awake()
        {
            if ( instance != null && instance != this )
            {
                Object.Destroy( gameObject );
                return;
            }
            instance = this;
            Object.DontDestroyOnLoad( gameObject );
        
            stringTable = JSON.Parse( strings.text );
            remainingArrayIndices = new Dictionary<string,List<int>>();
            DefaultLanguage = "" + SystemLanguage.English;
        }
    
        public JSONNode LanguageTable
        {
            get
            {
                string language = "" + Application.systemLanguage;
                if ( stringTable[ language ] == null )
                {
                    language = "" + DefaultLanguage;
                }
            
                return stringTable[ language ];
            }
        }
    
        public static JSONNode RawTable
        {
            get
            {
                return instance.LanguageTable;
            }
        }
    
        public static string DefaultLanguage
        {
            get;
            set;
        }
    
        public static bool IsInitialized
        {
            get
            {
                return instance != null;
            }
        }

        //Returns a localized string using the passed key and any substitutions.
        //See comments at the top for details.
        public static string Get( string key, params string[] substitutions )
        {
            JSONNode node = GetNodeForKey( key );
            if ( node == null )
            {
                return "ERROR: \"" + key + "\"";
            }
        
            return GetSubstitutedString( node.Value, substitutions );
        }
    
        public static int GetCount( string key )
        {
            JSONNode node = GetNodeForKey( key );
            if ( node == null )
            {
                return  -1;
            }
        
            return node.Count;
        }
    
        public static string Capitalize(string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            var sb = new StringBuilder(str.Length);
            bool capitalizeNext = true;

            foreach (char c in str)
            {
                if (capitalizeNext)
                {
                    sb.Append(char.ToUpper(c));
                    capitalizeNext = false;
                }
                else
                {
                    capitalizeNext = char.IsWhiteSpace(c);
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        public static string CapitalizeFirstOnly(string str)
        {
            return string.Concat(str[0].ToString().ToUpper(), str[1..]);
        }

        private static JSONNode GetNodeForKey( string key )
        {
            if ( instance == null )
            {
                return "NOINSTANCE: \"" + key + "\"";
            }
			
            if ( key == null )
            {
                return "ERROR: null key";
            }
        
            JSONNode languageTable = instance.LanguageTable;
        
            string[] keyParts = key.Split( new char[]{ '/' } );
            string parentKey = "";
            JSONNode node = languageTable;
            for ( int partIndex = 0; partIndex < keyParts.Length; partIndex++ )
            {
                if ( node == null )
                {
                    return "ERROR: \"" + key + "\"";
                }
            
                string keyPart = keyParts[ partIndex ];
            
                JSONArray arr = node.AsArray;
                if ( arr != null )
                {
                    node = GetArrayValue( arr, keyPart, parentKey );
                }
                else
                {
                    node = node[ keyPart ];
                }
            
                parentKey += keyPart;
            }
        
            return node;
        }
    
        private static string GetSubstitutedString( string str, string[] substitutions )
        {
            if ( substitutions.Length <= 0 )
            {
                return str;
            }
                
            if ( !str.Contains( "{" ) )
            {
                return str;
            }
        
            StringBuilder buffer = new StringBuilder( str );
            for ( int substitutionIndex = 0; substitutionIndex < substitutions.Length; substitutionIndex++ )
            {
                buffer = buffer.Replace( "{" + substitutionIndex + "}", substitutions[ substitutionIndex ] );
            }
            return buffer.ToString();
        }
    
        private static JSONNode GetArrayValue( JSONArray arr, string keyPart, string parentKey )
        {
            if ( keyPart == "?" )
            {
                return GetRandomElement( arr );
            }
            else if ( keyPart == "!" )
            {
                return GetRandomExcludedElement( arr, parentKey );
            }
            else if ( keyPart[ 0 ] == 'b' )
            {
                return GetBoundedElement( arr, keyPart.Substring( 1 ) );
            }
            else
            {
                return GetElement( arr, keyPart );
            }
        }
    
        private static JSONNode GetRandomElement( JSONArray arr )
        {
            if ( arr.Count <= 0 )
            {
                return null;
            }
        
            return arr[ Random.Range( 0, arr.Count ) ];
        }
    
        private static JSONNode GetRandomExcludedElement( JSONArray arr, string key )
        {
            if ( arr.Count <= 0 )
            {
                return null;
            }
        
            //create the list for this key if it does not yet exist
            if ( !remainingArrayIndices.ContainsKey( key ) )
            {
                remainingArrayIndices[ key ] = new List<int>();
            }
        
            //if the list is out of options, populate it
            if ( remainingArrayIndices[ key ].Count <= 0 )
            {
                for ( int index = 0; index < arr.Count; index++ )
                {
                    remainingArrayIndices[ key ].Add( index );
                }
            }
        
            //return a random element from the list, and remove that from the list
            int randIndex = Random.Range( 0, remainingArrayIndices[ key ].Count );
            int arrIndex = remainingArrayIndices[ key ][ randIndex ];
            remainingArrayIndices[ key ].RemoveAt( randIndex );
            return arr[ arrIndex ];
        }
    
        private static JSONNode GetBoundedElement( JSONArray arr, string key )
        {
            if ( arr.Count <= 0 )
            {
                return null;
            }
        
            try
            {
                int keyIndex = int.Parse( key );
                if ( keyIndex < 0 )
                {
                    keyIndex = 0;
                }
                else if ( keyIndex >= arr.Count )
                {
                    keyIndex = arr.Count - 1;
                }
                return arr[ keyIndex ];
            }
            catch
            {
                return null;
            }
        }
    
        private static JSONNode GetElement( JSONArray arr, string key )
        {
            try
            {
                return arr[ int.Parse( key ) ];
            }
            catch
            {
                return null;
            }
        }
    }
}