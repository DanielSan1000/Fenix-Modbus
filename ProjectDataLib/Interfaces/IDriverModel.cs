using System;
using System.Collections.Generic;
using System.Globalization;

namespace ProjectDataLib
{
    /// <summary>
    /// Interfejs modelu sterownika
    /// </summary>
    public interface IDriverModel
    {
        /// <summary>
        /// Unikatowy identyfikator
        /// </summary>
        Guid ObjId { get; }

        /// <summary>
        /// Nazwa sterownika
        /// </summary>
        String driverName { get; }

        /// <summary>
        /// Konfiguracja parametrow transmisji
        /// </summary>
        Object setDriverParam { get; set; }

        /// <summary>
        /// Cykliczna metoda wymiany informacji
        /// </summary>
        /// <param name="tagsList"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        Boolean activateCycle(List<ITag> tagsList);

        /// <summary>
        /// Dodanie tagow do komunikacji
        /// </summary>
        /// <param name="tagList"></param>
        /// <returns></returns>
        Boolean addTagsComm(List<ITag> tagList);

        /// <summary>
        /// Odjecie tagow od komunikacji
        /// </summary>
        /// <param name="tagList"></param>
        /// <returns></returns>
        Boolean removeTagsComm(List<ITag> tagList);

        /// <summary>
        /// Reconfiguruj dane. Zmiane zakre zapytania w w przypadku zmiany tagów
        /// </summary>
        /// <param name="tagList"></param>
        /// <returns></returns>
        Boolean reConfig();

        /// <summary>
        /// Zatrzymanie cyklicznej komunikacji
        /// </summary>
        /// <returns></returns>
        Boolean deactivateCycle();

        /// <summary>
        /// Dane zostaly odswierzone
        /// </summary>
        event EventHandler refreshedCycle;

        /// <summary>
        /// Odswierzenie pewnego obszaru pamieci
        /// </summary>
        event EventHandler refreshedPartial;

        /// <summary>
        /// Informacja o bledzie
        /// </summary>
        event EventHandler error;

        /// <summary>
        /// Informacja o zdarzeniach
        /// </summary>
        event EventHandler information;

        /// <summary>
        /// Dane wyslane ze sterownika
        /// </summary>
        event EventHandler dataSent;

        /// <summary>
        /// Dane dostarczone ze sterownika
        /// </summary>
        event EventHandler dataRecived;

        /// <summary>
        /// Informacja o obszarach
        /// </summary>
        MemoryAreaInfo[] MemoryAreaInf { get; }

        /// <summary>
        /// Formatowanie ramki w zaleznosci od protokolu
        /// </summary>
        String FormatFrameRequest(Byte[] frame, NumberStyles num);

        /// <summary>
        /// Formatowanie ramki w zaleznosci od protokolu
        /// </summary>
        String FormatFrameResponse(Byte[] frame, NumberStyles num);

        /// <summary>
        /// Sterownik dziala
        /// </summary>
        Boolean isAlive { get; set; }

        /// <summary>
        /// Sterownik pracuje
        /// </summary>
        Boolean isBusy { get; }

        /// <summary>
        /// Zapyanie jednostkowe w postacji bajtów z weryfikacją bladów
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        byte[] sendBytes(byte[] data);

        /// <summary>
        /// Plaginy dla Modbusa
        /// </summary>
        Object[] plugins { get; }

        /// <summary>
        /// [0] Wlacza lub wylacza dodatkowy adress dla urzadzenia
        /// </summary>
        Boolean[] AuxParam { get; }
    }
}