using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Steam;

namespace DependencyChecker
{
	public class Dialog_DependencyViolation : Window {
		private const string DownloadButtonCaption = "Download";
        private const string WarningLabel = "Note: most mods don't have a warning dialog like this. Therefore, always take a look at the mod's page to see if there are any dependencies before installing it.";
		private readonly Color DownloadButtonColor = Color.green;
        private static float lineHeight = 24f;
        private readonly Vector2 DownloadButtonSize = new Vector2(160, lineHeight - 2f);
        private List<Dependency> violatedDependencies;

		private readonly string title;
		private readonly string message;
		private readonly bool showDownloadButton;
		private bool closedLogWindow;
		public override Vector2 InitialSize {
			get { return new Vector2(500f, 400f); }
		}
		
		public Dialog_DependencyViolation(string title, string message, List<Dependency> violatedDependencies, bool showDownloadButton) {
			this.title = title;
			this.message = message;
			this.showDownloadButton = showDownloadButton;
            this.violatedDependencies = violatedDependencies;

            closeOnCancel = true;
			doCloseButton = false;
			doCloseX = false;
			forcePause = true;
			absorbInputAroundWindow = true;
            //doWindowBackground = true;
            onlyOneOfTypeAllowed = false;


        }

		public override void PostClose() {
			base.PostClose();
            List <Window> windows = Find.WindowStack.Windows.ToList<Window>();
			if (windows.Count((Window w) => w is Dialog_DependencyViolation) == 0) {
				EditWindow_Log.wantsToOpen = true;
			}
		}

		public override void DoWindowContents(Rect inRect) {
			var logWindow = Find.WindowStack.WindowOfType<EditWindow_Log>();
			if (logWindow != null) {
				// hide the log window while we are open
				logWindow.Close(false);
				closedLogWindow = true;
			}
			Text.Font = GameFont.Medium;
			var titleRect = new Rect(inRect.x, inRect.y, inRect.width, 40);
			Widgets.Label(titleRect, title);
			Text.Font = GameFont.Small;
            Widgets.Label(new Rect(inRect.x, inRect.y + titleRect.height, inRect.width, lineHeight * 3), message);
            float offset = lineHeight * 3 + inRect.y + titleRect.height;
            foreach (Dependency dep in violatedDependencies)
            {
                Widgets.Label(new Rect(inRect.x, offset, inRect.width/2, lineHeight), "- " + dep.modName);
                if (showDownloadButton)
                {
                    var prevColor = GUI.color;
                    GUI.color = DownloadButtonColor;
                    var downloadButtonRect = new Rect(inRect.x + inRect.width - DownloadButtonSize.x, offset, DownloadButtonSize.x, DownloadButtonSize.y);
                    if (Widgets.ButtonText(downloadButtonRect, DownloadButtonCaption))
                    {
                        Close();
                        var url = SteamManager.Initialized ? dep.linkSteam : dep.linkDirect;
                        Application.OpenURL(url);
                    }
                    GUI.color = prevColor;
                }
                offset += lineHeight;
            }
            if (inRect.height < offset + CloseButSize.y + lineHeight * 3)
            {
                inRect.height = offset + CloseButSize.y + lineHeight * 3;
            }
            Rect closeButtonRect;
            Widgets.Label(new Rect(inRect.x, inRect.height - CloseButSize.y - lineHeight * 3, inRect.width, lineHeight * 3), WarningLabel);
            closeButtonRect = new Rect(inRect.width/2f - CloseButSize.x/2f, inRect.height - CloseButSize.y, CloseButSize.x, CloseButSize.y);
			if (Widgets.ButtonText(closeButtonRect, "CloseButton".Translate())) {
				Close();
			}
		}
	}
}