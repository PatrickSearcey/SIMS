using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Safety.Models
{
    public class JHAModel
    {
        private int? _sha_site_id;
        private int? _site_jha_id;
        private int? _elem_jha_id;
        private int? _jha_id;
        private string _jha_description;

        public int? sha_site_id
        {
            get { return _sha_site_id; }
            set { _sha_site_id = value; }
        }
        public int? site_jha_id
        {
            get { return _site_jha_id; }
            set { _site_jha_id = value; }
        }
        public int? elem_jha_id
        {
            get { return _elem_jha_id; }
            set { _elem_jha_id = value; }
        }
        public int? jha_id
        {
            get { return _jha_id; }
            set { _jha_id = value; }
        }
        public string jha_description
        {
            get { return _jha_description; }
            set { _jha_description = value; }
        }
        public JHAModel()
        {
            _sha_site_id = sha_site_id;
            _site_jha_id = site_jha_id;
            _elem_jha_id = elem_jha_id;
            _jha_id = jha_id;
            _jha_description = jha_description;
        }
    }
}