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
    }
}
