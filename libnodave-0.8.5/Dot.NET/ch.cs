
//    Thomas,   I believe that I found the problem of libnova.net.dll., Ive tested at Windows Vista SP2 and Windows XP 
// SP3 and until now, no errors and no memory leak., my program test is running with cyclic 100ms(threadpool) with
// 25 tags(variables of plc)
//
//    I would like that all people test this code below (replace in libnodave.net)
    public class pseudoPointer
        {
    public IntPtr pointer;//=IntPtr.Zero;
           
    protected static extern int daveFree(IntPtr p);
    /* ******************REMOVED by ALEX***********
    ~pseudoPointer()
    {
        daveFree(pointer);
    }
    */
    //****************ADD by ALEX**************
    public int freePointer()
    {
    try
    {
    if (pointer !=null) //Im not sure if this is necessary, but more one protection
    if (pointer != IntPtr.Zero) // check if pointer was used
        daveFree(pointer);
    return 0; //free OK
    }
    catch
    {
    return -1; //free not OK
    }
    }

    //******************ADD by ALEX*******************

            protected static extern void daveFreeResults(IntPtr rs);
            public int freeResult()
            {
            try
            {
            if (pointer !=null) //Im not sure if this is necessary, but more one protection
            if (pointer !=IntPtr.Zero) // check if pointer was used
        daveFreeResults(pointer);
            return 0; //free OK
            }
            catch
            {
            return -1; //free not OK
            }
    }
        }

         public class resultSet:pseudoPointer
        {

    protected static extern IntPtr daveNewResultSet();
    public resultSet()
    {
        pointer=daveNewResultSet();
    }

    // *****************REMOVED by ALEX************
    //
            //protected static extern void daveFreeResults(IntPtr rs);
    // ~resultSet()
    // {
        // daveFreeResults(pointer);
    // }

        }

    Alex….