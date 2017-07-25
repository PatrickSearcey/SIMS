using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class Config
    {
#if DEBUG
        public static String SIMSServerURL { get { return "http://simsdev.cr.usgs.gov/" ; } }
#else
        public static String SIMSServerURL { get { return "http://simswater.usgs.gov/" ; } }
#endif

#if DEBUG
        public static String SIMSClassicURL { get { return "http://simsdev.cr.usgs.gov/SIMSClassic/" ; } }
#else
        public static String SIMSClassicURL { get { return "http://sims.water.usgs.gov/SIMSClassic/" ; } }
#endif

#if DEBUG
        public static String SIMSURL { get { return "http://simsdev.cr.usgs.gov/SIMS/"; } }
#else
        public static String SIMSURL { get { return "http://sims.water.usgs.gov/SIMS/" ; } }
#endif

#if DEBUG
        public static String SIMS2017URL { get { return "http://simsdev.cr.usgs.gov/SIMS2017/"; } }
#else
        public static String SIMS2017URL { get { return "http://sims.water.usgs.gov/SIMS2017/" ; } }
#endif

#if DEBUG
        public static String SLAPURL { get { return "http://simsdev.cr.usgs.gov/SLAP/"; } }
#else
        public static String SLAPURL { get { return "http://sims.water.usgs.gov/SLAP/" ; } }
#endif

#if DEBUG
        public static String PASSURL { get { return "http://simsdev.cr.usgs.gov/PASS/"; } }
#else
        public static String PASSURL { get { return "http://sims.water.usgs.gov/PASS/" ; } }
#endif

#if DEBUG
        public static String SafetyURL { get { return "http://simsdev.cr.usgs.gov/Safety/"; } }
#else
        public static String SafetyURL { get { return "http://sims.water.usgs.gov/Safety/"; } } 
#endif

#if DEBUG
        public static String RMSURL { get { return "http://simsdev.cr.usgs.gov/RMS/"; } }
#else
        public static String RMSURL { get { return "http://sims.water.usgs.gov/RMS/"; } } 
#endif

#if DEBUG
        public static String ConnectionInfo { get { return "Data Source=IGSKIACWVMGS012;Initial Catalog=simsdb;Integrated Security=True"; } }
#else
        public static String ConnectionInfo { get { return "Data Source=IGSKIACWVMGS011;Initial Catalog=simsdb;Integrated Security=True"; } }
#endif

        protected static List<int> SLAPWSC()
        {
            List<int> ret = new List<int>();
            ret.Add(1);
            ret.Add(3);
            ret.Add(4);
            ret.Add(5);
            ret.Add(9);
            ret.Add(11);
            ret.Add(12);
            ret.Add(13);
            ret.Add(14);
            ret.Add(15);
            ret.Add(18);
            ret.Add(22);
            ret.Add(25);
            ret.Add(33);
            ret.Add(35);
            ret.Add(31);
            
            return ret;
        }

        public static List<int> IsSLAPWSC
        {
            get
            {
                return SLAPWSC();
            }
        }

        public static int DischargeMeasElem
        {
            get { return 13; }
        }

        public static int LakeMeasElem
        {
            get { return 1002; }
        }

        public static int EcoMeasElem
        {
            get { return 1003; }
        }

        public static int AtmMeasElem
        {
            get { return 1004; }
        }

        public static int GWMeasElem
        {
            get { return 57; }
        }

        public static int QWMeasElem
        {
            get { return 124; }
        }

        public static int SiteHazardElem
        {
            get { return 45; }
        }


    }
}
