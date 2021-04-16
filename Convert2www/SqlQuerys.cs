namespace Convert2www
{
    public static class SqlQuerys
    {
        public const string SelectWareTable = @"
SELECT i.StanMag As StanMag,
	j.Nazwa AS jmNazwa,
	t.TowId, t.Nazwa, t.Kod, t.Stawka, t.CenaEw, t.CenaDet, t.CenaHurt, t.CenaDod, t.CenaNoc, t.Opis1, t.Opis2, t.Opis3, t.Opis4, t.IleWZgrzewce, t.Indeks1, t.WysylacNaSklepInternetowy
FROM Towar t
LEFT JOIN Istw i ON i.TowId = t.TowId
LEFT JOIN JM j ON j.JMId = t.JMid
WHERE
	t.Aktywny = 1 AND i.MagId = 1 and Indeks1 = 'IM-A-100278'

";

        public const string SelectContractorTable = @"SELECT kr.Kod AS krajKod,
										  kr.Nazwa AS krajNazwa,
										ko.*
									FROM Kontrahent ko
									LEFT JOIN Kraj kr ON kr.KrajId = ko.KontrKrajId
									WHERE
										ko.Aktywny = 1";
    }
}

//public const string SelectWareTable = @"
//SELECT SUM(i.StanMag) As StanMag,
//	j.Nazwa AS jmNazwa,
//	t.TowId, t.Nazwa, t.Kod, t.Stawka, t.CenaEw, t.CenaDet, t.CenaHurt, t.CenaDod, t.CenaNoc, t.Opis1, t.Opis2, t.Opis3, t.Opis4, t.IleWZgrzewce, t.Indeks1, t.WysylacNaSklepInternetowy
//FROM Towar t
//LEFT JOIN Istw i ON i.TowId = t.TowId
//LEFT JOIN JM j ON j.JMId = t.JMid
//WHERE
//	t.Aktywny = 1 AND i.MagId IN (1) and Indeks1 = 'IM-A-100278'
//GROUP BY j.Nazwa, t.TowId, t.Nazwa, t.Kod, t.Stawka, t.CenaEw, t.CenaDet, t.CenaHurt, t.CenaDod, t.CenaNoc, t.Opis1, t.Opis2, t.Opis3, t.Opis4, t.IleWZgrzewce, t.Indeks1, t.WysylacNaSklepInternetowy

//";