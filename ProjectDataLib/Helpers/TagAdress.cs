namespace ProjectDataLib
{
    /// <summary>
    /// Adress Taga
    /// </summary>
    public struct TagAdress
    {
        public int adress;
        public int secAdress;

        //Konstruktor
        public TagAdress(int adress, int secAdress)
        {
            this.adress = adress;
            this.secAdress = secAdress;
        }
    }
}