# SimpleUnityStrings
Simple but powerful interface for localizing strings in Unity. Also useful for general strings management, even if not localizing.
- To use, you must create a GameObject and add a Strings component to it.
     This should be placed in your very first scene.
     Add a JSON file with your strings in it to your project. Then, drag that JSON file to the "strings" TextAsset value in your GameObject.
- Then, simply call Strings.Get( "path/to/string" ) to get any string.

Your JSON file should have a root object for each language that matches a value in the SystemLanguage enum. The default language is English and will be used as
a fallback, however, you can change the default at any time by changing the DefaultLanguage property to whatever you want. From there, you can have whatever
structure to your strings you want, where you use a "/" to go to sub-objects or array elements.

ex:

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

Strings has a lot of simple but powerful functionality on top of basic string
lookups, as you can guess from the example above.
- To access a string in the root:
     `Strings.Get( "monkey" ) //if the language is Spanish, returns "melón", otherwise returns "melon"`
- To access a string underneath an object:
     `Strings.Get( "intro/hello" ) //Hello, World!`
- To access a string underneath multiple objects:
     `Strings.Get( "intro/options/copy" ) //Copy`
- To substitute values in a returned string:
     `Strings.Get( "intro/bye", "Eli", "Megan" ) //Goodbye, Eli! I hope you enjoy seeing Megan!`
- To return an array element:
     `Strings.Get( "choices/0" ) //Choice A`
     `Strings.Get( "choices/1" ) //Choice B`
- To return an array element that uses a bounded index:
     `Strings.Get( "choices/b-1" ) //Choice A`
     `Strings.Get( "choices/b0"  ) //Choice A`
     `Strings.Get( "choices/b1"  ) //Choice B`
     `Strings.Get( "choices/b2"  ) //Choice B`
- To return a random array element:
     `Strings.Get( "choices/?" ) //Either "Choice A" or "Choice B"`
     `Strings.Get( "choices/?" ) //Either "Choice A" or "Choice B"`
     `Strings.Get( "choices/?" ) //Either "Choice A" or "Choice B"`
- To return a random array element, enforcing even distribution:
     `Strings.Get( "choices/!" ) //Either "Choice A" or "Choice B"`
     `Strings.Get( "choices/!" ) //Either "Choice A" or "Choice B", whatever was not returned last time`
     `Strings.Get( "choices/!" ) //Either "Choice A" or "Choice B"`
- When you mess up:
     `Strings.Get( "missing/not/there" ) //ERROR: "missing/not/there"`

A simple TextLocalizer.cs MonoBehaviour is also included which can be attached to any UI Text or TextMesh in order to localize the string in that field.

You can open up the StringsTest.unity scene in the Test folder to see example usage in action.