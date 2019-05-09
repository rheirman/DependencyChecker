using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;

namespace DependencyChecker
{
    // do not move or rename. Has detection by full class name
    [StaticConstructorOnStartup]
    public class DependencyChecker : Mod {
		private const string LibraryModName = "DependencyChecker";
		private const string MissingLibraryTitle = "Missing dependencies";
		private const string MissingLibraryMessage = "<b>{0}</b> depends on the following mods. Make sure to install them, and place them ABOVE {0} in the load order!";
		private const string ImproperLoadOrderTitle = "Improper mod load order";
		private const string ImproperLoadOrderMessage = "The following mods should be placed ABOVE <b>{0}</b> in the load order: ";

		// do not rename- referenced by reflection
		public static bool ChecksPerformed;

		// entry point
		public DependencyChecker(ModContentPack content) : base(content) {
            Log.Message("DependencyChecker called!");
			RunAllChecks();
		}

		private void RunAllChecks() {
			try {
				if (ChecksAlreadyPerformed()) {
					return;
				}
				List<LibraryRelatedMod> relatedMods = EnumerateLibraryRelatedMods();

                foreach(LibraryRelatedMod mod in relatedMods)
                {
                    List<Dependency> missingDependencies = MissingDependencies(mod);
                    if (missingDependencies.Count > 0)
                    {
					    ScheduleDialog(MissingLibraryTitle, String.Format(MissingLibraryMessage, mod.name), missingDependencies, true);
                    }
                    else
                    {
                        List<Dependency> misloadedDependencies = MisLoadedDependencies(mod);
                        if (misloadedDependencies.Count > 0)
                        {
                            ScheduleDialog(ImproperLoadOrderTitle, ImproperLoadOrderMessage, misloadedDependencies, false);
                        }
                    }
                   
				}
			} catch (Exception e) {
				Log.Error("An exception was caused by the HugsLibChecker assembly. Exception was: "+e);
			}
		}

		private bool ChecksAlreadyPerformed() {
			var checkerTypeName = typeof(DependencyChecker).FullName;
			var checkerTypes = AppDomain.CurrentDomain.GetAssemblies()
				.Select(a => a == null ? null : a.GetType(checkerTypeName, false)).Where(t => t != null);
			var anyAssemblyAlreadyPerformedChecks = checkerTypes.Select(t => t.GetField("ChecksPerformed", BindingFlags.Public | BindingFlags.Static))
				.Any(f => f != null && (bool)f.GetValue(null));
			if (anyAssemblyAlreadyPerformedChecks) {
				return true;
			}
			ChecksPerformed = true;
			return false;
		}

		private List<LibraryRelatedMod> EnumerateLibraryRelatedMods() {
			var checkerTypeName = typeof(DependencyChecker).FullName;
			var relatedMods = new List<LibraryRelatedMod>();
			foreach (var modContentPack in LoadedModManager.RunningMods) {
                var versionFile = DependenciesFile.TryParseVersionFile(modContentPack);
				var checkerAssemblies = modContentPack.assemblies.loadedAssemblies.Where(a => a.GetType(checkerTypeName, false) != null).ToList();
				if (checkerAssemblies.Count > 0 || versionFile != null) {
                    if(versionFile == null)
                    {
                        Log.Warning("Couldn't parse version file for " + modContentPack.Name);
                    }
					relatedMods.Add(new LibraryRelatedMod(modContentPack.Name, versionFile, checkerAssemblies));
				}
			}
			return relatedMods;
		}
		private List<Dependency> MissingDependencies(LibraryRelatedMod mod) {
            if(mod.file == null)
            {
                return new List<Dependency>();
            }
            List<Dependency> missingDependencies = mod.file.Dependencies;

            foreach (ModContentPack pack in LoadedModManager.RunningMods ) {
                missingDependencies.RemoveAll((Dependency d) => d.modName == pack.Name);
			}
			return missingDependencies;
		}
        private List<Dependency> MisLoadedDependencies(LibraryRelatedMod mod)
        {
            if (mod.file == null)
            {
                return new List<Dependency>();
            }
            List<Dependency> missingDependencies = mod.file.Dependencies;
            bool modEncountered = false;
            foreach (ModContentPack pack in LoadedModManager.RunningMods)
            {
                if(pack.Name == mod.name)
                {
                    modEncountered = true;
                }
                if (modEncountered)
                {
                    missingDependencies.RemoveAll((Dependency d) => d.modName == pack.Name);
                }
            }
            return missingDependencies;
        }
        private static string GetAssemblyTitle (Assembly assembly)
        {
            return (assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false)[0] as AssemblyTitleAttribute).Title;
        }

		private void ScheduleDialog(string title, string message, List<Dependency> violatedDependencies, bool showDownloadButton) {
			LongEventHandler.QueueLongEvent(() => {
				Find.WindowStack.Add(new Dialog_DependencyViolation(title, message, violatedDependencies, showDownloadButton));
			}, null, false, null);
		}
		
		private class LibraryRelatedMod {
			public readonly string name;
			public readonly DependenciesFile file;
			//public readonly bool isLibrary;
			public readonly List<Assembly> checkerAssemblies; 

			public LibraryRelatedMod(string name, DependenciesFile file, List<Assembly> checkerAssemblies) {
				this.name = name;
				this.file = file;
				this.checkerAssemblies = checkerAssemblies;
				//isLibrary = name == LibraryModName;
			}
		}
	}
}