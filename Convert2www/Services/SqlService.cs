using Convert2www.Interfaces;
using Convert2www.Models;
using NLog;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Convert2www.Services
{
    internal class SqlService : ISqlService
    {
        public bool IsSqlConnectionActive { get; private set; } = false;

        private readonly ILogger _windowLogger;
        private readonly ILogger _logger;
        private readonly IConfig _config;
        private readonly IContext _context;
        private SqlCommand _sqlCommand;
        private SqlDataReader _sqlDataReader;
        private readonly SqlConnection _sqlConnection;

        public SqlService(IConfig config, IContext context)
        {
            _context = context;
            _config = config;
            _logger = LogManager.GetCurrentClassLogger();
            _windowLogger = LogManager.GetLogger(Constants.WindowLoggerName);
            _sqlConnection = new SqlConnection(SetSqlConnection());
            OpenSqlConnection();
        }

        public void WareReader()
        {
            _logger.Info("Reading wares data from sql");
            _windowLogger.Info("Operacje bazodanowe na towarach...");

            try
            {
                _sqlCommand = new SqlCommand(SqlQuerys.SelectWareTable, _sqlConnection);
                _sqlDataReader = _sqlCommand.ExecuteReader();
                while (_sqlDataReader.Read())
                {
                    var TTT = _sqlDataReader["StanMag"].ToString();

                    var ware = new WaresList
                    {
                        TowId = Convert.ToDecimal(_sqlDataReader["TowId"]),
                        Nazwa = _sqlDataReader["Nazwa"].ToString(),
                        Kod = _sqlDataReader["Kod"].ToString(),
                        Sww = _sqlDataReader["Indeks1"].ToString(),
                        JM = _sqlDataReader["jmNazwa"].ToString(),
                        Stan = Convert.ToDecimal(_sqlDataReader["StanMag"]),
                        Stawka = Convert.ToDouble(_sqlDataReader["Stawka"]),
                        CenaEw = ConvertPriceToNetto(Convert.ToDecimal(_sqlDataReader["CenaEw"])),
                        CenaDet = ConvertPriceToBrutto(Convert.ToDouble(_sqlDataReader["Stawka"]), Convert.ToDecimal(_sqlDataReader["CenaDet"])),
                        CenaHurt = ConvertPriceToNetto(Convert.ToDecimal(_sqlDataReader["CenaHurt"])),
                        CenaDod = ConvertPriceToBrutto(Convert.ToDouble(_sqlDataReader["Stawka"]), Convert.ToDecimal(_sqlDataReader["CenaDod"])),
                        CenaNoc = ConvertPriceToBrutto(Convert.ToDouble(_sqlDataReader["Stawka"]), Convert.ToDecimal(_sqlDataReader["CenaNoc"])),
                        Opis1 = _sqlDataReader["Opis1"].ToString(),
                        Opis2 = _sqlDataReader["Opis2"].ToString(),
                        Opis3 = _sqlDataReader["Opis3"].ToString(),
                        Opis4 = _sqlDataReader["Opis4"].ToString(),
                        IleWZgrzewce = Convert.ToDecimal(_sqlDataReader["IleWZgrzewce"])
                    };
                    _context.WareList.Add(ware);
                }
                _windowLogger.Info($"Przetworzonych towarów: {_context.WareList.Count}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Błąd podczas zapytania sql");
                _windowLogger.Error("Błąd. Szczegóły w log.");
            }
            finally
            {
                if (_sqlDataReader != null)
                    ((IDisposable)_sqlDataReader).Dispose();
            }
        }

        public void ConractorReader()
        {
            _logger.Info("Reading contrators data from sql");
            _windowLogger.Info("Operacje bazodanowe na kontrahentach...");

            try
            {
                _sqlCommand = new SqlCommand(SqlQuerys.SelectContractorTable, _sqlConnection);
                _sqlDataReader = _sqlCommand.ExecuteReader();
                while (_sqlDataReader.Read())
                {
                    var contractor = new ContractorsList
                    {
                        KontrId = Convert.ToDecimal(_sqlDataReader["KontrId"]),
                        Nazwa = _sqlDataReader["Nazwa"].ToString(),
                        Skrot = _sqlDataReader["Skrot"].ToString(),
                        Ulica = _sqlDataReader["Ulica"].ToString(),
                        Kod = _sqlDataReader["Kod"].ToString(),
                        Miasto = _sqlDataReader["Miasto"].ToString(),
                        Telefon = _sqlDataReader["Telefon"].ToString(),
                        Fax = _sqlDataReader["Fax"].ToString(),
                        EMail = _sqlDataReader["EMail"].ToString(),
                        NIP = _sqlDataReader["NIP"].ToString(),
                        Dostawca = Convert.ToDecimal(_sqlDataReader["Dostawca"]),
                        Odbiorca = Convert.ToDecimal(_sqlDataReader["Odbiorca"]),
                        NrDomu = _sqlDataReader["NrDomu"].ToString(),
                        NrLokalu = _sqlDataReader["NrLokalu"].ToString(),
                        KrajId = _sqlDataReader["krajKod"].ToString(),
                        KrajNazwa = _sqlDataReader["krajNazwa"].ToString()
                    };
                    _context.ContractorList.Add(contractor);
                }
                _windowLogger.Info($"Przetworzonych kontahentów: {_context.ContractorList.Count}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Błąd podczas zapytania sql");
                _windowLogger.Error("Błąd. Szczegóły w log.");
            }
            finally
            {
                if (_sqlDataReader != null)
                    ((IDisposable)_sqlDataReader).Dispose();
            }
        }

        private void OpenSqlConnection()
        {
            try
            {
                _sqlConnection.Open();
                _logger.Info("Sql connection is active");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Sql connection error");
                _windowLogger.Error("Błąd podczas podłączenia do serwera SQL. Szczegóły w log.");
            }
        }

        private decimal ConvertPriceToBrutto(double stawka, decimal netto)
        {
            if (stawka <= 0)
                return Math.Round(netto, 2);

            double _stawka = stawka / 100;
            double _netto = Convert.ToDouble(netto);
            double brutto = ((_stawka / 100) * _netto) + _netto;
            decimal value = Convert.ToDecimal(brutto);
            return Math.Round(value, 2);
        }

        private decimal ConvertPriceToNetto(decimal netto) => Math.Round(netto, 2);

        private string SetSqlConnection()
        {
            var connetionString = $"Server={_config.Sql.Host},{_config.Sql.Port}; Database={_config.Sql.Database}; User Id={_config.Sql.Username};Password={_config.Sql.Password}";
            var connetionStringAlternative = $"Data Source=(local)\\{_config.Sql.Instance}; Initial Catalog={_config.Sql.Database}; Integrated Security=True";
            var connection = new SqlConnection(connetionString);
            var connectionAlternative = new SqlConnection(connetionStringAlternative);
            var exceptionConnect = string.Empty;
            try
            {
                connection.Open();
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                    IsSqlConnectionActive = true;
                    return connetionString;
                }
            }
            catch (Exception ex)
            {
                exceptionConnect += ex;
            }

            try
            {
                connectionAlternative.Open();
                if (connectionAlternative.State == ConnectionState.Open)
                {
                    connectionAlternative.Close();
                    IsSqlConnectionActive = true;
                    return connetionStringAlternative;
                }
            }
            catch (Exception ex)
            {
                exceptionConnect += ex;
            }

            _logger.Error(exceptionConnect, "No access to the database");
            return null;
        }
    }
}