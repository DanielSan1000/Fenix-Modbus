using System;
using System.Collections.Generic;
using System.Globalization;

namespace ProjectDataLib
{
    public interface IDriverModel
    {
        Guid ObjId { get; }

        String driverName { get; }

        Object setDriverParam { get; set; }

        Boolean activateCycle(List<ITag> tagsList);

        Boolean addTagsComm(List<ITag> tagList);

        Boolean removeTagsComm(List<ITag> tagList);

        Boolean reConfig();

        Boolean deactivateCycle();

        event EventHandler refreshedCycle;

        event EventHandler refreshedPartial;

        event EventHandler error;

        event EventHandler information;

        event EventHandler dataSent;

        event EventHandler dataRecived;

        MemoryAreaInfo[] MemoryAreaInf { get; }

        String FormatFrameRequest(Byte[] frame, NumberStyles num);

        String FormatFrameResponse(Byte[] frame, NumberStyles num);

        Boolean isAlive { get; set; }

        Boolean isBusy { get; }

        byte[] sendBytes(byte[] data);

        Object[] plugins { get; }

        Boolean[] AuxParam { get; }
    }
}