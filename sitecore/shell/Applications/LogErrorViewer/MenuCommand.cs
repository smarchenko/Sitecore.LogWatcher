using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitecore.LogWatcher.Commands
{
    using Sitecore.Configuration;
    using Sitecore.Data;
    using Sitecore.Globalization;
    using Sitecore.Shell.Framework.Commands;
    using Sitecore.Web.UI.HtmlControls;
    using Sitecore.Web.UI.Sheer;

    [Serializable]
    public class MenuCommand : Command
    {
        public override void Execute(CommandContext context)
        {
            Menu contextMenu = new Menu();
            SheerResponse.DisableOutput();
            try
            {
                MenuItem child = new MenuItem();
                contextMenu.Controls.Add(child);
                child.Header = "Clear";
                child.Icon = "Applications/16x16/bullet_ball_green.png";
                child.Click = "logerrorwatcher:clear";//"/sitecore/shell/Applications/LogErrorViewer/LogWatcher.ashx?clear=1";
                child.Checked = false;

                child = new MenuItem();
                contextMenu.Controls.Add(child);
                child.Header = "Open Messages";
                child.Icon = "Applications/16x16/edit.png";
                child.Click = "logerrorwatcher:openmessages";
                child.Checked = false;
            }
            finally
            {
                SheerResponse.EnableOutput();
            }

            SheerResponse.ShowContextMenu("LogErrorWatcher", "above", contextMenu);
        }
    }
}
