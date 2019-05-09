using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using Verse;

namespace DependencyChecker {
	public class DependenciesFile {
        private List<Dependency> dependencies = new List<Dependency>();
		public const string VersionFileDir = "About";
		public const string VersionFileName = "Dependencies.xml";

		public static DependenciesFile TryParseVersionFile(ModContentPack pack) {
			var filePath = Path.Combine(pack.RootDir, Path.Combine(VersionFileDir, VersionFileName));
			if (!File.Exists(filePath)) return null;
			try {
				var doc = XDocument.Load(filePath);
				return new DependenciesFile(doc);
			} catch (Exception e) {
				Log.Error("[DependencyChecker] Exception while parsing version file at path: " + filePath + " Exception was: " + e);
			}
			return null;
		}
        public List<Dependency> Dependencies { 
            get { return dependencies; }
        }
		private DependenciesFile(XDocument doc) {
			ParseXmlDocument(doc);
		}

		private void ParseXmlDocument(XDocument doc) {
			if (doc.Root == null) throw new Exception("Missing root node");  
            var dependenciesElement = doc.Root.Element("dependencies");
            if(dependenciesElement != null)
            {
                foreach (XElement el in dependenciesElement.Elements())
                {
                    dependencies.Add(Deserialize(el));
                }
            }

		}
        static Dependency Deserialize(XElement element)
        {
            var serializer = new XmlSerializer(typeof(Dependency));
            return (Dependency)serializer.Deserialize(element.CreateReader());
        }
    }
}