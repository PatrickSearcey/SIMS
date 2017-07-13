using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Core
{
    public class DataListTemplate : ITemplate
    {
        private Data.SIMSDataContext db = new Data.SIMSDataContext();

        ListItemType _itemType;
        private string _no;
        private string _view;
        private string _recordType;
        private string _wy;

        private int _count;
        public string NoOfPeriods
        {
            get { return _no; }
            set { _no = value; }
        }

        public string WY
        {
            get { return _wy; }
            set { _wy = value; }
        }

        public string RecordType
        {
            get { return _recordType; }
            set { _recordType = value; }
        }

        public int Count
        {
            get { return _count; }
            set { _count = value; }
        }

        public string WhichView
        {
            get { return _view; }
            set { _view = value; }
        }

        public DataListTemplate(ListItemType Type)
        {
            _itemType = Type;
        }

        public void InstantiateIn(System.Web.UI.Control container)
        {
            Literal lc = new Literal();

            switch (_itemType)
            {
                case ListItemType.Header:

                    lc.Text = "<fieldset style=\"width:340px;\"><legend>" + RecordType + "</legend><div style=\"padding:5px;\">" + 
                        "<table cellpadding=\"3\" style=\"border: 1px solid #863d02;background-color:white;\">" + 
                        "  <tr>" + 
                        "    <td style=\"background-color:#863d02;color:white;text-align:center;\">Period ID</td>" + 
                        "    <td style=\"background-color:#863d02;color:white;text-align:center;\">Begin Date</td>" + 
                        "    <td style=\"background-color:#863d02;color:white;text-align:center;\">End Date</td>" + 
                        "    <td style=\"background-color:#863d02;color:white;text-align:center;\">Status</td>" + 
                        "  </tr>";

                    Count = 1;

                    container.Controls.Add(lc);
                    break;
                case ListItemType.Item:

                    Count += 1;

                    lc.DataBinding += TemplateControl_DataBinding;

                    container.Controls.Add(lc);
                    break;
                case ListItemType.Footer:

                    if (WhichView == "dates")
                    {
                        lc.Text = "</table>" + 
                            "<div style=\"text-align:right;padding-top:10px;\">showing the most recent record period(s)</div></div></fieldset><br /><br />";
                    }
                    else
                    {
                        lc.Text = "</table>" + 
                            "<div style=\"text-align:right;padding-top:10px;\">showing record period(s) for the " + WY + " WY</div></div></fieldset><br /><br />";
                    }

                    container.Controls.Add(lc);
                    break;
            }

        }

        private void TemplateControl_DataBinding(object sender, System.EventArgs e)
        {
            Literal lc = default(Literal);

            lc = (Literal)sender;

            DataListItem container = (DataListItem)lc.NamingContainer;

            string status_va = DataBinder.Eval(container.DataItem, "status_va").ToString();
            int period_id = Convert.ToInt32(DataBinder.Eval(container.DataItem, "period_id"));
            string lock_img = null;
            bool locked = false;

            var period = db.RecordAnalysisPeriods.FirstOrDefault(p => p.period_id == period_id);

            if (period.Record.RecordLock != null) locked = true;

            if (locked)
            {
                lock_img = " &nbsp;<img src=\"../images/lock.png\" alt=\"LOCKED\" />";
            }
            else
            {
                lock_img = "";
            }

            if (WhichView == "dates")
            {
                if (!locked)
                {
                    if (Count == Convert.ToInt32(NoOfPeriods))
                    {
                        lc.Text += "<tr><td>" + period_id + "</td><td><a href=\"PeriodDates.aspx?period_id=" + period_id + "&dt=beg2\">" + String.Format("{0:MM/dd/yyyy}", DataBinder.Eval(container.DataItem, "period_beg_dt")) + "</a>" + "</td><td><a href=\"PeriodDates.aspx?period_id=" + period_id + "&dt=end2\">" + String.Format("{0:MM/dd/yyyy}", DataBinder.Eval(container.DataItem, "period_end_dt")) + "</a></td><td>" + status_va + "</td></tr>";
                    }
                    else
                    {
                        lc.Text += "<tr><td>" + period_id + "</td><td>" + String.Format("{0:MM/dd/yyyy}", DataBinder.Eval(container.DataItem, "period_beg_dt")) + "</td><td><a href=\"PeriodDates.aspx?period_id=" + period_id + "&dt=end1\">" + String.Format("{0:MM/dd/yyyy}", DataBinder.Eval(container.DataItem, "period_end_dt")) + "</a></td><td>" + status_va + "</td></tr>";
                    }
                }
                else
                {
                    lc.Text += "<tr><td style=\"color:#CCCCCC;\">" + period_id + "</td><td style=\"color:#CCCCCC;\">" + String.Format("{0:MM/dd/yyyy}", DataBinder.Eval(container.DataItem, "period_beg_dt")) + "</td><td style=\"color:#CCCCCC;\">" + String.Format("{0:MM/dd/yyyy}", DataBinder.Eval(container.DataItem, "period_end_dt")) + "</td><td style=\"color:#CCCCCC;\">" + status_va + lock_img + "</td></tr>";
                }
            }
            else
            {
                if (Count == Convert.ToInt32(NoOfPeriods))
                {
                    if (!(status_va == "Analyzing") & !(status_va == "Approving") & !(status_va == "Reanalyze") & !locked)
                    {
                        lc.Text += "<tr><td>" + period_id + "</td><td>" + String.Format("{0:MM/dd/yyyy}", DataBinder.Eval(container.DataItem, "period_beg_dt")) + "</td><td>" + String.Format("{0:MM/dd/yyyy}", DataBinder.Eval(container.DataItem, "period_end_dt")) + "</td><td><a href=\"PeriodStatus.aspx?period_id=" + DataBinder.Eval(container.DataItem, "period_id") + "\">" + status_va + "</a></td></tr>";
                    }
                    else
                    {
                        lc.Text += "<tr><td style=\"color:#CCCCCC;\">" + period_id + "</td><td style=\"color:#CCCCCC;\">" + String.Format("{0:MM/dd/yyyy}", DataBinder.Eval(container.DataItem, "period_beg_dt")) + "</td><td style=\"color:#CCCCCC;\">" + String.Format("{0:MM/dd/yyyy}", DataBinder.Eval(container.DataItem, "period_end_dt")) + "</td><td style=\"color:#CCCCCC;\">" + status_va + lock_img + "</td></tr>";
                    }
                }
                else
                {
                    lc.Text += "<tr><td style=\"color:#CCCCCC;\">" + period_id + "</td><td style=\"color:#CCCCCC;\">" + String.Format("{0:MM/dd/yyyy}", DataBinder.Eval(container.DataItem, "period_beg_dt")) + "</td><td style=\"color:#CCCCCC;\">" + String.Format("{0:MM/dd/yyyy}", DataBinder.Eval(container.DataItem, "period_end_dt")) + "</td><td style=\"color:#CCCCCC;\">" + status_va + lock_img + "</td></tr>";
                }
            }




        }

    }
}
