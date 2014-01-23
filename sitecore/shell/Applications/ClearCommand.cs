using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sitecore.LogWatcher.Commands
{
    using Sitecore.Shell.Framework.Commands;
    using Sitecore.Web.UI.Sheer;

    [Serializable]
    public class ClearCommand : Command
    {
        public override void Execute(CommandContext context)
        {
            Services.LogWatcher.Clear();
            SheerResponse.Eval("scLogWatcherObj.update();");
        }
    }
}
