
		
			JURASSIC UNITY DEMO README
			

	This is a demonstration of how the Jurassic JavaScript interpreter could be used inside Unity3D to provide a run time scripting language.  Run the project and click one of the sample script buttons on the right side to load it.  While the "running" box is checked the script will be executed once per Update().  Uncheck this before you edit scripts or you will get a lot of errors.  You can click "Test Execute" to execute your script one time. (Good to avoid error spam)

	The user functions available in this demo are:

	• These provide get/set for the GameObject's transform.position.
		double GetX()
		double GetY()
		double GetZ()
		jsVectorInstance GetPos()
		void SetPos(double x, double y, double z)
		void SetPosVec(jsVectorInstance pos)

	• These get/set a GUI text string attached to the GameObject.
		void SetText(string text)
		string GetText()

	• The custom type Vector can be instantiated with a constructor.
		new Vector(double x, double y, double z)

	Jurassic includes suport for all the regular JavaScript functions such as arrays, string operations, conversions, Math functions, regular expressions, etc. 

	The included Jurassic.dll is a .NET 4.0 assembly built in visual studio with some of the file io functions removed to allow it to run inside a Web Player.  Detailed info here: http://forum.unity3d.com/threads/83456-Javascript-engine-inside-a-Unity-game-%28aka-user-programmable-games%29?highlight=jurassic

	The JurassicExampleExecute script is the core of the project and contains commented code demonstrating how to:

		• Instantiate an instance of the Jurassic engine.
	
		• Expose .NET classes to JavaScript using
		  the "seamless .NET interop" feature.
	
		• Expose .NET functions to JavaScript using delegates.
	
		• Create a class that can be exposed to JavaScript by
		  inheriting from Jurassic.Library.ObjectInstance.
	
		• Create a class that can be instantiated in JavaScript
		  with with a constructor using the "new" keyword.

	
	For more in-depth information please visit the Jurassic web site. http://jurassic.codeplex.com/
