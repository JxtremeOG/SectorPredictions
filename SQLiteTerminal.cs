using System;
using System.Text;
using System.Data;
using Microsoft.Data.Sqlite;

class SQLiteTerminal
{
    public static List<string> sectorNames = new List<string> {
        "Technology",
        "Financial Services",
        "Consumer Cyclical",
        "Healthcare",
        "Communication Services",
        "Industrials",
        "Consumer Defensive",
        "Energy",
        "Basic Materials",
        "Real Estate",
        "Utilities"
    };
    Dictionary<string, double> techSectorStocks = new Dictionary<string, double>
    {
        { "AAPL", 3.557E12 }, { "MSFT", 2.951E12 }, { "NVDA", 2.706E12 }, { "TSM", 912.055E9 }, 
        { "AVGO", 838.509E9 }, { "ORCL", 418.264E9 }, { "SAPGF", 329.105E9 }, { "SAP", 325.85E9 }, 
        { "SSNLF", 301.846E9 }, { "ASMLF", 285.371E9 }, { "ASML", 283.774E9 }, { "CRM", 273.75E9 }, 
        { "CSCO", 252.547E9 }, { "IBM", 229.95E9 }, { "ACN", 213.601E9 }, { "ADBE", 195.792E9 }, 
        { "PLTR", 183.085E9 }, { "NOW", 178.895E9 }, { "QCOM", 173.36E9 }, { "XIACY", 172.945E9 }, 
        { "TXN", 172.719E9 }, { "XIACF", 172.272E9 }, { "INTU", 169.733E9 }, { "AMD", 160.431E9 }, 
        { "UBER", 157.844E9 }, { "SNEJF", 151.482E9 }, { "SONY", 147.675E9 }, { "SHOP", 132.022E9 }, 
        { "FI", 125.134E9 }, { "AMAT", 124.93E9 }, { "ARM", 123.934E9 }, { "ADP", 123.239E9 }, 
        { "ADI", 110.56E9 }, { "ANET", 106.248E9 }, { "KYCCF", 101.749E9 }, { "MU", 99.517E9 }, 
        { "LRCX", 98.393E9 }, { "KLAC", 92.579E9 }, { "INTC", 89.495E9 }, { "APP", 87.13E9 }, 
        { "CRWD", 85.65E9 }, { "INFY", 81.805E9 }, { "FTNT", 77.964E9 }, { "APH", 74.446E9 }, 
        { "CNSWF", 72.095E9 }, { "MSTR", 71.231E9 }, { "HNHPF", 70.825E9 }, { "MSI", 70.744E9 }, 
        { "TOELF", 68.964E9 }, { "SNPS", 68.692E9 }, { "WDAY", 67.5E9 }, { "TOELY", 67.08E9 }, 
        { "CDNS", 66.69E9 }, { "TEAM", 65.28E9 }, { "DELL", 65.173E9 }, { "ROP", 63.082E9 }, 
        { "MRVL", 62.544E9 }, { "PANW", 58.838E9 }, { "DASTY", 57.024E9 }, { "DASTF", 57.017E9 }, 
        { "ADYYF", 56.465E9 }, { "ADSK", 56.116E9 }, { "ADYEY", 55.684E9 }, { "NXPI", 54.509E9 }, 
        { "PAYX", 53.945E9 }, { "SNOW", 51.564E9 }, { "IFNNF", 51.317E9 }, { "IFNNY", 51.044E9 }, 
        { "BSQKZ", 50.505E9 }, { "NET", 44.965E9 }, { "FICO", 44.228E9 }, { "TEL", 44.064E9 }, 
        { "FJTSF", 42.821E9 }, { "GRMN", 42.612E9 }, { "CTSH", 41.431E9 }, { "ADTTF", 40.331E9 }, 
        { "FJTSY", 39.024E9 }, { "GLW", 38.966E9 }, { "FIS", 37.895E9 }, { "DDOG", 37.866E9 }, 
        { "ATEYY", 37.421E9 }, { "IT", 37.262E9 }, { "NIPNF", 36.479E9 }, { "AMADF", 35.653E9 }, 
        { "AMADY", 34.748E9 }, { "CAJFF", 34.43E9 }, { "MRAAF", 34.081E9 }, { "HXGBF", 33.727E9 }, 
        { "HUBS", 33.664E9 }, { "DISPF", 32.382E9 }, { "CAJPY", 32.37E9 }, { "MRAAY", 32.3E9 }, 
        { "TTD", 32.072E9 }, { "MCHP", 31.286E9 }, { "ZS", 31.019E9 }, { "HXGBY", 30.217E9 }, 
        { "NTTDF", 30.192E9 }, { "PCRFF", 29.489E9 }, { "RNECF", 29.215E9 }, { "RNECY", 28.945E9 }
    };
    Dictionary<string, double> financeSectorStocks = new Dictionary<string, double>
    {
        { "BRK-A", 1.075E12 }, { "BRK-B", 1.073E12 }, { "JPM", 828.48E9 }, { "V", 666.008E9 }, 
        { "MA", 504.642E9 }, { "BAC", 414.113E9 }, { "IDCBY", 340.353E9 }, { "JPM-PD", 335.727E9 }, 
        { "JPM-PC", 330.962E9 }, { "BML-PG", 319.806E9 }, { "BML-PH", 314.308E9 }, { "BAC-PK", 311.617E9 }, 
        { "BML-PL", 305.59E9 }, { "BAC-PL", 304.687E9 }, { "BAC-PE", 303.006E9 }, { "ACGBY", 267.564E9 }, 
        { "BML-PJ", 253.267E9 }, { "BAC-PB", 252.278E9 }, { "WFC", 241.887E9 }, { "BACHY", 231.002E9 }, 
        { "CICHY", 221.135E9 }, { "HBCYF", 219.399E9 }, { "GS", 214.327E9 }, { "HSBC", 214.136E9 }, 
        { "MS", 208.898E9 }, { "WFC-PY", 196.867E9 }, { "AXP", 194.173E9 }, { "WFC-PL", 189.081E9 }, 
        { "C", 177.622E9 }, { "BX", 176.632E9 }, { "CMWAY", 175.352E9 }, { "CBAUF", 171.123E9 }, 
        { "MBFJF", 169.685E9 }, { "PGR", 164.713E9 }, { "CIHKY", 161.05E9 }, { "RY", 160.869E9 }, 
        { "SPGI", 159.559E9 }, { "MUFG", 156.22E9 }, { "HDB", 150.527E9 }, { "BLK", 148.628E9 }, 
        { "ALIZF", 144.443E9 }, { "ALIZY", 143.13E9 }, { "SCHW", 138.317E9 }, { "PIAIF", 133.963E9 }, 
        { "UNCRY", 132.341E9 }, { "UNCFF", 132.059E9 }, { "PNGAY", 129.36E9 }, { "WFC-PC", 121.979E9 }, 
        { "MMC", 116.106E9 }, { "CB", 115.403E9 }, { "TD", 109.467E9 }, { "SMFNF", 107.427E9 }, 
        { "KKR", 107.165E9 }, { "UBS", 106.293E9 }, { "BNPQY", 104.32E9 }, { "BNPQF", 102.53E9 }, 
        { "BCDRF", 102.499E9 }, { "SAN", 101.037E9 }, { "SMFG", 100.271E9 }, { "IBN", 98.514E9 }, 
        { "IITSF", 98.403E9 }, { "DBSDF", 98.339E9 }, { "ZFSVF", 97.888E9 }, { "DBSDY", 97.413E9 }, 
        { "ICE", 97.409E9 }, { "ZURVY", 96.272E9 }, { "IVSXF", 94.031E9 }, { "USB-PH", 93.987E9 }, 
        { "IVSBF", 93.381E9 }, { "ISNPY", 92.542E9 }, { "AXAHY", 91.575E9 }, { "CME", 91.546E9 }, 
        { "AXAHF", 89.153E9 }, { "AAGIY", 88.54E9 }, { "GS-PA", 88.292E9 }, { "AON", 86.852E9 }, 
        { "MS-PA", 85.742E9 }, { "MCO", 85.22E9 }, { "MS-PK", 84.342E9 }, { "BBVXF", 83.318E9 }, 
        { "GS-PD", 82.116E9 }, { "MS-PI", 82.109E9 }, { "PSTVY", 82.074E9 }, { "AAIGF", 81.571E9 }, 
        { "MS-PF", 80.778E9 }, { "MURGY", 80.39E9 }, { "IBKR", 80.152E9 }, { "BBVA", 79.926E9 }, 
        { "MS-PE", 79.453E9 }, { "MURGF", 79.388E9 }, { "BN", 79.311E9 }, { "APO", 76.261E9 }, 
        { "LNSTY", 75.785E9 }, { "CGXYY", 75.517E9 }, { "LDNXF", 75.22E9 }, { "TKOMF", 74.919E9 }, 
        { "AJG", 74.579E9 }, { "SBKFF", 74.373E9 }, { "USB-PP", 74.009E9 }, { "MFG", 72.88E9 }
    };
    Dictionary<string, double> cyclicalSectorStocks = new Dictionary<string, double>
    {
        { "AMZN", 2.11E12 }, { "TSLA", 845.69E9 }, { "BABAF", 382.197E9 }, { "HD", 379.196E9 }, 
        { "LVMHF", 350.417E9 }, { "LVMUY", 350.191E9 }, { "BABA", 334.161E9 }, { "HESAF", 296.87E9 }, 
        { "HESAY", 291.23E9 }, { "TOYOF", 247.021E9 }, { "TM", 246.377E9 }, { "MCD", 222.417E9 }, 
        { "IDEXF", 169.942E9 }, { "IDEXY", 166.798E9 }, { "PDD", 166.402E9 }, { "BKNG", 155.421E9 }, 
        { "BYDDY", 146.586E9 }, { "BYDDF", 146.403E9 }, { "MPNGF", 140.326E9 }, { "LOW", 137.492E9 }, 
        { "MPNGY", 137.171E9 }, { "TJX", 135.63E9 }, { "CHDRY", 126.742E9 }, { "CHDRF", 121.887E9 }, 
        { "SBUX", 119.582E9 }, { "CFRHF", 119.301E9 }, { "NKE", 115.671E9 }, { "CFRUY", 114.105E9 }, 
        { "MELI", 103.689E9 }, { "FRCOF", 102.068E9 }, { "FRCOY", 96.806E9 }, { "ABNB", 84.201E9 }, 
        { "RACE", 80.14E9 }, { "SE", 79.919E9 }, { "JDCMF", 78.815E9 }, { "ORLY", 76.843E9 }, 
        { "MAR", 72.983E9 }, { "MBGAF", 71.369E9 }, { "MBGYY", 71.123E9 }, { "JD", 71E9 }, 
        { "CMG", 70.133E9 }, { "HLT", 61.218E9 }, { "AZO", 61.122E9 }, { "VWAPY", 59.774E9 }, 
        { "VLKPF", 59.194E9 }, { "VWAGY", 59.033E9 }, { "VLKAF", 58.623E9 }, { "RCL", 57.953E9 }, 
        { "CMPGF", 57.146E9 }, { "WFAFF", 57.119E9 }, { "CMPGY", 56.975E9 }, { "BAMXF", 56.058E9 }, 
        { "TRPCF", 55.937E9 }, { "BMWKY", 55.867E9 }, { "DRPRF", 54.632E9 }, { "WFAFY", 52.211E9 }, 
        { "GM", 51.901E9 }, { "ANCTF", 48.71E9 }, { "BYMOF", 47.767E9 }, { "ADDYY", 46.312E9 }, 
        { "ADDDF", 46.142E9 }, { "FLUT", 45.794E9 }, { "ROST", 45.747E9 }, { "HNDAF", 45.533E9 }, 
        { "HMC", 44.536E9 }, { "YUM", 44.445E9 }, { "DHI", 43.024E9 }, { "LULU", 42.374E9 }, 
        { "CPNG", 41.68E9 }, { "TCOM", 41.358E9 }, { "DNZOF", 40.288E9 }, { "F", 38.193E9 }, 
        { "DNZOY", 37.61E9 }, { "OLCLF", 37.392E9 }, { "STLA", 36.587E9 }, { "MAHMF", 35.661E9 }, 
        { "LEN-B", 34.454E9 }, { "HSHCY", 34.343E9 }, { "OLCLY", 34.161E9 }, { "LEN", 34E9 }, 
        { "PPRUY", 33.912E9 }, { "ANPDY", 33.855E9 }, { "HYMTF", 33.825E9 }, { "PPRUF", 33.411E9 }, 
        { "LVS", 32.909E9 }, { "EBAY", 32.51E9 }, { "ANPDF", 32.315E9 }, { "QSR", 30.315E9 }, 
        { "ARLUF", 29.197E9 }, { "LI", 28.761E9 }, { "BRDCF", 28.677E9 }, { "CCL", 28.004E9 }, 
        { "LAAOF", 27.209E9 }, { "CUKPF", 27.204E9 }, { "BRDCY", 27.022E9 }, { "DRPRY", 26.51E9 }, 
        { "GWLLY", 26.154E9 }, { "SZKMF", 25.735E9 }, { "ROL", 25.039E9 }, { "YAHOY", 24.759E9 }
    };
    Dictionary<string, double> healthCareSectorStocks = new Dictionary<string, double>
    {
        { "LLY", 820.86E9 }, { "UNH", 448.841E9 }, { "NONOF", 415.583E9 }, { "JNJ", 399.256E9 }, 
        { "NVO", 395.026E9 }, { "ABBV", 372.425E9 }, { "RHHBF", 284.338E9 }, { "RHHBY", 276.101E9 }, 
        { "RHHVF", 273.154E9 }, { "AZN", 240.206E9 }, { "AZNCF", 239.493E9 }, { "MRK", 237.786E9 }, 
        { "ABT", 234.291E9 }, { "NVS", 225.658E9 }, { "NVSEF", 224.018E9 }, { "TMO", 201.822E9 }, 
        { "ISRG", 191.827E9 }, { "AMGN", 170.839E9 }, { "DHR", 155.152E9 }, { "SNYNF", 152.419E9 }, 
        { "SNY", 148.977E9 }, { "PFE", 148.702E9 }, { "BSX", 148.562E9 }, { "SYK", 145.743E9 }, 
        { "GILD", 144.617E9 }, { "ESLOF", 134.263E9 }, { "ESLOY", 133.018E9 }, { "VRTX", 125.087E9 }, 
        { "BMY", 122.056E9 }, { "MDT", 119.42E9 }, { "ELV", 93.412E9 }, { "CI", 87.793E9 }, 
        { "CMXHF", 83.866E9 }, { "CHGCF", 83.56E9 }, { "CVS", 82.111E9 }, { "HCA", 81.498E9 }, 
        { "MCK", 81.456E9 }, { "CHGCY", 80.801E9 }, { "GSK", 80.182E9 }, { "CSLLY", 79.506E9 }, 
        { "GLAXF", 78.01E9 }, { "REGN", 76.752E9 }, { "ZTS", 75.457E9 }, { "MKGAF", 66.187E9 }, 
        { "MKKGY", 66.068E9 }, { "BDX", 65.151E9 }, { "SEMHF", 64.691E9 }, { "SMMNY", 63.037E9 }, 
        { "TKPHF", 51.026E9 }, { "COR", 49.049E9 }, { "ARGNF", 49.003E9 }, { "HLNCF", 48.672E9 }, 
        { "TAK", 47.555E9 }, { "HLN", 46.978E9 }, { "LZAGF", 46.743E9 }, { "LZAGY", 46.443E9 }, 
        { "ALC", 45.563E9 }, { "DSNKY", 44.39E9 }, { "DSKYF", 43.92E9 }, { "UCBJF", 43.529E9 }, 
        { "EW", 42.348E9 }, { "HOCPF", 41.577E9 }, { "HOCPY", 40.735E9 }, { "UCBJY", 39.6E9 }, 
        { "GEHC", 39.022E9 }, { "VEEV", 38.239E9 }, { "ARGX", 36.666E9 }, { "RSMDF", 36.302E9 }, 
        { "A", 36.091E9 }, { "IDXX", 35.131E9 }, { "IQV", 33.663E9 }, { "RMD", 33.034E9 }, 
        { "ALNY", 32.004E9 }, { "HUM", 32E9 }, { "DXCM", 31.056E9 }, { "CAH", 30.738E9 }, 
        { "CNC", 30.014E9 }, { "GDERF", 29.78E9 }, { "GALDY", 29.209E9 }, { "WUXIF", 28.712E9 }, 
        { "TRUMF", 28.067E9 }, { "TRUMY", 27.435E9 }, { "OTSKY", 26.902E9 }, { "MTD", 26.854E9 }, 
        { "BNTX", 26.57E9 }, { "BAYRY", 26.368E9 }, { "OTSKF", 26.227E9 }, { "BAYZF", 25.877E9 }, 
        { "CLPBF", 25.353E9 }, { "PHG", 25.34E9 }, { "WUXAY", 25.198E9 }, { "RYLPF", 24.869E9 }, 
        { "FSNUY", 24.828E9 }, { "CLPBY", 24.325E9 }, { "FSNUF", 23.817E9 }, { "WAT", 22.939E9 }, 
        { "STE", 22.925E9 }, { "SAUHF", 22.648E9 }, { "BIIB", 21.475E9 }, { "SAUHY", 21.302E9 }
    };
    Dictionary<string, double> communicationSectorStocks = new Dictionary<string, double>
    {
        { "GOOG", 2.121E12 }, { "GOOGL", 2.119E12 }, { "META", 1.585E12 }, { "TCTZF", 634.89E9 }, 
        { "TCEHY", 631.501E9 }, { "NFLX", 387.247E9 }, { "TMUS", 305.151E9 }, { "T", 191.795E9 }, 
        { "DIS", 190.927E9 }, { "VZ", 186.192E9 }, { "DTEGF", 178.683E9 }, { "DTEGY", 177.488E9 }, 
        { "CMCSA", 138.789E9 }, { "PROSY", 115.551E9 }, { "SPOT", 109.812E9 }, { "PROSF", 106.519E9 }, 
        { "RCRRF", 93.827E9 }, { "RCRUY", 91.138E9 }, { "NTDOF", 88.465E9 }, { "NTDOY", 87.365E9 }, 
        { "SFTBF", 83.888E9 }, { "NTTYY", 81.409E9 }, { "SFTBY", 76.796E9 }, { "DASH", 74.817E9 }, 
        { "SOBKY", 68.741E9 }, { "NTES", 66.395E9 }, { "KDDIF", 66.273E9 }, { "KDDIY", 66.148E9 }, 
        { "NETTF", 65.406E9 }, { "UNVGY", 54.615E9 }, { "CHTR", 54.448E9 }, { "UMGNF", 53.408E9 }, 
        { "NAPRF", 48.4E9 }, { "NPSNY", 47.591E9 }, { "AMX", 45.171E9 }, { "BECEF", 42.759E9 }, 
        { "GSAT", 42.427E9 }, { "SGAPY", 41.994E9 }, { "RBLX", 38.435E9 }, { "TTWO", 36.508E9 }, 
        { "EA", 35.876E9 }, { "BAIDF", 34.828E9 }, { "ORANY", 33.15E9 }, { "BIDU", 32.912E9 }, 
        { "TLGPY", 31.336E9 }, { "FNCTF", 30.912E9 }, { "CHT", 30.37E9 }, { "SWZCF", 30.314E9 }, 
        { "LYV", 30.234E9 }, { "SCMWY", 29.983E9 }, { "KUASF", 29.057E9 }, { "PGPEF", 27.586E9 }, 
        { "RDDT", 26.463E9 }, { "WBD", 26.249E9 }, { "PUBGY", 24.949E9 }, { "FOXA", 24.618E9 }, 
        { "AVIVF", 24.302E9 }, { "FOX", 24.14E9 }, { "PINS", 23.323E9 }, { "TU", 23.305E9 }, 
        { "VOD", 23.186E9 }, { "THQQF", 23.023E9 }, { "FWONA", 23.02E9 }, { "FWONB", 22.872E9 }, 
        { "FWONK", 22.798E9 }, { "RPGRY", 22.608E9 }, { "BCE", 22.342E9 }, { "TME", 22.197E9 }, 
        { "AVIFY", 21.413E9 }, { "RPGRF", 20.987E9 }, { "TELNY", 18.164E9 }, { "TELNF", 17.9E9 }, 
        { "BOIVF", 17.848E9 }, { "KONMY", 17.637E9 }, { "WMG", 17.519E9 }, { "ZG", 17.343E9 }, 
        { "Z", 17.132E9 }, { "NWS", 17.04E9 }, { "NWSA", 16.584E9 }, { "RCIAF", 16.535E9 }, 
        { "OMC", 16.119E9 }, { "SNAP", 16.101E9 }, { "RCI", 15.286E9 }, { "TLK", 15.209E9 }, 
        { "IFPJF", 13.781E9 }, { "VIV", 13.763E9 }, { "TLSNY", 13.33E9 }, { "IFJPY", 13.117E9 }, 
        { "MTNOF", 12.833E9 }, { "VDMCY", 12.606E9 }, { "LBRDA", 12.54E9 }, { "LBRDK", 12.472E9 }, 
        { "PSORF", 12.159E9 }, { "TKO", 11.928E9 }, { "NEXOF", 11.618E9 }, { "ROKU", 11.558E9 }, 
        { "MTNOY", 11.489E9 }, { "CCOEF", 11.326E9 }, { "CEVMY", 11.326E9 }, { "NEXOY", 11.314E9 }
    };
    Dictionary<string, double> industrialsSectorStocks = new Dictionary<string, double>
    {
        { "RCIT", 293.477E9 }, { "GE", 212.183E9 }, { "SMAWF", 204.42E9 }, { "SIEGY", 201.882E9 }, 
        { "RTX", 170.743E9 }, { "CAT", 164.64E9 }, { "UNP", 148.697E9 }, { "EADSF", 147.942E9 }, 
        { "EADSY", 147.001E9 }, { "SBGSF", 142.893E9 }, { "HON", 137.371E9 }, { "SBGSY", 134.303E9 }, 
        { "DE", 132.105E9 }, { "SAFRF", 127.692E9 }, { "SAFRY", 124.175E9 }, { "BA", 118.534E9 }, 
        { "HTHIF", 118.265E9 }, { "ETN", 109.688E9 }, { "LMT", 109.392E9 }, { "ABLZF", 102.916E9 }, 
        { "UPS", 102.831E9 }, { "ABBNY", 101.305E9 }, { "WM", 90.693E9 }, { "RYCEY", 89.387E9 }, 
        { "RLXXF", 88.778E9 }, { "RYCEF", 88.033E9 }, { "RELX", 87.672E9 }, { "ATLKY", 84.035E9 }, 
        { "ATLCY", 82.517E9 }, { "PH", 81.213E9 }, { "CTAS", 80.902E9 }, { "MMM", 80.018E9 }, 
        { "TRI", 78.771E9 }, { "ITW", 78.287E9 }, { "TT", 76.897E9 }, { "TDG", 75.612E9 }, 
        { "CP", 73.165E9 }, { "GD", 72.743E9 }, { "RSG", 72.492E9 }, { "VCISY", 71.679E9 }, 
        { "NOC", 69.234E9 }, { "ITOCF", 68.989E9 }, { "MTSUY", 67.757E9 }, { "MSBHF", 67.677E9 }, 
        { "EMR", 66.337E9 }, { "VLVLY", 65.209E9 }, { "ITOCY", 64.846E9 }, { "BAESF", 64.835E9 }, 
        { "BAESY", 64.805E9 }, { "CNI", 64.1E9 }, { "VCISF", 63.575E9 }, { "VOLAF", 63.055E9 }, 
        { "VOLVF", 62.843E9 }, { "FDX", 61.029E9 }, { "HTHIY", 59.903E9 }, { "MITSF", 59.713E9 }, 
        { "CSX", 59.646E9 }, { "CARR", 58.652E9 }, { "THLEF", 57.389E9 }, { "RNMBF", 56.79E9 }, 
        { "MHVYF", 56.56E9 }, { "MHVIY", 56.325E9 }, { "RNMBY", 56.082E9 }, { "PCAR", 56.043E9 }, 
        { "CODYY", 56.037E9 }, { "DPSTF", 55.775E9 }, { "THLLY", 55.338E9 }, { "MITSY", 53.924E9 }, 
        { "NSC", 53.89E9 }, { "OSAGY", 53.844E9 }, { "JCI", 53.683E9 }, { "DHLGY", 53.522E9 }, 
        { "OSAGF", 52.832E9 }, { "CPRT", 51.77E9 }, { "CODGF", 51.743E9 }, { "LTOUF", 51.143E9 }, 
        { "HWM", 50.575E9 }, { "SMEGF", 50.134E9 }, { "DSNKY", 49.571E9 }, { "SMNEY", 48.988E9 }, 
        { "DSDVF", 48.672E9 }, { "GWW", 48.497E9 }, { "WCN", 47.928E9 }, { "CMI", 47.309E9 }, 
        { "FAST", 44.599E9 }, { "EXPGF", 43.922E9 }, { "AME", 42.593E9 }, { "EXPGY", 42.336E9 }, 
        { "VRSK", 41.708E9 }, { "URI", 41.072E9 }, { "OTIS", 40.973E9 }, { "LHX", 40.549E9 }, 
        { "AXON", 38.075E9 }, { "DTRUY", 37.946E9 }, { "ODFL", 37.913E9 }, { "CTPCY", 36.712E9 }, 
        { "MIELY", 36.681E9 }, { "MIELF", 36.356E9 }, { "SHLRF", 36.262E9 }, { "ASAZY", 36.198E9 }
    };
    Dictionary<string, double> defensiveSectorStocks = new Dictionary<string, double>
    {
        { "WMT", 760.74E9 }, { "COST", 454.869E9 }, { "PG", 411.331E9 }, { "KO", 303.528E9 },
        { "NSRGY", 256E9 }, { "NSRGF", 255.599E9 }, { "PM", 238.744E9 }, { "PEP", 212.123E9 },
        { "LRLCF", 210.153E9 }, { "LRLCY", 208.845E9 }, { "FMX", 171.618E9 }, { "FMXUF", 153.667E9 },
        { "UNLYF", 145.622E9 }, { "UL", 144.062E9 }, { "BUD", 123.097E9 }, { "BUDFF", 121.689E9 },
        { "MO", 96.621E9 }, { "MDLZ", 89.271E9 }, { "BTI", 88.541E9 }, { "BTAFF", 85.079E9 },
        { "CL", 76.219E9 }, { "DEO", 63.436E9 }, { "DGEAF", 63.287E9 }, { "MNST", 54.16E9 },
        { "TGT", 52.259E9 }, { "HINKF", 50.553E9 }, { "GPDNF", 49.174E9 }, { "DANOY", 49.02E9 },
        { "HEINY", 48.969E9 }, { "KMB", 47.492E9 }, { "RBGLY", 47.21E9 }, { "WMMVY", 46.739E9 },
        { "KR", 46.177E9 }, { "RBGPF", 45.887E9 }, { "JAPAY", 45.764E9 }, { "KDP", 45.482E9 },
        { "KVUE", 44.442E9 }, { "JAPAF", 42.897E9 }, { "LBLCF", 40.71E9 }, { "CCEP", 38.927E9 },
        { "SVNDY", 38.433E9 }, { "KHC", 37.823E9 }, { "HENOF", 37.627E9 }, { "SYY", 36.886E9 },
        { "HENKY", 36.631E9 }, { "HSY", 36.426E9 }, { "HENOY", 35.812E9 }, { "SVNDF", 35.73E9 },
        { "AHODF", 35.474E9 }, { "GIS", 34.542E9 }, { "HELKF", 34.145E9 }, { "ADRNY", 33.441E9 },
        { "TSCDY", 32.625E9 }, { "BDRFY", 32.586E9 }, { "STZ", 32.495E9 }, { "BDRFF", 32.055E9 },
        { "CHLSY", 31.283E9 }, { "DLMAF", 30.054E9 }, { "IMBBF", 29.663E9 }, { "IMBBY", 29.546E9 },
        { "K", 28.413E9 }, { "PRNDY", 28.197E9 }, { "PDRDF", 28.147E9 }, { "CHD", 27.168E9 },
        { "WOLWF", 26.262E9 }, { "EL", 25.681E9 }, { "ADM", 22.998E9 }, { "CLEGF", 22.966E9 },
        { "WNGRF", 22.032E9 }, { "MKC-V", 22.005E9 }, { "MKC", 21.965E9 }, { "TSN", 21.54E9 },
        { "HKHHY", 21.538E9 }, { "KAOCF", 21.445E9 }, { "HKHHF", 21.224E9 }, { "ETTYF", 21.069E9 },
        { "AJINY", 20.558E9 }, { "AJINF", 20.15E9 }, { "KAOOY", 20.019E9 }, { "KRYAF", 18.776E9 },
        { "COCSF", 18.75E9 }, { "ASBFF", 18.609E9 }, { "ASBRF", 18.539E9 }, { "KOF", 18.38E9 },
        { "CLX", 18.365E9 }, { "CABGY", 18.035E9 }, { "CABJF", 17.951E9 }, { "ASBFY", 17.928E9 },
        { "KRYAY", 17.917E9 }, { "EMBVF", 17.434E9 }, { "BF-A", 16.953E9 }, { "BF-B", 16.876E9 },
        { "DG", 16.767E9 }, { "HRL", 15.921E9 }, { "DQJCY", 15.803E9 }, { "CCHGY", 15.788E9 },
        { "USFD", 15.563E9 }, { "WLMIY", 15.326E9 }, { "MTRAF", 14.91E9 }, { "DLTR", 14.859E9 }
    };
    Dictionary<string, double> energySectorStocks = new Dictionary<string, double>
    {
        { "XOM", 473E9 }, { "CVX", 272.698E9 }, { "RYDAF", 202.4E9 }, { "SHEL", 201.565E9 },
        { "TTFNF", 137.959E9 }, { "TTE", 137.147E9 }, { "COP", 114.609E9 }, { "CSUAY", 98.647E9 },
        { "EBBNF", 92.555E9 }, { "ENB", 90.603E9 }, { "BPAQF", 83.531E9 }, { "BP", 82.964E9 },
        { "PBR", 78.693E9 }, { "PBR-A", 78.404E9 }, { "EPD", 70.988E9 }, { "EOG", 68.726E9 },
        { "WMB", 66.57E9 }, { "STOHF", 61.663E9 }, { "EQNR", 61.198E9 }, { "ET", 60.603E9 },
        { "CNQ", 59.083E9 }, { "KMI", 58.451E9 }, { "SLB", 56.147E9 }, { "MPLX", 53.304E9 },
        { "OKE", 52.431E9 }, { "TNCAF", 51.392E9 }, { "PSX", 50.711E9 }, { "LNG", 48.33E9 },
        { "TRP", 47.219E9 }, { "EIPAF", 46.251E9 }, { "E", 44.554E9 }, { "MPC", 44.21E9 },
        { "HES", 44.166E9 }, { "SU", 43.685E9 }, { "OXY", 42.892E9 }, { "BKR", 41.461E9 },
        { "FANG", 40.849E9 }, { "VLO", 39.89E9 }, { "TRGP", 39.683E9 }, { "IMO", 34.089E9 },
        { "WOPEF", 31.115E9 }, { "TCANF", 30.443E9 }, { "TPL", 29.982E9 }, { "CQP", 29.841E9 },
        { "PUTRY", 29.077E9 }, { "WDS", 28.652E9 }, { "EQT", 27.543E9 }, { "CVE", 23.162E9 },
        { "DVN", 22.249E9 }, { "EXE", 22.241E9 }, { "PEXNY", 21.914E9 }, { "PBA", 21.898E9 },
        { "HAL", 21.479E9 }, { "TS", 20.441E9 }, { "TNRSF", 19.882E9 }, { "EC", 19.715E9 },
        { "PSKOF", 19.037E9 }, { "CTRA", 18.787E9 }, { "CCOZY", 18.718E9 }, { "CCJ", 18.335E9 },
        { "JXHGF", 16.912E9 }, { "TRMLF", 16.268E9 }, { "IPXHF", 16.197E9 }, { "OMVKY", 15.431E9 },
        { "IPXHY", 15.374E9 }, { "JXHLY", 15.115E9 }, { "YZCAY", 14.82E9 }, { "REPYY", 14.683E9 },
        { "REPYF", 14.568E9 }, { "WES", 14.412E9 }, { "YPF", 14.159E9 }, { "PAA", 13.37E9 },
        { "OMVJF", 13.03E9 }, { "AKRBF", 12.566E9 }, { "AKRBY", 12.212E9 }, { "GLPEF", 12.072E9 },
        { "GLPEY", 11.507E9 }, { "FTI", 11.193E9 }, { "AR", 10.511E9 }, { "AETUF", 10.296E9 },
        { "PR", 9.994E9 }, { "OVV", 9.89E9 }, { "IDKOY", 9.7E9 }, { "NATKY", 9.505E9 },
        { "DTM", 9.084E9 }, { "KLYCY", 8.676E9 }, { "HESM", 8.665E9 }, { "IDKOF", 8.567E9 },
        { "RRC", 8.538E9 }, { "AM", 7.841E9 }, { "VNOM", 7.815E9 }, { "SUN", 7.739E9 },
        { "NTOIF", 7.689E9 }, { "ATGFF", 7.507E9 }, { "VARRY", 7.152E9 }, { "APA", 6.937E9 },
        { "NFG", 6.681E9 }, { "DCCPF", 6.66E9 }, { "KEYUF", 6.352E9 }, { "CHRD", 6.263E9 }
    };
    Dictionary<string, double> realEstateSectorStocks = new Dictionary<string, double>
    {
        { "GDVTZ", 272.79E9 }, { "PLD", 111.419E9 }, { "AMT", 96.696E9 }, { "WELL", 92.762E9 },
        { "EQIX", 84.128E9 }, { "SPG", 65.535E9 }, { "PSA", 54.032E9 }, { "DLR", 50.182E9 },
        { "O", 50.176E9 }, { "SPG-PJ", 49.504E9 }, { "PLDGP", 42.537E9 }, { "CCI", 41.365E9 },
        { "CBRE", 40.902E9 }, { "PSA-PH", 38.792E9 }, { "GMGSF", 37.122E9 }, { "EXR", 34.198E9 },
        { "PSA-PK", 34.027E9 }, { "VICI", 33.702E9 }, { "CSGP", 31.51E9 }, { "AVB", 31.079E9 },
        { "BEKE", 29.323E9 }, { "SUHJY", 28.719E9 }, { "VTR", 28.397E9 }, { "EQR", 28.123E9 },
        { "DWAHF", 26.245E9 }, { "SUHJF", 25.358E9 }, { "DLR-PK", 25.291E9 }, { "CRBJY", 25.101E9 },
        { "MTSFY", 24.631E9 }, { "MTSFF", 24.535E9 }, { "IRM", 24.296E9 }, { "VNNVF", 24.069E9 },
        { "CLNXF", 23.886E9 }, { "CLLNY", 23.692E9 }, { "SBAC", 23.52E9 }, { "DLR-PJ", 23.065E9 },
        { "VONOY", 23.007E9 }, { "WY", 21.957E9 }, { "CAOVY", 21.446E9 }, { "DWAHY", 21.338E9 },
        { "INVH", 20.7E9 }, { "VTAGY", 20.595E9 }, { "VTWRF", 20.262E9 }, { "ESS", 20.076E9 },
        { "MAA", 20.005E9 }, { "MITEF", 18.583E9 }, { "MITEY", 18.483E9 }, { "CNGKY", 18.016E9 },
        { "ARE", 17.441E9 }, { "SUI", 17.418E9 }, { "UDR", 16.612E9 }, { "LINE", 15.378E9 },
        { "AMH", 15.181E9 }, { "SURDF", 15.163E9 }, { "KIM", 14.473E9 }, { "DOC", 14.08E9 },
        { "WPC", 13.998E9 }, { "GLPI", 13.728E9 }, { "NLY-PG", 13.672E9 }, { "REG", 13.595E9 },
        { "ELS", 13.562E9 }, { "NLY-PF", 13.481E9 }, { "CPT", 12.996E9 }, { "LAMR", 12.617E9 },
        { "MAA-PI", 12.273E9 }, { "JLL", 12.187E9 }, { "NLY", 11.945E9 }, { "UNBLF", 11.879E9 },
        { "SEGXF", 11.825E9 }, { "BXP", 11.759E9 }, { "SZRRF", 11.481E9 }, { "HST", 11.195E9 },
        { "CPPBY", 10.78E9 }, { "VNORP", 10.447E9 }, { "DTCWY", 10.13E9 }, { "VNO-PL", 10.062E9 },
        { "BPYPP", 10.005E9 }, { "OHI", 9.983E9 }, { "HNGKY", 9.949E9 }, { "VNO-PM", 9.856E9 },
        { "DWHHF", 9.637E9 }, { "CUBE", 9.609E9 }, { "LGFRY", 9.568E9 }, { "REXR", 9.393E9 },
        { "KLPEF", 9.352E9 }, { "AYAAY", 9.191E9 }, { "SNLAY", 9.186E9 }, { "EGP", 9.186E9 },
        { "IFSUF", 9.127E9 }, { "AGNCM", 9.03E9 }, { "AGNC", 8.98E9 }, { "AGNCN", 8.933E9 },
        { "FRT-PC", 8.733E9 }, { "FRT", 8.639E9 }, { "BALDF", 8.521E9 }, { "AMH-PH", 8.475E9 },
        { "ADC", 8.163E9 }, { "WARFY", 8.091E9 }, { "BRX", 8.074E9 }, { "AMH-PG", 8.055E9 }
    };
    Dictionary<string, double> basicMaterialsSectorStocks = new Dictionary<string, double>
    {
        { "LIN", 221.966E9 }, { "BHP", 128.542E9 }, { "BHPLF", 128.232E9 }, { "RTNTF", 112.629E9 },
        { "AIQUF", 111.908E9 }, { "AIQUY", 111.493E9 }, { "RIO", 103.743E9 }, { "RTPPF", 95.857E9 },
        { "SHW", 90.687E9 }, { "ECL", 75.392E9 }, { "SCCO", 71.785E9 }, { "APD", 69.151E9 },
        { "CRH", 66.998E9 }, { "HCMLY", 64.048E9 }, { "CTA-PB", 63.228E9 }, { "HCMLF", 62.934E9 },
        { "SHECF", 62.308E9 }, { "SHECY", 60.097E9 }, { "ZIJMY", 58.052E9 }, { "PTCAY", 54.165E9 },
        { "FCX", 54.144E9 }, { "BASFY", 51.928E9 }, { "BFFAF", 51.766E9 }, { "GLNCY", 50.834E9 },
        { "CTA-PA", 50.134E9 }, { "NEM", 50.001E9 }, { "AEM", 49.088E9 }, { "SKFOF", 43.727E9 },
        { "SXYAY", 43.044E9 }, { "CTVA", 41.829E9 }, { "VALE", 41.748E9 }, { "GVDNY", 40.734E9 },
        { "GVDBF", 38.833E9 }, { "NGLOY", 38.164E9 }, { "AAUKF", 37.917E9 }, { "DD", 32.918E9 },
        { "HLBZF", 32.41E9 }, { "GOLD", 32.269E9 }, { "WPM", 31.847E9 }, { "FSUMF", 31.498E9 },
        { "VMC", 31.057E9 }, { "NUE", 30.993E9 }, { "FSUGY", 30.924E9 }, { "NVZMF", 29.224E9 },
        { "MLM", 29.087E9 }, { "DSFIY", 27.616E9 }, { "FNV", 27.463E9 }, { "NVZMY", 27.055E9 },
        { "PPG", 26.766E9 }, { "DSMFF", 26.579E9 }, { "SLMNP", 26.551E9 }, { "MT", 26.391E9 },
        { "AMSYF", 26.346E9 }, { "DOW", 26.211E9 }, { "NTR", 25.234E9 }, { "LYB", 24.652E9 },
        { "NPSCY", 23.831E9 }, { "ANFGF", 23.177E9 }, { "KLBAY", 23.101E9 }, { "NISTF", 23.088E9 },
        { "IFF", 20.981E9 }, { "TECK", 20.75E9 }, { "TCKRF", 20.669E9 }, { "STLD", 19.069E9 },
        { "AHCHY", 19.03E9 }, { "JHIUF", 18.848E9 }, { "GFI", 17.292E9 }, { "UPMKF", 16.845E9 },
        { "EMSHF", 16.51E9 }, { "GFIOF", 16.468E9 }, { "UPMMY", 16.442E9 }, { "RPM", 16.009E9 },
        { "AU", 15.538E9 }, { "PKX", 15.341E9 }, { "RS", 15.334E9 }, { "NPCPF", 15.272E9 },
        { "SYIEF", 14.889E9 }, { "SYIEY", 14.363E9 }, { "WLK", 14.254E9 }, { "JHX", 13.812E9 },
        { "TYNPF", 13.781E9 }, { "NDEKY", 13.761E9 }, { "NDEKF", 13.721E9 }, { "KGC", 13.692E9 },
        { "IVPAF", 13.428E9 }, { "CF", 13.332E9 }, { "NHYDY", 12.91E9 }, { "NESRF", 12.853E9 },
        { "NHYKF", 12.378E9 }, { "SUZ", 11.872E9 }, { "COVTY", 11.811E9 }, { "EVKIF", 11.783E9 },
        { "SQM", 11.766E9 }, { "AKZOF", 11.479E9 }, { "CVVTF", 11.323E9 }, { "AKZOY", 11.262E9 },
        { "EMN", 11.237E9 }, { "EVKIY", 10.998E9 }, { "SEOFF", 10.849E9 }, { "FQVLF", 10.845E9 }
    };
    Dictionary<string, double> utilitiesSectorStocks = new Dictionary<string, double>
    {
        { "NEE", 143.969E9 }, { "SO", 97.209E9 }, { "IBDRY", 91.534E9 }, { "IBDSF", 91.041E9 },
        { "DUK", 88.712E9 }, { "GEV", 80.824E9 }, { "ESOCF", 73.316E9 }, { "ENLAY", 72.605E9 },
        { "CEG", 64.959E9 }, { "DUK-PA", 61.583E9 }, { "NEE-PR", 60.335E9 }, { "NGGTF", 59.503E9 },
        { "NGG", 58.086E9 }, { "AEP", 54.636E9 }, { "D", 45.1E9 }, { "SRE", 44.097E9 },
        { "ENGIY", 43.76E9 }, { "EXC", 43.087E9 }, { "PCG", 42.954E9 }, { "ENGQF", 40.503E9 },
        { "PEG", 38.961E9 }, { "VST", 38.925E9 }, { "XEL", 38.853E9 }, { "EONGY", 35.31E9 },
        { "ED", 34.961E9 }, { "ENAKF", 34.52E9 }, { "WEC", 32.838E9 }, { "OEZVF", 29.62E9 },
        { "DTE", 27.035E9 }, { "AWK", 26.93E9 }, { "AEE", 25.929E9 }, { "OEZVY", 25.673E9 },
        { "GASNY", 25.38E9 }, { "PPL", 25.01E9 }, { "GASNF", 24.944E9 }, { "RWEOY", 24.77E9 },
        { "ELEZF", 24.368E9 }, { "ELEZY", 24.028E9 }, { "RWNFF", 23.275E9 }, { "ATO", 22.743E9 },
        { "FE", 22.056E9 }, { "UNPRF", 21.965E9 }, { "CNP", 21.768E9 }, { "FTS", 21.722E9 },
        { "CLPHY", 21.594E9 }, { "ES", 21.511E9 }, { "CMS", 21.405E9 }, { "EIX", 21.324E9 },
        { "CZAVF", 21.094E9 }, { "SSEZF", 21.016E9 }, { "CLPHF", 20.944E9 }, { "SSEZY", 20.769E9 },
        { "HRNNF", 19.781E9 }, { "DNNGY", 19.077E9 }, { "DOGEF", 18.91E9 }, { "NRG", 18.462E9 },
        { "CKISF", 18.141E9 }, { "NI", 18.013E9 }, { "CKISY", 17.637E9 }, { "ETR", 17.406E9 },
        { "TERRF", 17.05E9 }, { "TNABY", 16.567E9 }, { "TEZNY", 16.441E9 }, { "LNT", 15.96E9 },
        { "SNMRY", 15.896E9 }, { "EVRG", 15.116E9 }, { "EBR", 14.62E9 }, { "EBR-B", 14.609E9 },
        { "HGKGF", 14.562E9 }, { "HGKGY", 14.534E9 }, { "BEP", 14.41E9 }, { "FOJCF", 13.549E9 },
        { "CLPXY", 13.456E9 }, { "EDPFY", 13.366E9 }, { "BIP", 13.327E9 }, { "TKGSF", 12.453E9 },
        { "TKGSY", 12.182E9 }, { "EMRAF", 11.965E9 }, { "OGFGY", 11.946E9 }, { "GAILF", 11.835E9 },
        { "KAEPF", 11.503E9 }, { "CRPJY", 11.487E9 }, { "OGFGF", 11.406E9 }, { "ERRAF", 11.364E9 },
        { "KAEPY", 11.252E9 }, { "MERVF", 11.199E9 }, { "SBS", 11.107E9 }, { "WTRG", 10.559E9 },
        { "PNW", 10.319E9 }, { "RDEIF", 9.994E9 }, { "CPYYY", 9.899E9 }, { "KEP", 9.822E9 },
        { "RDEIY", 9.741E9 }, { "EDRVY", 9.323E9 }, { "SVTRF", 9.252E9 }, { "STRNY", 9.198E9 },
        { "TLN", 9.181E9 }, { "IESFY", 9.034E9 }, { "EDRVF", 8.984E9 }, { "CGASY", 8.907E9 }
    };
    
    private SqliteConnection sqlite;

    public SQLiteTerminal()
    {
        sqlite = new SqliteConnection("Data Source=MARKETADMIN.db");
    }

    public void CreateNewsTable() {
        try
        {
            sqlite.Open();  // Open connection to the DB
            using (SqliteCommand createTable = sqlite.CreateCommand())
            {
                createTable.CommandText = @"
                    CREATE TABLE IF NOT EXISTS MARKET_NEWS_DATA (
                        SECTOR TEXT,
                        TICKER TEXT,
                        YEAR TEXT,
                        QUARTER TEXT,
                        TOTAL_POSITIVE INTEGER,
                        TOTAL_NEGATIVE INTEGER,
                        TOTAL_NEUTRAL INTEGER,
                        SENTIMENT_SCORE REAL,
                        PRIMARY KEY (SECTOR, TICKER, QUARTER, YEAR)
                    );
                ";
                createTable.ExecuteNonQuery();
            }
        }
        catch (SqliteException ex)
        {
            Console.WriteLine("Error creating table: " + ex.Message);
        }
        finally
        {
            sqlite.Close();
        }
    }

    public void ClearNewsTable() {
        try
        {
            sqlite.Open();  // Open connection to the DB
            using (SqliteCommand createTable = sqlite.CreateCommand())
            {
                createTable.CommandText = @"
                    DELETE FROM MARKET_NEWS_DATA;
                ";
                createTable.ExecuteNonQuery();
            }
        }
        catch (SqliteException ex)
        {
            Console.WriteLine("Error creating table: " + ex.Message);
        }
        finally
        {
            sqlite.Close();
        }
    }

    public void ClearStockTables() {
        try
        {
            sqlite.Open();  // Open connection to the DB
            using (SqliteCommand createTable = sqlite.CreateCommand())
            {
                createTable.CommandText = @"
                    DELETE FROM HISTORICAL_STOCK_DATA;
                ";
                createTable.ExecuteNonQuery();
            }
        }
        catch (SqliteException ex)
        {
            Console.WriteLine("Error creating table: " + ex.Message);
        }
        finally
        {
            sqlite.Close();
        }
    }

    public void AlterNewsTable() {
        try
        {
            sqlite.Open();  // Open connection to the DB
            using (SqliteCommand createTable = sqlite.CreateCommand())
            {
                createTable.CommandText = @"
                    DROP TABLE BASIC_STOCK_DATA;
                ";
                createTable.ExecuteNonQuery();
            }
        }
        catch (SqliteException ex)
        {
            Console.WriteLine("Error altering table: " + ex.Message);
        }
        finally
        {
            sqlite.Close();
        }
    }

    public void CreateSectorsTable() {
        try
        {
            sqlite.Open();  // Open connection to the DB
            using (SqliteCommand createTable = sqlite.CreateCommand())
            {
                createTable.CommandText = @"
                    CREATE TABLE IF NOT EXISTS SECTOR_STOCKS (
                        SECTOR TEXT,
                        TICKER TEXT,
                        MARKET_CAP REAL,
                        PRIMARY KEY (SECTOR, TICKER)
                    );
                ";
                createTable.ExecuteNonQuery();
            }
        }
        catch (SqliteException ex)
        {
            Console.WriteLine("Error creating table: " + ex.Message);
        }
        finally
        {
            sqlite.Close();
        }
    }

    public void CreateStocksDataTable() {
        try
        {
            sqlite.Open();  // Open connection to the DB
            using (SqliteCommand createTable = sqlite.CreateCommand())
            {
                //"Technology",
//         "Financial Services",
//         "Consumer Cyclical",
//         "Healthcare",
//         "Communication Services",
//         "Industrials",
//         "Consumer Defensive",
//         "Energy",
//         "Basic Materials",
//         "Real Estate",
//         "Utilities"
                createTable.CommandText = @"
                    CREATE TABLE IF NOT EXISTS BASIC_STOCK_DATA (
                        SECTOR TEXT,
                        TICKER TEXT,
                        DATE DATE,
                        OPEN REAL,
                        HIGH REAL,
                        LOW REAL,
                        CLOSE REAL,
                        ADJUSTED_CLOSE REAL,
                        VOLUME INTEGER,
                        PRIMARY KEY (SECTOR, TICKER, DATE)
                    );
                ";
                createTable.ExecuteNonQuery();
            }
        }
        catch (SqliteException ex)
        {
            Console.WriteLine("Error creating table: " + ex.Message);
        }
        finally
        {
            sqlite.Close();
        }
    }

    public void InsertSectorsTable() {
        try
        {
            sqlite.Open();  // Open connection to the DB
            using (SqliteCommand insertCommand = sqlite.CreateCommand())
            {
                insertCommand.CommandText = @"
                    INSERT OR IGNORE INTO SECTOR_STOCKS (SECTOR, TICKER, MARKET_CAP) 
                    VALUES (@Sector, @Ticker, @MarketCap);
                ";

                foreach (var sector in sectorNames)
                {
                    Dictionary<string, double> sectorStocks = GetSectorStocks(sector);

                    foreach (var stock in sectorStocks)
                    {
                        insertCommand.Parameters.Clear();
                        insertCommand.Parameters.AddWithValue("@Sector", sector);
                        insertCommand.Parameters.AddWithValue("@Ticker", stock.Key);
                        insertCommand.Parameters.AddWithValue("@MarketCap", stock.Value);
                        insertCommand.ExecuteNonQuery();
                    }
                }
            }
        }
        catch (SqliteException ex)
        {
            Console.WriteLine("Error inserting sector stocks: " + ex.Message);
        }
        finally
        {
            sqlite.Close();
        }
    }

    private Dictionary<string, double> GetSectorStocks(string sector) {
        return sector switch
        {
            "Technology" => techSectorStocks,
            "Financial Services" => financeSectorStocks,
            "Consumer Cyclical" => cyclicalSectorStocks,
            "Healthcare" => healthCareSectorStocks,
            "Communication Services" => communicationSectorStocks,
            "Industrials" => industrialsSectorStocks,
            "Consumer Defensive" => defensiveSectorStocks,
            "Energy" => energySectorStocks,
            "Basic Materials" => basicMaterialsSectorStocks,
            "Real Estate" => realEstateSectorStocks,
            "Utilities" => utilitiesSectorStocks,
            _ => new Dictionary<string, double>()
        };
    }

    
}
