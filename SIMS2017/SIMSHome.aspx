<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SIMSHome.aspx.cs" Inherits="SIMS2017.SIMSHome" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Site Information Management System</title>
    <meta name="viewport" content="initial-scale=1.0, minimum-scale=1, maximum-scale=1.0, user-scalable=no" />
    <link href="styles/base.css" rel="stylesheet" />
    <link href="styles/default.css" rel="stylesheet" />
    <link href="styles/home.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
    <telerik:RadScriptManager runat="server"></telerik:RadScriptManager>
    <telerik:RadFormDecorator RenderMode="Lightweight" runat="server" DecoratedControls="All" Skin="Bootstrap" />
    <div id="wrapper">
        <telerik:RadPageLayout runat="server" ID="HomeLayout" GridType="Fluid">
            <Rows>
                <%--Header--%>
                <telerik:LayoutRow CssClass="header">
                    <Columns>
                        <%--Logo--%>
                        <telerik:LayoutColumn Span="2" SpanMd="3" SpanSm="12" SpanXs="12">
                            <a href="#" class="logo">
                                <img src="images/USGSHeaderLogo.png" alt="USGS"/>
                            </a>
                        </telerik:LayoutColumn>

                        <%--Main Nav--%>
                        <telerik:LayoutColumn Span="10" SpanMd="9" SpanSm="12" SpanXs="12">
                            <telerik:RadMenu ID="rmTop" runat="server" RenderMode="Auto" Skin="Bootstrap">
                            </telerik:RadMenu>
                        </telerik:LayoutColumn>
                    </Columns>
                </telerik:LayoutRow>

                <telerik:LayoutRow>
                    <Columns>
                        <telerik:LayoutColumn Span="12" SpanMd="12" SpanSm="12" SpanXs="12" CssClass="subheader">
                            internal only | logged in as: <asp:Literal ID="ltlUserID" runat="server" />
                        </telerik:LayoutColumn>
                    </Columns>
                </telerik:LayoutRow>

                <%--Main--%>
                <telerik:LayoutRow>
                    <Columns>

                        <%--Content--%>
                        <telerik:CompositeLayoutColumn Span="12" SpanMd="12" SpanSm="12" SpanXs="12">
                            <Content>
                                <telerik:RadPageLayout runat="server" ID="rplTop">
                                    <Rows>
                                        <telerik:LayoutRow>
                                            <Columns>
                                                <telerik:LayoutColumn CssClass="jumbotron">
                                                    <img src="images/SIMSTitle.png" alt="SIMS" />
                                                </telerik:LayoutColumn>
                                            </Columns>
                                        </telerik:LayoutRow>
                                    </Rows>
                                </telerik:RadPageLayout>
                                <telerik:RadPageLayout runat="server" ID="rplContent">
                                    <Rows>
                                        <telerik:LayoutRow>
                                            <Columns>
                                                <telerik:LayoutColumn Span="12" SpanMd="12" SpanSm="12" HiddenXs="true">
                                                    <div class="roundedTopSubPanel">
                                                        <h3>Latest News</h3>
                                                        <strong>&raquo; <a href="https://collaboration.usgs.gov/wg/owi/specialprojects/SIMS/Shared%20Documents/updates.html" style="color:#cb0000;">UPDATED 01/17/2018</a> &laquo;</strong>
                                                    </div>
                                                    <div class="roundedTopSubPanel">
                                                        <h3>Quick Jump</h3>
                                                        <div style="float:left;border-right:solid 1px #cccccc;padding-right:15px;">
                                                            <span style="font-weight:bold;font-size:small;margin-bottom:3px;">By WSC:&nbsp;&nbsp;
                                                            <telerik:RadDropDownList ID="rddlWSC" runat="server" AutoPostBack="true" OnSelectedIndexChanged="rddlWSC_SelectedIndexChanged" DataTextField="wsc_nm" DataValueField="wsc_id" /></span>
                                                        </div>
                                                        <div style="float:right;padding-left:15px;">
                                                            <span style="font-weight:bold;font-size:small;margin-bottom:3px;">Site No: <asp:TextBox ID="tbSiteNo" runat="server" Width="150px" /> 
                                                            Agency Code: <asp:TextBox ID="tbAgencyCd" runat="server" Width="80px" Text="USGS" /> 
                                                            <asp:Button ID="btnGo" runat="server" OnCommand="btnGo_OnCommand" Text="Go!" CommandArgument="go" /></span>
                                                        </div>
                                                    </div>
                                                </telerik:LayoutColumn>
                                            </Columns>
                                        </telerik:LayoutRow>
                                        <telerik:LayoutRow>
                                            <Columns>
                                                <telerik:LayoutColumn Span="12" SpanMd="12" SpanSm="12" HiddenXs="true">
                                                    <div style="width:950px">
                                                    <p>SIMS (Site Information Management System) is an authoritative data source for metadata not stored within NWIS,  integration and 
                                                        display of data from multiple sources, sharing of stored data with other applications. SIMS provides hydrographers/managers 
                                                        operational relevance about sites (field trips, WSC views), stores site characterization information, provides site strategies for 
                                                        "How do I.. , Where do I.., How often do I.. and How do I do it safely". Provides performance metrics which are used for employee 
                                                        evaluations; referenced in several policy memos and the USGS Safety Requirements Handbook. The core SIMS application manages and 
                                                        supplies station descriptions, manuscripts, DCP information and E-mail communication. SIMS compliant modules include Platform 
                                                        Assignment and Scheduling System (PASS), continuous records processing tools (RMS and CRP), Site Levels Archiving and Processing 
                                                        (SLAP), Site Hazard Analyses (SHA), Traffic Control Plans (TCP), and current information about cableways.</p>
                                                    
                                                    <div class="roundedSubPanel">
                                                        <h3>National Reports</h3>
                                                        <b><asp:HyperLink ID="hlCRPCharts" runat="server" Text="CRP Charts" /></b><br />
                                                        <b>Cableways:</b><br />
                                                        &nbsp;&nbsp;&nbsp;<b><asp:HyperLink ID="hlCablewayInfo" runat="server" Text="Information" /></b><br />
                                                        &nbsp;&nbsp;&nbsp;<b><asp:HyperLink ID="hlCablewayStatus" runat="server" Text="Inspection Status" /></b><br />
                                                        &nbsp;&nbsp;&nbsp;<b><asp:HyperLink ID="hlCablewayReports" runat="server" Text="Other Reports" /></b><br />
                                                        <b><asp:HyperLink ID="hlEvalMaps" runat="server" Text="Evaluation Maps" /></b> (coming soon!)<br />
                                                        <b><asp:HyperLink ID="hlSHA" runat="server" Text="Site Hazard Analyses" /></b><br />
                                                        <b><asp:HyperLink ID="hlWYSummary" runat="server" Text="Water Year Summary Details" /></b>
                                                        <h3>SIMS Compliant Applications</h3>
                                                        <b><asp:HyperLink ID="hlPASSHome" runat="server" Text="PASS Home" Target="_blank" /></b><br />
                                                        <b><asp:HyperLink ID="hlSLAPHome" runat="server" Text="SLAP Home" Target="_blank" /></b><br />
                                                        <b><asp:HyperLink ID="hlDECODES" runat="server" Text="DECODES Repository" NavigateUrl="https://waterdata.usgs.gov/nwisweb/get_decodes" Target="_blank" /></b>
                                                    </div>

                                                    <div class="roundedMainPanel">
                                                        <h3>Select state to obtain access to your local SIMS page.</h3>
                                                        <div style="text-align:center;">
                                                            <img src="images/usmap.gif" alt="Click on a state to go to that WSC's SIMS page." usemap="#Map1" />
                                                            <map name="Map1" id="Map1">
                                                              <area shape="poly" coords="32,12,36,29,53,36,92,30,90,7,47,7,46,16,35,13" href="SIMSWSCHome.aspx?wsc_id=33" alt="WA" />
                                                              <area shape="poly" coords="36,35,33,65,91,67,88,46,92,34,60,40" href="SIMSWSCHome.aspx?wsc_id=28" alt="OR" />
                                                              <area shape="poly" coords="95,8,96,37,94,65,140,66,139,45,118,44,109,34,104,21" href="SIMSWSCHome.aspx?wsc_id=15" alt="ID" />
                                                              <area shape="poly" coords="102,7,113,24,126,40,144,39,198,39,199,5" href="SIMSWSCHome.aspx?wsc_id=8" alt="MT" />
                                                              <area shape="poly" coords="201,6,203,31,260,32,256,17,254,5" href="SIMSWSCHome.aspx?wsc_id=37" alt="ND" />
                                                              <area shape="poly" coords="144,42,142,76,198,76,197,41" href="SIMSWSCHome.aspx?wsc_id=8" alt="WY" />
                                                              <area shape="poly" coords="37,72,66,70,68,96,113,135,111,153,93,151,65,136,46,105,34,87" href="SIMSWSCHome.aspx?wsc_id=12" alt="CA" />
                                                              <area shape="poly" coords="180,159,206,158,209,118,233,118,233,137,281,145,280,156,282,176,265,189,253,200,256,212,242,206,225,183,217,179,206,185,194,174" href="SIMSWSCHome.aspx?wsc_id=31" alt="TX" />
                                                              <area shape="poly" coords="71,72,115,69,115,117,111,126,72,95" href="SIMSWSCHome.aspx?wsc_id=22" alt="NV" />
                                                              <area shape="poly" coords="120,71,138,71,138,80,156,80,156,111,119,111" href="SIMSWSCHome.aspx?wsc_id=32" alt="UT" />
                                                              <area shape="poly" coords="160,79,160,112,214,111,214,78" href="SIMSWSCHome.aspx?wsc_id=13" alt="CO" />
                                                              <area shape="poly" coords="119,116,156,116,157,163,143,164,116,155" href="SIMSWSCHome.aspx?wsc_id=11" alt="AZ" />
                                                              <area shape="poly" coords="160,116,207,117,205,156,176,158,160,162" href="SIMSWSCHome.aspx?wsc_id=24" alt="NM" />
                                                              <area shape="poly" coords="202,34,201,57,260,61,257,36" href="SIMSWSCHome.aspx?wsc_id=37" alt="SD" />
                                                              <area shape="poly" coords="202,61,201,77,219,76,219,85,268,85,257,63" href="SIMSWSCHome.aspx?wsc_id=21" alt="NE" />
                                                              <area shape="poly" coords="104,180,105,221,140,246,134,253,99,229,76,223,53,241,31,255,43,237,24,228,18,212,32,204,9,198,31,191,19,181,39,169,70,171" href="SIMSWSCHome.aspx?wsc_id=10" alt="AK" />
                                                              <area shape="poly" coords="218,88,217,112,276,112,271,89" href="SIMSWSCHome.aspx?wsc_id=18" alt="KS" />
                                                              <area shape="poly" coords="237,116,237,136,279,141,276,116" href="SIMSWSCHome.aspx?wsc_id=27" alt="OK" />
                                                              <area shape="poly" coords="259,7,263,35,264,53,304,54,294,43,292,32,298,23,310,14,280,8" href="SIMSWSCHome.aspx?wsc_id=39" alt="UMW" />
                                                              <area shape="poly" coords="300,28,296,37,310,58,315,62,330,62,333,42,327,32,312,28" href="SIMSWSCHome.aspx?wsc_id=39" alt="UMW" />
                                                              <area shape="poly" coords="264,57,270,80,302,79,310,66,301,55" href="SIMSWSCHome.aspx?wsc_id=17" alt="IA" />
                                                              <area shape="poly" coords="273,83,303,83,312,98,320,115,314,119,280,117,279,99" href="SIMSWSCHome.aspx?wsc_id=20" alt="MO" />
                                                              <area shape="poly" coords="315,66,331,65,334,73,333,99,324,112,306,83" href="SIMSWSCHome.aspx?wsc_id=16" alt="IL" />
                                                              <area shape="poly" coords="345,70,369,69,375,60,374,50,363,52,371,41,361,35,345,27,326,23,324,27,338,36,342,50" href="SIMSWSCHome.aspx?wsc_id=39" alt="UMW" />
                                                              <area shape="poly" coords="338,73,356,73,358,95,349,103,335,106,339,96" href="SIMSWSCHome.aspx?wsc_id=40" alt="IN" />
                                                              <area shape="poly" coords="361,73,377,76,393,70,390,89,377,98,367,97,361,92" href="SIMSWSCHome.aspx?wsc_id=40" alt="OH" />
                                                              <area shape="poly" coords="281,121,314,122,305,142,305,148,283,148" href="SIMSWSCHome.aspx?wsc_id=1" alt="AR" />
                                                              <area shape="poly" coords="284,151,303,151,305,157,301,166,316,167,318,175,313,183,302,180,286,177" href="SIMSWSCHome.aspx?wsc_id=1" alt="LA" />
                                                              <area shape="poly" coords="314,134,331,134,327,158,328,172,323,175,319,166,305,164,308,149" href="SIMSWSCHome.aspx?wsc_id=1" alt="MS" />
                                                              <area shape="poly" coords="326,117,368,116,379,109,372,101,362,97,352,107,335,109" href="SIMSWSCHome.aspx?wsc_id=40" alt="KY" />
                                                              <area shape="poly" coords="315,130,322,120,380,120,365,130" href="SIMSWSCHome.aspx?wsc_id=1" alt="TN" />
                                                              <area shape="poly" coords="335,134,350,133,356,165,337,167,337,172,332,172,331,160" href="SIMSWSCHome.aspx?wsc_id=1" alt="AL" />
                                                              <area shape="poly" coords="355,134,369,133,388,158,384,168,373,170,359,168" href="SIMSWSCHome.aspx?wsc_id=3" alt="GA" />
                                                              <area shape="poly" coords="342,170,363,172,385,173,397,205,394,219,387,218,379,199,376,186,367,178,356,181" href="SIMSWSCHome.aspx?wsc_id=2" alt="FL" />
                                                              <area shape="poly" coords="373,132,389,131,392,136,403,136,409,142,391,156" href="SIMSWSCHome.aspx?wsc_id=3" alt="SC" />
                                                              <area shape="poly" coords="373,129,385,120,429,119,426,129,417,136,412,140,402,133,389,129" href="SIMSWSCHome.aspx?wsc_id=3" alt="NC" />
                                                              <area shape="poly" coords="376,116,432,117,426,109,418,100,413,94,406,103,396,111,384,112" href="SIMSWSCHome.aspx?wsc_id=9" alt="VA" />
                                                              <area shape="poly" coords="420,91,433,92,452,100,455,110,446,122,437,123,434,107,425,103" href="SIMSWSCHome.aspx?wsc_id=6" alt="MD-DEL-DC" />
                                                              <area shape="poly" coords="378,102,383,109,394,107,401,101,411,94,400,91,392,93,384,97" href="SIMSWSCHome.aspx?wsc_id=9" alt="WV" />
                                                              <area shape="poly" coords="396,70,395,88,434,87,435,78,433,70" href="SIMSWSCHome.aspx?wsc_id=29" alt="PA" />
                                                              <area shape="poly" coords="442,76,452,86,459,93,454,99,443,92,439,86" href="SIMSWSCHome.aspx?wsc_id=23" alt="NJ" />
                                                              <area shape="poly" coords="405,66,436,66,448,73,449,44,440,43,433,49,432,56,409,57" href="SIMSWSCHome.aspx?wsc_id=25" alt="NY" />
                                                              <area shape="poly" coords="452,71,463,72,472,86,465,92,455,86,451,79" href="SIMSWSCHome.aspx?wsc_id=7" alt="CT" />
                                                              <area shape="poly" coords="454,62,454,68,467,70,476,84,485,73,490,58,478,59,465,63" href="SIMSWSCHome.aspx?wsc_id=7" alt="MA-RI" />
                                                              <area shape="poly" coords="452,41,471,41,471,59,453,59" href="SIMSWSCHome.aspx?wsc_id=7" alt="NH-VT" />
                                                              <area shape="poly" coords="488,19,496,22,498,35,501,44,489,48,475,55,474,38,482,26" href="SIMSWSCHome.aspx?wsc_id=7" alt="ME" />
                                                              <area shape="poly" coords="169,219,177,222,185,229,188,243,202,247,224,240,206,225,189,217,172,213" href="SIMSWSCHome.aspx?wsc_id=14" alt="HI" />
                                                              <area shape="poly" coords="303,231,307,246,316,246,330,243,338,235,323,231" href="SIMSWSCHome.aspx?wsc_id=2" alt="PR" />
                                                            </map>
                                                        </div>
                                                    </div>
                                                    </div>
                                                </telerik:LayoutColumn>
                                            </Columns>
                                        </telerik:LayoutRow>
                                    </Rows>
                                </telerik:RadPageLayout>
                            </Content>
                        </telerik:CompositeLayoutColumn>
                    </Columns>
                </telerik:LayoutRow>

                <%--Footer--%>
                <telerik:LayoutRow>
                    <Columns>
                        <telerik:LayoutColumn CssClass="footer">
                            <hr />
                            U.S. Geological Survey, <a href="mailto:GS-W_Help_SIMS@usgs.gov">GS-W Help SIMS@usgs.gov</a>, Page Last Updated: 08/30/2017
                        </telerik:LayoutColumn>
                    </Columns>
                </telerik:LayoutRow>
            </Rows>
        </telerik:RadPageLayout>
       </div>
    </form>
</body>
</html>
