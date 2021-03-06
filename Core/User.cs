﻿using System;
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
        private String _id, _firstName, _lastName, _email, _primaryOU;
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
                
                _active = (bool)user.active;
                _showreports = (bool)user.show_reports;
                _email = _id + "@usgs.gov";
                //If possible, get the email address for the user from AD
                var reg_user = db.spz_GetUserInfoFromAD(_id).ToList();
                foreach (var result in reg_user)
                {
                    _email = result.mail;
                    _primaryOU = result.primaryOU;
                }

                //Add WSCs for which the user has exceptions
                foreach (int wsc in user.Exceptions.Select(p => p.exc_wsc_id).ToList())
                {
                    _wsc_id.Add(wsc);
                }
                //Add WSCs for which the WSC of the user has exceptions
                foreach (int wsc in db.ExceptionWSCs.Where(p => p.AD_OU == _primaryOU).Select(p => p.exc_wsc_id).ToList())
                {
                    _wsc_id.Add(wsc);
                }
            }
            else
            {
                try
                {
                    var unreg_user = db.spz_GetUserInfoFromAD(_id).ToList();
                    string primaryOU = "AustinTX-W";
                    string email = _id + "@usgs.gov";
                    foreach (var result in unreg_user)
                    {
                        primaryOU = result.primaryOU;
                        email = result.mail;
                    }
                    var wsc = db.WSCs.FirstOrDefault(p => p.AD_OU.Contains(primaryOU));
                    var office = db.Offices.FirstOrDefault(p => p.wsc_id == wsc.wsc_id);

                    _office_id = office.office_id;
                    _wsc_id.Add(wsc.wsc_id);
                    _email = email;
                    _active = false;
                    _showreports = false;
                    //Add WSCs for which the WSC of the user has exceptions
                    foreach (int w in db.ExceptionWSCs.Where(p => p.AD_OU == primaryOU).Select(p => p.exc_wsc_id).ToList())
                    {
                        _wsc_id.Add(w);
                    }
                }
                catch (Exception ex)
                {
                    _office_id = 348;
                    _wsc_id.Add(31);
                    _email = "GS-W_Help_SIMS@usgs.gov";
                    _active = false;
                    _showreports = false;
                }
                
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
        public String Email { get { return _email;  } }
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
