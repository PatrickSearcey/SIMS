<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/SIMSSiteTopLevel.Master" CodeBehind="Default.aspx.vb" Inherits="SIMS._Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">

    <div style="width:100%;padding-bottom:20px;">
        <img src="images/HomepageTitle.png" alt="Site Information Management System" />
    </div>

    <div style="width:100%;">
        <div style="width:260px;float:left;padding-top:15px;">
            <div class="roundedSidePanel" style="text-align:center;">
                <h3>Latest News</h3>
                <strong>&raquo; <a href="https://collaboration.usgs.gov/wg/owi/specialprojects/SIMS/Shared%20Documents/updates.html" style="color:#cb0000;">UPDATED 6/27/2016</a> &laquo;</strong>
            </div>
            <div class="roundedSidePanel">
                <div class="sidePanelHeadings">
                    <h3>Quick Jump</h3>
                </div>
                <p style="font-weight:bold;font-size:small;margin-bottom:3px;">By WSC:&nbsp;&nbsp;
                <asp:DropDownList ID="ddlWSC" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlWSC_SelectedIndexChanged" DataTextField="wsc_nm" DataValueField="wsc_id" /></p> 
                <hr />
                <p style="font-weight:bold;font-size:small;margin-top:3px;">Jump to a Station Info Page:<br />
                Site No: <asp:TextBox ID="tbSiteNo" runat="server" Width="120px" /> <br />
                Agency Code: <asp:TextBox ID="tbAgencyCd" runat="server" Width="50px" Text="USGS" /> 
                <asp:Button ID="btnGo" runat="server" OnCommand="btnGo_OnCommand" Text="Go!" CommandArgument="go" /></p>
            </div>
            <div class="roundedSidePanel">
                <div class="sidePanelHeadings">
                    <h3>National Reports</h3>
                </div>
                <ul>
                    <li><a href="/SIMSReports/CRP/" target="_blank">CRP Charts</a></li>
                    <li>Cableways:<br />
                        &nbsp;&nbsp;<a href="CablewayReport.aspx?tp=nw">Information</a><br />
                        &nbsp;&nbsp;<a href="CablewayReport.aspx?tp=status">Inspection Status</a><br />
                        &nbsp;&nbsp;<a href="/SIMSReports/Cableways/" target="_blank">Other Reports</a></li>
                    <li><a href="EvalMaps.aspx">Evaluation Maps</a></li>
                    <li><a href="SHAReport.aspx">Site Hazard Analyses</a></li>
                    <li><a href="/SIMSReports/WYS/wys_details.html" target="_blank">Water Year Summary Details</a></li>
                </ul>
                <div class="sidePanelHeadings">
                    <h3>SIMS Compliant Applications</h3>
                </div>
                <ul>
                    <li><a href="/PASS/" target="_blank">PASS Home</a></li>
                    <li><a href="/SLAP/" target="_blank">SLAP Home</a></li>
                    <li><a href="http://waterdata.usgs.gov/nwisweb/get_decodes" target="_blank">DECODES Repository</a></li>
                </ul>
            </div>
        </div>
        <div style="width:700px;float:right;">
            <div style="width:100%;padding-top:0px;margin-top:0px;font-size:11pt;padding-bottom:10px;">
                <p>SIMS (Site Information Management System) serves as a framework for information used by audiences ranging from hydrologists, hydrographers, managers and the bureau safety community.  The core SIMS 
                application manages and supplies station descriptions, manuscripts, station analyses, DCP information and E-mail communication. SIMS compliant modules include Platform Assignment and Scheduling System 
                (PASS), continuous records processing tools (RMS and CRP), Site Hazard Analyses (SHA), Traffic Control Plans, and current information about cableways. All of this is available due to integration with 
                other systems such as NWIS, NWISWeb, and the Annual Datra Report (ADR). On the horizon are integration with other non-SIMS applications such Site Levels Archiving and Processing (SLAP), cooperation and 
                site funding applications, equipment inventories and more robust records management tools.</p>
            </div>
            <div class="roundedMainPanel">
                <h3>Select state to obtain access to your local SIMS page.</h3>
                <div style="text-align:center;">
                    <img src="images/usmap.gif" alt="Click on a state to go to that WSC's SIMS page." usemap="#Map1" />
                    <map name="Map1" id="Map1">
                      <area shape="poly" coords="32,12,36,29,53,36,92,30,90,7,47,7,46,16,35,13" href="/SIMSClassic/StationsRpts.asp?wsc_id=33&fm=y" alt="WA" />
                      <area shape="poly" coords="36,35,33,65,91,67,88,46,92,34,60,40" href="/SIMSClassic/StationsRpts.asp?wsc_id=28&fm=y" alt="OR" />
                      <area shape="poly" coords="95,8,96,37,94,65,140,66,139,45,118,44,109,34,104,21" href="/SIMSClassic/StationsRpts.asp?wsc_id=15&fm=y" alt="ID" />
                      <area shape="poly" coords="102,7,113,24,126,40,144,39,198,39,199,5" href="/SIMSClassic/StationsRpts.asp?wsc_id=8&fm=y" alt="MT" />
                      <area shape="poly" coords="201,6,203,31,260,32,256,17,254,5" href="/SIMSClassic/StationsRpts.asp?wsc_id=37&fm=y" alt="ND" />
                      <area shape="poly" coords="144,42,142,76,198,76,197,41" href="/SIMSClassic/StationsRpts.asp?wsc_id=8&fm=y" alt="WY" />
                      <area shape="poly" coords="37,72,66,70,68,96,113,135,111,153,93,151,65,136,46,105,34,87" href="/SIMSClassic/StationsRpts.asp?wsc_id=12&fm=y" alt="CA" />
                      <area shape="poly" coords="180,159,206,158,209,118,233,118,233,137,281,145,280,156,282,176,265,189,253,200,256,212,242,206,225,183,217,179,206,185,194,174" href="/SIMSClassic/StationsRpts.asp?wsc_id=31&fm=y" alt="TX" />
                      <area shape="poly" coords="71,72,115,69,115,117,111,126,72,95" href="/SIMSClassic/StationsRpts.asp?wsc_id=22&fm=y" alt="NV" />
                      <area shape="poly" coords="120,71,138,71,138,80,156,80,156,111,119,111" href="/SIMSClassic/StationsRpts.asp?wsc_id=32&fm=y" alt="UT" />
                      <area shape="poly" coords="160,79,160,112,214,111,214,78" href="/SIMSClassic/StationsRpts.asp?wsc_id=13&fm=y" alt="CO" />
                      <area shape="poly" coords="119,116,156,116,157,163,143,164,116,155" href="/SIMSClassic/StationsRpts.asp?wsc_id=11&fm=y" alt="AZ" />
                      <area shape="poly" coords="160,116,207,117,205,156,176,158,160,162" href="/SIMSClassic/StationsRpts.asp?wsc_id=24&fm=y" alt="NM" />
                      <area shape="poly" coords="202,34,201,57,260,61,257,36" href="/SIMSClassic/StationsRpts.asp?wsc_id=37&fm=y" alt="SD" />
                      <area shape="poly" coords="202,61,201,77,219,76,219,85,268,85,257,63" href="/SIMSClassic/StationsRpts.asp?wsc_id=21&fm=y" alt="NE" />
                      <area shape="poly" coords="104,180,105,221,140,246,134,253,99,229,76,223,53,241,31,255,43,237,24,228,18,212,32,204,9,198,31,191,19,181,39,169,70,171" href="/SIMSClassic/StationsRpts.asp?wsc_id=10&fm=y" alt="AK" />
                      <area shape="poly" coords="218,88,217,112,276,112,271,89" href="/SIMSClassic/StationsRpts.asp?wsc_id=18&fm=y" alt="KS" />
                      <area shape="poly" coords="237,116,237,136,279,141,276,116" href="/SIMSClassic/StationsRpts.asp?wsc_id=27&fm=y" alt="OK" />
                      <area shape="poly" coords="259,7,263,35,264,53,304,54,294,43,292,32,298,23,310,14,280,8" href="/SIMSClassic/StationsRpts.asp?wsc_id=19&fm=y" alt="MN" />
                      <area shape="poly" coords="300,28,296,37,310,58,315,62,330,62,333,42,327,32,312,28" href="/SIMSClassic/StationsRpts.asp?wsc_id=34&fm=y" alt="WI" />
                      <area shape="poly" coords="264,57,270,80,302,79,310,66,301,55" href="/SIMSClassic/StationsRpts.asp?wsc_id=17&fm=y" alt="IA" />
                      <area shape="poly" coords="273,83,303,83,312,98,320,115,314,119,280,117,279,99" href="/SIMSClassic/StationsRpts.asp?wsc_id=20&fm=y" alt="MO" />
                      <area shape="poly" coords="315,66,331,65,334,73,333,99,324,112,306,83" href="/SIMSClassic/StationsRpts.asp?wsc_id=16&fm=y" alt="IL" />
                      <area shape="poly" coords="345,70,369,69,375,60,374,50,363,52,371,41,361,35,345,27,326,23,324,27,338,36,342,50" href="/SIMSClassic/StationsRpts.asp?wsc_id=5&fm=y" alt="MI" />
                      <area shape="poly" coords="338,73,356,73,358,95,349,103,335,106,339,96" href="/SIMSClassic/StationsRpts.asp?wsc_id=4&fm=y" alt="IN" />
                      <area shape="poly" coords="361,73,377,76,393,70,390,89,377,98,367,97,361,92" href="/SIMSClassic/StationsRpts.asp?wsc_id=5&fm=y" alt="OH" />
                      <area shape="poly" coords="281,121,314,122,305,142,305,148,283,148" href="/SIMSClassic/StationsRpts.asp?wsc_id=1&fm=y" alt="AR" />
                      <area shape="poly" coords="284,151,303,151,305,157,301,166,316,167,318,175,313,183,302,180,286,177" href="/SIMSClassic/StationsRpts.asp?wsc_id=1&fm=y" alt="LA" />
                      <area shape="poly" coords="314,134,331,134,327,158,328,172,323,175,319,166,305,164,308,149" href="/SIMSClassic/StationsRpts.asp?wsc_id=1&fm=y" alt="MS" />
                      <area shape="poly" coords="326,117,368,116,379,109,372,101,362,97,352,107,335,109" href="/SIMSClassic/StationsRpts.asp?wsc_id=4&fm=y" alt="KY" />
                      <area shape="poly" coords="315,130,322,120,380,120,365,130" href="/SIMSClassic/StationsRpts.asp?wsc_id=1&fm=y" alt="TN" />
                      <area shape="poly" coords="335,134,350,133,356,165,337,167,337,172,332,172,331,160" href="/SIMSClassic/StationsRpts.asp?wsc_id=1&fm=y" alt="AL" />
                      <area shape="poly" coords="355,134,369,133,388,158,384,168,373,170,359,168" href="/SIMSClassic/StationsRpts.asp?wsc_id=3&fm=y" alt="GA" />
                      <area shape="poly" coords="342,170,363,172,385,173,397,205,394,219,387,218,379,199,376,186,367,178,356,181" href="/SIMSClassic/StationsRpts.asp?wsc_id=2&fm=y" alt="FL" />
                      <area shape="poly" coords="373,132,389,131,392,136,403,136,409,142,391,156" href="/SIMSClassic/StationsRpts.asp?wsc_id=3&fm=y" alt="SC" />
                      <area shape="poly" coords="373,129,385,120,429,119,426,129,417,136,412,140,402,133,389,129" href="/SIMSClassic/StationsRpts.asp?wsc_id=3&fm=y" alt="NC" />
                      <area shape="poly" coords="376,116,432,117,426,109,418,100,413,94,406,103,396,111,384,112" href="/SIMSClassic/StationsRpts.asp?wsc_id=9&fm=y" alt="VA" />
                      <area shape="poly" coords="420,91,433,92,452,100,455,110,446,122,437,123,434,107,425,103" href="/SIMSClassic/StationsRpts.asp?wsc_id=6&fm=y" alt="MD-DEL-DC" />
                      <area shape="poly" coords="378,102,383,109,394,107,401,101,411,94,400,91,392,93,384,97" href="/SIMSClassic/StationsRpts.asp?wsc_id=9&fm=y" alt="WV" />
                      <area shape="poly" coords="396,70,395,88,434,87,435,78,433,70" href="/SIMSClassic/StationsRpts.asp?wsc_id=29&fm=y" alt="PA" />
                      <area shape="poly" coords="442,76,452,86,459,93,454,99,443,92,439,86" href="/SIMSClassic/StationsRpts.asp?wsc_id=23&fm=y" alt="NJ" />
                      <area shape="poly" coords="405,66,436,66,448,73,449,44,440,43,433,49,432,56,409,57" href="/SIMSClassic/StationsRpts.asp?wsc_id=25&fm=y" alt="NY" />
                      <area shape="poly" coords="452,71,463,72,472,86,465,92,455,86,451,79" href="/SIMSClassic/StationsRpts.asp?wsc_id=7&fm=y" alt="CT" />
                      <area shape="poly" coords="454,62,454,68,467,70,476,84,485,73,490,58,478,59,465,63" href="/SIMSClassic/StationsRpts.asp?wsc_id=7&fm=y" alt="MA-RI" />
                      <area shape="poly" coords="452,41,471,41,471,59,453,59" href="/SIMSClassic/StationsRpts.asp?wsc_id=7&fm=y" alt="NH-VT" />
                      <area shape="poly" coords="488,19,496,22,498,35,501,44,489,48,475,55,474,38,482,26" href="/SIMSClassic/StationsRpts.asp?wsc_id=7&fm=y" alt="ME" />
                      <area shape="poly" coords="169,219,177,222,185,229,188,243,202,247,224,240,206,225,189,217,172,213" href="/SIMSClassic/StationsRpts.asp?wsc_id=14&fm=y" alt="HI" />
                      <area shape="poly" coords="303,231,307,246,316,246,330,243,338,235,323,231" href="/SIMSClassic/StationsRpts.asp?wsc_id=2&fm=y" alt="PR" />
                    </map>
                </div>
            </div>
        </div>
    </div>

</asp:Content>
