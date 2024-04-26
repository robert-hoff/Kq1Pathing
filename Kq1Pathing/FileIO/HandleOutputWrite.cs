namespace Kq1Pathing.FileIO
{
    /*
     *
     * Examples
     * -------
     *
     * Debug.Write, same as (x)=>{Debug.Write(x);}
     * sw.Write (StreamWriter)
     * buffer.Write (StringBuffer)
     * (x)=>{} (disable output)
     *
     *
     *
     */
    public delegate void HandleOutputWrite(string s);
}
