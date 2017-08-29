using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Safety.Models
{
    public class RefLevelModel
    {
        private int? _site_reflevel_id;
        private int? _site_jha_id;
        private int? _reflevel_id;
        private string _reflevel_tp;
        private double _reflevel_va;
        private string _reflevel_units;
        private string _remarks;
        private string _reflevel_tp_desc;
        private string _reflevel_desc;

        public int? site_reflevel_id
        {
            get { return _site_reflevel_id; }
            set { _site_reflevel_id = value; }
        }
        public int? site_jha_id
        {
            get { return _site_jha_id; }
            set { _site_jha_id = value; }
        }
        public int? reflevel_id
        {
            get { return _reflevel_id; }
            set { _reflevel_id = value; }
        }
        public string reflevel_tp
        {
            get { return _reflevel_tp; }
            set { _reflevel_tp = value; }
        }
        public double reflevel_va
        {
            get { return _reflevel_va; }
            set { _reflevel_va = value; }
        }
        public string reflevel_units
        {
            get { return _reflevel_units; }
            set { _reflevel_units = value; }
        }
        public string remarks
        {
            get { return _remarks; }
            set { _remarks = value; }
        }
        public string reflevel_tp_desc
        {
            get { return _reflevel_tp_desc; }
            set { _reflevel_tp_desc = value; }
        }
        public string reflevel_desc
        {
            get { return _reflevel_desc; }
            set { _reflevel_desc = value; }
        }
        public RefLevelModel()
        {
            _site_reflevel_id = site_reflevel_id;
            _site_jha_id = site_jha_id;
            _reflevel_id = reflevel_id;
            _reflevel_tp = reflevel_tp;
            _reflevel_va = reflevel_va;
            _reflevel_units = reflevel_units;
            _remarks = remarks;
            _reflevel_tp_desc = reflevel_tp_desc;
            _reflevel_desc = reflevel_desc;
        }

    }
}