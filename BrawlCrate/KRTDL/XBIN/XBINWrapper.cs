using BrawlLib.SSBB;
using BrawlLib.SSBB.ResourceNodes;

namespace BrawlCrate.NodeWrappers
{
    [NodeWrapper(ResourceType.XBIN)]
    public class XBINWrapper : GenericWrapper
    {
        public override string ExportFilter => FileFilters.XBIN;
    }
}