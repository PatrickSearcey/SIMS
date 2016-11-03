using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Core
{
    /// <summary>
    /// A Class for holding User Information, like permissions, user name etc.
    /// </summary>
    public class WindowsAuthenticationUser
    {
        #region Local Variables
        private bool _superUser = false;
        private bool _admin = false;
        private bool _safetyapprover = false;
        private bool _passadmin = false;
        private bool _passsuperuser = false;
        private bool _showreports = false;
        private bool _active = false;
        private int _office_id;
        private List<int> _wsc_id = new List<int>();
        private String _id, _firstName, _lastName;
        #endregion

        #region Constructors
        public WindowsAuthenticationUser()
        {
            //DB Connection
            Data.SIMSDataContext db = new Data.SIMSDataContext();
            //Grab the User ID from windows authentication
            _id = HttpContext.Current.User.Identity.Name.Replace("-pr", "");
            int pos = _id.IndexOf("\\");
            _id = _id.Substring(pos + 1);
#if DEBUG
            _id = "dterry";
#endif
            //try to see if the user is in the database
            var user = db.Employees.FirstOrDefault(p => p.user_id == _id);
            //If the user isn't null it has Admin priveldges, or doesn't
            if (user != null)
            {
                if (user.administrator_va == "SuperUser") _superUser = true;
                if (user.administrator_va == "WSC") _admin = true;
                if ((bool)user.approver_va) _safetyapprover = true;
                if (user.pass_access == "WSC") _passadmin = true;
                if (user.pass_access == "SuperUser") _passsuperuser = true;

                _firstName = user.first_nm;
                _lastName = user.last_nm;
                _office_id = (int)user.office_id;
                _wsc_id.Add((int)user.Office.wsc_id);
                foreach (int wsc in user.Exceptions.Select(p => p.exc_wsc_id).ToList())
                {
                    _wsc_id.Add(wsc);
                }
                _active = (bool)user.active;
                _showreports = (bool)user.show_reports;
            }
        }

        /// <summary>
        /// Used for Testing Purposes
        /// </summary>
        /// <param name="SuperUser"></param>
        public WindowsAuthenticationUser(Boolean superUser, Boolean admin)
        {
            _superUser = superUser;
            _admin = admin;
        }
        #endregion

        #region Methods
        
        #endregion

        #region Properties
        public Boolean IsSuperUser { get { return _superUser; } }
        public Boolean IsAdmin { get { return _admin || _superUser; } }
        public String ID { get { return _id; } }
        public String FirstName { get { return _firstName; } }
        public String LastName { get { return _lastName; } }
        public int OfficeID { get { return _office_id; } }
        public List<int> WSCID { get { return _wsc_id; } }
        public Boolean Active { get { return _active; } }
        public Boolean ShowReports { get { return _showreports; } }
        public Boolean IsSafetyApprover { get { return _safetyapprover; } }
        public Boolean IsPASSSuperUser { get { return _passsuperuser; } }
        public Boolean IsPASSAdmin { get { return _passadmin || _passsuperuser; } }
        #endregion
    }
}
