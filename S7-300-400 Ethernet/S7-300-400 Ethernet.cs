using ProjectDataLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace nmDriver
{
    public class Driver : IDriverModel, IDisposable
    {

        /// <summary>
        /// Konstruktor
        /// </summary>
        public Driver()
        {
            //Parametry Tramsmisji
            DriverParam_ = new S7DriverParam();

            ObjId_ = Guid.NewGuid();

            //Obsluga Backgriundworker
            bWorker = new BackgroundWorker();
            bWorker.WorkerSupportsCancellation = true;
            bWorker.DoWork += new DoWorkEventHandler(bWorker_DoWork);
            bWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bWorker_RunWorkerCompleted);
        }

        S7DriverParam DriverParam_;
        Boolean isLive = false;
        static libnodave.daveOSserialType fds;
        static libnodave.daveInterface di;
        static libnodave.daveConnection dc;

        List<Tag> tagList = new List<Tag>();
        List<PO> MPoints = new List<PO>();
        List<PO> IPoints = new List<PO>();
        List<PO> QPoints = new List<PO>();
        List<List<PO>> DBPoints = new List<List<PO>>();

        static string M = "M";
        static string DB = "DB";
        static string I = "I";
        static string Q = "Q";

        EventHandler sendInfoEv;
        EventHandler errorSendEv;

        EventHandler refreshedPartial;
        EventHandler refreshCycleEv;

        EventHandler sendLogInfoEv;
        EventHandler reciveLogInfoEv;

        //Beckgroundworker
        [NonSerialized]
        //Watek wtle
        BackgroundWorker bWorker;

        Guid ObjId_;
        Guid IDriverModel.ObjId
        {
            get { return ObjId_; }
        }

        /// <summary>
        /// Nazwa sterownika
        /// </summary>
        string IDriverModel.driverName
        {
            get { return "S7-300-400 Ethernet"; }
        }

        /// <summary>
        /// Parametry sterownika
        /// </summary>
        object IDriverModel.setDriverParam
        {
            get
            {
                return DriverParam_;
            }
            set
            {
                DriverParam_ = (S7DriverParam)value;
            }
        }

        /// <summary>
        /// Aktywacja cyklu
        /// </summary>
        /// <param name="tagsList"></param>
        /// <returns></returns>
        bool IDriverModel.activateCycle(List<ITag> tagsList2)
        {
            if (bWorker.IsBusy)
                return false;

            //Przypisanie danych
            this.tagList.Clear();
            this.tagList = (from c in tagsList2 where c is Tag select (Tag)c).ToList();


            ConfigData(this.tagList);

            //Otwarcie danych
            fds.rfd = libnodave.openSocket(DriverParam_.Port, DriverParam_.Ip);
            fds.wfd = fds.rfd;

            //Sprawdznie czy nie ma bledu
            if (fds.rfd > 0)
            {
                //Parametry
                di = new libnodave.daveInterface(fds, "IF1", 0, libnodave.daveProtoISOTCP, libnodave.daveSpeed187k);
                di.setTimeout(DriverParam_.Timeout);
                dc = new libnodave.daveConnection(di, 0, DriverParam_.Rack, DriverParam_.Slot);

                //Worker
                if (dc.connectPLC() == 0)
                {
                    bWorker.RunWorkerAsync();
                    isLive = true;

                    foreach (IDriverModel it in this.tagList)
                        it.isAlive = true;
                }

                return true;
            }
            else
            {
                if (errorSendEv != null)
                    errorSendEv(this, new ProjectEventArgs(new byte[] { 0 }, DateTime.Now,"Couldn't open TCP connaction"));

                return false;
            }
        }

        /// <summary>
        /// Dodanie tagow
        /// </summary>
        /// <param name="tagList"></param>
        /// <returns></returns>
        bool IDriverModel.addTagsComm(List<ITag> tagList)
        {
            try
            {

                List<Tag> tgs = (from c in tagList select (Tag)c).ToList();

                //Dodanie Tagow
                this.tagList.AddRange(tgs);

                //Rekonfiguracja
                ConfigData(this.tagList);

                return true;
            }
            catch (Exception Ex)
            {
                if (errorSendEv != null)
                    errorSendEv(this, new ProjectEventArgs(new byte[] { 0 }, DateTime.Now, Ex.Message,Ex));

                return false;
            }
        }

        /// <summary>
        /// Usuniecie Tagow
        /// </summary>
        /// <param name="tagList"></param>
        /// <returns></returns>
        bool IDriverModel.removeTagsComm(List<ITag> tagList2)
        {
            try
            {
                List<Tag> tgs = (from c in tagList2 select (Tag)c).ToList();

                foreach(Tag tn in tgs)
                    tagList.Remove(tn);

                //Rekonfiguracja
                ConfigData(this.tagList);

                //
                return true;
            }
            catch (Exception Ex)
            {
                if (errorSendEv != null)
                   errorSendEv(this, new ProjectEventArgs(new byte[] { 0 }, DateTime.Now, Ex.Message,Ex));

                return false;
            }
        }

        /// <summary>
        /// Rekonfiguruj dane
        /// </summary>
        /// <returns></returns>
        bool IDriverModel.reConfig()
        {
            try
            {

                ConfigData(this.tagList);

                return true;
            }
            catch (Exception Ex)
            {
                if (errorSendEv != null)
                    errorSendEv(this, new ProjectEventArgs(new byte[] { 0 }, DateTime.Now, Ex.Message,Ex));

                return false;
            }
        }

        /// <summary>
        /// Deaktywacja cyklu
        /// </summary>
        /// <returns></returns>
        bool IDriverModel.deactivateCycle()
        {
            //Rozlaczenie
            if (isLive)
            {
                dc.disconnectPLC();
                libnodave.closeSocket(fds.rfd);
                isLive = false;

                foreach (IDriverModel it in this.tagList)
                    it.isAlive = false;
            }

            return true;
        }

        /// <summary>
        /// Głowna praca sterownika - pętla obiegowa
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void bWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {

                #region Area M
                foreach (PO p in MPoints)
                {

                    byte[] data = new byte[((p.Y - p.X) + 1)];
                    sendLogInfoEv?.Invoke(this, new ProjectEventArgs(new byte[] { (byte)libnodave.daveFlags }, DateTime.Now, "M.READ.REQUEST"));
                    if (dc.readBytes(libnodave.daveFlags, 0, p.X, ((p.Y - p.X) + 1), data) == 0)
                    {

                        BitArray btArr = new BitArray(data);

                        reciveLogInfoEv?.Invoke(this, new ProjectEventArgs(data, DateTime.Now, "M.READ.RESPONSE"));

                        Boolean[] bitArray = new Boolean[btArr.Length];
                        btArr.CopyTo(bitArray, 0);

                        //Obszar pamiecie
                        MemoryAreaInfo mInfo = ((IDriverModel)this).MemoryAreaInf.Where(x => x.fctCode == 1).ToArray()[0];

                        //Selekcja tagow do przeslania do odswierzenia
                            var zm = from n in tagList
                                where  n.areaData == mInfo.Name
                                && n.bitAdres >= p.X * mInfo.AdresSize
                                && n.bitAdres + n.coreData.Length <= (p.X + ((p.Y - p.X) + 1)) * mInfo.AdresSize
                                select n;

                        //Lista
                        List<Tag> refDataTag = zm.ToList();

                        //Przypisane danych do tagow
                        for (int i = 0; i < refDataTag.Count; i++)
                        {
                            Array.Copy(bitArray, refDataTag[i].bitAdres - p.X * mInfo.AdresSize, refDataTag[i].coreData, 0, refDataTag[i].coreData.Length);
                            refDataTag[i].refreshData();
                        }

                        //Odswierzenie danych informacja. Delegat do pozostałych klass
                        if (refreshCycleEv != null)
                            refreshCycleEv(this, new ProjectEventArgs(refDataTag));
                    }
                    else
                    {
                        if (errorSendEv != null)
                            errorSendEv(this, new ProjectEventArgs(new byte[] { 0 }, DateTime.Now, "Communication error. Problem with reading M area"));
                    }
                }
                #endregion

                #region Area DB
                foreach (List<PO> pp in DBPoints)
                {
                    foreach (PO p in pp)
                    {
                        byte[] data = new byte[((p.Y - p.X) + 1)];

                        sendLogInfoEv?.Invoke(this, new ProjectEventArgs(new byte[] { (byte)libnodave.daveDB}, DateTime.Now, "DB.READ.REQUEST"));

                        if (dc.readBytes(libnodave.daveDB, p.BlockNum, p.X, ((p.Y - p.X) + 1), data) == 0)
                        {

                            BitArray btArr = new BitArray(data);

                            //Zdarzenia
                            reciveLogInfoEv?.Invoke(this, new ProjectEventArgs(data, DateTime.Now,"DB.READ.RESPONSE"));

                            Boolean[] bitArray = new Boolean[btArr.Length];
                            btArr.CopyTo(bitArray, 0);

                            //Obszar pamiecie
                            MemoryAreaInfo mInfo = ((IDriverModel)this).MemoryAreaInf.Where(x => x.fctCode == 4).ToArray()[0];

                            //Selekcja tagow do przeslania do odswierzenia
                            var zm = from n in tagList
                                     where n.areaData == mInfo.Name
                                     && n.BlockAdress == p.BlockNum
                                     && n.bitAdres >= p.X * mInfo.AdresSize
                                     && n.bitAdres + n.coreData.Length <= (p.X + ((p.Y - p.X) + 1)) * mInfo.AdresSize
                                     select n;

                            //Lista
                            List<Tag> refDataTag = zm.ToList();

                            //Przypisane danych do tagow
                            for (int i = 0; i < refDataTag.Count; i++)
                            {
                                Array.Copy(bitArray, refDataTag[i].bitAdres - p.X * mInfo.AdresSize, refDataTag[i].coreData, 0, refDataTag[i].coreData.Length);
                                refDataTag[i].refreshData();
                            }

                            //Odswierzenie danych informacja. Delegat do pozostałych klass
                            if (refreshCycleEv != null)
                                refreshCycleEv(this, new ProjectEventArgs(refDataTag));
                        }
                        else
                        {
                            if (errorSendEv != null)
                                errorSendEv(this, new ProjectEventArgs(new byte[] { 0 }, DateTime.Now, "Communication error. Problem with reading DB area"));
                        }
                    }
                }

                #endregion

                #region Area I
                foreach (PO p in IPoints)
                {

                    byte[] data = new byte[((p.Y - p.X) + 1)];

                    sendLogInfoEv?.Invoke(this, new ProjectEventArgs(new byte[] { (byte)libnodave.daveInputs }, DateTime.Now, "I.READ.REQUEST"));

                    if (dc.readBytes(libnodave.daveInputs, 0, p.X, ((p.Y - p.X) + 1), data) == 0)
                    {

                        BitArray btArr = new BitArray(data);

                        reciveLogInfoEv?.Invoke(this, new ProjectEventArgs(data, DateTime.Now, "I.READ.RESPONSE"));

                        Boolean[] bitArray = new Boolean[btArr.Length];
                        btArr.CopyTo(bitArray, 0);

                        //Obszar pamiecie
                        MemoryAreaInfo mInfo = ((IDriverModel)this).MemoryAreaInf.Where(x => x.fctCode == 2).ToArray()[0];

                        //Selekcja tagow do przeslania do odswierzenia
                        var zm = from n in tagList
                                 where n.areaData == mInfo.Name
                                 && n.bitAdres >= p.X * mInfo.AdresSize
                                 && n.bitAdres + n.coreData.Length <= (p.X + ((p.Y - p.X) + 1)) * mInfo.AdresSize
                                 select n;

                        //Lista
                        List<Tag> refDataTag = zm.ToList();

                        //Przypisane danych do tagow
                        for (int i = 0; i < refDataTag.Count; i++)
                        {
                            Array.Copy(bitArray, refDataTag[i].bitAdres - p.X * mInfo.AdresSize, refDataTag[i].coreData, 0, refDataTag[i].coreData.Length);
                            refDataTag[i].refreshData();
                        }

                        //Odswierzenie danych informacja. Delegat do pozostałych klass
                        if (refreshCycleEv != null)
                            refreshCycleEv(this, new ProjectEventArgs(refDataTag));
                    }
                    else
                    {
                        if (errorSendEv != null)
                            errorSendEv(this, new ProjectEventArgs(new byte[] { 0 }, DateTime.Now, "Communication error. Problem with reading I area"));
                    }
                }
                #endregion

                #region Area Q
                foreach (PO p in QPoints)
                {

                    byte[] data = new byte[((p.Y - p.X) + 1)];

                    sendLogInfoEv?.Invoke(this, new ProjectEventArgs(new byte[] { (byte)libnodave.daveOutputs }, DateTime.Now, "Q.READ.REQUEST"));

                    if (dc.readBytes(libnodave.daveOutputs, 0, p.X, ((p.Y - p.X) + 1), data) == 0)
                    {

                        BitArray btArr = new BitArray(data);

                        reciveLogInfoEv?.Invoke(this, new ProjectEventArgs(data, DateTime.Now, "Q.READ.RESPONSE"));

                        Boolean[] bitArray = new Boolean[btArr.Length];
                        btArr.CopyTo(bitArray, 0);

                        //Obszar pamiecie
                        MemoryAreaInfo mInfo = ((IDriverModel)this).MemoryAreaInf.Where(x => x.fctCode == 3).ToArray()[0];

                        //Selekcja tagow do przeslania do odswierzenia
                        var zm = from n in tagList
                                 where n.areaData == mInfo.Name
                                 && n.bitAdres >= p.X * mInfo.AdresSize
                                 && n.bitAdres + n.coreData.Length <= (p.X + ((p.Y - p.X) + 1)) * mInfo.AdresSize
                                 select n;

                        //Lista
                        List<Tag> refDataTag = zm.ToList();

                        //Przypisane danych do tagow
                        for (int i = 0; i < refDataTag.Count; i++)
                        {
                            Array.Copy(bitArray, refDataTag[i].bitAdres - p.X * mInfo.AdresSize, refDataTag[i].coreData, 0, refDataTag[i].coreData.Length);
                            refDataTag[i].refreshData();
                        }

                        //Odswierzenie danych informacja. Delegat do pozostałych klass
                        if (refreshCycleEv != null)
                            refreshCycleEv(this, new ProjectEventArgs(refDataTag));
                    }
                    else
                    {
                        if (errorSendEv != null)
                            errorSendEv(this, new ProjectEventArgs(new byte[] { 0 }, DateTime.Now, "Communication error. Problem with reading Q area"));
                    }
                }
                #endregion

                #region Zapisz dane na porcie
                //Tagi z obszaru Coils do zapisu
                List<Tag> TgWrite = tagList.Where(x => x.setMod).ToList();
                if (TgWrite.Count > 0)
                {
                    try
                    {
                        //Próba zapisu Taga
                        foreach (Tag tg in TgWrite)
                        {
                            //Utworzenie do wyslania
                            BitArray btArray = new BitArray(tg.coreDataSend);

                            //Utworzenie tablicy byte
                            Byte[] buffByteArr = new byte[tg.coreDataSend.Length / 8 == 0 ? 1 : tg.coreDataSend.Length / 8];

                            //Konwersja i przeniesienie
                            btArray.CopyTo(buffByteArr, 0);

                            //Zapis
                            if (tg.areaData == M)
                            {
                                dc.writeBytes(libnodave.daveFlags, 0, tg.startData, tg.coreDataSend.Length / 8, buffByteArr);
                                tg.setMod = false;

                                sendLogInfoEv?.Invoke(this, new ProjectEventArgs(new byte[] { (byte)libnodave.daveFlags }, DateTime.Now, "M.WRITE.REQUEST"));
                            }

                            //Zapis
                            if (tg.areaData == DB)
                            {
                                dc.writeBytes(libnodave.daveDB, tg.BlockAdress, tg.startData, tg.coreDataSend.Length / 8, buffByteArr);
                                tg.setMod = false;

                                sendLogInfoEv?.Invoke(this, new ProjectEventArgs(new byte[] { (byte)libnodave.daveDB }, DateTime.Now, "DB.WRITE.REQUEST"));
                            }

                            //Zapis
                            if (tg.areaData == I)
                            {
                                dc.writeBytes(libnodave.daveInputs, 0, tg.startData, tg.coreDataSend.Length / 8, buffByteArr);
                                tg.setMod = false;

                                sendLogInfoEv?.Invoke(this, new ProjectEventArgs(new byte[] { (byte)libnodave.daveInputs }, DateTime.Now, "I.WRITE.REQUEST"));
                            }

                            //Zapis
                            if (tg.areaData == Q)
                            {
                                dc.writeBytes(libnodave.daveOutputs, 0, tg.startData, tg.coreDataSend.Length / 8, buffByteArr);
                                tg.setMod = false;

                                sendLogInfoEv?.Invoke(this, new ProjectEventArgs(new byte[] { (byte)libnodave.daveOutputs}, DateTime.Now, "Q.WRITE.REQUEST"));
                            }
                        }
                    }
                    catch (Exception Ex)
                    {

                        if (errorSendEv != null)
                            errorSendEv(this, new ProjectEventArgs(new byte[] { 0 }, DateTime.Now, "SiemensS7.bWorker_DoWork.Write :" + Ex.Message));
                    }
                }
                #endregion

            }
            catch (Exception Ex)
            {
                if (errorSendEv != null)
                    errorSendEv(this, new ProjectEventArgs(new byte[]{0}, DateTime.Now, "S7-300/400.bWorker_DoWork :" + Ex.Message,Ex));
            }

            //Opoznienie wątku o dany czas
            Thread.Sleep(DriverParam_.ReplyTime);
        }

        /// <summary>
        /// Praca wykonana i przejscie do nastpnej albo zatrzymanie
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void bWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                //Reaktywacja pobierania danych
                if (isLive)
                {
                    bWorker.RunWorkerAsync();
                }
                else
                {
                    dc.disconnectPLC();
                    libnodave.closeSocket(fds.rfd);
                }
            }
            catch (Exception Ex)
            {
                if (errorSendEv != null)
                    errorSendEv(this, new ProjectEventArgs(new byte[] { 0 }, DateTime.Now, "S7-300/400.bWorker_RunWorkerCompleted :" + Ex.Message,Ex));
            }
        }

        /// <summary>
        /// Konfiguracja parmamertów Bufforów dla rejestrow
        /// </summary>
        /// <param name="tagList"></param>
        void ConfigData(List<Tag> tagList)
        {
            try
            {

                if (tagList == null)
                    return;

                getFrameSizeM(tagList, M, 230*8, 130);
                getFrameSizeI(tagList, I, 230 * 8, 130);
                getFrameSizeQ(tagList, Q, 230 * 8, 130);
                getFrameSizeDB(tagList, DB, 230 * 8, 130);

            }
            catch (Exception Ex)
            {
               if (errorSendEv != null)
                   errorSendEv(this, new ProjectEventArgs(new byte[] { 0 }, DateTime.Now, "S7-300/400.configSingleDevice.CO :" + Ex.Message,Ex));
            }

        }

        /// <summary>
        /// Pobranie rozmiarw elementow do pobrania
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="type"></param>
        /// <param name="maxFrSize"></param>
        /// <param name="maxSize"></param>
        /// <returns></returns>
        void getFrameSizeM(List<Tag> tags,string type,int maxFrSize, int maxSize)
        {
            try
            {
                MPoints.Clear();

                //Pobranie odpowiednich typow tagow
                var tgs = (from t in tags where t.areaData == type select t).ToList();

                if (tgs.Count == 0)
                    return;

                //Najnizszy i najwyzszy adres tagowy.
                int min = tgs.Min(x => x.startData);
                int max = tgs.Max(x => (x.startData + x.coreData.Length / 8)) - 1;
                int marker = 0;

                //Dodanie buffora na punkty
                List<PO> points = new List<PO>();
                points.Add(new PO(min, 0));

                //Petla obiegowa
                for (int i = min; i <= max; i++)
                {
                    #region Zabezpiecznie dlugiej ramki

                    if (points[points.Count - 1].Y - points[points.Count - 1].X >= maxFrSize)
                    {

                        //Jestemy w miejscu Taga trzeba podac koniec
                        Tag halfTag = tgs.Where(x => x.startData <= i && (x.startData + x.coreData.Length / 8) > i).ToList()[0];
                        if (halfTag != null)
                        {

                            points[points.Count - 1].Y = halfTag.startData + halfTag.coreData.Length / 8 - 1;
                            i = points[points.Count - 1].Y + 1;
                            points.Add(new PO(i, 0));
                        }


                        else
                            points.Add(new PO(i, 0));

                    }

                    #endregion

                    #region Zabezpieczenie przed przekroczeniem wolnego odstepu

                    if (marker == maxSize)
                        points.Add(new PO(i, 0));

                    #endregion

                    #region Sprawdzenie czy tagi sa w zakresie
                    if (tgs.Where(x => x.startData <= i && (x.startData + x.coreData.Length / 8) > i).ToList().Count > 0)
                    {
                        //DANIE
                        points[points.Count - 1].Y = i;

                        //Dociagnicie do adresu po powrocie do odliczania
                        if (marker >= maxSize)
                            points[points.Count - 1].X = i;

                        marker = 0;
                    }
                    else
                    {
                        //Przesuwaj X
                        if (marker >= maxSize)
                            points[points.Count - 1].X = i;

                        //Inkrementuj marker
                        marker++;
                    }
                    #endregion
                }


                MPoints.AddRange(points);
                return;
            }
            catch (Exception)
            {
                throw;
            }
        
        }

        /// <summary>
        /// Pobranie rozmiarw elementow do pobrania
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="type"></param>
        /// <param name="maxFrSize"></param>
        /// <param name="maxSize"></param>
        /// <returns></returns>
        void getFrameSizeI(List<Tag> tags, string type, int maxFrSize, int maxSize)
        {
            try
            {
                IPoints.Clear();

                //Pobranie odpowiednich typow tagow
                var tgs = (from t in tags where t.areaData == type select t).ToList();

                if (tgs.Count == 0)
                    return;

                //Najnizszy i najwyzszy adres tagowy.
                int min = tgs.Min(x => x.startData);
                int max = tgs.Max(x => (x.startData + x.coreData.Length / 8)) - 1;
                int marker = 0;

                //Dodanie buffora na punkty
                List<PO> points = new List<PO>();
                points.Add(new PO(min, 0));

                //Petla obiegowa
                for (int i = min; i <= max; i++)
                {
                    #region Zabezpiecznie dlugiej ramki

                    if (points[points.Count - 1].Y - points[points.Count - 1].X >= maxFrSize)
                    {

                        //Jestemy w miejscu Taga trzeba podac koniec
                        Tag halfTag = tgs.Where(x => x.startData <= i && (x.startData + x.coreData.Length / 8) > i).ToList()[0];
                        if (halfTag != null)
                        {

                            points[points.Count - 1].Y = halfTag.startData + halfTag.coreData.Length / 8 - 1;
                            i = points[points.Count - 1].Y + 1;
                            points.Add(new PO(i, 0));
                        }


                        else
                            points.Add(new PO(i, 0));

                    }

                    #endregion

                    #region Zabezpieczenie przed przekroczeniem wolnego odstepu

                    if (marker == maxSize)
                        points.Add(new PO(i, 0));

                    #endregion

                    #region Sprawdzenie czy tagi sa w zakresie
                    if (tgs.Where(x => x.startData <= i && (x.startData + x.coreData.Length / 8) > i).ToList().Count > 0)
                    {
                        //DANIE
                        points[points.Count - 1].Y = i;

                        //Dociagnicie do adresu po powrocie do odliczania
                        if (marker >= maxSize)
                            points[points.Count - 1].X = i;

                        marker = 0;
                    }
                    else
                    {
                        //Przesuwaj X
                        if (marker >= maxSize)
                            points[points.Count - 1].X = i;

                        //Inkrementuj marker
                        marker++;
                    }
                    #endregion
                }


                IPoints.AddRange(points);
                return;
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// Pobranie rozmiarw elementow do pobrania
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="type"></param>
        /// <param name="maxFrSize"></param>
        /// <param name="maxSize"></param>
        /// <returns></returns>
        void getFrameSizeQ(List<Tag> tags, string type, int maxFrSize, int maxSize)
        {
            try
            {
                QPoints.Clear();

                //Pobranie odpowiednich typow tagow
                var tgs = (from t in tags where t.areaData == type select t).ToList();

                if (tgs.Count == 0)
                    return;

                //Najnizszy i najwyzszy adres tagowy.
                int min = tgs.Min(x => x.startData);
                int max = tgs.Max(x => (x.startData + x.coreData.Length / 8)) - 1;
                int marker = 0;

                //Dodanie buffora na punkty
                List<PO> points = new List<PO>();
                points.Add(new PO(min, 0));

                //Petla obiegowa
                for (int i = min; i <= max; i++)
                {
                    #region Zabezpiecznie dlugiej ramki

                    if (points[points.Count - 1].Y - points[points.Count - 1].X >= maxFrSize)
                    {

                        //Jestemy w miejscu Taga trzeba podac koniec
                        Tag halfTag = tgs.Where(x => x.startData <= i && (x.startData + x.coreData.Length / 8) > i).ToList()[0];
                        if (halfTag != null)
                        {

                            points[points.Count - 1].Y = halfTag.startData + halfTag.coreData.Length / 8 - 1;
                            i = points[points.Count - 1].Y + 1;
                            points.Add(new PO(i, 0));
                        }


                        else
                            points.Add(new PO(i, 0));

                    }

                    #endregion

                    #region Zabezpieczenie przed przekroczeniem wolnego odstepu

                    if (marker == maxSize)
                        points.Add(new PO(i, 0));

                    #endregion

                    #region Sprawdzenie czy tagi sa w zakresie
                    if (tgs.Where(x => x.startData <= i && (x.startData + x.coreData.Length / 8) > i).ToList().Count > 0)
                    {
                        //DANIE
                        points[points.Count - 1].Y = i;

                        //Dociagnicie do adresu po powrocie do odliczania
                        if (marker >= maxSize)
                            points[points.Count - 1].X = i;

                        marker = 0;
                    }
                    else
                    {
                        //Przesuwaj X
                        if (marker >= maxSize)
                            points[points.Count - 1].X = i;

                        //Inkrementuj marker
                        marker++;
                    }
                    #endregion
                }


                QPoints.AddRange(points);
                return;
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// Pobranie rozmiarw elementow do pobrania
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="type"></param>
        /// <param name="maxFrSize"></param>
        /// <param name="maxSize"></param>
        /// <returns></returns>
        void getFrameSizeDB(List<Tag> tags, string type, int maxFrSize, int maxSize)
        {
            try
            {
                
                IEnumerable<IGrouping<int, Tag>> tagi = tags.Where(x => x.areaData == type).GroupBy(x => x.BlockAdress, x => x).ToList();
                foreach(IGrouping<int, Tag> xx in tagi)
                {
                    //Pobranie odpowiednich typow tagow
                    var tgs = from t in xx select t;

                    //Najnizszy i najwyzszy adres tagowy.
                    int min = tgs.Min(x => x.startData);
                    int max = tgs.Max(x => (x.startData + x.coreData.Length / 8)) - 1;
                    int marker = 0;

                    List<PO> points = new List<PO>();
                    points.Add(new PO(min, 0));
                    points.Last().BlockNum = tgs.Last().BlockAdress;

                    //Petla obiegowa
                    for (int i = min; i <= max; i++)
                    {
                        #region Zabezpiecznie dlugiej ramki

                        if (points[points.Count - 1].Y - points[points.Count - 1].X >= maxFrSize)
                        {

                            //Jestemy w miejscu Taga trzeba podac koniec
                            Tag halfTag = tgs.Where(x => x.startData <= i && (x.startData + x.coreData.Length / 8) > i).ToList()[0];
                            if (halfTag != null)
                            {

                                points[points.Count - 1].Y = halfTag.startData + halfTag.coreData.Length / 8 - 1;
                                i = points[points.Count - 1].Y + 1;
                                points.Add(new PO(i, 0));
                            }


                            else
                                points.Add(new PO(i, 0));

                        }

                        #endregion

                        #region Zabezpieczenie przed przekroczeniem wolnego odstepu

                        if (marker == maxSize)
                            points.Add(new PO(i, 0));

                        #endregion

                        #region Sprawdzenie czy tagi sa w zakresie
                        if (tgs.Where(x => x.startData <= i && (x.startData + x.coreData.Length / 8) > i).ToList().Count > 0)
                        {
                            //DANIE
                            points[points.Count - 1].Y = i;

                            //Dociagnicie do adresu po powrocie do odliczania
                            if (marker >= maxSize)
                                points[points.Count - 1].X = i;

                            marker = 0;
                        }
                        else
                        {
                            //Przesuwaj X
                            if (marker >= maxSize)
                                points[points.Count - 1].X = i;

                            //Inkrementuj marker
                            marker++;
                        }
                        #endregion
                    }

                    DBPoints.Add(points);
                }

                return;
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// Odswierzenie danych
        /// </summary>
        event EventHandler IDriverModel.refreshedCycle
        {
            add { refreshCycleEv += value; }
            remove { refreshCycleEv -= value; }
        }

        /// <summary>
        /// Odswierzenie częsciowe
        /// </summary>
        event EventHandler IDriverModel.refreshedPartial
        {
            add { refreshedPartial += value; }
            remove { refreshedPartial -= value; }
        }

        /// <summary>
        /// Dane wychodzące ze sterownika
        /// </summary>
        event EventHandler IDriverModel.dataSent
        {
            add { sendLogInfoEv += value; }
            remove { sendLogInfoEv -= value; }
        }

        /// <summary>
        /// dane przychodzoce do sterownika
        /// </summary>
        event EventHandler IDriverModel.dataRecived
        {
            add { reciveLogInfoEv += value; }
            remove { reciveLogInfoEv -= value; }
        }

        /// <summary>
        /// Informacje o blędach
        /// </summary>
        event EventHandler IDriverModel.error
        {
            add { errorSendEv += value; }
            remove { errorSendEv -= value; }
        }

        /// <summary>
        /// Inne informacje z klasy
        /// </summary>
        event EventHandler IDriverModel.information
        {
            add { sendInfoEv += value; }
            remove { sendInfoEv -= value; }
        }

        /// <summary>
        /// Rodzaje rejestrw
        /// </summary>
        MemoryAreaInfo[] IDriverModel.MemoryAreaInf
        {
            get
            {
                return new MemoryAreaInfo[] { 
                    new MemoryAreaInfo(M,8,1),
                    new MemoryAreaInfo(I,8,2),
                    new MemoryAreaInfo(Q,8,3),
                    new MemoryAreaInfo(DB,8,4)
                
                };
            }
        }

        /// <summary>
        /// Request
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        string IDriverModel.FormatFrameRequest(byte[] frame, System.Globalization.NumberStyles num)
        {
          
            if (frame == null)
                return "Empty reponse";

            switch (num)
            {
                case NumberStyles.HexNumber:                
                    return frame.Aggregate("", (a, e) => a + e.ToString("X"));

                case NumberStyles.Integer:
                    return frame.Aggregate("", (a, e) => a + e.ToString());

                default:
                    return "Internal problem";

            }
        }

        /// <summary>
        /// response
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        string IDriverModel.FormatFrameResponse(byte[] frame, System.Globalization.NumberStyles num)
        {
            if (frame == null)
                return "Empty reponse";

            switch (num)
            {
                case NumberStyles.HexNumber:
                    return frame.Aggregate("", (a, e) => a + " " + e.ToString("X"));

                case NumberStyles.Integer:
                    return frame.Aggregate("", (a, e) => a + e.ToString());

                default:
                    return "Internal problem";
            }
        }

        /// <summary>
        /// Aktywnosc poloczenia
        /// </summary>
        bool IDriverModel.isAlive
        {
            get { return isLive; }
            set
            {
                isLive = value;
               
            }
        }

        /// <summary>
        /// Zajetosc sterownika
        /// </summary>
        bool IDriverModel.isBusy
        {
            get { return bWorker.IsBusy; }
        }

        /// <summary>
        /// Send data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        byte[] IDriverModel.sendBytes(byte[] data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Pluginy
        /// </summary>
        object[] IDriverModel.plugins
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Rozne parametry
        /// 1- Wlazenie rozszerzonej adresacji dla Device
        /// 2- Wlaczenie Block Adress
        /// </summary>
        bool[] IDriverModel.AuxParam
        {
            get { return new Boolean[] { false ,true }; }
        }

        string getAreaFromFrame(byte[] ask)
        {
            return "cos";
            
        }

        //Do wyswietlania
        public override string ToString()
        {
            return "Driver: " + ((IDriverModel)this).driverName;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    bWorker.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Driver() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
