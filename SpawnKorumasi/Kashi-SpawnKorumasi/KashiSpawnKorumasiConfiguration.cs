using Rocket.API;
using System.Collections.Generic;
using System.Xml.Serialization;

public class KashiSpawnKorumasiConfiguration : IRocketPluginConfiguration
{
    [XmlArray("SilahID")]
    [XmlArrayItem("SilahID")]
    public List<ushort> SilahID { get; set; }
    public float KorumaSuresi { get; set; }
    public ushort PartikulEfektiID { get; set; }

    public MesajlarConfig Mesajlar { get; set; }

    public void LoadDefaults()
    {
        SilahID = new List<ushort> { 363, 364, 365 };
        KorumaSuresi = 30f;
        PartikulEfektiID = 132;

        Mesajlar = new MesajlarConfig
        {
            Koruma = new KorumaMesajlari
            {
                MesajKorumaAktif = "Şu anda {0} saniye boyunca hasardan korunuyorsunuz.",
                MesajKorumaPasif = "Hasar korumanız sona erdi."
            },
            IptalMesajlari = new IptalMesajlari
            {
                MesajKorumaIptalYumruk = "Koruma yumruk attığınız için iptal edildi.",
                MesajKorumaIptalOlum = "Koruma öldüğünüz için iptal edildi.",
                MesajKorumaIptalSilah = "Silah eklendiği için koruma iptal edildi!"
            },
            KomutMesajlari = new KomutMesajlari
            {
                MesajSilahIDEkleBasarisiz = "ID girmeniz gerekmektedir!",
                MesajSilahIDEkleBasari = "Silah ekleme başarılı!",
                MesajSilahIDMevcut = "Bu ID zaten mevcut.",
                MesajSilahIDSilBasarisiz = "ID girmeniz gerekmektedir!",
                MesajSilahIDSilBasari = "Silah ID başarıyla silindi.",
                MesajSilahIDSilMevcutDegil = "Bu ID mevcut değil."
            },
            Genel = new GenelMesajlar
            {
                MesajKorumaCanliHasar = "Koruma süresi boyunca canlılara hasar veremezsiniz!"
            }
        };
    }
}

public class MesajlarConfig
{
    public KorumaMesajlari Koruma { get; set; }
    public IptalMesajlari IptalMesajlari { get; set; }
    public KomutMesajlari KomutMesajlari { get; set; }
    public GenelMesajlar Genel { get; set; }
}

public class KorumaMesajlari
{
    public string MesajKorumaAktif { get; set; }
    public string MesajKorumaPasif { get; set; }
}

public class IptalMesajlari
{
    public string MesajKorumaIptalYumruk { get; set; }
    public string MesajKorumaIptalOlum { get; set; }
    public string MesajKorumaIptalSilah { get; set; }
}

public class KomutMesajlari
{
    public string MesajSilahIDEkleBasarisiz { get; set; }
    public string MesajSilahIDEkleBasari { get; set; }
    public string MesajSilahIDMevcut { get; set; }
    public string MesajSilahIDSilBasarisiz { get; set; }
    public string MesajSilahIDSilBasari { get; set; }
    public string MesajSilahIDSilMevcutDegil { get; set; }
}

public class GenelMesajlar
{
    public string MesajKorumaCanliHasar { get; set; }
}
