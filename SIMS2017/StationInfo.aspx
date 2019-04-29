<%@ Page Title="" Language="C#" MasterPageFile="~/SIMSSingleMenu.Master" AutoEventWireup="true" CodeBehind="StationInfo.aspx.cs" Inherits="SIMS2017.StationInfo" %>
<%@ Register Src="~/Control/SitePageHeading.ascx" TagName="PageHeading" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="styles/stationinfo.css" rel="stylesheet" />
    <script type="text/javascript">
        function openWin(_id, _type) {
            if (_type == "field trip") var oWnd = radopen("Modal/FieldTripEdit.aspx?site_id=" + _id, "rwEditFieldTrips");
            else if (_type == "newrecord") var oWnd = radopen("Modal/RecordEdit.aspx?site_id=" + _id + "&type=" + _type, "rwEditRecords");
            else if (_type == "record") var oWnd = radopen("Modal/RecordEdit.aspx?rms_record_id=" + _id + "&type=" + _type, "rwEditRecords");
            else var oWnd = radopen("https://sims.water.usgs.gov/Safety/Modal/TCP.aspx?TCPID=" + _id + "&type=review", "rwTCPReview");
        }

        function OnClientClose(oWnd, args) {
            //get the transferred arguments
            var arg = args.get_argument();
            if (arg && arg.type == "field trip") {
                var fieldTrips = arg.fieldTrips;
                var hfFieldTripIDs = document.getElementById("<%= hfFieldTripIDs.ClientID %>");
                hfFieldTripIDs.value = fieldTrips;
                $find("<%= ram.ClientID %>").ajaxRequest("RebindFieldTrips");
            }
            else if (arg && arg.type == "record") {
                $find("<%= ram.ClientID %>").ajaxRequest("RebindRecords");
            }
            else {
                $find("<%= ram.ClientID %>").ajaxRequest("RebindSafety");
            }
        }

        function OpenSWR(_SWRurl) {
            open(_SWRurl, 'SWRPopup', 'toolbar=yes, menubar=no, width=840, height=500, scrollbars=yes');
        }

        function ShowAnalysisPopup(period_id) {
            var SAUrl = '/RMS/Modal/ReportPopup.aspx?view=analysisbyperiod&period_id=' + period_id;
            open(SAUrl, 'SAPU', 'toolbar=yes, menubar=no, width=740, height=500, scrollbars=yes');
        }

        function ShowAuditPopup(period_id) {
            var AudUrl = '/RMS/Modal/ViewAudit.aspx?period_id=' + period_id;
            open(AudUrl, 'SAudPU', 'toolbar=yes, menubar=no, width=900, height=600, scrollbar=yes');
        }

        function OnKeyPress(sender, args)
        {
            if (args.get_keyCode() == 13)
            {
                document.getElementById('rbJump').click();
                args.set_cancel(true);
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph1" runat="server">
    <telerik:RadAjaxManager ID="ram" runat="server" OnAjaxRequest="ram_AjaxRequest">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="lbEditPubName">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlPubNameView" />
                    <telerik:AjaxUpdatedControl ControlID="pnlPubNameEdit" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rbPubName">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlPubNameView" />
                    <telerik:AjaxUpdatedControl ControlID="pnlPubNameEdit" />
                    <telerik:AjaxUpdatedControl ControlID="ph1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="lbEditOffice">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlOfficeView" />
                    <telerik:AjaxUpdatedControl ControlID="pnlOfficeEdit" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rddlOffice">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlOfficeView" />
                    <telerik:AjaxUpdatedControl ControlID="pnlOfficeEdit" />
                    <telerik:AjaxUpdatedControl ControlID="ph1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="ram">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlFieldTripView" />
                    <telerik:AjaxUpdatedControl ControlID="pnlRMS" LoadingPanelID="ralp" />
                    <telerik:AjaxUpdatedControl ControlID="pnlSafety" LoadingPanelID="ralp" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rddlWYs">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="ltlHistoricPeriod" />
                    <telerik:AjaxUpdatedControl ControlID="rddlWYs" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="dlDCPTable">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="dlDCPTable" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadAjaxLoadingPanel ID="ralp" runat="server" Skin="Bootstrap" />
    <telerik:RadWindowManager RenderMode="Lightweight" ID="rwm" ShowContentDuringLoad="false" VisibleStatusbar="false" ReloadOnShow="true" runat="server" EnableShadow="true">
        <Windows>
            <telerik:RadWindow RenderMode="Lightweight" ID="rwEditFieldTrips" runat="server" Behaviors="Close" OnClientClose="OnClientClose" Width="900" Height="400" />
            <telerik:RadWindow RenderMode="Lightweight" ID="rwEditRecords" runat="server" Behaviors="Close" OnClientClose="OnClientClose" Width="700" Height="820" />
            <telerik:RadWindow RenderMode="Lightweight" ID="rwTCPReview" runat="server" Behaviors="Close" OnClientClose="OnClientClose" Width="900" Height="900" Modal="true" />
        </Windows>
    </telerik:RadWindowManager>
    <asp:HiddenField ID="hfFieldTripIDs" runat="server" />

    <uc:PageHeading id="ph1" runat="server" />
    <div class="linkbar">
        <div style="float:left;">
            <telerik:RadTextBox ID="rtbSiteNo" runat="server" EmptyMessage="enter site number" ClientEvents-OnKeyPress="OnKeyPress"  />
            <telerik:RadButton ID="rbJump" runat="server" Text="jump!" OnClick="rbJump_Click" ClientIDMode="Static" />
        </div>
        <div style="float:right;">
            <asp:HyperLink ID="hlNWISWeb" runat="server" Text="Go to NWISWeb" Target="_blank" /> |
            <asp:HyperLink ID="hlNWISOpsRequest" runat="server" Text="NWIS Ops Request" /> |
            <a href="mailto:GS-W_Help_SIMS@usgs.gov">GS-W Help SIMS</a>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph2" runat="server">
    <div class="mainContent">
        <div class="leftColumn">
            <div>
                <h4>Station Details</h4>
                <asp:Panel ID="pnlPubNameView" runat="server">
                    Published name: <b><asp:Literal ID="ltlPubName" runat="server" /></b> <asp:LinkButton ID="lbEditPubName" runat="server" Text="edit" OnCommand="Edit_Command" CommandArgument="PubName" />
                </asp:Panel>
                <asp:Panel ID="pnlPubNameEdit" runat="server" Visible="false">
                    Published name: <telerik:RadTextBox ID="rtbPubName" runat="server" Width="300px" /> <telerik:RadButton ID="rbPubName" runat="server" Text="commit" OnCommand="Commit_Command" CommandArgument="PubName" />
                </asp:Panel>
                <asp:Panel ID="pnlOfficeView" runat="server">
                    Office assignment: <b><asp:Literal ID="ltlOffice" runat="server" /></b> <asp:LinkButton ID="lbEditOffice" runat="server" Text="edit" OnCommand="Edit_Command" CommandArgument="Office" />
                </asp:Panel>
                <asp:Panel ID="pnlOfficeEdit" runat="server" Visible="false">
                    Office assignment: <telerik:RadDropDownList ID="rddlOffice" runat="server" DataValueField="office_id" DataTextField="office_nm" OnSelectedIndexChanged="Commit_Command" AutoPostBack="true" Width="350px" />
                </asp:Panel>
                <asp:Panel ID="pnlFieldTripView" runat="server">
                    Field trip(s): <b><asp:Literal ID="ltlFieldTrip" runat="server" /></b> <asp:HyperLink ID="hlMapTrips" runat="server" Target="_blank" Text="map trip" /> | <asp:LinkButton ID="lbEditFieldTrip" runat="server" Text="edit" />
                </asp:Panel> 
                <p><asp:HyperLink ID="hlSiFTA" runat="server" Target="_blank" Text="Take me to SiFTA for this site." Font-Bold="true" /></p>
            </div>

            <div>
                <h4>Station Documents</h4>
                <div style="padding-left:15px;">
                    <asp:HyperLink ID="hlEditDocs" runat="server" Text="Edit Documents" /><br />
                    <asp:HyperLink ID="hlSDESC" runat="server" Text="Station Description" /><br />
                    <asp:HyperLink ID="hlMANU" runat="server" Text="Manuscript" /> &nbsp;&nbsp;<asp:Literal ID="ltlApproved" runat="server" /><br />
                    <asp:HyperLink ID="hlSANAL" runat="server" Text="Station Analysis" /><br />
                    <asp:HyperLink ID="hlCustomReport" runat="server" Text="Custom Report" /><br />
                    <asp:HyperLink ID="hlArchives" runat="server" Text="Retrieve Archived Elements" /><br />
                    <asp:HyperLink ID="hlSLAP" runat="server" Text="SLAP: Historic Level Summary" Target="_blank" />
                </div>
            </div>

            <div>
                <h4>Safety</h4>
                <asp:Panel ID="pnlSafety" runat="server" CssClass="SafetyPanel">
                    <div style="width:95%;text-align:right;font-size:9pt;">
                        &raquo; <asp:HyperLink ID="hlSHATutorial" runat="server" target="_blank" Text="Download the SHA Tutorial" />
                    </div>
                    <asp:Panel ID="pnlSHACreate" runat="server">
                        <asp:HyperLink ID="hlSHACreate" runat="server" Text="Create an SHA for this site" />
                    </asp:Panel>
                    <asp:Panel ID="pnlSHAEdit" runat="server">
                        <b><asp:HyperLink ID="hlSHAEdit" runat="server" Text="Site Hazard Analysis" /></b> &nbsp;&nbsp;<asp:HyperLink ID="hlSHAPrintVersion" runat="server" Text="&laquo; view print version" Font-Size="Small" />
                        <div style="padding-left:10px;font-size:10pt;">
                            <asp:Literal ID="ltlSHAReviewed" runat="server" /><br />
                            <asp:Literal ID="ltlSHAApproved" runat="server" />
                        </div>
                    </asp:Panel>
                    <hr />
                    <div style="width:95%;text-align:right;margin-top:-5px;font-size:9pt;">
                        &raquo; <asp:HyperLink ID="hlTCPTutorial" runat="server" target="_blank" Text="Download the TCP Tutorial" />
                    </div>
                    <asp:Panel ID="pnlTCPCreate" runat="server">
                        <asp:HyperLink ID="hlTCPCreate" runat="server" Text="Create a TCP for this site" />
                    </asp:Panel>
                    <asp:Panel ID="pnlTCPEdit" runat="server">
                        <b>Traffic Control Plans (last approved):</b>
                        <div style="padding-left:10px;font-size:10pt;">
                            <asp:HyperLink ID="hlTCPEdit" runat="server" Text="Edit/Add/Delete Traffic Control Plans" Font-Bold="true" /><br />
                            <asp:DataList ID="dlTCPs" runat="server" OnItemDataBound="dlTCPs_ItemDataBound">
                                <ItemTemplate>
                                    <asp:HyperLink ID="hlTCP" runat="server" Text='<%# Eval("TCPName") %>' NavigateUrl='<%# Eval("TCPURL") %>' /> (<%# Eval("LastApprovedDt") %>)<br />
                                    <img src="images/underarrow.png" alt="look here!" style="padding-left:15px;" /> <asp:LinkButton ID="lbTCPReview" runat="server" />
                                </ItemTemplate>
                            </asp:DataList>
                            <asp:HyperLink ID="hlTCPTrackStatus" runat="server" Text="Track Approval Status" Font-Bold="true" />
                        </div>
                    </asp:Panel>
                </asp:Panel>
            </div>

            <div>
                <h4>DCP/Realtime Ops</h4>
                <asp:Panel ID="pnlDCPTable" runat="server" CssClass="DCPTable">
                    <div>
                        <b>Office Time:</b> <asp:Literal ID="ltlDCPOfficeTime" runat="server" /><br />
                        <b>Site Time:</b> <asp:Literal ID="ltlDCPSiteTime" runat="server" /><br />
                        <b>GMT Time:</b> <asp:Literal ID="ltlDCPGMTTime" runat="server" />
                    </div>
                    <asp:DataList ID="dlDCPTable" runat="server">
                        <ItemTemplate>
                            <table width="700">
                                <tr>
                                    <td width="180">
                                        <b>Next Transmit Time:</b>
                                        <table style="border:1px solid #acb274;" cellpadding="3px">
                                            <tr>
                                                <td><b>Local</b></td>
                                                <td style="background-color:white;color: #808080;"><b><%# Eval("LocalTransmitTime") %></b></td>
                                            </tr>
                                            <tr>
                                                <td><b>GMT</b></tdstyle="background-color:white;>
                                                <td style="background-color:white;color: #808080;"><b><%# Eval("GMTTransmitTime") %></b></td>
                                            </tr>
                                        </table>
                                        <b>Minutes to next trans.: <span style="background-color:white;padding:0 3px 0 3px;color: #808080;"><%# Eval("MinutesToNext") %></span></b>
                                    </td>
                                    <td valign="top">
                                        <table style="border:1px solid #acb274;" cellpadding="3px">
                                            <tr>
                                                <td style="text-align:center;"><b>DCPID</b></td>
                                                <td style="text-align:center;"><b>Prim./<br />Rndm. Channel</b></td>
                                                <td style="text-align:center;"><b>Prim./<br />Rndm. Baud</b></td>
                                                <td style="text-align:center;"><b>Satellite Azimuth/<br />Elevation</b></td>
                                                <td style="text-align:center;"><b>Trans. Time/<br />Interval/ Window</b></td>
                                            </tr>
                                            <tr>
                                                <td style="background-color:white;text-align:center;color: #808080;"><%# Eval("dcp_id") %></td>
                                                <td style="background-color:white;text-align:center;color: #808080;"><%# Eval("primary_ch") %> / <%# Eval("random_ch") %></td>
                                                <td style="background-color:white;text-align:center;color: #808080;"><%# Eval("primary_bd") %> / <%# Eval("random_bd") %></td>
                                                <td style="background-color:white;text-align:center;color: #808080;"><%# Eval("satellite") %><%# Eval("ant_azimuth") %> / <%# Eval("ant_elev") %></td>
                                                <td style="background-color:white;text-align:center;color: #808080;"><%# Eval("assigned_time") %> / <%# Eval("trans_interval") %> / <%# Eval("window") %></td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2" valign="top">
                                        <div style="width:100%;">
                                            <div style="width:30%;float:left;">
                                                <b><a href='<%# String.Format("https://hads.ncep.noaa.gov/cgi-bin/hads/interactiveDisplays/displayMetaData.pl?table=dcp&nesdis_id={0}", Eval("dcp_id")) %>' target="_blank">NWS HADS System</a></b><br />
                                                <b><a href='<%# String.Format("http://eddn.usgs.gov/cgi-bin/configSummary.cgi?dcpid={0}&type=view", Eval("dcp_id")) %>' target="_blank">EDDN Platform Configuration</a></b><br />
                                                <b><a href='<%# String.Format("{0}", Eval("PASSURL")) %>' target="_blank">PASS Home</a></b><br />
                                            </div>
                                            <div style="float:right;width:70%;">
                                                <b>View data for specified hours:</b> <asp:TextBox ID="tbDCPViewData" runat="server" Text="8" Width="40px" /> 
                                                <asp:Button ID="btnDCPViewData" runat="server" OnCommand="btnDCPViewData_Command" CommandArgument='<%# Eval("dcp_id") %>' Text="Go!" /><br />
                                                <!--<b><i>This option currently not available. Maintainers of this web site have been notified of the problem.</i></b>-->
                                            </div>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </ItemTemplate>
                    </asp:DataList>
                </asp:Panel>
                <asp:Literal ID="ltlNoDCP" runat="server" />
            </div>
        </div>
        <div class="rightColumn">
            <div>
                <h4>Continuous Records Processing</h4>
                <asp:Panel ID="pnlRMS" runat="server" CssClass="RMSPanel">
                    <div style="width:100%;text-align:right;font-size:9pt;">
                        <asp:HyperLink ID="hlAutoReview" runat="server" target="_blank" Text="Click here to view Auto Review (if applicable)" />
                    </div> 
                    <asp:DataList ID="dlRecords" runat="server" OnItemDataBound="dlRecords_ItemDataBound">
                        <ItemTemplate>
                            <asp:HiddenField ID="hfRMSRecordID" runat="server" />
                            <div style="width:100%;">
                                <h5><%# Eval("type_ds") %></h5>
                                <div style="float:right;margin-top:-30px">
                                    <asp:LinkButton ID="lbEditRecord" runat="server" Text="edit" /> | 
                                    <asp:HyperLink ID="hlAuditRecord" runat="server" Text="audit" />
                                </div>
                            </div>
                            <asp:Panel ID="pnlRecord" runat="server" CssClass="RMSRecordPanel">
                                <div>
                                    Operator/Analyst/Approver/Auditor:<br />&nbsp;&nbsp;&nbsp;<b><%# Eval("personnel") %></b><br />
                                    Status: <b><%# Eval("active") %></b> 
                                    <%# Eval("cat_no") %><br />
                                    Time-series: <b><%# Eval("time_series") %></b><br />
                                    Responsible office: <b><%# Eval("office_cd") %></b><br />
                                    <asp:Label ID="lblAQApprovalHistory" runat="server" Text="AQ Approval History" Font-Bold="true" />
                                    <telerik:RadToolTip RenderMode="Lightweight" runat="server" ID="rttAQApprovalHistory" TargetControlID="lblAQApprovalHistory" IsClientID="false"
                                        ShowEvent="OnMouseOver" HideEvent="Default" Position="MiddleRight" RelativeTo="Mouse" Width="300px" Height="270px" Skin="Bootstrap" AutoCloseDelay="9000" 
                                        Text="If data approved in AQ need to be modified, the audit step and revision policy need to be followed. If a period in RMS was accidentally set to approved and the data in 
                                        AQ for that period are not approved, please send a note to GS-W Help SIMS to have that period set back to analyzing.<br /><br />Click on the time-series parameter codes above to visit the AQ approval history for that ID.">
                                    </telerik:RadToolTip><br />
                                </div>
                                <div class="NewPeriod">                                                    
                                    <asp:HyperLink ID="hlNewPeriod" runat="server" Text="analyze new period" /> <asp:Image ID="imgLock" runat="server" />
                                </div>

                                <asp:Literal ID="ltlHistoricPeriod" runat="server" />

                                <asp:Literal ID="ltlCurrentPeriods" runat="server" />

                                <div style="height:30px;">
                                    <div style="float:left;padding-top:5px;">View approved records from WY:</div>
                                    <div style="float:right;"><telerik:RadDropDownList ID="rddlWYs" runat="server" DataValueField="WY" DataTextField="WY" AutoPostBack="true" OnSelectedIndexChanged="rddlWYs_SelectedIndexChanged" Width="80px" /></div>
                                </div>
                            </asp:Panel>
                            <asp:Panel ID="pnlInactive" runat="server" CssClass="RMSRecordPanel">
                                <p style="font-weight:bold;line-height:8pt;">This record is currently inactive.</p>
                            </asp:Panel>
                        </ItemTemplate>
                    </asp:DataList>
                    <asp:LinkButton ID="lbNewRecordType" runat="server" Text="Assign another record-type" Font-Bold="true" />
                </asp:Panel>
            </div>
        </div>
    </div>
</asp:Content>
