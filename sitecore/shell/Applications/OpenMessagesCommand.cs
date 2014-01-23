using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sitecore.Shell.Framework;
using Sitecore.Shell.Framework.Commands;

namespace Sitecore.LogWatcher.Commands
{
    [Serializable]
    public class OpenMessagesCommand : Command
    {
        public override void Execute(CommandContext context)
        {
            Windows.RunApplication("LogMessageViewer", string.Empty);

        }
    }
}
