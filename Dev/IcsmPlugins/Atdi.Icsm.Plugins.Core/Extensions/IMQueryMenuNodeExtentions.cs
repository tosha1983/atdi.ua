using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSM;

namespace Atdi.Icsm.Plugins.Core
{
    public static class IMQueryMenuNodeExtentions
    {
        public static void AddContextMenuToolForEachRecords(this List<IMQueryMenuNode> nodes, string caption, IMQueryMenuNode.Handler handler)
        {
            nodes.Add(
                    new IMQueryMenuNode(
                        caption, null,
                        handler,
                        IMQueryMenuNode.ExecMode.EachRecord)
                );
        }
        public static void AddContextMenuToolForSelectionOfRecords(this List<IMQueryMenuNode> nodes, string caption, IMQueryMenuNode.Handler handler)
        {
            nodes.Add(
                    new IMQueryMenuNode(
                        caption, null,
                        handler,
                        IMQueryMenuNode.ExecMode.SelectionOfRecords)
                );
        }
    }
}
