# DependencyChecker

Simple dependency checker for Rimworld. It can check whether dependencies are loaded, and if they are loaded *before* a given mod. If not, an in-game dialog is shown, warning the user, and providing a download link if needed. 

## How to use

Just add the DependencyChecker.dll to the Assemblies folder of your mod. Also add a Dependencies.xml file to the About folder with content as described below. 

## Dependencies.xml 

This file should be added to the About folder of your mod, and should have a similar structure as in the example below. The file describes what dependencies your mod has, and contains download links users are referred to when they miss a dependency.  
```
<?xml version="1.0" encoding="utf-8"?>
<DependencyData>
	<dependencies>
		<Dependency>
			<modName>Giddy-up! Core</modName><!-- exact mod name here !--> 
			<linkSteam>https://steamcommunity.com/sharedfiles/filedetails/?id=1216999901</linkSteam><!-- URL to steam page --> 
			<linkDirect>https://github.com/rheirman/GiddyUpCore/releases/latest</linkDirect><!-- URL to direct download page --> 
		</Dependency>
		<Dependency>
			<modName>What the hack?!</modName>
			<linkSteam>https://steamcommunity.com/sharedfiles/filedetails/?id=1505914869</linkSteam>
			<linkDirect>https://github.com/rheirman/WhatTheHack/releases/latest</linkDirect>
		</Dependency>
	</dependencies>
</DependencyData>
```
In this example (taken from Giddy-up! Mechanoids), Giddy-up! Core, and What the hack?! are listed as dependencies, and if they're missing, users will get the following warning dialog: 

## Credits

- UnlimitedHugs. This mod is a heavily modified verion of HugsLibChecker created by UnlimitedHugs. So credits go to him for creating that mod!

