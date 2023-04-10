using BrawlLib.Internal;
using BrawlLib.SSBB.Types;

namespace BrawlLib.SSBB.ResourceNodes
{
    public unsafe class XBINNode : BRESEntryNode  // based on PLT0 Node  
    {
        internal XBIN* XBINHeader => (XBIN*) WorkingUncompressed.Address;
        public override ResourceType ResourceFileType => ResourceType.XBIN;
        public override int DataAlign => 0x20;
        public override string Tag => "XBIN";

        public XBINNode()
        {
        }

        public XBINNode(string name)
        {
            _name = name;
        }

        public override bool OnInitialize()
        {
            // qwe Todo: look at base OnInitialize() and split out the relavant bits regarding size to here.
            // Think about how you can set the element name. Can you find the group via this class or the parent?
            // You need the BRESGroupNode : ResourceNode - Group (ResourceGroup)

            base.OnInitialize();
            return false;
        }

        internal override void GetStrings(StringTable table)
        {
            base.GetStrings(table);
        }

        public override int OnCalculateSize(bool force)
        {
            return base.OnCalculateSize(force);
        }

        public override void OnRebuild(VoidPtr address, int length, bool force)
        {
            base.OnRebuild(address, length, force);
        }

        protected internal override void PostProcess(VoidPtr bresAddress, VoidPtr dataAddress, int dataLength, StringTable stringTable)
        {
            // base.PostProcess(bresAddress, dataAddress, dataLength, stringTable);
        }

        internal static ResourceNode TryParse(DataSource source, ResourceNode parent)
        {
            return ((XBIN*) source.Address)->_tag.Get() == "XBIN" ? new XBINNode() : null;
        }
    }
}